using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopOpener
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
    }
}
