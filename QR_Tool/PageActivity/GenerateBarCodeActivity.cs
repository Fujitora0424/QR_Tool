using System;
using System.Collections.Generic;


using Android.App;

using Android.OS;

using Android.Widget;
using Android.Graphics;
using System.Net.Http;
using System.Net;
using System.Text;

namespace QR_Tool.PageActivity
{
    [Activity(Label = "二维码测试助手")]
    public class GenerateBarCodeActivity : Activity
    {
        private EditText qrStrEditText;
        private ImageView qrImgImageView;
        private Button generateQRCodeButton;
        private Button requestQRCodeButton;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.GenerateBarCodePage);


         qrStrEditText = (EditText)this.FindViewById(Resource.Id.et_qr_string);
         qrImgImageView = (ImageView)this.FindViewById(Resource.Id.iv_qr_image);
         generateQRCodeButton = (Button)this.FindViewById(Resource.Id.btn_add_qrcode);
         generateQRCodeButton.Click += generateQRCodeButton_Click;
         requestQRCodeButton = (Button)this.FindViewById(Resource.Id.btn_request_qrcode);
         requestQRCodeButton.Click += requestQRCodeButton_Click;
            // Create your application here
        }

        private async void requestQRCodeButton_Click(object sender, EventArgs e)
        {
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

                //使用FormUrlEncodedContent做HttpContent
                DateTime d = DateTime.Now;
                string dateString = d.ToString("yyyyMMddHHmmss");
                var content = new FormUrlEncodedContent(new Dictionary<string, string>()
                {

                    { "version", "5.1.0"},          //版本号 全渠道默认值
                    { "encoding", "UTF-8" },  //字符集编码 可以使用UTF-8,GBK两种方式
                    { "signMethod", "01" }, //签名方法
                    { "txnType", "01" },                      //交易类型 01:消费
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
                    { "backUrl", "null" }

            });

                //await异步等待回应

                var response = await http.PostAsync(url, content);
                //确保HTTP成功状态值
                response.EnsureSuccessStatusCode();
                //await异步读取最后的JSON（注意此时gzip已经被自动解压缩了，因为上面的AutomaticDecompression = DecompressionMethods.GZip）
                string messge  = await response.Content.ReadAsStringAsync();
                Dictionary<string, string> reDictionary = UP_SDK.SDKUtil.parseQString(messge, Encoding.UTF8);
                string qrCodeString = reDictionary["qrCode"];
                BarCode bar = new BarCode();
                Bitmap qrCodeBitmap = bar.GenerateQrImage(qrCodeString, 600, 600);


                qrImgImageView.SetImageBitmap(qrCodeBitmap);
            }
        }

        private void generateQRCodeButton_Click(object sender, EventArgs e)
        {
            BarCode bar = new BarCode();
            string contentString = qrStrEditText.Text.Trim();
            if (!contentString.Equals(""))
            {
              
             
                Bitmap qrCodeBitmap = bar.GenerateQrImage(contentString, 600, 600);
              

                qrImgImageView.SetImageBitmap(qrCodeBitmap);
            }
            else
            {
                Toast.MakeText(this, "Text can not be empty", ToastLength.Short).Show();
            }
        }
    }
}