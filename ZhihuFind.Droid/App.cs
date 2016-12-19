using Android.App;
using Android.Runtime;
using Com.Umeng.Socialize;
using System;

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
            PlatformConfig.SetWeixin("wxdad334f38b612cc7", "b3155bbdc192edf3910e6fb799e77b1f");
        }
    }
}