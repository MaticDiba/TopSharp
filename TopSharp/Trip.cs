using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopSharp
{
    class Trip
    {
        Int64 Time;
        DateTime TimeDT;
        string Comment;
        Int16 declination;

        public Trip(BinaryReader byteFile)
        {
            this.Time = byteFile.ReadInt64();
            this.TimeDT = new DateTime(this.Time);
            this.Comment = byteFile.ReadString();
            this.declination = byteFile.ReadInt16();
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(Time); bw.Write(Comment); bw.Write(declination);
        }
    }
}
