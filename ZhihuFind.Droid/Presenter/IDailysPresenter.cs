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
    interface IDailysPresenter
    {
        Task GetServiceDailys(string date = null);
        Task GetClientDailys();
    }
}