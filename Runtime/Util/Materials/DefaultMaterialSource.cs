using BSPImporter.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BSPImporter.Materials
{
    public class DefaultMaterialSource : IMaterialSource
    {
        private Material DefaultMaterial;

        public DefaultMaterialSource(Material defaultMaterial)
        {
            DefaultMaterial = defaultMaterial;
        }

        public Material BuildMaterial(WadTextureData textureData)
        {
            var material = new Material(DefaultMaterial);
            material.mainTexture = textureData.Texture;
            material.name = textureData.Name;

            return material;
        }
    }
}
