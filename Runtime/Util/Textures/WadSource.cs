using BSPImporter.Textures.WadMetaFiles;
using LibBSP;
using Scopa.Wad;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace BSPImporter.Textures
{
    public class WadSource : ITextureSource
    {
        List<WadFilePair> Wads;

        public WadSource()
        {
            Wads = new List<WadFilePair>();
        }

        public WadSource AddWadFile(string wadPath, string metaWadPath)
        {
            WadFile file = WadLoader.ParseWad(wadPath);
            WadMetaFile metaFile = null;

            if (metaWadPath != null)
            {
                _ = WadMetaFileLoader.TryParse(metaWadPath, out metaFile);
            }

            Wads.Add(new WadFilePair()
            {
                wadPath = wadPath,
                File = file,
                MetaFile = metaFile
            });
            return this;
        }

        public WadSource AddWadFolder(string wadFolderPath)
        {
            Regex metaWadPathRegex = new Regex($"(.*)\\.wad$", RegexOptions.Compiled);

            Directory.CreateDirectory(wadFolderPath);
            string[] paths = Directory.GetFiles(wadFolderPath, "*.wad", SearchOption.AllDirectories);
            foreach (var path in paths)
            {
                var match = metaWadPathRegex.Match(path);
                if (!match.Success)
                {
                    throw new FormatException("Failed to deconstruct wadFile path");
                }

                string metaWadPath = $"{match.Groups[1].Value}.metawad";
                AddWadFile(path, metaWadPath);
            }
            return this;
        }

        public WadTextureData? LoadTexture(string textureName)
        {
            foreach(var wad in Wads)
            {
                if (WadLoader.TryFindWadTexture(wad.File, textureName, out Texture2D texture))
                {
                    if (wad.MetaFile == null || !wad.MetaFile.MetaDatas.TryGetValue(textureName, out Dictionary<string,string> metadata))
                    {
                        metadata = new Dictionary<string,string>();
                    }

                    return new WadTextureData()
                    {
                        Name = textureName,
                        Texture = texture,
                        Metadata = metadata
                    };
                }
            }

            Debug.LogWarning("Texture " + textureName + " could not be found. Are you missing a .wad?");
            return null;
        }

        private struct WadFilePair
        {
            public string wadPath;
            public WadFile File;
            public WadMetaFile MetaFile;
        }
    }
}
