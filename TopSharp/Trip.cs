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

        public Trip()
        {
            this.TimeDT = DateTime.Now;
            this.Time = this.TimeDT.Ticks;
            this.Comment = "";
            this.declination = Int16.MinValue;
        }

        public Trip(DateTime p1, string p2, Int16 p3)
        {
            // TODO: Complete member initialization
            this.Time = p1.Ticks;
            this.TimeDT = p1;
            this.Comment = p2;
            this.declination = p3;
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(Time); bw.Write(Comment); bw.Write(declination);
        }
    }
}
