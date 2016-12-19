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
using Com.Umeng.Analytics;
using Android.Support.V7.App;

namespace ZhihuFind.Droid.UI.Activitys
{
    [Activity]
    public abstract class BaseActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(LayoutResource);
        }
        protected abstract int LayoutResource
        {
            get;
        }
        protected override void OnResume()
        {
            base.OnResume();
            MobclickAgent.OnResume(this);
        }
        protected override void OnPause()
        {
            base.OnPause();
            MobclickAgent.OnPause(this);
        }
    }
}