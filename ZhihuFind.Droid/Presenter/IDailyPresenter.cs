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
using System.Threading.Tasks;

namespace ZhihuFind.Droid.Presenter
{
    interface IDailyPresenter
    {
        Task GetServiceDaily(int id);
        Task GetServiceDailyExtra(int id);
        Task GetClientDaily(int id);
        Task GetClientDailyExtra(int id);
    }
}