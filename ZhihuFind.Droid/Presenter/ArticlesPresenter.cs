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
    public class ArticlesPresenter : IArticlesPresenter
    {
        private int limit = 10;
        private IArticlesView articlesView;
        public ArticlesPresenter(IArticlesView articlesView)
        {
            this.articlesView = articlesView;
        }
        public async Task GetServiceArticles(int offset)
        {
            try
            {
                var articles = JsonConvert.DeserializeObject<List<ArticleModel>>(await OkHttpUtils.Instance.GetAsyn(ApiUtils.GetRecommendationArticles(limit, offset)));
                await SQLiteUtils.Instance.UpdateArticles(articles);
                articlesView.GetArticlesSuccess(articles);
            }
            catch (Exception ex)
            {
                articlesView.GetArticlesFail(ex.Message);
            }
        }
        public async Task GetClientArticles()
        {
            try
            {
                articlesView.GetArticlesSuccess(await SQLiteUtils.Instance.QueryArticles(limit));
            }
            catch (Exception ex)
            {
                articlesView.GetArticlesFail(ex.Message);
            }
        }
    }
}