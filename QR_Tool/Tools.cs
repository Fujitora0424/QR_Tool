using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace QR_Tool
{
    class Tools
    {
        public static string Xor(string LeftBlock, string RightBlock)

        {
            byte[] ByteLeft = Tools.StringToBytes(LeftBlock);
            byte[] ByteRight = Tools.StringToBytes(RightBlock);

            byte[] block = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                block[i] = (byte)(ByteLeft[i] ^ ByteRight[i]);
            }
            string result = Tools.BytesToHexString(block);
            return result;
        }
        static public byte[] StringToBytes(string s)
        {
            byte[] bytes;
            if (s.Length % 2 != 0)
                return null;
            bytes = new byte[s.Length / 2];
            for (int i = 0; i < s.Length / 2; i++)
            {
                bytes[i] = Convert.ToByte(s.Substring(i * 2, 2), 16);
            }
            return bytes;
        }
        static public string BytesToHexString(byte[] bytes)
        {
            String s = null;
            if (null == bytes)
            {
                return "";
            }
            foreach (byte b in bytes)
            {
                s += b.ToString("X2");
            }
            return s;
        }
        static public string AscStringtoHexstring(string Ascstring)
        {
            byte[] buff = new byte[Ascstring.Length / 2];
            int index = 0;
            for (int i = 0; i < Ascstring.Length; i += 2)
            {

                buff[index] = Convert.ToByte(Ascstring.Substring(i, 2), 16);
                ++index;

            }
            string result = Encoding.Default.GetString(buff);
            return result;

        }
        static public string Codertracler(string Data, string CodeMethod)
        {
            string resultdata = "";

            Encoding e;

            byte[] textByte = new byte[Data.Length / 2];
            int m = Data.Length;

            for (int i = 0; i < textByte.Length; i++)
            {

                textByte[i] = Convert.ToByte(Data.Substring(i * 2, 2), 16);

            }


            switch (CodeMethod)
            {
                case "GB2312":
                    e = System.Text.Encoding.GetEncoding("GB2312");
                    resultdata = new string(e.GetChars(textByte));
                    break;
                case "Unicode":
                    e = System.Text.Encoding.Unicode;
                    resultdata = new string(e.GetChars(textByte));
                    break;
                case "ASCII":
                    e = System.Text.Encoding.ASCII;
                    resultdata = new string(e.GetChars(textByte));
                    break;
            }
            return resultdata;

        }  
    }
}