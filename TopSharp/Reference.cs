using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopSharp
{
    class Reference
    {
        Id Station;
        UInt64 East;
        UInt64 North;
        UInt32 Altitude;
        string Comment;

        public Reference(BinaryReader byteFile)
        {
            this.Station = new Id(byteFile);
            this.East = byteFile.ReadUInt64();
            this.North = byteFile.ReadUInt64();
            this.Altitude = byteFile.ReadUInt32();
            this.Comment = byteFile.ReadString();
        }

        public void Write(BinaryWriter bw)
        {
            Station.Write(bw); bw.Write(East); bw.Write(North); bw.Write(Altitude); bw.Write(Comment);
            
        }
    }
}
