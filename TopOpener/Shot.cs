using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopOpener
{
    public class Shot
    {
        public Id From;
        public Id To;
        UInt32 Distance;
        UInt16 Azimuth;
        UInt16 Inclination;
        byte flags;
        byte roll;
        UInt16 tripIndex;
        string Comment;

        public Shot(BinaryReader byteFile)
        {
            this.From = new Id(byteFile);
            this.To = new Id(byteFile);
            this.Distance = byteFile.ReadUInt32();
            this.Azimuth = byteFile.ReadUInt16();
            this.Inclination = byteFile.ReadUInt16();
            this.flags = byteFile.ReadByte();

            this.roll = byteFile.ReadByte();
            this.tripIndex = byteFile.ReadUInt16();
            if (this.flags == 2)
            {
                this.Comment = byteFile.ReadString();
            }
        }
    }
}
