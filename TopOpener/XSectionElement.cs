using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopOpener
{
    class XSectionElement
    {
        Byte id = 3;
        Point pos;
        Id station;
        UInt32 direction;

        public XSectionElement(BinaryReader byteFile)
        {
            //this.id = byteFile.ReadByte();
            this.pos = new Point(byteFile);
            this.station = new Id(byteFile);
            this.direction = byteFile.ReadUInt32();
        }
    }
}
