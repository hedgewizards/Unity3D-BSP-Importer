using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSPImporter.Textures
{
    public class TextureNotFoundEventArgs
    {
        public TextureNotFoundEventArgs(string textureName)
        {
            TextureName = textureName;
        }

        public string TextureName { get; private set; }
    }
}
