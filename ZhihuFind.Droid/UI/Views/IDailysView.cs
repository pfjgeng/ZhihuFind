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
    public interface IDailysView
    {
        void GetServiceDailysFail(string msg);
        void GetServiceDailysSuccess(string date, List<DailysModel> lists);
        void GetServiceTopDailysSuccess(List<TopDailysModel> lists);
        void GetClientDailysSuccess(string date, List<DailysModel> lists);
        void GetClientTopDailysSuccess(List<TopDailysModel> lists);
    }
}