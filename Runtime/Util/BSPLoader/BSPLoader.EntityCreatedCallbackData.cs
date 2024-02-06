using System.Collections.Generic;

namespace BSPImporter
{

    public partial class BSPLoader
    {
        public struct EntityCreatedCallbackData
        {
            public IBSPLoaderContext Context;
            public EntityInstance Instance;

            public EntityCreatedCallbackData(IBSPLoaderContext context, EntityInstance createdEntity)
            {
                Context = context;
                Instance = createdEntity;
            }

            public IReadOnlyList<EntityInstance> Targets => Context.GetNamedEntities(Instance.entity["target"]);
        }
    }
}
