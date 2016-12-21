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
    public class DailyPresenter : IDailyPresenter
    {
        private IDailyView dailyView;
        public DailyPresenter(IDailyView dailyView)
        {
            this.dailyView = dailyView;
        }
        public async Task GetServiceDaily(int id)
        {
            try
            {
                var daily = JsonConvert.DeserializeObject<DailyModel>(await OkHttpUtils.Instance.GetAsyn(ApiUtils.GetDaily(id)));
                await SQLiteUtils.Instance().UpdateDaily(daily);
                dailyView.GetServiceDailySuccess(daily);
            }
            catch (Exception ex)
            {
                dailyView.GetServiceDailyFail(ex.Message);
            }
        }

        public async Task GetClientDaily(int id)
        {
            try
            {
                dailyView.GetClientDailySuccess(await SQLiteUtils.Instance().QueryDaily(id));
            }
            catch (Exception ex)
            {
                dailyView.GetServiceDailyFail(ex.Message);
            }
        }

        public async Task GetServiceDailyExtra(int id)
        {
            try
            {
                var extra = JsonConvert.DeserializeObject<DailyExtraModel>(await OkHttpUtils.Instance.GetAsyn(ApiUtils.GetDailyExtra(id.ToString())));
                extra.id = id;
                await SQLiteUtils.Instance().UpdateDailyExtra(extra);
                dailyView.GetDailyExtraSuccess(extra);
            }
            catch (Exception ex)
            {
                dailyView.GetDailyExtraFail(ex.Message);
            }
        }
        public async Task GetClientDailyExtra(int id)
        {
            try
            {
                dailyView.GetDailyExtraSuccess(await SQLiteUtils.Instance().QueryDailyExtra(id));
            }
            catch (Exception ex)
            {
                dailyView.GetServiceDailyFail(ex.Message);
            }
        }
    }
}