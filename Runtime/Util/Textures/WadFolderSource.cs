using LibBSP;
using Scopa.Wad;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BSPImporter.Textures
{
    public class WadFolderSource : ITextureSource
    {
        List<WadFile> Wads;

        public WadFolderSource(string wadFolderPath)
        {
            Wads = new List<WadFile>();
            string[] paths = Directory.GetFiles(wadFolderPath, "*.wad", SearchOption.AllDirectories);
            foreach(var path in paths)
            {
                var newWad = WadLoader.ParseWad(path);
                Wads.Add(newWad);
            }
        }

        public Texture2D LoadTexture(string textureName)
        {
            foreach(var wad in Wads)
            {
                if (WadLoader.TryFindWadTexture(wad, textureName, out Texture2D texture))
                {
                    return texture;
                }
            }

            Debug.LogWarning("Texture " + textureName + " could not be found. Are you missing a .wad?");
            return null;
        }
    }
}
