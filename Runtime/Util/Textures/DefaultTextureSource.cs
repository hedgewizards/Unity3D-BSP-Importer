
using System.IO;
using UnityEditor;
using UnityEngine;
using static BSPImporter.BSPLoader;

namespace BSPImporter.Textures
{
    public class DefaultTextureSource : ITextureSource
    {
        public string TexturePath;
        private bool UseAssetDatabase;

        public DefaultTextureSource(string texturePath, bool useAssetDatabase)
        {
            TexturePath = texturePath;
            UseAssetDatabase = useAssetDatabase;

#if UNITY_EDITOR
            if (texturePath.StartsWith(Application.dataPath))
            {
                texturePath = "Assets/" + texturePath.Substring(Application.dataPath.Length + 1);
                UseAssetDatabase = true;
            }
#endif
        }

        public Texture2D LoadTexture(string textureName)
        {
            string texturePath;
            if (TexturePath.Contains(":"))
            {
                texturePath = Path.Combine(TexturePath, textureName).Replace('\\', '/');
            }
            else
            {
#if UNITY_EDITOR
                texturePath = Path.Combine(Path.Combine("Assets", TexturePath), textureName).Replace('\\', '/');
                UseAssetDatabase = true;
#else
                texturePath = Path.Combine(TexturePath, textureName).Replace('\\', '/');
#endif
            }


            Texture2D texture = null;
            try
            {
                if (File.Exists(texturePath + ".png"))
                {
                    texture = LoadTextureAtPath(texturePath + ".png");
                }
                else if (File.Exists(texturePath + ".jpg"))
                {
                    texture = LoadTextureAtPath(texturePath + ".jpg");
                }
                else if (File.Exists(texturePath + ".tga"))
                {
                    texture = LoadTextureAtPath(texturePath + ".tga");
                }
                else if (File.Exists(texturePath + ".ftx"))
                {
                    texture = LoadTextureAtPath(texturePath + ".ftx");
                }
            }
            catch
            {
            }
            if (texture == null)
            {
                Debug.LogWarning("Texture " + textureName + " could not be loaded (does the file exist?)");
            }

            return texture;
        }

        /// <summary>
        /// Loads the <see cref="Texture2D"/> at <paramref name="texturePath"/> and returns it.
        /// </summary>
        /// <param name="texturePath">
        /// The path to the <see cref="Texture2D"/>. If within Assets, it will use the texture
        /// asset rather than loading it directly from the HDD.
        /// </param>
        /// <returns>The loaded <see cref="Texture2D"/>.</returns>
        private Texture2D LoadTextureAtPath(string texturePath)
        {
#if UNITY_EDITOR
            if (UseAssetDatabase)
            {
                return AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D)) as Texture2D;
            }
            else
#endif
            {
                if (texturePath.EndsWith(".tga"))
                {
                    return Paloma.TargaImage.LoadTargaImage(texturePath);
                }
                else if (texturePath.EndsWith(".ftx"))
                {
                    return FTXLoader.LoadFTX(texturePath);
                }
                else
                {
                    Texture2D texture = new Texture2D(0, 0);
                    texture.LoadImage(File.ReadAllBytes(texturePath));
                    return texture;
                }
            }
        }
    }
}
