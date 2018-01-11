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
using ZXing.Mobile;
using System.Threading.Tasks;
using ZXing;
using Android.Graphics;
using System.IO;
using ZXing.QrCode.Internal;
using System.Net.Http;
using System.Net;

namespace QR_Tool
{
    class Https
    {
        public static async Task sendBarCodeAsync(Dictionary<string,string> dic,string surl)
        {

            {

        try {
                Dictionary<string, string> send_Dic = new Dictionary<string, string>(dic);
                    string queryID = send_Dic["queryId"];
                    string encoding = send_Dic["encoding"];

                    string url = surl; /*"https://1715m7746k.51mypc.cn:15107/";*/

                    //设置HttpClientHandler的AutomaticDecompression
                    var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };


                    //创建HttpClient（注意传入HttpClientHandler）
                    using (var http = new HttpClient(handler))
                    {
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                        System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                            (se, cert, chain, sslerror) =>
                            {
                                return true;
                            };

                        BarCode newBarCode = new BarCode();
                        byte[] scanResult = await newBarCode.ScanBarcodeAsync();
                        string barCodeString = System.Text.Encoding.UTF8.GetString(scanResult);
                      
                        //使用FormUrlEncodedContent做HttpContent
                        DateTime d = DateTime.Now;
                        string dateString = d.ToString("yyyyMMddHHmmss");
                   var content = new FormUrlEncodedContent(new Dictionary<string, string>()
                {

                    { "version", "5.1.0"},          //版本号 全渠道默认值
                    { "queryId", queryID },
                    { "signMethod", "01" }, //签名方法
                    { "txnType", "99" },                      //交易类型 01:消费
                    { "txnSubType", "00" },
                    { "orderId", "201706091040394406588" },                    //商户订单号，8-40位数字字母，不能含“-”或“_”，可以自行定制规则	
                    { "currencyCode", "156"},
                    {"qrCode",barCodeString },
                     {"encoding",encoding }

            });

                        //await异步等待回应

                        var response = await http.PostAsync(url, content);
                        //确保HTTP成功状态值
                        response.EnsureSuccessStatusCode();
                        //await异步读取最后的JSON（注意此时gzip已经被自动解压缩了，因为上面的AutomaticDecompression = DecompressionMethods.GZip）
                        string messge = await response.Content.ReadAsStringAsync();
               

                }
                }
                catch(Exception e)
                {
                   var a = e.Message;
                }
            }


        }


        public static async Task sendOfflineBarCodeAsync(Dictionary<string, string> dic, string surl)
        {

            {

                try
                {
                    Dictionary<string, string> send_Dic = new Dictionary<string, string>(dic);
                    string encoding = send_Dic["encoding"];

                    string url = surl; /*"https://1715m7746k.51mypc.cn:15107/";*/

                    //设置HttpClientHandler的AutomaticDecompression
                    var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };


                    //创建HttpClient（注意传入HttpClientHandler）
                    using (var http = new HttpClient(handler))
                    {
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                        System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                            (se, cert, chain, sslerror) =>
                            {
                                return true;
                            };

                        BarCode newBarCode = new BarCode();
                        byte[] scanResult = await newBarCode.ScanBarcodeAsync();
                        string barCodeString = Encoding.GetEncoding(encoding).GetString(scanResult);
                        Dictionary<string, string> send_Dictionnary = new Dictionary<string, string>();
                        List<string> s = barCodeString.Split('/').ToList();
                        send_Dictionnary = UP_SDK.SDKUtil.parseQString(s[5], Encoding.GetEncoding(encoding));

                        //使用FormUrlEncodedContent做HttpContent

                        var content = new FormUrlEncodedContent(send_Dictionnary);

                        //await异步等待回应

                        var response = await http.PostAsync(url, content);
                        //确保HTTP成功状态值
                        response.EnsureSuccessStatusCode();
                        //await异步读取最后的JSON（注意此时gzip已经被自动解压缩了，因为上面的AutomaticDecompression = DecompressionMethods.GZip）
                        string messge = await response.Content.ReadAsStringAsync();


                    }
                }
                catch (Exception e)
                {
                    var a = e.Message;
                }
            }


        }


    }

}
