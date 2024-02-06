using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnityEngine.GraphicsBuffer;

namespace BSPImporter
{
    public partial class BSPLoader : IBSPLoaderContext
    {
        public IReadOnlyList<EntityInstance> GetNamedEntities(string target)
        {
            if (!string.IsNullOrEmpty(target) && namedEntities.ContainsKey(target))
            {
                return namedEntities[target];
            }
            else
            {
                return new List<EntityInstance>(0);
            }
        }
    }
}
