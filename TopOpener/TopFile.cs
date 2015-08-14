using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopOpener
{
    public class TopFile
    {
        char ch0;
        char ch1;
        char ch2;
        byte ver;

        UInt32 TripCount;
        List<Trip> Trips = new List<Trip>();
        UInt32 ShotsCount;
        List<Shot> Shots = new List<Shot>();
        UInt32 RefCount;
        List<Reference> References = new List<Reference>();
        Mapping OverView;
        Drawing OutLine;
        Drawing SideView;
         
        public TopFile(BinaryReader byteFile)
        {
            ch0 = byteFile.ReadChar();
            ch1 = byteFile.ReadChar();
            ch2 = byteFile.ReadChar();

            ver = byteFile.ReadByte();

            this.TripCount = byteFile.ReadUInt32();
            this.Trips = LoadTripList(byteFile, this.TripCount);

            this.ShotsCount = byteFile.ReadUInt32();
            this.Shots = LoadShotList(byteFile, this.ShotsCount);
            var neki = Shots[38].From.Name;
            var neki2 = Shots[38].To.Name;
            this.RefCount = byteFile.ReadUInt32();
            this.References = LoadRefList(byteFile, this.RefCount);

            this.OverView = new Mapping(byteFile);
            this.OutLine = new Drawing(byteFile);
            this.SideView = new Drawing(byteFile);
        }


        private List<Trip> LoadTripList(BinaryReader byteFile, UInt32 listCount)
        {
            List<Trip> genericList = new List<Trip>();

            for (int i = 0; i < listCount; i++)
            {
                genericList.Add(new Trip(byteFile));
            }

            return genericList;
        }
        private List<Shot> LoadShotList(BinaryReader byteFile, UInt32 listCount)
        {
            List<Shot> genericList = new List<Shot>();

            for (int i = 0; i < listCount; i++)
            {
                genericList.Add(new Shot(byteFile));
            }

            return genericList;
        }
        private List<Reference> LoadRefList(BinaryReader byteFile, UInt32 listCount)
        {
            List<Reference> genericList = new List<Reference>();

            for (int i = 0; i < listCount; i++)
            {
                genericList.Add(new Reference(byteFile));
            }

            return genericList;
        }
    }
}
