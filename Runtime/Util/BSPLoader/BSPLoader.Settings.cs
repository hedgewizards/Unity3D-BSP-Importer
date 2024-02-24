using System;
using System.Collections.Generic;
using UnityEngine;

using LibBSP;

namespace BSPImporter
{

    public partial class BSPLoader
    {
        /// <summary>
        /// Struct containing various settings for the BSP Import process.
        /// </summary>
        [Serializable]
        public struct Settings
        {
            /// <summary>
            /// The path to the BSP file.
            /// </summary>
            public string path;
            /// <summary>
            /// The path to the textures for the BSP file. At edit-time, if the path is within the Assets folder, links textures with generated <see cref="Material"/>s.
            /// </summary>
            public string texturePath;
            /// <summary>
            /// How to combine generated <see cref="Mesh"/> objects.
            /// </summary>
            public MeshCombineOptions meshCombineOptions;
            /// <summary>
            /// At edit-time, which generated assets should be saved into the Assets folder.
            /// </summary>
            public AssetSavingOptions assetSavingOptions;
            /// <summary>
            /// (Overridden by custom IMaterialSource) Use this as the default material
            /// </summary>
            public Material defaultMaterial;
            /// <summary>
            /// At edit=time, path within Assets to save generated <see cref="Material"/>s to.
            /// </summary>
            public string materialPath;
            /// <summary>
            /// At edit=time, path within Assets to save generated <see cref="Mesh"/>es to.
            /// </summary>
            public string meshPath;
            /// <summary>
            /// Amount of detail used to tessellate patch curves into <see cref="Mesh"/>es.
            /// Higher values give smoother curves with exponentially more vertices.
            /// </summary>
            public int curveTessellationLevel;
            /// <summary>
            /// Callback that runs for each <see cref="Entity"/> after <see cref="Mesh"/>es are
            /// generated and the hierarchy is set up. Can be used to add custom post-processing
            /// to generated <see cref="GameObject"/>s using <see cref="Entity"/> information. Also
            /// contains a <see cref="List{T}"/> of <see cref="EntityInstance"/>s for each
            /// <see cref="Entity"/> the <see cref="Entity"/> targets.
            /// </summary>
            [Obsolete("Use entityCreatedCallback instead")]
            public Action<EntityInstance, List<EntityInstance>> legacy_entityCreatedCallback;

            /// <summary>
            /// Callback that runs for each <see cref="Entity"/> after <see cref="Mesh"/>es are
            /// generated and the hierarchy is set up. Can be used to add custom post-processing
            /// to generated <see cref="GameObject"/>s using <see cref="Entity"/> information. Also
            /// contains a <see cref="List{T}"/> of <see cref="EntityInstance"/>s for each
            /// <see cref="Entity"/> the <see cref="Entity"/> targets.
            /// </summary>
            public Action<EntityCreatedCallbackData> entityCreatedCallback;
            /// <summary>
            /// Amount to scale the BSP by.
            /// </summary>
            public float scaleFactor;
        }
    }
}
