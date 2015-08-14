using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopOpener
{
    class Mapping
    {
        Point orgin;
        UInt32 scale;
        public Mapping(BinaryReader byteFile)
        {
            this.orgin = new Point(byteFile);
            this.scale = byteFile.ReadUInt32();
        }
    }
}
