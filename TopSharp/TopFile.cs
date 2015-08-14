using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopSharp
{
    public class TopFile
    {
        char ch0;
        char ch1;
        char ch2;
        byte ver;

        UInt32 TripCount;
        List<Trip> Trips = new List<Trip>();
        private List<Trip> LoadTripList(BinaryReader byteFile, UInt32 listCount)
        {
            List<Trip> genericList = new List<Trip>();

            for (int i = 0; i < listCount; i++)
            {
                genericList.Add(new Trip(byteFile));
            }

            return genericList;
        }

        UInt32 ShotsCount;
        public List<Shot> Shots = new List<Shot>();
        private List<Shot> LoadShotList(BinaryReader byteFile, UInt32 listCount)
        {
            List<Shot> genericList = new List<Shot>();

            for (int i = 0; i < listCount; i++)
            {
                genericList.Add(new Shot(byteFile));
            }

            return genericList;
        }

        UInt32 RefCount;
        List<Reference> References = new List<Reference>();
        private List<Reference> LoadRefList(BinaryReader byteFile, UInt32 listCount)
        {
            List<Reference> genericList = new List<Reference>();

            for (int i = 0; i < listCount; i++)
            {
                genericList.Add(new Reference(byteFile));
            }

            return genericList;
        }

        Mapping OverView;
        Drawing OutLine;
        Drawing SideView;
        private bool FileLoaded = false;

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

            this.RefCount = byteFile.ReadUInt32();
            this.References = LoadRefList(byteFile, this.RefCount);

            this.OverView = new Mapping(byteFile);
            this.OutLine = new Drawing(byteFile);
            this.SideView = new Drawing(byteFile);
            this.FileLoaded = true;
        }

        public void SaveFile(string FileName)
        {
            using (BinaryWriter bw = new BinaryWriter(File.Open(FileName, FileMode.Create)))
            {
                bw.Write(ch0); bw.Write(ch1); bw.Write(ch2); bw.Write(ver);
                bw.Write(this.TripCount);
                foreach (Trip trip in this.Trips) trip.Write(bw);

                bw.Write(this.ShotsCount);
                foreach (Shot shot in this.Shots) shot.Write(bw);

                bw.Write(this.RefCount);
                foreach (Reference refe in this.References) refe.Write(bw);

                this.OverView.Write(bw);
                this.OutLine.Write(bw);
                this.SideView.Write(bw);
            }
        }

    }
}
