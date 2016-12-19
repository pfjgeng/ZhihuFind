using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Fragment = Android.Support.V4.App.Fragment;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using ZhihuFind.Droid.UI.Adapters;
using ZhihuFind.Droid.UI.Listeners;
using ZhihuFind.Droid.Utils;
using ZhihuFind.Droid.ViewModel;
using ZhihuFind.Droid.UI.Views;
using ZhihuFind.Droid.Presenter;

namespace ZhihuFind.Droid.UI.Fragments
{
    public class ArticlesFragment : Fragment, IArticlesView, IOnLoadMoreListener, SwipeRefreshLayout.IOnRefreshListener
    {
        private Handler handler;
        private SwipeRefreshLayout swipeRefreshLayout;
        private RecyclerView recyclerView;
        private ArticlesAdapter adapter;
        private int offset = 0;
        private IArticlesPresenter articlesPresenter;
        private List<int> offsetList = new List<int>();

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            handler = new Handler();
            articlesPresenter = new ArticlesPresenter(this);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            return inflater.Inflate(Resource.Layout.fragment_articles, container, false);
        }
        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            swipeRefreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            swipeRefreshLayout.SetColorSchemeResources(Resource.Color.primary);
            swipeRefreshLayout.SetOnRefreshListener(this);
            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView);
            recyclerView.SetLayoutManager(new LinearLayoutManager(this.Activity));
            adapter = new ArticlesAdapter();
            adapter.OnLoadMoreListener = this;

            recyclerView.SetAdapter(adapter);
            recyclerView.Post(async () =>
            {
                await articlesPresenter.GetClientArticles();
            });
        }
        public async void OnLoadMoreRequested()
        {
            await articlesPresenter.GetServiceArticles(offset);
        }

        public async void OnRefresh()
        {
            if (offset > 0)
                offset = 0;
            await articlesPresenter.GetServiceArticles(offset);
        }

        public void GetArticlesFail(string msg)
        {
            if (offset > 0)
            {
                adapter.ShowLoadMoreFailedView();
            }
            else
            {
                if (swipeRefreshLayout.Refreshing)
                {
                    swipeRefreshLayout.Refreshing = false;
                }
                Toast.MakeText(this.Activity, msg, ToastLength.Short).Show();
            }
        }

        public void GetArticlesSuccess(List<ArticleModel> lists)
        {
            if (offset == 0)
            {
                if (lists == null || lists.Count == 0)
                {
                    swipeRefreshLayout.Refreshing = true;
                    OnRefresh();
                }
                else
                {
                    if (swipeRefreshLayout.Refreshing)
                    {
                        swipeRefreshLayout.Refreshing = false;
                    }
                    adapter.NewData(lists);
                    adapter.RemoveAllFooterView();
                    offset += lists.Count;
                }
            }
            else
            {
                adapter.AddData(lists);
                offset += lists.Count;
            }
        }
        public override void OnHiddenChanged(bool hidden)
        {
            base.OnHiddenChanged(hidden);
            if (hidden)
            {
                if (swipeRefreshLayout.Refreshing)
                {
                    swipeRefreshLayout.Refreshing = false;
                }
            }
        }
    }
}