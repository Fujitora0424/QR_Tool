using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using System;
using Android.Text.Method;
using System.IO;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text;
using ZXing.Mobile;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;

namespace QR_Tool.PageActivity
{
    [Activity(Label = "二维码测试助手")]
    public class ToolPageActivity : Activity
    {
        private Button clearButton;
        private Button createButton;
        private Button hidemessageButton;
        private TextView messageText;
        private EditText portText;
        private RelativeLayout pagelayout;
        private RelativeLayout messagelayout;

        private Context mContext;


        private bool serverRuning = false;
        private Thread mThreadServer = null;
        private TcpListener myListener = null;
        private TcpClient newClient = null;
        NetworkStream clientStream = null;






        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ToolPage);
            mContext = this;
            MobileBarcodeScanner.Initialize(Application);//初始化二维码扫描仪
            StrictMode.SetThreadPolicy(new StrictMode.ThreadPolicy.Builder()
                 .DetectDiskReads().DetectDiskWrites().DetectNetwork() // or
                                                                       // .detectAll()
                                                                       // for
                                                                       // all
                                                                       // detectable
                                                                       // problems
                 .PenaltyLog().Build());
            StrictMode.SetVmPolicy(new StrictMode.VmPolicy.Builder()
                    .DetectLeakedSqlLiteObjects().PenaltyLog().PenaltyDeath()
                    .Build());

            portText = (EditText)FindViewById(Resource.Id.portText);
            portText.Text = "16908";
            portText.InputType = Android.Text.InputTypes.Null;


            pagelayout = (RelativeLayout)FindViewById((Resource.Id.PageLayout));
            messagelayout = (RelativeLayout)FindViewById(Resource.Id.messageLayout);


            clearButton = (Button)FindViewById(Resource.Id.ClearButton);
            clearButton.Click += ClearButton_Click;

            createButton = (Button)FindViewById(Resource.Id.CreateConnect);
            createButton.Click += CreateButton_Click;

            messageText = (TextView)FindViewById(Resource.Id.messageText);
            messageText.MovementMethod = ScrollingMovementMethod.Instance;



            hidemessageButton = (Button)FindViewById(Resource.Id.HideMessageButton);
            hidemessageButton.Click += HidemessageButton_Click;

        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (serverRuning)
            {
                serverRuning = false;


                if (myListener != null)
                {
                    myListener.Stop();
                    myListener = null;
                }
                if (mThreadServer != null)
                {
                    mThreadServer.Interrupt();
                }


            }



        }
        private void HidemessageButton_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void CreateButton_Click(object sender, EventArgs e)
        {
            if (serverRuning)
            {
                serverRuning = false;

                try
                {
                    if (myListener != null)
                    {
                        myListener.Stop();
                        myListener = null;
                    }
                    if (newClient != null)
                    {
                        newClient.Close();
                        newClient = null;
                    }
                }
                catch
                {
                    // TODO Auto-generated catch block
                    Toast.MakeText(this, "creat fail", ToastLength.Long);

                }
                mThreadServer.Interrupt();
                createButton.Text = "创建服务";
                messageText.Text = "";
                portText.Enabled = true;

            }
            else
            {
                serverRuning = true;
                mThreadServer = new Thread(RunListenServer);
                mThreadServer.IsBackground = true;
                mThreadServer.Start();
                createButton.Text = "停止服务";
                portText.Enabled = false;

            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {

            messageText.Text = "";


        }

        private  void RunListenServer()
        {
            try
            {
                int port = Convert.ToInt32(portText.Text.Trim());//服务器port
                IPAddress ip = IPAddress.Parse(IP.GetLocalIP());//服务器端ip
                myListener = new TcpListener(ip, port);//创建TcpListener实例
                myListener.Start();//start
                this.RunOnUiThread(() => //this 指代的是Activity对象，RunOnUiThread 是Activity的一个成员方法  
                {
                    RefreshMessageView("请连接:" + IP.GetLocalIP() + ":" + port);
                });
                while (serverRuning)
                {
               newClient = myListener.AcceptTcpClient();//等待客户端连接,长连接
                //this.RunOnUiThread(() =>
                //{
                //    RefreshMessageView("链接成功");
                //});

                //while (serverRuning)
                //{
                    //try
                    //{
                        clientStream = newClient.GetStream();//利用TcpClient对象GetStream方法得到网络流
                        TCPHelper newtcp = new TCPHelper();
                        byte[] btemp = newtcp.ReceiveByteArray(clientStream);
                        Message mes = new Message(btemp);
                        byte messagetype = mes.Date_9F01.Data[0];
                    string url = "";
                    string logData = "";
                    Dictionary<string, string> log ; 
                    byte[] barCodeData = null;

                        switch (messagetype)
                        {
                            case 0x00:
                            logData = Encoding.Default.GetString(mes.Date_9F03.Data);
                            url = Encoding.Default.GetString(mes.Date_9F05.Data);
                            log = UP_SDK.SDKUtil.CoverStringToDictionary(logData, Encoding.UTF8);
                            var re_00 = Https.sendOfflineBarCodeAsync(log, url);//脱机扫码
                            break;
                            case 0x01:
                                logData = Encoding.Default.GetString(mes.Date_9F03.Data);
                                url = Encoding.Default.GetString(mes.Date_9F05.Data);
                                log = UP_SDK.SDKUtil.CoverStringToDictionary(logData, Encoding.UTF8);
                                var re_01 = Https.sendBarCodeAsync(log, url);
                            break;
                            case 0x02:
                                //图片扫描
                                break;
                            default:
                                break;
                        }


                    byte[] byteArray = mes.PackageMessage(barCodeData, null, true);
                    newtcp.SendByteArray(clientStream, byteArray);
                    clientStream.Close();
                    newClient.Close();
                    newClient.Dispose();

                        // Shutdown and end connection
                }

            }
            catch (Exception e)
            {
                this.RunOnUiThread(() =>
                {
                    RefreshMessageView("异常信息" + e.Message);
                });
                if (clientStream != null)
                {
                    clientStream.Close();
                    clientStream = null;
                }
                if (myListener != null)
                {
                    myListener.Stop();
                    myListener = null;
                }
                if(!e.Message.Equals("interrupted"))
                {
                    this.RunOnUiThread(() =>
                    {
                        RefreshMessageView("异常信息" + e.Message);
                        createButton.PerformClick();
                    });
           
                }

                return;
            }




        }

     


        void RefreshMessageView(string msg)
        {

            messageText.Append(msg + "\n");

            int offset = messageText.LineCount * messageText.LineHeight;
            if (offset > messageText.Height)
            {
                messageText.ScrollTo(0, offset - messageText.Height);
            }

        }
    }


}


