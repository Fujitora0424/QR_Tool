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


    /// <summary>
    /// TLV格式数据解包组包工具类
    /// </summary>
    public class TLVHelper
    {
        /// <summary>
        /// 解包data数据的第一个TAG以及其下嵌套的所有子TAG
        /// </summary>
        /// <param name="data">待解包的数据</param>
        /// <param name="seek">起始偏移量</param>
        /// <param name="count">data长度</param>
        /// <param name="IsLenOneByte">Len是否固定1个字节</param>
        /// <param name="IsForceParseAll">强制递归解包嵌套的TAG</param>
        /// <returns></returns>
        private static TLVMOD UnParseOne(byte[] data, ref int seek, int count, bool IsLenOneByte, bool IsForceParseAll)
        {
            TLVMOD model = new TLVMOD();
            int tempSeek = seek;

            while (true)
            {
                if (tempSeek + count > data.Length || seek >= tempSeek + count)
                {
                    return null;
                }

                if (data[seek] == 0x00 || data[seek] == 0xFF)
                {
                    seek++;
                    //tempSeek++;
                }
                else
                {
                    break;
                }
            }

            if (tempSeek + count > data.Length || seek >= tempSeek + count)
            {
                return null;
            }

            //b6位决定当前的TLV数据是一个单一的数据和复合结构的数据
            if ((data[seek] & 0x20) == 0x00)
            {
                model.IsPrimitive = false;
            }
            else
            {
                model.IsPrimitive = true;
            }

            //b5~b1如果全为1，则说明这个tag下面还有一个子字节. 占两个字节, 否则tag占一个字节
            if ((data[seek] & 0x1F) == 0x1F)
            {
                if (data.Length <= seek + 1)
                {
                    return null;
                }
                model.Tag = data[seek] * 256 + data[seek + 1];
                seek += 2;
            }
            else
            {
                model.Tag = data[seek];
                seek += 1;
            }

            if (seek >= tempSeek + count)
            {
                return null;
            }

            if (IsLenOneByte)
            {
                model.Len = data[seek];
                seek++;
            }
            else
            {
                //如果b8为1, b7~b1的值指示了下面有几个子字节. 下面子字节的值就是value域的长度
                //如果第一个字节的最高位b8为0, b7~b1的值就是value域的长度
                if ((data[seek] & 0x80) == 0x80)
                {
                    int lenCount = data[seek] & 0x7F;
                    if (lenCount > 3)
                    {
                        return null;
                    }
                    seek++;
                    if (seek >= tempSeek + count)
                    {
                        return null;
                    }
                    model.Len = data[seek];
                    for (int i = 1; i < lenCount; i++)
                    {
                        model.Len <<= 8;
                        seek++;
                        if (seek >= tempSeek + count)
                        {
                            return null;
                        }
                        model.Len += data[seek];
                    }
                    seek++;
                    if (seek >= tempSeek + count)
                    {
                        return null;
                    }
                }
                else
                {
                    model.Len = data[seek] & 0x7F;
                    seek++;
                }
            }
            if (seek > tempSeek + count)
            {
                return null;
            }
            model.Data = new byte[model.Len];
            if (model.Len + seek > tempSeek + count)
            {
                return null;
            }
            Array.Copy(data, seek, model.Data, 0, model.Len);
            seek += model.Len;

            //递归解包嵌套的TAG
            if (IsForceParseAll || model.IsPrimitive)
            {
                int innerSeek = 0;
                while (innerSeek != model.Len)
                {
                    TLVMOD tempModel = UnParseOne(model.Data, ref innerSeek, model.Data.Length - innerSeek, IsLenOneByte, IsForceParseAll);
                    if (tempModel == null)
                    {
                        model.TLVChildList.Clear();
                        break;
                    }
                    else
                    {
                        model.TLVChildList.Add(tempModel);
                    }
                }
            }

            return model;
        }

        /// <summary>
        /// 解包TLV格式的数据
        /// </summary>
        /// <param name="data">待解包的数据</param>
        /// <param name="IsForceParseAll">强制递归解包嵌套的TAG</param>
        /// <returns></returns>
        public static List<TLVMOD> UnPack(byte[] data, bool IsForceParseAll)
        {
            return UnPack(data, 0, data.Length, IsForceParseAll);
        }

        /// <summary>
        /// 解包TLV格式的数据
        /// </summary>
        /// <param name="data">待解包的数据</param>
        /// <param name="seek">偏移量</param>
        /// <param name="count">长度</param>
        /// <param name="IsForceParseAll">强制递归解包嵌套的TAG</param>
        /// <returns></returns>
        public static List<TLVMOD> UnPack(byte[] data, int seek, int count, bool IsForceParseAll)
        {
            List<TLVMOD> list = new List<TLVMOD>();
            int maxSeek = seek + count;
            while (seek != maxSeek)
            {
                TLVMOD model = UnParseOne(data, ref seek, maxSeek - seek, false, IsForceParseAll);
                if (model == null)
                {
                    return null;
                }
                else
                {
                    list.Add(model);
                }
            }
            return list;
        }

        /// <summary>
        /// 解包DOL数据
        /// </summary>
        /// <param name="data">待解包的数据</param>
        /// <returns></returns>
        public static List<TLVMOD> UnPackDOL(byte[] data)
        {
            return UnPackDOL(data, 0, data.Length);
        }

        /// <summary>
        /// 解包DOL数据
        /// </summary>
        /// <param name="data">待解包的数据</param>
        /// <param name="seek">偏移量</param>
        /// <param name="count">长度</param>
        /// <returns></returns>
        public static List<TLVMOD> UnPackDOL(byte[] data, int seek, int count)
        {
            List<TLVMOD> list = new List<TLVMOD>();
            int maxSeek = seek + count;
            while (seek != maxSeek)
            {
                TLVMOD model = new TLVMOD();
                //b5~b1如果全为1，则说明这个tag下面还有一个子字节. 占两个字节, 否则tag占一个字节
                if ((data[seek] & 0x1F) == 0x1F)
                {
                    model.Tag = data[seek] * 256 + data[seek + 1];
                    seek += 2;
                }
                else
                {
                    model.Tag = data[seek];
                    seek += 1;
                }

                //如果b8为1, b7~b1的值指示了下面有几个子字节. 下面子字节的值就是value域的长度
                //如果第一个字节的最高位b8为0, b7~b1的值就是value域的长度
                if ((data[seek] & 0x80) == 0x80)
                {
                    int lenCount = data[seek] & 0x7F;
                    if (lenCount > 3)
                    {
                        return null;
                    }
                    seek++;

                    model.Len = data[seek];
                    for (int i = 1; i < lenCount; i++)
                    {
                        model.Len <<= 8;
                        seek++;
                        model.Len += data[seek];
                    }
                    seek++;
                }
                else
                {
                    model.Len = data[seek] & 0x7F;
                    seek++;
                }

                list.Add(model);
            }
            return list;
        }

        /// <summary>
        /// 解包TLV格式的数据，其中Len固定为1个字节
        /// </summary>
        /// <param name="data">待解包的数据</param>
        /// <returns></returns>
        public static List<TLVMOD> UnPack_LenOneByte(byte[] data)
        {
            return UnPack_LenOneByte(data, 0, data.Length);
        }

        /// <summary>
        /// 解包TLV格式的数据，其中Len固定为1个字节
        /// </summary>
        /// <param name="data">待解包的数据</param>
        /// <param name="seek">偏移量</param>
        /// <param name="count">长度</param>
        /// <returns></returns>
        public static List<TLVMOD> UnPack_LenOneByte(byte[] data, int seek, int count)
        {
            List<TLVMOD> list = new List<TLVMOD>();
            int maxSeek = seek + count;
            while (seek != maxSeek)
            {
                TLVMOD model = UnParseOne(data, ref seek, maxSeek - seek, true, false);
                if (model == null)
                {
                    return null;
                }
                else
                {
                    list.Add(model);
                }
            }
            return list;
        }

        /// <summary>
        /// 组包一个TLV数据实体类对象，包括它嵌套的所有TLV数据类对象
        /// </summary>
        /// <param name="model">待组包的数据结构体</param>
        /// <returns></returns>
        private static List<byte> ParseOne(TLVMOD model)
        {
            List<byte> data = new List<byte>(1024);

            if (model.TLVChildList != null && model.TLVChildList.Count > 0)
            {
                List<byte> tempList = new List<byte>();
                for (int i = 0; i < model.TLVChildList.Count; i++)
                {
                    List<byte> tempData = ParseOne(model.TLVChildList[i]);
                    if (tempData == null)
                    {
                        return null;
                    }
                    tempList.AddRange(tempData);
                }
                model.Data = tempList.ToArray();
                model.Len = model.Data.Length;
            }

            //组包TAG
            if (model.Tag < 0x100)
            {
                if ((model.Tag & 0x1F) == 0x1F)
                {
                    return null;
                }
                data.Add((byte)model.Tag);
            }
            else
            {
                if (((model.Tag / 0x100) & 0x1F) != 0x1F)
                {
                    return null;
                }
                data.Add((byte)(model.Tag / 0x100));
                data.Add((byte)(model.Tag % 0x100));
            }

            //组包长度
            if (model.Len <= 0x7F)
            {
                data.Add((byte)model.Len);
            }
            else
            {
                if (model.Len < 0x100)
                {
                    data.Add(0x81);
                    data.Add((byte)model.Len);
                }
                else
                {
                    data.Add(0x81);
                    data.Add((byte)(model.Len / 0x100));
                    data.Add((byte)(model.Len % 0x100));
                }
            }
            data.AddRange(model.Data);
            return data;
        }

        /// <summary>
        /// 组包TLV数据
        /// </summary>
        /// <param name="list">TLV数据对象集合</param>
        /// <returns></returns>
        public static List<byte> Pack(List<TLVMOD> list)
        {
            if (list != null && list.Count > 0)
            {
                List<byte> data = new List<byte>(1024);
                for (int i = 0; i < list.Count; i++)
                {
                    data.AddRange(ParseOne(list[i]));
                }
                return data;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 组包TLV数据
        /// </summary>
        /// <param name="model">TLV数据对象</param>
        /// <returns></returns>
        public static List<byte> Pack(TLVMOD model)
        {
            if (model != null)
            {
                return ParseOne(model);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取TAG标签的描述
        /// </summary>
        /// <param name="Tag">TAG标签</param>
        /// <returns></returns>
        public static string GetTagDescription(int Tag)
        {
            switch (Tag)
            {
                case 0x9F01://01为开启二维码扫描，02开启图片扫描
                    return "报文类型";
                case 0x9F02:
                    return "运行结果";//运行状态 0x00为成功 0x01为失败
                case 0x9F03:
                    return "二维码数据";//存储二维码数据
                case 0x9F04:
                    return "图片数据";//存储图片数据
                default:
                    return "未知标签";
            }
        }
    }

    /// <summary>
    /// TLV数据实体类
    /// </summary>
    public class TLVMOD
    {
        /// <summary>
        /// TAG标签
        /// </summary>
        public int Tag { get; set; }

        /// <summary>
        /// 长度
        /// </summary>
        public int Len { get; set; }

        /// <summary>
        /// 数据域
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// 是否为单一数据
        /// </summary>
        public bool IsPrimitive { get; set; }

        /// <summary>
        /// 如果为复合数据时的嵌套TLV数据
        /// </summary>
        public List<TLVMOD> TLVChildList { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public TLVMOD()
        {
            TLVChildList = new List<TLVMOD>();
        }
    }


}