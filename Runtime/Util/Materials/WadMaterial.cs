using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BSPImporter.Materials
{
    public struct WadMaterial
    {
        public string Name;
        public Material Material;
        public Dictionary<string, string> Metadata;
    }
}
