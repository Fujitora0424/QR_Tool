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

namespace QR_Tool.PageActivity
{
   [Activity(Label = "二维码测试助手", MainLauncher = true, Icon = "@drawable/icon")]
    public class HomeActivity : Activity
    {

        private LinearLayout toolPage = null;
        private LinearLayout generatebarcodepage = null;
        private LinearLayout scanbarcodrpage = null;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.HomePage);
            toolPage = (LinearLayout)FindViewById(Resource.Id.fun_1);
            toolPage.Click += toolPage_Click;
            scanbarcodrpage = (LinearLayout)FindViewById(Resource.Id.fun_2);
            scanbarcodrpage.Click += scanbarcodrpage_Click;

            generatebarcodepage = (LinearLayout)FindViewById(Resource.Id.fun_3);
            generatebarcodepage.Click += generatebarcodepage_Click;

            // Create your application here
        }

        private void scanbarcodrpage_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(ScanBarCodeActivity));
            StartActivity(intent);
        }

        private void generatebarcodepage_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(GenerateBarCodeActivity));
            StartActivity(intent);
        }

        private void toolPage_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(ToolPageActivity));
            StartActivity(intent);
    }
    }
}