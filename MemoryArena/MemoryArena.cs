using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public unsafe class MemoryArena : IDisposable
{
    /// <summary>Represents a contiguous memory block with its own local offset and size.</summary>
    public unsafe struct Chunk
    {
        public byte* MemoryBlock;
        public int Offset;
        public int Size;

        public int UsedBytes => Offset;
        public int FreeBytes => Size - Offset;
    }

    private const int MIN_CHUNK_SIZE = 256;
    private const int MIN_ALIGNMENT = 4;

    private readonly List<Chunk> chunks = new List<Chunk>(4);
    private int currentChunkIndex = 0;
    private int capacityPerChunk;
    private readonly int alignment;

    /// <summary>True if there are multiple chunks of different sizes allocated.</summary>
    public bool ChunksAreMixedSize
    {
        get
        {
            for (int i = 0; i < chunks.Count; i++)
            {
                if (chunks[i].Size != capacityPerChunk)
                {
                    return true;
                }
            }

            return false;
        }
    }

    /// <summary>Total bytes allocated across all chunks.</summary>
    public int TotalUsedBytes
    {
        get
        {
            int _total = 0;

            for (int i = 0; i < chunks.Count; i++)
            {
                _total += chunks[i].UsedBytes;
            }

            return _total;
        }
    }

    /// <summary>Total free bytes across all chunks.</summary>
    public int TotalFreeBytes
    {
        get
        {
            int _total = 0;

            for (int i = 0; i < chunks.Count; i++)
            {
                _total += chunks[i].FreeBytes;
            }

            return _total;
        }
    }

    /// <summary>Total bytes allocated across all chunks.</summary>
    public int TotalAllocatedBytes
    {
        get
        {
            int _total = 0;

            for (int i = 0; i < chunks.Count; i++)
            {
                _total += chunks[i].Size;
            }

            return _total;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryArena"/> class with the specified size and alignment.
    /// </summary>
    /// <remarks>The constructor allocates the first memory chunk immediately upon initialization. The size
    /// and alignment values are adjusted to ensure they meet the minimum requirements.</remarks>
    /// <param name="_sizeInBytes">The size, in bytes, of each memory chunk. The value is rounded up to the nearest power of two and must be at
    /// least the minimum chunk size.</param>
    /// <param name="_alignmentBytes">The alignment, in bytes, for memory allocations. The value is rounded up to the nearest power of two and must be
    /// at least the minimum alignment. Defaults to 16 bytes.</param>
    public MemoryArena(int _sizeInBytes, int _alignmentBytes = 16)
    {
        capacityPerChunk = Mathf.NextPowerOfTwo(_sizeInBytes.ClampMin(MIN_CHUNK_SIZE));
        alignment = Mathf.NextPowerOfTwo(_alignmentBytes.ClampMin(MIN_ALIGNMENT));

        // Allocate first chunk immediately.
        allocateNewChunk();
    }

    /// <summary>
    /// Finalizes the <see cref="MemoryArena"/> instance by releasing unmanaged resources.
    /// </summary>
    /// <remarks>This destructor ensures that unmanaged resources are released by calling the <see
    /// cref="Dispose"/> method. It is recommended to explicitly call <see cref="Dispose"/> to release resources
    /// deterministically, rather than relying on the finalizer.</remarks>
    ~MemoryArena()
    {
        Dispose();
    }

    /// <summary>
    /// Allocates a block of memory for a specified number of unmanaged elements of type <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>This method manages memory allocation within a chunk-based arena. If the requested memory
    /// does not fit in the  current chunk, it attempts to reuse an existing chunk or allocates a new chunk as needed.
    /// The memory is aligned  to ensure proper alignment for the type <typeparamref name="T"/>.</remarks>
    /// <typeparam name="T">The type of unmanaged elements to allocate memory for.</typeparam>
    /// <param name="_count">The number of elements of type <typeparamref name="T"/> to allocate. Must be greater than zero. Defaults to 1.</param>
    /// <returns>A pointer to the allocated memory block, aligned appropriately for the type <typeparamref name="T"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="_count"/> is less than or equal to zero.</exception>
    public T* Allocate<T>(int _count = 1) where T : unmanaged
    {
        if (_count <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(_count));
        }

        int _size = sizeof(T) * _count;

        // If a single allocation is larger than current capacity baseline, grow the arena.
        if (_size > capacityPerChunk)
        {
            growCapacity(_size);
        }

        // Local copy of current chunk
        Chunk _chunk = chunks[currentChunkIndex];
        int _alignedOffset = alignUp(_chunk.Offset);

        // If it does not fit in current chunk, try reuse next chunk if it exists, else allocate new with current base capacity.
        if (_alignedOffset + _size > _chunk.Size)
        {
            // try reuse next chunk if it exists
            if (currentChunkIndex + 1 < chunks.Count)
            {
                currentChunkIndex++;
                _chunk = chunks[currentChunkIndex];
                _alignedOffset = alignUp(_chunk.Offset);

                if (_alignedOffset + _size <= _chunk.Size)
                {
                    byte* reusePtr = _chunk.MemoryBlock + _alignedOffset;
                    _chunk.Offset = _alignedOffset + _size;
                    chunks[currentChunkIndex] = _chunk;
                    return (T*)reusePtr;
                }
            }

            // allocate new chunk with current base capacity and use it there (guaranteed to fit)
            allocateNewChunk();
            _chunk = chunks[currentChunkIndex];
            _alignedOffset = alignUp(_chunk.Offset);
            _chunk.Offset = _alignedOffset + _size;
            chunks[currentChunkIndex] = _chunk;
            return (T*)(_chunk.MemoryBlock + _alignedOffset);
        }
        else
        {
            // Fits in current chunk
            byte* ptr = _chunk.MemoryBlock + _alignedOffset;
            _chunk.Offset = _alignedOffset + _size;
            chunks[currentChunkIndex] = _chunk;
            return (T*)ptr;
        }
    }

    /// <summary>
    /// Resets the internal state of the object, releasing or reallocating memory as needed.
    /// </summary>
    /// <remarks>If the chunks are of mixed sizes or there are multiple chunks, all chunks are released, and a
    /// single new chunk  is allocated based on the current capacity baseline. If the total used bytes exceed the
    /// current capacity baseline,  the capacity is grown to the next power of two before allocating the new chunk.  If
    /// the chunks are of uniform size, their offsets are reset, and the current chunk index is set to zero.</remarks>
    public void Reset()
    {
        // If chunks are of mixed sizes or there are multiple chunks, release all and allocate a single chunk.
        if (ChunksAreMixedSize || chunks.Count > 1)
        {
            int _totalUsedBytes = TotalUsedBytes;

            // Release all and allocate a single chunk with current capacity baseline.
            freeAllChunks();

            // If total used bytes exceed current capacity baseline, grow to next power-of-two of that.
            if (_totalUsedBytes > capacityPerChunk)
            {
                capacityPerChunk = Mathf.NextPowerOfTwo(_totalUsedBytes);
            }

            // Allocate a single fresh chunk, with current (possibly grown) capacity baseline.
            allocateNewChunk();
            return;
        }

        // Just reset offsets if sizes are uniform.
        for (int i = 0; i < chunks.Count; i++)
        {
            Chunk _chunk = chunks[i];
            _chunk.Offset = 0;
            chunks[i] = _chunk;
        }

        currentChunkIndex = 0;
    }

    /// <summary>
    /// Releases all resources used by the current instance of the class.
    /// </summary>
    /// <remarks>This method should be called when the instance is no longer needed to free allocated
    /// resources.  After calling this method, the instance is in an unusable state and should not be used
    /// further.</remarks>
    public void Dispose()
    {
        freeAllChunks();
        currentChunkIndex = 0;
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Increases the capacity of the current chunk to accommodate the required size, if necessary.
    /// </summary>
    /// <remarks>If the required size exceeds the current capacity, the capacity is increased to the next
    /// power of two  greater than or equal to the required size. This method allocates a new chunk when the capacity is
    /// updated.</remarks>
    /// <param name="_requiredSize">The minimum size required to ensure sufficient capacity.</param>
    private void growCapacity(int _requiredSize)
    {
        int _newCapacity = Mathf.NextPowerOfTwo(_requiredSize);

        if (_newCapacity <= capacityPerChunk)
        {
            return;
        }

        capacityPerChunk = _newCapacity;
        allocateNewChunk();
    }

    /// <summary>
    /// Allocates a new memory chunk and adds it to the collection of chunks.
    /// </summary>
    /// <remarks>This method allocates a block of memory with the specified capacity and alignment,  creates a
    /// new chunk to manage the memory, and appends it to the internal list of chunks.  The current chunk index is
    /// updated to point to the newly allocated chunk.</remarks>
    private void allocateNewChunk()
    {
        byte* _memoryBlock = (byte*)UnsafeUtility.Malloc(capacityPerChunk, alignment, Allocator.Persistent);

        Chunk _chunk = new Chunk
        {
            MemoryBlock = _memoryBlock,
            Offset = 0,
            Size = capacityPerChunk
        };

        chunks.Add(_chunk);
        currentChunkIndex = chunks.Count - 1;
    }

    /// <summary>
    /// Releases all memory blocks associated with the chunks and resets their state.
    /// </summary>
    /// <remarks>This method iterates through all chunks, frees their allocated memory, and resets each chunk
    /// to its default state.  Afterward, the collection of chunks is cleared. This operation ensures that all resources
    /// are properly released.</remarks>
    private void freeAllChunks()
    {
        for (int i = 0; i < chunks.Count; i++)
        {
            if (chunks[i].MemoryBlock != null)
            {
                UnsafeUtility.Free(chunks[i].MemoryBlock, Allocator.Persistent);
                chunks[i] = default;
            }
        }

        chunks.Clear();
    }

    /// <summary>
    /// Aligns the specified value to the nearest multiple of the alignment value.
    /// </summary>
    /// <remarks>The alignment value is determined by the field <c>alignment</c>. This method ensures that the
    /// result is a multiple of the alignment value.</remarks>
    /// <param name="_value">The value to be aligned. Must be non-negative.</param>
    /// <returns>The smallest multiple of the alignment value that is greater than or equal to <paramref name="_value"/>.</returns>
    private int alignUp(int _value)
    {
        int _mask = alignment - 1;
        return (_value + _mask) & ~_mask;
    }
}
