using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopSharp
{
    class PolygonElement
    {
        Byte id = 1;
        UInt32 pointCount = 0;
        List<Point> points;
        Byte color;

        public PolygonElement(BinaryReader byteFile)
        {
            //this.id = byteFile.ReadByte();
            this.pointCount = byteFile.ReadUInt32();
            this.points = ReadAllPoints(byteFile);
            this.color = byteFile.ReadByte();
        }

        private List<Point> ReadAllPoints(BinaryReader byteFile)
        {
            List<Point> tmppoints = new List<Point>();
            for (int i = 0; i < this.pointCount; i++)
            {
                tmppoints.Add(new Point(byteFile));
            }
            return tmppoints;
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(id); bw.Write(pointCount);

            foreach (Point point in points) point.Write(bw);
            bw.Write(color);
        }
    }
}
