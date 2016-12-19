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
    public interface IArticleView
    {
        void GetArticleFail(string msg);
        void GetServiceArticleSuccess(ArticleModel article);
        void GetClientArticleSuccess(ArticleModel article);
    }
}