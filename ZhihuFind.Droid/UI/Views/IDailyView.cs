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
using ZhihuFind.Droid.ViewModel;

namespace ZhihuFind.Droid.UI.Views
{
    public interface IDailyView
    {
        void GetServiceDailyFail(string msg);
        void GetServiceDailySuccess(DailyModel daily);
        void GetDailyExtraFail(string msg);
        void GetDailyExtraSuccess(DailyExtraModel extra);
        void GetClientDailySuccess(DailyModel daily);
    }
}