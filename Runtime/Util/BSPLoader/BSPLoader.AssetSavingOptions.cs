using System;
using UnityEngine;

#if UNITY_EDITOR
#endif

namespace BSPImporter
{

    public partial class BSPLoader
    {
        /// <summary>
        /// Enum with flags defining which generated assets to save from the import process, at edit-time only.
        /// </summary>
        [Flags]
        public enum AssetSavingOptions
        {
            /// <summary>
            /// Do not save any assets.
            /// </summary>
            None = 0,
            /// <summary>
            /// Save generated <see cref="Material"/> assets only.
            /// </summary>
            Materials = 1,
            /// <summary>
            /// Save generated <see cref="Mesh"/> assets only.
            /// </summary>
            Meshes = 2,
            /// <summary>
            /// Save generated <see cref="Material"/> and <see cref="Mesh"/> assets.
            /// </summary>
            MaterialsAndMeshes = 3,
            /// <summary>
            /// Save generated <see cref="GameObject"/> as a prefab only.
            /// </summary>
            Prefab = 4,
            /// <summary>
            /// Save generated <see cref="Material"/> assets and <see cref="GameObject"/> prefab.
            /// </summary>
            MaterialsAndPrefab = 5,
            /// <summary>
            /// Save generated <see cref="Mesh"/> assets and <see cref="GameObject"/> prefab.
            /// </summary>
            MeshesAndPrefab = 6,
            /// <summary>
            /// Save all generated <see cref="Mesh"/> and <see cref="Material"/> assets and <see cref="GameObject"/> prefab.
            /// </summary>
            MaterialsMeshesAndPrefab = 7,
        }
    }
}
