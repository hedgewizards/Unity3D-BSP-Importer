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
        public Texture2D LoadTexture(string textureName);
    }
}
