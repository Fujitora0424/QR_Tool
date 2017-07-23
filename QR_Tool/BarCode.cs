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

namespace QR_Tool
{
    class BarCode
    {
        MobileBarcodeScanner scanner = null;
       
        public BarCode()
        {
            scanner = new MobileBarcodeScanner();
        }
         public  async Task<byte[]> ScanBarcodeAsync()
            {
            string msg = "";
            byte[] scanData = null;
            MobileBarcodeScanningOptions scanoptions = new MobileBarcodeScanningOptions();
            scanoptions.TryInverted = true;
            scanoptions.TryHarder = true;
            scanoptions.AutoRotate = false;
            scanner.UseCustomOverlay = false;
            scanner.AutoFocus();

            scanner.TopText = "请扫码";
            scanner.BottomText = "Wait for the barcode to automatically scan!";

            //Start scanning
            var result = await scanner.Scan(scanoptions);
           

            if (result != null && !string.IsNullOrEmpty(result.Text))
            {
                msg = result.Text;
                scanData = System.Text.Encoding.Default.GetBytes(msg);

            }

            return scanData;
        }

        public Bitmap GenerateQrImage(string content, int width, int height)
        {

            var writer = new ZXing.Mobile.BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Height = height,
                    Width = width,
                    Margin = 0,
                    PureBarcode = true,
                }

            };
            writer.Renderer = new ZXing.Mobile.BitmapRenderer { Background = Color.Red, Foreground = Color.Black };
            var bitmap = writer.Write(content);
            //var stream = new MemoryStream();
            //bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);  // this is the diff between iOS and Android
            //stream.Position = 0;
            return bitmap;
        }
     
    }

    }
