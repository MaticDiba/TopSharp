using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopSharp;

namespace TopFileEditor
{
    public static class ImportExportHelper
    {

        internal static void ConnectMultipleTopFiles(List<TopFile> multipleTopFiles)
        {
            {
                List<string> checkedPairs = new List<string>();

                for (int i = 0; i < multipleTopFiles.Count; i++)
                {
                    TopFile srcFile = multipleTopFiles[i];
                    List<string> allSrcStationNames = srcFile.Shots.Select(s => new string[] { s.From, s.To }).SelectMany(item => item).Where(item => item != null).Distinct().ToList<string>();

                    for (int j = i; j < multipleTopFiles.Count; j++)
                    {
                        
                        if (j == i) continue;
                        TopFile destFile = multipleTopFiles[j];
                        List<string> allDestStationNames = destFile.Shots.Select(s => new string[] { s.From, s.To }).SelectMany(item => item).Where(item => item != null).Distinct().ToList<string>();
                        if (allDestStationNames.Any(dest => allSrcStationNames.Any(src => src == dest)))
                        {
                            List<string> stations = allDestStationNames.Where(dest => allSrcStationNames.Contains(dest)).ToList();
                            foreach(var station in stations)
                            {
                                //if(srcFile.Shots)
                                Tuple<int, string, string> newTuple = new Tuple<int, string, string>(destFile.MultipleIndex, station, station);
                                if (!srcFile.ConnectingStation.Contains(newTuple) && srcFile.MultipleIndex != i)
                                {
                                    srcFile.ConnectingStation.Add(newTuple);
                                }

                                Tuple<int, string, string> newTupleDest = new Tuple<int, string, string>(srcFile.MultipleIndex, station, station);
                                if (!destFile.ConnectingStation.Contains(newTupleDest) && destFile.MultipleIndex != i)
                                {
                                    destFile.ConnectingStation.Add(newTupleDest);
                                }
                            }
                            if(srcFile.ConnectingStation.Count > 5 || destFile.ConnectingStation.Count > 5)
                            {

                            }
                           
                            /*if (srcFile.ConnectingStation.Contains() srcFile.ConnectingStation = station;
                            if (destFile.ConnectingStation == null) destFile.ConnectingStation = station;*/
                            //break;
                        }
                        if (srcFile.ConnectingStation.Count == 0)
                        {

                        }

                    }
                }
                //CleanDoubleConnections(multipleTopFiles);
            }

        }

        private static void CleanDoubleConnections(List<TopFile> multipleTopFiles)
        {
            
            foreach(var file in multipleTopFiles)
            {
                RecursivelyRemoveConnectionFromDest(multipleTopFiles, file.MultipleIndex - 1);
            }
        }

        private static void RecursivelyRemoveConnectionFromDest(List<TopFile> files, int currentFile)
        {
            for(int i = 0; i < files[currentFile].ConnectingStation.Count; i++)
            {
                var connection = files[currentFile].ConnectingStation[i];
                if (files[connection.Item1-1].ConnectingStation.Count(conn => conn.Item2 == connection.Item2) > 0)
                {
                    files[currentFile].ConnectingStation.RemoveAt(i);
                }
            }
        }

        internal static List<TopFile> LoadMultipleFilesFromFolder(string path4MultipleFiles)
        {
            List<TopFile> MultipleTopFiles = new List<TopFile>();
            int pos = 1;
            foreach (var filename in Directory.GetFiles(path4MultipleFiles).Where(s => s.EndsWith(".top")))
            {
                using (BinaryReader b = new BinaryReader(File.Open(filename, FileMode.Open)))
                {

                    int length = (int)b.BaseStream.Length;
                    TopFile localTopFile = new TopFile(b);
                    localTopFile.MultipleIndex = pos;
                    localTopFile.FileName = Path.GetFileName(filename);
                    MultipleTopFiles.Add(localTopFile);
                    pos += 1;
                }
            }
            ImportExportHelper.ConnectMultipleTopFiles(MultipleTopFiles);

            return MultipleTopFiles;
        }

        internal static string ExportMultipleFiles2SurvexFile(List<TopFile> multipleTopFiles, string sFileName, string sXCoor, string sYCoor, string sAltitude, string sFirstPoint)
        {
            string filePrefix = sFileName.Substring(0, 2);

            StringBuilder sbSurvexFile = new StringBuilder();

            sbSurvexFile.AppendFormat("*begin {0}\r\n", sFileName);
            sbSurvexFile.AppendFormat("*data normal from to tape compass clino\r\n");

            sbSurvexFile.AppendFormat("*set decimal (,)\r\n");
            string[] firstPoint = sFirstPoint.Split(',');
            sbSurvexFile.AppendFormat("*fix {5}{0}.{1}    {2}  {3} {4}\r\n", firstPoint[0], firstPoint[1], sXCoor, sYCoor, sAltitude, filePrefix);
            sbSurvexFile.AppendFormat("*entrance {2}{0}.{1}\r\n", firstPoint[0], firstPoint[1], filePrefix);
            int i = 0;
            List<int> processedFiles = new List<int>();
            foreach (var file in multipleTopFiles)
            {
                sbSurvexFile.AppendFormat("; FileName: {0}\r\n", file.FileName);
                if (i > 0)
                {
                    foreach(var station in file.ConnectingStation)
                    {
                        if (!processedFiles.Contains(station.Item1))
                            continue;
                        if (station.Item1 != file.MultipleIndex)
                        {
                            //var tmp = multipleTopFiles[0].Shots.First(s => s.From == station.Item2 || s.To == station.Item2);
                            //var tmp2 = file.Shots.First(s => s.From == station.Item2 || s.To == station.Item2);
                            var connectionShot = file.Shots.Where(stat => (stat.From == station.Item2 || stat.To == station.Item2)
                                                                            && stat.From != null && stat.To != null).Select(
                                                                            stat => stat.From != station.Item2 ? stat.From : stat.To).Distinct().ToList();

                            var neki = file.Shots.Where(stat => (stat.From == station.Item2 || stat.To == station.Item2)
                                                                            && stat.From != null && stat.To != null);
                            if (connectionShot.Count > 1 || connectionShot.Count == 0)
                            { }
                            //file.Shots.ForEach(statio => { Console.Write("{0}\t->\t{1}\t:\t", statio.From, statio.To != null ? statio.To : "\t"); Console.WriteLine(); });
                            sbSurvexFile.AppendFormat("*equate {2}{0} 	{2}{1}\r\n", station.Item2.Replace(",", "."), station.Item3.Replace(",", "."), filePrefix);
                            //sbSurvexFile.AppendFormat("*equate r{0} 	r{1}\r\n", station.Item2.Replace(",", "."), connectionShot[1].Replace(",", "."));
                        }
                    }
                    
                }
                
                sbSurvexFile.AppendFormat("*begin {1}{0}\r\n", file.MultipleIndex, filePrefix);
                sbSurvexFile.AppendFormat("*alias station - ..\r\n");

                List<string> ProcessedFromToShots = new List<string>();
                foreach (var station in file.Shots)
                {
                    string fromToShot = string.Format("{0}-{1}", station.From, station.To);
                    if (!ProcessedFromToShots.Contains(fromToShot) && station.From != null && station.To != null)
                    {
                        string from = station.From.Substring(station.From.IndexOf(",") + 1);
                        string to = station.To.Substring(station.To.IndexOf(",") + 1);
                        string dist = string.Format("{0:0.00}", file.Shots.Where(s => s.From == station.From && s.To == station.To).Select(s => s.Distance).Average());
                        string azi = string.Format("{0:0.00}", file.Shots.Where(s => s.From == station.From && s.To == station.To).Select(s => s.Azimuth).Average()); 
                        string incl = string.Format("{0:0.00}", file.Shots.Where(s => s.From == station.From && s.To == station.To).Select(s => s.Inclination).Average()); 
                        if (station.From != null && station.To != null)
                            sbSurvexFile.AppendFormat("{0}\t{1}\t{2}\t{3}\t{4}\r\n", from, to, dist, azi, incl);
                        ProcessedFromToShots.Add(fromToShot);
                    }else if (station.From == null || station.To == null)
                    {
                        if (station.From == null) continue;
                        string from = station.From.Substring(station.From.IndexOf(",") + 1);

                        string dist = string.Format("{0:0.00}", station.Distance);
                        string azi = string.Format("{0:0.00}", station.Azimuth);
                        string incl = string.Format("{0:0.00}", station.Inclination);

                        sbSurvexFile.AppendFormat("{0}\t{1}\t{2}\t{3}\t{4}\r\n", from, "-", dist, azi, incl);
                    }
                }
                processedFiles.Add(file.MultipleIndex);
                sbSurvexFile.AppendFormat("*end {1}{0}\r\n", file.MultipleIndex, filePrefix);
                i += 1;
            }

            sbSurvexFile.AppendFormat("*end {0}\r\n", sFileName);
            return sbSurvexFile.ToString();
        }

        internal static List<TopFile> RenumberStationsForFiles(List<TopFile> multipleTopFiles)
        {
            Dictionary<string, string> mappingOfOldValue = new Dictionary<string, string>();
            foreach(var file in multipleTopFiles)
            {
                mappingOfOldValue = new Dictionary<string, string>();
                int fileIdx = file.MultipleIndex;
                int i = 0;
                foreach(var station in file.Shots)
                {
                    //Console.Write("{0}\t->\t{1}\t:\t", station.From, station.To != null ? station.To : "\t");
                    //station
                    string newValueFrom = CreateNewStation(station.From, fileIdx, i);

                    if (mappingOfOldValue.ContainsKey(station.From))
                        newValueFrom = mappingOfOldValue[station.From];
                    else
                    {
                        mappingOfOldValue.Add(station.From, newValueFrom);
                        i += 1;
                    }
                    station.From = newValueFrom;
                    if (station.To != null)
                    {
                        string newValueTo = CreateNewStation(station.To, fileIdx, i);
                        if (mappingOfOldValue.ContainsKey(station.To))
                            newValueTo = mappingOfOldValue[station.To];
                        else
                        {
                            mappingOfOldValue.Add(station.To, newValueTo);
                            i += 1;
                        }
                        station.To = newValueTo;
                    }
                    // Console.Write("{0}\t->\t{1}", station.From, station.To != null ? station.To : "\t");
                    // Console.WriteLine();
                }
                for(int k = 0; k< multipleTopFiles.Count; k++)
                {
                    var fileSecond = multipleTopFiles[k];

                    if (k == file.MultipleIndex - 1) continue;

                    if(fileSecond.ConnectingStation.Count(stat => stat.Item1 == file.MultipleIndex) > 0)
                    {
                        for (int j = 0; j < fileSecond.ConnectingStation.Count; j++)
                        {
                            var connections = fileSecond.ConnectingStation[j];
                            if (connections.Item1 != file.MultipleIndex) continue;
                            string newValueFrom = mappingOfOldValue[connections.Item2];
                            fileSecond.ConnectingStation[j] = new Tuple<int, string, string>(connections.Item1, newValueFrom, connections.Item2);
                        }
                    }
                    
                }
                
                for(int j = 0; j < file.ConnectingStation.Count;j++)
                {
                    var connections = file.ConnectingStation[j];
                    string newValueFrom = mappingOfOldValue[connections.Item3];
                    file.ConnectingStation[j] = new Tuple<int, string, string>(connections.Item1, connections.Item2, newValueFrom);
                }
                file.StationMappings = mappingOfOldValue;
            }
            return multipleTopFiles;
        }

        private static string CreateNewStation(string oldStationValue, int fileIdx, int i)
        {
            return string.Format("{0},{1}", fileIdx, i);
        }
    }
}
