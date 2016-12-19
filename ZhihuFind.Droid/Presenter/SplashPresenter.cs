using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ZhihuFind.Droid.UI.Views;
using Newtonsoft.Json;
using ZhihuFind.Droid.Utils;
using ZhihuFind.Droid.ViewModel;

namespace ZhihuFind.Droid.Presenter
{
    public class SplashPresenter : ISplashPresenter
    {
        private ISplashView splashView;
        public SplashPresenter(ISplashView splashView)
        {
            this.splashView = splashView;
        }
        public async Task GetStartImage()
        {
            try
            {
                var img = JsonConvert.DeserializeObject<StartImageModel>(await OkHttpUtils.Instance.GetAsyn(ApiUtils.GetStartImage()));
                splashView.GetStartImageSuccess(img);
            }
            catch (Exception ex)
            {
                splashView.GetStartImageFail(ex.Message);
            }
        }
        
    }
}