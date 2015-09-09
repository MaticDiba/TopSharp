using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopSharp
{
    class Drawing
    {
        Mapping mapping;
        List<PolygonElement> PolygonElements = new List<PolygonElement>();
        List<XSectionElement> XSectionElements = new List<XSectionElement>();
        public Drawing(BinaryReader byteFile)
        {
            this.mapping = new Mapping(byteFile);

            byte TypeOfElement = byteFile.ReadByte();
            while (TypeOfElement != 0) {
                if (TypeOfElement == 1)
                {
                    PolygonElements.Add(new PolygonElement(byteFile));
                }
                else if (TypeOfElement == 3)
                {
                    XSectionElements.Add(new XSectionElement(byteFile));
                }
                TypeOfElement = byteFile.ReadByte();
            }
        }

        public Drawing()
        {
            this.mapping = new Mapping();
            this.PolygonElements = new List<PolygonElement>();
            this.XSectionElements = new List<XSectionElement>();
        }

        public void Write(BinaryWriter bw)
        {
            mapping.Write(bw);

            foreach (PolygonElement el in PolygonElements) el.Write(bw);
            bw.Write(0);
            foreach (XSectionElement el in XSectionElements) el.Write(bw);
            bw.Write(0);
        }
    }

    
}
