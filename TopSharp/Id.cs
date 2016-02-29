using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TopSharp
{
    public class Id
    {
        UInt32 val;
        public string Name
        {
            get{
                string hexValue = val.ToString("X");
                if (val == 2147483648)
                    return null;//Convert.ToDouble(val);
                else if (hexValue.Length <= 4)
                {
                    return ((val)*(decimal)0.1).ToString();//Convert.ToDecimal(val)*(decimal)0.1;
                }
                else if (hexValue.Length > 4)
                {
                    int firsPart = int.Parse(hexValue.Substring(0, hexValue.Length - 4), System.Globalization.NumberStyles.HexNumber);
                    int secPart = int.Parse(hexValue.Substring(hexValue.Length - 4), System.Globalization.NumberStyles.HexNumber);
                    return string.Format("{0},{1}", firsPart, secPart);//Convert.ToDecimal(string.Format("{0},{1}", firsPart, secPart));
                }
                return this.val.ToString();
            }
            set
            {
                if (value == null || value.Length == 0)
                {
                    this.val = 2147483648;
                }
                else
                {
                    string[] vals = value.Split(new char[]{',', '.'});//string.Format("{0:0.0}", value).Split(new char[]{',', '.'});
                    if (Convert.ToDouble(value) < 1)
                    {
                        string hexValue1 = (Convert.ToInt32(vals[1])).ToString("X");
                        int hex = int.Parse(hexValue1, System.Globalization.NumberStyles.HexNumber);
                        this.val = (UInt32)hex;
                    }
                    else
                    {
                        string hexValue1 = (Convert.ToInt32(vals[0])).ToString("X");
                        string hexValue2 = (Convert.ToInt32(vals[1])).ToString("X4");
                        /*if (Convert.ToInt32(vals[1]) == 0)
                            hexValue2 = string.Empty;*/
                        int hex = int.Parse(string.Format("{0}{1}", hexValue1, hexValue2), System.Globalization.NumberStyles.HexNumber);
                        this.val = (UInt32)hex;
                    }
                }
            }
        }
        public Id(BinaryReader byteFile)
        {
            //byte smtn = byteFile.ReadByte();
            this.val = byteFile.ReadUInt32();
        }
        public Id(string stationName)
        {
            this.Name = stationName;
        }
        public void Write(BinaryWriter bw)
        {
            bw.Write(val);
        }
    }
}
