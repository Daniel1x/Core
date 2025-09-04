using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

/// <summary>
/// Simple growing linear arena allocator implemented on top of unmanaged memory.
/// Allocations are very fast (pointer bump). When current chunk is full, a new chunk is used or allocated.
/// Memory is only released when calling <see cref="Dispose"/>. Use <see cref="Reset"/> to reuse all allocated chunks.
/// Thread-safety: NOT thread-safe.
/// </summary>
public unsafe class MemoryArena : IDisposable
{
    /// <summary>Represents a single contiguous memory block (chunk) with its own local offset.</summary>
    public unsafe struct Chunk
    {
        public byte* MemoryBlock; // Raw pointer to the chunk memory.
        public int Offset;        // Current write offset (in bytes) inside this chunk.
    }

    private readonly List<Chunk> chunks = new List<Chunk>(4);
    private int currentChunkIndex = 0;

    private readonly int capacityPerChunk;
    private readonly int alignment;

    /// <summary>Size (in bytes) of each chunk. All chunks share the same capacity.</summary>
    public int ChunkCapacity => capacityPerChunk;

    /// <summary>Total number of chunks currently allocated.</summary>
    public int ChunkCount => chunks.Count;

    /// <summary>Index of the chunk that is currently used for allocations.</summary>
    public int CurrentChunkIndex => currentChunkIndex;

    /// <summary>Total bytes reserved by all chunks (allocated capacity, not actually used).</summary>
    public long TotalReservedBytes => (long)capacityPerChunk * chunks.Count;

    /// <summary>
    /// Total bytes used (sum of all previous full chunks + current chunk offset).
    /// Note: This does NOT include alignment padding after the last allocation.
    /// </summary>
    public long UsedBytes
    {
        get
        {
            long _full = (long)currentChunkIndex * capacityPerChunk;

            if (currentChunkIndex < chunks.Count)
            {
                _full += chunks[currentChunkIndex].Offset;
            }
            
            return _full;
        }
    }

    /// <summary>
    /// Creates a new memory arena. The first chunk is allocated immediately.
    /// The provided size is rounded up to the next power of two (minimum 16).
    /// Alignment is also promoted to next power of two.
    /// </summary>
    public MemoryArena(int _sizeInBytes, int _alignmentBytes = 16)
    {
        _sizeInBytes = Mathf.NextPowerOfTwo(_sizeInBytes.ClampMin(16));
        alignment = Mathf.NextPowerOfTwo(_alignmentBytes.ClampMin(2));
        capacityPerChunk = _sizeInBytes;
        allocateNewChunk();
    }

    ~MemoryArena()
    {
        Dispose();
    }

    /// <summary>
    /// Allocates space for <paramref name="_count"/> elements of unmanaged type T.
    /// Reuses an already allocated next chunk (if it exists and has space)
    /// before allocating a brand-new chunk.
    /// </summary>
    public T* Allocate<T>(int _count = 1) where T : unmanaged
    {
        if (_count <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(_count));
        }

        int _size = sizeof(T) * _count;

        if (_size > capacityPerChunk)
        {
            throw new Exception($"MemoryArena: Requested block ({_size} B) exceeds chunk capacity ({capacityPerChunk} B).");
        }

        // Work on a local copy (Chunk is a struct).
        Chunk _chunk = chunks[currentChunkIndex];

        int _alignedOffset = alignUp(_chunk.Offset, alignment);

        // Not enough space in current chunk.
        if (_alignedOffset + _size > capacityPerChunk)
        {
            // 1) Try to reuse an already existing next chunk (allocated earlier and reset to Offset = 0).
            if (currentChunkIndex + 1 < chunks.Count)
            {
                currentChunkIndex++;
                _chunk = chunks[currentChunkIndex];

                _alignedOffset = alignUp(_chunk.Offset, alignment);

                if (_alignedOffset + _size <= capacityPerChunk)
                {
                    byte* reusePtr = _chunk.MemoryBlock + _alignedOffset;
                    _chunk.Offset = _alignedOffset + _size;
                    chunks[currentChunkIndex] = _chunk;
                    return (T*)reusePtr;
                }
                // If even that chunk is full, fall through to allocate a new one.
            }

            // 2) Allocate a new chunk.
            allocateNewChunk();
            _chunk = chunks[currentChunkIndex];

            _alignedOffset = alignUp(_chunk.Offset, alignment); // usually 0
            _chunk.Offset = _alignedOffset + _size;
            chunks[currentChunkIndex] = _chunk;

            return (T*)(_chunk.MemoryBlock + _alignedOffset);
        }
        else
        {
            // Space fits inside current chunk.
            byte* _ptr = _chunk.MemoryBlock + _alignedOffset;
            _chunk.Offset = _alignedOffset + _size;
            chunks[currentChunkIndex] = _chunk;
            return (T*)_ptr;
        }
    }

    /// <summary>
    /// Resets all chunk offsets to 0 enabling re-use of the arena without deallocating memory.
    /// Only the first chunk will receive subsequent allocations after reset.
    /// </summary>
    public void Reset()
    {
#if UNITY_EDITOR
        Debug.Log($"MemoryArena Reset :: used {UsedBytes} / {TotalReservedBytes} (chunks: {ChunkCount})");
#endif
        for (int i = 0; i < chunks.Count; i++)
        {
            Chunk _chunk = chunks[i];
            _chunk.Offset = 0;
            chunks[i] = _chunk;
        }

        currentChunkIndex = 0;
    }

    /// <summary>
    /// Frees all allocated chunks and clears the arena.
    /// After calling Dispose the arena should not be used.
    /// </summary>
    public void Dispose()
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
        currentChunkIndex = 0;
        GC.SuppressFinalize(this);
    }

    /// <summary>Allocates a new chunk and switches to it.</summary>
    private void allocateNewChunk()
    {
        byte* _block = (byte*)UnsafeUtility.Malloc(capacityPerChunk, alignment, Allocator.Persistent);

        Chunk _chunk = new Chunk
        {
            MemoryBlock = _block,
            Offset = 0
        };

        chunks.Add(_chunk);
        currentChunkIndex = chunks.Count - 1;
    }

    /// <summary>Aligns value up to the next multiple of alignment (power-of-two alignment).</summary>
    private static int alignUp(int _value, int _align)
    {
        int mask = _align - 1;
        return (_value + mask) & ~mask;
    }
}
