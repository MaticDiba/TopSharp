using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopSharp
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

        public Mapping()
        {
            this.orgin = new Point();
            this.scale = 500;
        }
        public void Write(BinaryWriter bw)
        {
            orgin.Write(bw);

            bw.Write(scale);
        }
    }
}
