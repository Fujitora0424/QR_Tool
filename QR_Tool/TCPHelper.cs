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
using System.Net.Sockets;

namespace QR_Tool
{
    class TCPHelper
    {

        int messageLength = 4;

         public byte[] ReceiveByteArray(NetworkStream stream)
        {
            try
            {
                int bufferlen = GetSize(stream);
                byte[] resultbyte = new byte[bufferlen];

                int offset = 0, bytesread = 0;
                while (offset < bufferlen)
                {
                    bytesread = stream.Read(resultbyte, offset, bufferlen - offset);
                    if (bytesread == 0)
                        throw new Exception("网络异常断开，数据读取不完整。");
                    else
                        offset += bytesread;
                }
                return resultbyte;
            }
            catch (Exception)
            {
                throw new Exception("接收data异常"); 
            }
        }

         private int GetSize(NetworkStream stream)
        {
            int count = 0;
            byte[] countBytes = new byte[messageLength];
            try
            {
                if (stream.Read(countBytes, 0, messageLength) == messageLength)
                {
                    if (BitConverter.IsLittleEndian)
                     Array.Reverse(countBytes);
                    count = BitConverter.ToInt32(countBytes, 0);
                }
                else
                {
                    return 0;
                }
            }
            catch 
            {
                throw new Exception("接收数据长度异常"); ;
            }
            return count;
        }

        public void SendByteArray(NetworkStream stream,byte[] sendData)
        {
            try
            {
                byte[] countBytes = new byte[messageLength];
                byte[] sendBytes = new byte[messageLength+sendData.Length];
                countBytes = BitConverter.GetBytes(sendData.Length);
                if (BitConverter.IsLittleEndian)
                { Array.Reverse(countBytes); }
                countBytes.CopyTo(sendBytes, 0);
                sendData.CopyTo(sendBytes, 4);



                stream.Write(sendBytes, 0, sendBytes.Length);
            }
            catch (Exception)
            {
                throw new Exception("发送数据异常");
            }
        }
    }
}