using UnityEngine;

#if UNITY_EDITOR
#endif
using LibBSP;

namespace BSPImporter
{

    public partial class BSPLoader
    {
        /// <summary>
        /// Struct linking a generated <see cref="GameObject"/> with the <see cref="Entity"/> used to create it.
        /// </summary>
        public struct EntityInstance
        {
            /// <summary>
            /// The <see cref="Entity"/> used to generate <see cref="gameObject"/>.
            /// </summary>
            public Entity entity;
            /// <summary>
            /// The <see cref="GameObject"/> generated from <see cref="entity"/>.
            /// </summary>
            public GameObject gameObject;
        }
    }
}
