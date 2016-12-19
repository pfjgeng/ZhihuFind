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
using ZhihuFind.Droid.Utils;
using Newtonsoft.Json;
using ZhihuFind.Droid.ViewModel;
using System.IO;
using ZhihuFind.Droid.UI.Views;

namespace ZhihuFind.Droid.Presenter
{
    public class MainPresenter : IMainPresenter
    {
        private IMainView mainView;

        public MainPresenter(IMainView mainView)
        {
            this.mainView = mainView;
        }
        public void SwitchNavigationBar(int id)
        {
            switch (id)
            {
                case Resource.Id.bb_menu_dailys:
                    mainView.SwitchDailys();
                    break;
                case Resource.Id.bb_menu_articles:
                    mainView.SwitchArticles();
                    break;
                default:
                    break;
            }
        }
        public void HideNavigationBar(int id)
        {
            switch (id)
            {
                case Resource.Id.bb_menu_dailys:
                    mainView.HideDailys();
                    break;
                case Resource.Id.bb_menu_articles:
                    mainView.HideArticles();
                    break;
                default:
                    break;
            }
        }
    }
}