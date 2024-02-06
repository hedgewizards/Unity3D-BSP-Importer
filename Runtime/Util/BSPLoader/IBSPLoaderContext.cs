using System.Collections.Generic;

namespace BSPImporter
{
    public interface  IBSPLoaderContext
    {
        public IReadOnlyList<BSPLoader.EntityInstance> GetNamedEntities(string name);
    }
}
