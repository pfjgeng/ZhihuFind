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

namespace ZhihuFind.Droid.UI.Views
{
    public interface IMainView
    {
        void SwitchDailys();
        void SwitchArticles();
        void HideDailys();
        void HideArticles();
    }
}