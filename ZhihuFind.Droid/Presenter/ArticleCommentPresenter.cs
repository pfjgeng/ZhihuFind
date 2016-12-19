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
    public class ArticleCommentPresenter : IArticleCommentPresenter
    {
        private IArticleCommentView commentView;
        private int limit = 10;
        public ArticleCommentPresenter(IArticleCommentView commentView)
        {
            this.commentView = commentView;
        }
        public async Task GetComment(int slug,int offset)
        {
            try
            {
                var comments = JsonConvert.DeserializeObject<List<ArticleCommentModel>>(await OkHttpUtils.Instance.GetAsyn(ApiUtils.GetArticleComment(slug, limit,offset)));
                commentView.GetCommentSuccess(comments);
            }
            catch (Exception ex)
            {
                commentView.GetCommentFail(ex.Message);
            }
        }
    }
}