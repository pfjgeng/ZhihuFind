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
using ZhihuFind.Droid.ViewModel;
using ZhihuFind.Droid.UI.Views;
using Newtonsoft.Json;
using ZhihuFind.Droid.Utils;

namespace ZhihuFind.Droid.Presenter
{
    public class ArticlePresenter : IArticlePresenter
    {
        IArticleView articleView;
        public ArticlePresenter(IArticleView articleView)
        {
            this.articleView = articleView;
        }
        public async Task GetServiceArticle(int slug)
        {
            try
            {
                var article = JsonConvert.DeserializeObject<ArticleModel>(await OkHttpUtils.Instance.GetAsyn(ApiUtils.GetArticle(slug)));
                article.UpdateTime = DateTime.Now;
                await SQLiteUtils.Instance.UpdateArticle(article);
                articleView.GetServiceArticleSuccess(article);
            }
            catch (Exception ex)
            {
                articleView.GetArticleFail(ex.Message);
            }
        }
        public async Task GetClientArticle(int slug)
        {
            try
            {
                articleView.GetClientArticleSuccess(await SQLiteUtils.Instance.QueryArticle(slug));
            }
            catch (Exception ex)
            {
                articleView.GetArticleFail(ex.Message);
            }
        }
    }
}