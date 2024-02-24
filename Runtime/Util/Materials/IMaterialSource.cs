using BSPImporter.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BSPImporter.Materials
{
    public interface IMaterialSource
    {
        public Material BuildMaterial(WadTextureData textureData);
    }
}
