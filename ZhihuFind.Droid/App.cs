using Android.App;
using Android.Runtime;
using Com.Umeng.Socialize;
using System;
using ZhihuFind.Droid.Utils;

namespace ZhihuFind.Droid
{
    [Application]
    public class App : Application
    {
        public App()
        {

        }
        public App(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {

        }
        public override void OnCreate()
        {
            base.OnCreate();
            UMShareAPI.Get(this);
            SQLiteUtils.Instance();
            PlatformConfig.SetWeixin("wxdad334f38b612cc7", "6dfd3bc792c8fc51f6f6f0bbe037bbfd");
        }
    }
}