using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopOpener
{
    class Program
    {
        static void Main(string[] args)
        {
            using (BinaryReader b = new BinaryReader(File.Open("file.top", FileMode.Open)))
            {
                int pos = 0;
                int length = (int)b.BaseStream.Length;

                char ch0 = b.ReadChar();
                char ch1 = b.ReadChar();
                char ch2 = b.ReadChar();

                byte verr = b.ReadByte();
                /*Int32 tripCount =  b.ReadInt32();
                for (int i = 0; i < tripCount; i++)
                {
                    Int64 time = b.ReadInt64();
                    string comment = b.ReadString();
                    Int16 declination = b.ReadInt16();
                }
                pos += sizeof(char) * 3;*/
                TopFile file = new TopFile(b);
                
            }

            byte[] fajl = File.ReadAllBytes("file.top");

/*          char ch0 = Convert.ToChar(fajl[0]);
            char ch1 = Convert.ToChar(fajl[1]);
            char ch2 = Convert.ToChar(fajl[2]);
            byte ver = fajl[3];*/
            //TopFile file = new TopFile(fajl, 4);
        }
    }
}
