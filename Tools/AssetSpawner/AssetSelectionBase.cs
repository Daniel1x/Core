namespace DL.AssetLoading
{
    using System.Collections.Generic;
    using UnityEngine;

    [SelectionBase, ExecuteAlways]
    public class AssetSelectionBase : MonoBehaviour
    {
        public static int ActiveInstancesCount => activeInstances.Count;

        private static List<AssetSelectionBase> activeInstances = new();

        public GameObject Redirection { get; set; } = null;

        private void OnEnable()
        {
            if (!activeInstances.Contains(this))
            {
                activeInstances.Add(this);
            }
        }

        private void OnDisable()
        {
            activeInstances.Remove(this);
        }
    }
}
