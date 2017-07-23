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
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using ZXing.Mobile;

namespace QR_Tool.PageActivity
{
    [Activity(Label = "二维码测试助手")]
    public class ScanBarCodeActivity : Activity
    {
        private EditText ipEditText;
        private EditText portEditText;
        private Button ScanQRCodeButton;




        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ScanBarCodePage);
            MobileBarcodeScanner.Initialize(Application);//初始化二维码扫描仪
            ipEditText = (EditText)this.FindViewById(Resource.Id.et_ip);
            portEditText = (EditText)this.FindViewById(Resource.Id.et_port);

            ScanQRCodeButton = (Button)this.FindViewById(Resource.Id.btn_scan_barcode);
            ScanQRCodeButton.Click += ScanQRCodeButton_Click;

            

            // Create your application here
        }

        private async void ScanQRCodeButton_Click(object sender, EventArgs e)
        {
           


                await send();
            
         
            }

        private async Task send()
        {

            {

                string urlC = String.Format("https://{0}:{1}/", ipEditText.Text.Trim(), portEditText.Text.Trim());

                string url = "https://1715m7746k.51mypc.cn:15107/";

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
                    { "encoding", "UTF-8" },  //字符集编码 可以使用UTF-8,GBK两种方式
                    { "signMethod", "01" }, //签名方法
                    { "txnType", "99" },                      //交易类型 01:消费
                    { "txnSubType", "07" },                 //交易子类 07：申请消费二维码
                    { "bizType", "000000" },                  //填写000000
                    { "channelType", "08" },                 //渠道类型 08手机

                    /***商户接入参数***/
                    { "merId", "898310173990680" },                       //商户号码，请改成自己申请的商户号或者open上注册得来的777商户号测试
                    { "accessType", "0" },                     //接入类型，商户接入填0 ，不需修改（0：直连商户， 1： 收单机构 2：平台商户）
                    { "orderId", "12345678" },                    //商户订单号，8-40位数字字母，不能含“-”或“_”，可以自行定制规则	
                    { "txnTime",dateString },                 //订单发送时间，取系统时间，格式为YYYYMMDDhhmmss，必须取当前时间，否则会报txnTime无效
                    { "txnAmt", "1" },                     //交易金额 单位为分，不能带小数点
                    { "currencyCode", "156"},
                    { "backUrl", "null" },
                    {"barcode",barCodeString }

            });

                    //await异步等待回应

                    var response = await http.PostAsync(url, content);
                    //确保HTTP成功状态值
                    response.EnsureSuccessStatusCode();
                    //await异步读取最后的JSON（注意此时gzip已经被自动解压缩了，因为上面的AutomaticDecompression = DecompressionMethods.GZip）
                    string messge = await response.Content.ReadAsStringAsync();

                }
            }


        }
       
    }

}