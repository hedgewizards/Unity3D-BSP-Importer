using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BSPImporter.Textures
{
    public interface ITextureSource
    {
        public WadTextureData? LoadTexture(string textureName);
    }
}
