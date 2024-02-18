// Bodged from https://github.com/radiatoryang/scopa
using Scopa.Formats.Id;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Scopa.Wad
{
    public static class WadLoader
    {
        static Color32[] palette = new Color32[256];

        public static IEnumerable<Texture2D> LoadWadTexturesFromPath(string path)
        {
            var wad = ParseWad(path);
            var textures = BuildAllWadTextures(wad);
            return textures;
        }

        public static WadFile ParseWad(string filePath)
        {
            using (var fStream = System.IO.File.OpenRead(filePath))
            {
                var newWad = new WadFile(fStream);
                newWad.Name = System.IO.Path.GetFileNameWithoutExtension(filePath);
                return newWad;
            }
        }

        public static bool TryFindWadTexture(WadFile wadFile, string textureName, out Texture2D wadTexture)
        {
            foreach (var entry in wadFile.Entries)
            {
                if ((entry.Type == LumpType.RawTexture || entry.Type == LumpType.MipTexture)
                    && entry.Name == textureName)
                {
                    wadTexture = BuildWadTexture(wadFile, entry);
                    return true;
                }
            }

            wadTexture = default;
            return false;
        }

        private static Texture2D BuildWadTexture(WadFile wadFile, Entry entry)
        {
            var texData = (wadFile.GetLump(entry) as MipTexture);

            // Half-Life GoldSrc textures use individualized 256 color palettes; Quake textures will have a reference to the hard-coded Quake palette
            var width = System.Convert.ToInt32(texData.Width);
            var height = System.Convert.ToInt32(texData.Height);

            for (int i = 0; i < 256; i++)
            {
                palette[i] = new Color32(texData.Palette[i * 3], texData.Palette[i * 3 + 1], texData.Palette[i * 3 + 2], 0xff);
            }

            // the last color is reserved for transparency
            if ((palette[255].r == QuakePalette.Data[255 * 3] && palette[255].g == QuakePalette.Data[255 * 3 + 1] && palette[255].b == QuakePalette.Data[255 * 3 + 2])
                || (palette[255].r == 0x00 && palette[255].g == 0x00 && palette[255].b == 0xff))
            {
                palette[255] = new Color32(0x00, 0x00, 0x00, 0x00);
            }

            var mipSize = texData.MipData[0].Length;
            var pixels = new Color32[mipSize];

            // for some reason, WAD texture bytes are flipped? have to unflip them for Unity
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int paletteIndex = texData.MipData[0][(height - 1 - y) * width + x];
                    pixels[y * width + x] = palette[paletteIndex];
                }
            }

            // we have all pixel color data now, so we can build the Texture2D
            var newTexture = new Texture2D(width, height);
            newTexture.name = texData.Name.ToLowerInvariant();
            newTexture.SetPixels32(pixels);
            newTexture.filterMode = FilterMode.Point;
            newTexture.anisoLevel = 1;
            newTexture.Apply();
            return newTexture;
        }

        public static List<Texture2D> BuildAllWadTextures(WadFile wad)
        {
            if (wad == null || wad.Entries == null || wad.Entries.Count == 0)
            {
                Debug.LogError("Couldn't parse WAD file " + wad.Name);
            }

            var textureList = new List<Texture2D>();

            foreach (var entry in wad.Entries)
            {
                if (entry.Type != LumpType.RawTexture && entry.Type != LumpType.MipTexture)
                    continue;

                // Debug.Log(entry.Name);
                // Debug.Log( "BITMAP: " + string.Join(", ", texData.MipData[0].Select( b => b.ToString() )) );
                // Debug.Log( "PALETTE: " + string.Join(", ", texData.Palette.Select( b => b.ToString() )) );

                var textureResult = BuildWadTexture(wad, entry);
                textureList.Add(textureResult);
            }

            return textureList;
        }
    }
}
