using UnityEngine;

#if UNITY_EDITOR
#endif
using LibBSP;

namespace BSPImporter
{

    public partial class BSPLoader
    {
        /// <summary>
        /// Enum with options for combining <see cref="Mesh"/>es in the BSP import process.
        /// </summary>
        public enum MeshCombineOptions
        {
            /// <summary>
            /// Do not combine <see cref="Mesh"/>es.
            /// </summary>
            None,
            /// <summary>
            /// Combine all <see cref="Mesh"/>es in an <see cref="Entity"/> which use the same <see cref="Material"/>.
            /// </summary>
            PerMaterial,
            /// <summary>
            /// Combine all <see cref="Mesh"/>es in an <see cref="Entity"/> into a single <see cref="Mesh"/>
            /// </summary>
            PerEntity,
        }
    }
}
