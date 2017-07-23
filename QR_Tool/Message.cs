using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace QR_Tool
{
    class Message
    {


        List<TLVMOD> tlvData = null;
        List<TLVMOD> packagetlvData = new List<TLVMOD>();
        List<byte> packagebytes = null;
       private byte[] message = null;
       private TLV _9F01 = new TLV(); //交易类型
       private TLV _9F02 = new TLV();//运行结果 0x00成功 0x01 失败
       private TLV _9F03 = new TLV();//平台返回数据
       private TLV _9F04 = new TLV();//图片数据
        private TLV _9F05 = new TLV();//URL


        public Message(byte[] data)
        {
            message = data;
            MessageParse();

        }

        public byte[] PackageMessage(byte[] bardata,byte[] picturedata,bool runResult )
        {
            if (_9F01.Data != null)
            {
                TLVMOD package_9F01 = new TLVMOD();
                package_9F01.Data = _9F01.Data;
                package_9F01.Len = _9F01.Len;
                package_9F01.Tag = _9F01.Tag;
                packagetlvData.Add(package_9F01);

            }
            TLVMOD package_9F02 = new TLVMOD();
            package_9F02.Len = 0x01;
            package_9F02.Tag = 0x9F02;
            if (runResult)
            {

                package_9F02.Data = new byte[1] { 0x00 };
             

            }
            else
            {
                package_9F02.Data = new byte[1] { 0x01 }; 

            }
            packagetlvData.Add(package_9F02);
          
            if (bardata!=null)
            {
                TLVMOD package_9F03 = new TLVMOD();
                package_9F03.Data = bardata;
                package_9F03.Len = bardata.Length;
                package_9F03.Tag = 0x9F03;
                packagetlvData.Add(package_9F03);

            }
            if (picturedata != null)
            {
                TLVMOD package_9F04 = new TLVMOD();
                package_9F04.Data = picturedata;
                package_9F04.Len = picturedata.Length;
                package_9F04.Tag = 0x9F04;
                packagetlvData.Add(package_9F04);

            }
            packagebytes = TLVHelper.Pack(packagetlvData);
            return packagebytes.ToArray();




        }

        public TLV Date_9F01 { get => _9F01; set => _9F01 = value; }
        public TLV Date_9F02 { get => _9F02; set => _9F02 = value; }
        public TLV Date_9F03 { get => _9F03; set => _9F03 = value; }
        public TLV Date_9F04 { get => _9F04; set => _9F04 = value; }
        public TLV Date_9F05 { get => _9F05; set => _9F05 = value; }

        private void MessageParse()
        {

            tlvData = TLVHelper.UnPack(message, false);
            TLVMOD[] tlvDataArry = tlvData.ToArray();
            foreach (var tlvData in tlvDataArry)
            {
                if(tlvData.Tag==0x9F01)
                {
                    _9F01.Tag = tlvData.Tag;
                    _9F01.Len = tlvData.Len;
                    _9F01.Data = tlvData.Data;

                }
               else if (tlvData.Tag == 0x9F02)
                {
                    _9F02.Tag = tlvData.Tag;
                    _9F02.Len = tlvData.Len;
                    _9F02.Data = tlvData.Data;

                }
                else if (tlvData.Tag == 0x9F03)
                {
                    _9F03.Tag = tlvData.Tag;
                    _9F03.Len = tlvData.Len;
                    _9F03.Data = tlvData.Data;

                }
                else if (tlvData.Tag == 0x9F04)
                {
                    _9F04.Tag = tlvData.Tag;
                    _9F04.Len = tlvData.Len;
                    _9F04.Data = tlvData.Data;

                }
                else if (tlvData.Tag == 0x9F05)
                {
                    _9F05.Tag = tlvData.Tag;
                    _9F05.Len = tlvData.Len;
                    _9F05.Data = tlvData.Data;

                }
                else
                {

                }

            }


        }


    }

    public class TLV
    {
        private int tag;
        private int len;
        private byte[] data;



        /// <summary>
        /// 标签
        /// </summary>
        public int Tag { get => tag; set => tag = value; }
        /// <summary>
        /// 长度
        /// </summary>
        public int Len { get => len; set => len = value; }


        /// <summary>
        /// 数据
        /// </summary>
        public byte[] Data { get => data; set => data = value; }


        public TLV()
        {
            tag = 0;
            len = 0;
            data = null;
        }
    }
}