using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BSPImporter.Textures
{
    public struct WadTextureData
    {
        public string Name;
        public Texture2D Texture;
        public Dictionary<string, string> Metadata;

        public string ShaderName => Metadata?.GetValueOrDefault("shader") ?? null;
    }
}
