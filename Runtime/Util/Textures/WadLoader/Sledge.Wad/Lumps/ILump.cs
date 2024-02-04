using System.IO;

namespace Scopa.Wad.Lumps
{
    public interface ILump
    {
        LumpType Type { get; }
        int Write(BinaryWriter bw);
    }
}
