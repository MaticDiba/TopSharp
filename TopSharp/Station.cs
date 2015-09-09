using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopSharp
{
    public class Station
    {
        public string From { get; set; }
        public string To { get; set; }
        private double dist;
        public double Distance
        {
            get
            {
                return this.dist;
            }
            set
            {
                if (value.ToString().Contains(","))
                {

                }
                this.dist = value;
            }
        }
        public double Azimuth { get; set; }
        public double Inclination { get; set; }
        public string Comment { get; set; }
        public int Idx { get; set; }

        public byte flags;
        public byte roll;
        public UInt16 tripIndex;

        public Station(Shot shot)
        {
            this.From = shot.From;
            this.To = shot.To;
            this.Distance = shot.Distance;
            this.Azimuth = shot.Azimuth;
            this.Inclination = shot.Inclination;
            this.Comment = shot.Comment;
            this.flags = shot.flags;
            this.roll = shot.roll;
            this.tripIndex = shot.tripIndex;
        }

        public Station() { }

        public Station Clone(int id = -1)
        {
            Station station = new Station() { Azimuth = this.Azimuth, Comment = this.Comment, Distance = this.Distance, From = this.From, Inclination = this.Inclination, To = this.To };
            if (id >= 0)
                station.Idx = id;
            return station;
        }

    }
}
