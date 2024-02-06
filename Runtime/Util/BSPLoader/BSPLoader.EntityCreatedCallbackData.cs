using System.Collections.Generic;

namespace BSPImporter
{

    public partial class BSPLoader
    {
        public struct EntityCreatedCallbackData
        {
            public IBSPLoaderContext Context;
            public EntityInstance CreatedEntity;

            public EntityCreatedCallbackData(IBSPLoaderContext context, EntityInstance createdEntity)
            {
                Context = context;
                CreatedEntity = createdEntity;
            }

            public IReadOnlyList<EntityInstance> Targets => Context.GetNamedEntities(CreatedEntity.entity["target"]);
        }
    }
}
