using CoreAudioKit;
using Foundation;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;

namespace ZhihuFind.IOS.Controllers
{
    public class TabBarController : UITabBarController
    {
        UIViewController tabDailys, tabArticles;
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            tabDailys = new UIViewController();
            tabDailys.View.BackgroundColor = UIColor.White;            
            tabDailys.TabBarItem = new UITabBarItem("日报", UIImage.FromFile("Images/daily.png"), 0);

            tabArticles = new UIViewController();
            tabArticles.View.BackgroundColor = UIColor.White;
            tabArticles.TabBarItem = new UITabBarItem("文章", UIImage.FromFile("Images/form.png"), 1);
            tabDailys.NavigationItem.Title = "文章";

            var tabs = new UIViewController[] { tabDailys, tabArticles };
            ViewControllers = tabs;
            SelectedViewController = tabDailys;
            this.Title = "日报";
        }        
    }
}
