﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public List<Tuple<int, string, string>> ConnectingStation { get; set; }
        public int MultipleIndex { get; set; }
        public string FileName { get; set; }
        public Dictionary<string, string> StationMappings { get; set; }

        public TopFile(StreamReader b)
        {
            ConnectingStation = new List<Tuple<int, string, string>>();
            ch0 = 'T';
            ch1 = 'o';
            ch2 = 'p';

            ver = (byte)3;


            this.TripCount = 1;
            this.Trips = new List<Trip>() { new Trip(DateTime.Now, "", 0) };

            this.ShotsCount = LoadSurvexFile(b);
            //this.Shots = LoadShotList(b, this.ShotsCount);
            
            this.RefCount = 0;
            this.References = new List<Reference>();

            this.OverView = new Mapping();
            this.OutLine = new Drawing();
            this.SideView = new Drawing();
            this.FileLoaded = true;
        }

        private uint LoadSurvexFile(StreamReader b)
        {
            uint noShots = 0;
            int prefix = 600;
            int prefixAdd = 0;
            Dictionary<string, int> rPrefix = new Dictionary<string, int>();
            while (!b.EndOfStream)
            {
                string line = b.ReadLine();
                if (line.Trim().Length == 0) continue;

                List<string> splittedLine = Regex.Split(line, "[\\s]+").ToList();

                if (splittedLine[0].Contains(";") || splittedLine[0].Contains("*"))
                {
                    if (splittedLine[0].Contains("begin"))
                    {
                        rPrefix.Add(splittedLine[1], prefix);
                        int.TryParse(Regex.Match(line, "\\d+").Value, out prefixAdd);
                    }
                    else if (splittedLine[0].Contains("equate"))
                    {
                        string equate0 = splittedLine[1];
                        string equate1 = splittedLine[2];

                        Shot connectingShot = new Shot(equate0, equate1, prefix, rPrefix, prefixAdd);
                        this.Shots.Add(connectingShot);
                        noShots += 1;
                    }
                    else
                    {
                        //if (splittedLine[0].Contains("end")) prefix += 1;
                        continue;
                    }
                    
                }
                if(splittedLine.Count < 5){
                    continue;
                }
                Shot newShot = new Shot(splittedLine, prefix, prefixAdd);
                this.Shots.Add(newShot);

                noShots += 1;
            }
            return noShots;
        }


        public TopFile(BinaryReader byteFile)
        {
            this.ConnectingStation = new List<Tuple<int, string, string>>();
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
        public TopFile(List<Station> Stations, TopFile file = null)
        {
            ch0 = 'T';
            ch1 = 'o';
            ch2 = 'p';
            ver = 3;

            this.TripCount = 1;
            this.Trips = new List<Trip>() { new Trip() };

            this.ShotsCount = (uint)Stations.Count;
            this.Shots = new List<Shot>();
            foreach (Station station in Stations)
            {
                Shots.Add(new Shot(station));
            }

            this.RefCount = 0;
            this.References = new List<Reference>();

            if (file != null)
            {
                this.OverView = file.OverView;
                this.OutLine = file.OutLine;
                this.SideView = file.SideView;
            }
            else
            {
                this.OverView = new Mapping();
                this.OutLine = new Drawing();
                this.SideView = new Drawing();
            }
            this.FileLoaded = true;
        }

        public TopFile()
        {
            ch0 = 'T';
            ch1 = 'o';
            ch2 = 'p';

            ver = 3;

            this.TripCount = 0;
            this.Trips = new List<Trip>();

            this.ShotsCount = 0;
            this.Shots = new List<Shot>();

            this.RefCount = 0;
            this.References = new List<Reference>();

            this.OverView = new Mapping();
            this.OutLine = new Drawing();
            this.SideView = new Drawing();
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
