using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopSharp
{
    public class Shot
    {
        Id Fr;
        public string From
        {
            get
            {
                return Fr.Name;
            }
            set
            {
                this.Fr.Name = value;
            }
        }

        Id T;
        public string To
        {
            get
            {
                return T.Name;
            }
            set
            {
                this.T.Name = value;
            }
        }

        UInt32 Dist;
        public double Distance
        {
            get
            {
                return this.Dist * 0.001;
            }
            set
            {
                this.Dist = (UInt32)(Math.Round(value * 1000));
            }
        }

        UInt16 Azi;
        public double Azimuth
        {
            get
            {
                if (0 <= Azi && Azi <= 16384)
                {
                    return (Azi * 90) / (double)16384;
                }
                else if (16384 <= Azi && Azi <= 32768)
                {
                    return ((Azi * 180) / (double)32768);
                }
                else if (32768 <= Azi && Azi <= 49152)
                {
                    return ((Azi * 270) / (double)49152);
                }else{
                    return ((Azi * 360) / (double)65536);
                }
            }
            set
            {
                this.Azi = (UInt16)((value * 65536) / 360);
            }
        }

        UInt16 Incl;
        public double Inclination
        {
            get
            {
                if (0 <= Incl && Incl <= 16384)
                {
                    return (Incl * 90) / (double)16384;
                }
                else if (Incl >= 49152)
                {
                    int tmpIncl = 65536 - Incl;
                    return -(tmpIncl * 90) / (double)16384;
                }
                return 0.0;
            }
            set
            {
                if (value == 0) this.Incl = 0;
                else if (value > 0)
                {
                    this.Incl = (UInt16)((value * 16384) / 90);
                }
                else if (value < 0)
                {
                    double tmpVal = (((-value) * 16384) / 90);
                    this.Incl = (UInt16)(65536 - tmpVal);
                }
            }
        }

        public byte flags;
        public byte roll;
        public UInt16 tripIndex;
        public string Comment;

        public Shot(BinaryReader byteFile)
        {
            this.Fr = new Id(byteFile);
            this.T = new Id(byteFile);
            this.Dist = byteFile.ReadUInt32();
            this.Azi = byteFile.ReadUInt16();
            this.Incl = byteFile.ReadUInt16();
            this.flags = byteFile.ReadByte();

            this.roll = byteFile.ReadByte();
            this.tripIndex = byteFile.ReadUInt16();
            if (this.flags == 2)
            {
                this.Comment = byteFile.ReadString();
            }
        }

        public Shot(Station station)
        {
            this.Fr = new Id(station.From);
            this.T = new Id(station.To);
            this.Distance = station.Distance;
            this.Azimuth = station.Azimuth;
            this.Inclination = station.Inclination;
            this.flags = station.flags;

            this.roll = station.roll;
            this.tripIndex = station.tripIndex;
            if (this.flags == 2)
            {
                this.Comment = station.Comment;
            }
        }

        public void Write(BinaryWriter bw)
        {
            Fr.Write(bw); T.Write(bw); bw.Write(Dist); bw.Write(Azi); bw.Write(Incl); bw.Write(flags); bw.Write(roll); bw.Write(tripIndex);
            if (this.flags == 2)
                bw.Write(this.Comment);
        }
    }
}
