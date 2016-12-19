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
using Square.OkHttp;
using Java.Util.Concurrent;
using Java.Net;

namespace ZhihuFind.Droid.Presenter
{
    public class DailysPresenter : IDailysPresenter
    {
        private IDailysView dailysView;
        public DailysPresenter(IDailysView dailysView)
        {
            this.dailysView = dailysView;
        }
        public async Task GetServiceDailys(string date = null)
        {
            try
            {
                var dailys = JsonConvert.DeserializeObject<DailysModel>(await OkHttpUtils.Instance.GetAsyn(date == null ? ApiUtils.GetDailyLatest() : ApiUtils.GetDailyBefore(date)));

                foreach (var item in dailys.Stories)
                {
                    try
                    {
                        await Task.Run(async () =>
                         {
                             var extra = JsonConvert.DeserializeObject<DailyExtraModel>(await OkHttpUtils.Instance.GetAsyn(ApiUtils.GetDailyExtra(item.Id.ToString())));
                             item.extra = extra;
                         });
                    }
                    catch
                    {
                    }
                }
                dailysView.GetServiceDailysSuccess(dailys.Date, dailys.Stories);
                if (date == null)
                {
                    dailysView.GetServiceTopDailysSuccess(dailys.Top_stories);

                    await SQLiteUtils.Instance.DeleteAllDailys();
                    await SQLiteUtils.Instance.UpdateAllDailys(dailys.Stories);

                    await SQLiteUtils.Instance.DeleteAllTopDailys();
                    await SQLiteUtils.Instance.UpdateAllTopDailys(dailys.Top_stories);
                }
            }
            catch (Exception ex)
            {
                dailysView.GetServiceDailysFail(ex.Message);
            }
        }
        public async Task GetClientDailys()
        {
            try
            {
                var dailys = await SQLiteUtils.Instance.QueryAllTopDailys();
                if (dailys.Count > 0)
                {
                    dailys = dailys.OrderByDescending(d => d.Id).ToList();
                    dailysView.GetClientTopDailysSuccess(dailys);
                }
            }
            catch (Exception)
            {

            }
            try
            {
                var dailys = await SQLiteUtils.Instance.QueryAllDailys();
                if (dailys.Count > 0)
                {
                    dailys = dailys.OrderByDescending(d => d.Id).ToList();
                    dailysView.GetClientDailysSuccess(dailys[0].Date, dailys);
                }
            }
            catch (Exception)
            {

            }
        }
        public class DailysModel
        {
            public string Date { get; set; }
            public List<ViewModel.DailysModel> Stories { get; set; }
            public List<ViewModel.TopDailysModel> Top_stories { get; set; }
        }
    }
}