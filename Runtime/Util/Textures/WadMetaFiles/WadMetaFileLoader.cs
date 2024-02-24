using Scopa.Wad;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BSPImporter.Textures.WadMetaFiles
{
    public static class WadMetaFileLoader
    {
        public static bool TryParse(string filePath, out WadMetaFile file)
        {
            IEnumerable<string> lines;
            try
            {
                lines = File.ReadLines(filePath);
            }
            catch(FileNotFoundException)
            {
                file = null;
                return false;
            }

            file = new WadMetaFile();
            file.MetaDatas = new Dictionary<string, Dictionary<string, string>>();
            Regex commentPattern = new Regex("^\\s*#", RegexOptions.Compiled);

            // (texturename).(property) = (value)
            Regex linePattern = new Regex("(\\w+)\\.(\\w+)\\s*=\\s*(.+)");

            int lineNumber = 1;
            foreach (var line in lines)
            {
                lineNumber++;
                if (string.IsNullOrWhiteSpace(line))
                {
                    // empty line skip!!!
                    continue;
                }

                Match match = commentPattern.Match(line);
                if (match.Success)
                {
                    // this is a comment skip!!!
                    continue;
                }

                match = linePattern.Match(line);
                if (match.Success)
                {
                    // add this entry to our metadatas.
                    string propertyName = match.Groups[1].Value.ToLowerInvariant();

                    if (!file.MetaDatas.TryGetValue(propertyName, out var metadata))
                    {
                        metadata = new Dictionary<string, string>();
                        file.MetaDatas.Add(propertyName, metadata);
                    }

                    metadata.Add(match.Groups[2].Value.ToLowerInvariant(), match.Groups[3].Value.ToLowerInvariant());
                    continue;
                }

                throw new FormatException($"Failed to parse MetaFile line {lineNumber} \'{line}\'");
            }

            return true;
        }
    }
}
