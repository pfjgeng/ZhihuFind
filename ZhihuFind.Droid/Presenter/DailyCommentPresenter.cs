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
    public class DailyCommentPresenter : IDailyCommentPresenter
    {
        private IDailyCommentView commentView;
        public DailyCommentPresenter(IDailyCommentView commentView)
        {
            this.commentView = commentView;
        }
        public async Task GetComment(string id)
        {
            try
            {
                var comments = new List<DailyCommentModel>();

                var longs = JsonConvert.DeserializeObject<Comments>(await OkHttpUtils.Instance.GetAsyn(ApiUtils.GetDailyCommentLong(id)));
                comments.AddRange(longs.comments);

                var shorts = JsonConvert.DeserializeObject<Comments>(await OkHttpUtils.Instance.GetAsyn(ApiUtils.GetDailyCommentShort(id)));
                comments.AddRange(shorts.comments);

                commentView.GetCommentSuccess(comments);
            }
            catch (Exception ex)
            {
                commentView.GetCommentFail(ex.Message);
            }
        }
        private class Comments
        {
            public List<DailyCommentModel> comments { get; set; }
        }
    }
}