namespace DL.Core.Maze
{
    using UnityEngine;

    public class PolygonPermutationData : IPolygonModifier
    {
        private readonly PolygonMaterialVariants variants;
        private readonly PolygonModifier modifier;

        public PolygonMaterialVariants Variants => variants;
        public PolygonModifier Modifier => modifier;
        public int BitMask => modifier.BitMask;
        public float AngleOffset => modifier.AngleOffset;

        public PolygonPermutationData(Material _material, PolygonModifier _modifier)
        {
            if (_material == null)
            {
                throw new System.ArgumentNullException(nameof(_material), "Material cannot be null.");
            }

            modifier = _modifier;
            variants = new PolygonMaterialVariants(_material);
        }

        public PolygonPermutationData(PolygonMaterialVariants _variants, PolygonModifier _modifier)
        {
            if (_variants == null)
            {
                throw new System.ArgumentNullException(nameof(_variants), "PolygonMaterialVariants cannot be null.");
            }

            variants = _variants;
            modifier = _modifier;
        }
    }
}
