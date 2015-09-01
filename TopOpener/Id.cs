using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TopOpener
{
    public class Id
    {
        UInt32 value;
        public double Name {
            get{
                string hexValue = value.ToString("X");
                if (value == 2147483648)
                    return Convert.ToDouble(value);
                else if (hexValue.Length <= 4)
                {
                    return Convert.ToDouble(value);
                }
                else if (hexValue.Length > 4)
                {
                    int firsPart = int.Parse(hexValue.Substring(0, hexValue.Length - 4), System.Globalization.NumberStyles.HexNumber);
                    int secPart = int.Parse(hexValue.Substring(hexValue.Length - 4), System.Globalization.NumberStyles.HexNumber);
                    return Convert.ToDouble(string.Format("{0},{1}", firsPart, secPart));
                }
                return this.value;
            }
        }
        public Id(BinaryReader byteFile)
        {
            //byte smtn = byteFile.ReadByte();
            this.value = byteFile.ReadUInt32();
        }
    }
}
