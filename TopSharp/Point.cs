using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopSharp
{
    class Point
    {
        UInt32 x;
        UInt32 y;

        public Point(BinaryReader byteFile)
        {
            this.x = byteFile.ReadUInt32();
            this.y = byteFile.ReadUInt32(); 
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(x); bw.Write(y);
        }
    }
}
