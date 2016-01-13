using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public Shot(List<string> splittedLine, int prefix, int prefixAdd)
        {
            Regex matchNumber = new Regex("\\d+");
            string iFrom = string.Format("{0},{1}", prefix + prefixAdd, matchNumber.Match(splittedLine[0]).Value);
            string iTo = string.Format("{0},{1}", prefix + prefixAdd, matchNumber.Match(splittedLine[1]).Value);

            this.Fr = new Id(iFrom);
            this.T = new Id(iTo);
            this.Distance = Math.Round(double.Parse(splittedLine[2]), 3);
            this.Azimuth = Math.Round(double.Parse(splittedLine[3]), 3);
            this.Inclination = double.Parse(splittedLine[4]);
            //TODO: A je prou da je 0?
            this.flags = 0;
            //TODO: A je to prou da je 0?
            this.roll = 0;
            this.tripIndex = 0;
            if (this.flags == 2)
            {
                this.Comment = "";
            }
        }

        public Shot(string equate0, string equate1, int prefix, Dictionary<string, int> rPrefix, int prefixAdd)
        {
            // TODO: Complete member initialization
            string[] prefixOne = equate0.Trim().Split('.');
            string[] prefixTwo = equate1.Trim().Split('.');
            int pref1, pref2;
            int.TryParse(Regex.Match(prefixOne[0], "\\d+").Value, out pref1);
            int.TryParse(Regex.Match(prefixTwo[0], "\\d+").Value, out pref2);

            int post1, post2;
            int.TryParse(Regex.Match(prefixOne[1], "\\d+").Value, out post1);
            int.TryParse(Regex.Match(prefixTwo[1], "\\d+").Value, out post2);
            string iFrom = string.Format("{0},{1}", prefix + pref1, post1);
            string iTo = string.Format("{0},{1}", prefix + pref2, post2);

            this.Fr = new Id(iFrom);
            this.T = new Id(iTo);
            this.Distance = 0.0;
            this.Azimuth = 0.0;
            this.Inclination = 0.0;
            //TODO: A je prou da je 0?
            this.flags = 0;
            //TODO: A je to prou da je 0?
            this.roll = 0;
            this.tripIndex = 0;
            if (this.flags == 2)
            {
                this.Comment = "";
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
