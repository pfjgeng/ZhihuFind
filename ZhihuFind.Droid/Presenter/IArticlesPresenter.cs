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
using ZhihuFind.Droid.ViewModel;

namespace ZhihuFind.Droid.Presenter
{
    public interface IArticlesPresenter
    {
        Task GetServiceArticles(int offset);
        Task GetClientArticles();
    }
}