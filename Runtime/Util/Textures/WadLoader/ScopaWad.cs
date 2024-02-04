using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Scopa.Formats;
using Scopa.Formats.Id;
using UnityEngine;

#if UNITY_EDITOR
using System.Diagnostics;
using UnityEditor;
using Debug = UnityEngine.Debug;
#endif

namespace Scopa.Wad {
    /// <summary> main class for WAD import and export </summary>
    public static class ScopaWad {

        // static buffers for all reading and writing operations, to try to reduce GC
        static Color32[] palette = new Color32[256];
        public static Texture2D resizedTexture { get; private set; }

        #region WAD Reading

        public static WadFile ParseWad(string fileName)
        {
            using (var fStream = System.IO.File.OpenRead(fileName))
            {
                var newWad = new WadFile(fStream);
                newWad.Name = System.IO.Path.GetFileNameWithoutExtension(fileName);
                return newWad;
            }
        }

        public static List<Texture2D> BuildWadTextures(WadFile wad, ScopaWadConfig config) {
            if ( wad == null || wad.Entries == null || wad.Entries.Count == 0) {
                Debug.LogError("Couldn't parse WAD file " + wad.Name);
            }

            var textureList = new List<Texture2D>();

            foreach ( var entry in wad.Entries ) {
                if ( entry.Type != LumpType.RawTexture && entry.Type != LumpType.MipTexture )
                    continue;

                var texData = (wad.GetLump(entry) as MipTexture);
                // Debug.Log(entry.Name);
                // Debug.Log( "BITMAP: " + string.Join(", ", texData.MipData[0].Select( b => b.ToString() )) );
                // Debug.Log( "PALETTE: " + string.Join(", ", texData.Palette.Select( b => b.ToString() )) );

                // Half-Life GoldSrc textures use individualized 256 color palettes; Quake textures will have a reference to the hard-coded Quake palette
                var width = System.Convert.ToInt32(texData.Width);
                var height = System.Convert.ToInt32(texData.Height);

                for (int i=0; i<256; i++) {
                    palette[i] = new Color32( texData.Palette[i*3], texData.Palette[i*3+1], texData.Palette[i*3+2], 0xff );
                }

                // the last color is reserved for transparency
                var paletteHasTransparency = false;
                if ( (palette[255].r == QuakePalette.Data[255*3] && palette[255].g == QuakePalette.Data[255*3+1] && palette[255].b == QuakePalette.Data[255*3+2])
                    || (palette[255].r == 0x00 && palette[255].g == 0x00 && palette[255].b == 0xff) ) {
                    paletteHasTransparency = true;
                    palette[255] = new Color32(0x00, 0x00, 0x00, 0x00);
                }
                
                var mipSize = texData.MipData[0].Length;
                var pixels = new Color32[mipSize];
                var usesTransparency = false;

                // for some reason, WAD texture bytes are flipped? have to unflip them for Unity
                for( int y=0; y < height; y++) {
                    for (int x=0; x < width; x++) {
                        int paletteIndex = texData.MipData[0][(height-1-y)*width + x];
                        pixels[y*width+x] = palette[paletteIndex];
                        if ( !usesTransparency && paletteHasTransparency && paletteIndex == 255) {
                            usesTransparency = true;
                        }
                    }
                }

                // we have all pixel color data now, so we can build the Texture2D
                var newTexture = new Texture2D( width, height, usesTransparency ? TextureFormat.RGBA32 : TextureFormat.RGB24, true, config.linearColorspace);
                newTexture.name = texData.Name.ToLowerInvariant();
                newTexture.SetPixels32(pixels);
                newTexture.alphaIsTransparency = usesTransparency;
                newTexture.filterMode = config.filterMode;
                newTexture.anisoLevel = config.anisoLevel;
                newTexture.Apply();
                if ( config.compressTextures ) {
                    newTexture.Compress(false);
                }
                textureList.Add( newTexture );
                
            }

            return textureList;
        }

        public static Material BuildMaterialForTexture( Texture2D texture, ScopaWadConfig config ) {
            var material = texture.alphaIsTransparency ? GenerateMaterialAlpha(config) : GenerateMaterialOpaque(config);
            material.name = texture.name;
            material.mainTexture = texture;

            return material;
        }

        public static Material GenerateMaterialOpaque( ScopaWadConfig config ) {
            // TODO: URP, HDRP
            Material material;

            if ( config.opaqueTemplate != null ) {
                material = new Material(config.opaqueTemplate);
            }
            else {
                material = new Material(Shader.Find("Standard"));
                material.SetFloat("_Glossiness", 0.1f);
            }

            return material;
        }

        public static Material GenerateMaterialAlpha( ScopaWadConfig config ) {
			// TODO: URP, HDRP
			Material material;

			if ( config.alphaTemplate != null ) {
				material = new Material(config.alphaTemplate);
			}
			else {
				material = new Material(Shader.Find("Standard"));
				material.SetFloat("_Glossiness", 0.1f);
				material.SetFloat("_Mode", 1);
				material.EnableKeyword("_ALPHATEST_ON");
				material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
				material.renderQueue = 2450;
			}

            return material;
        }
        #endregion
    }

}
