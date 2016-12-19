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
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using ZhihuFind.Droid.Presenter;
using ZhihuFind.Droid.UI.Views;
using ZhihuFind.Droid.ViewModel;
using Android.Support.V4.Widget;
using Com.Umeng.Analytics;
using Android.Support.V7.Widget;
using ZhihuFind.Droid.UI.Adapters;
using ZhihuFind.Droid.UI.Listeners;

namespace ZhihuFind.Droid.UI.Activitys
{
    [Activity(Label = "@string/comments")]
    public class ArticleCommentActivity : BaseActivity, View.IOnClickListener, IArticleCommentView, IOnLoadMoreListener, SwipeRefreshLayout.IOnRefreshListener
    {
        private int slug;
        private Handler handler;
        private IArticleCommentPresenter commentPresenter;

        private Toolbar toolbar;
        private SwipeRefreshLayout swipeRefreshLayout;
        private RecyclerView recyclerView;
        private ArticleCommentsAdapter adapter;
        private View notLoadingView;
        private int offset = 0;
        public static void Start(Context context, int slug)
        {
            Intent intent = new Intent(context, typeof(ArticleCommentActivity));
            intent.PutExtra("slug", slug);
            context.StartActivity(intent);
        }
        protected override int LayoutResource
        {
            get { return Resource.Layout.comment; }
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            slug = Intent.GetIntExtra("slug", 0);
            handler = new Handler();
            commentPresenter = new ArticleCommentPresenter(this);

            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            toolbar.SetNavigationIcon(Resource.Drawable.back_24dp);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            toolbar.SetNavigationOnClickListener(this);

            swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            swipeRefreshLayout.SetColorSchemeResources(Resource.Color.primary);
            swipeRefreshLayout.SetOnRefreshListener(this);

            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            recyclerView.SetLayoutManager(new LinearLayoutManager(this));
            adapter = new ArticleCommentsAdapter();
            adapter.OnLoadMoreListener = this;

            recyclerView.SetAdapter(adapter);
            recyclerView.Post(() =>
            {
                swipeRefreshLayout.Refreshing = true;
                OnRefresh();
            });
        }

        public void OnClick(View v)
        {
            this.Finish();
        }
        public void GetCommentFail(string msg)
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
                Toast.MakeText(this, msg, ToastLength.Short).Show();
            }
        }

        public void GetCommentSuccess(List<ArticleCommentModel> comments)
        {
            if (comments.Count > 0)
            {
                if (offset == 0)
                {
                    handler.Post(() =>
                    {
                        if (swipeRefreshLayout.Refreshing)
                        {
                            swipeRefreshLayout.Refreshing = false;
                        }
                        adapter.NewData(comments);
                        adapter.RemoveAllFooterView();
                        offset += comments.Count;
                    });
                }
                else
                {
                    adapter.AddData(comments);
                    offset += comments.Count;
                }
            }
            else
            {
                adapter.LoadComplete();
                if (notLoadingView == null)
                {
                    notLoadingView = LayoutInflater.Inflate(Resource.Layout.recyclerview_notloading, (ViewGroup)recyclerView.Parent, false);
                }
                adapter.RemoveAllFooterView();
                adapter.AddFooterView(notLoadingView);
            }
        }

        public async void OnRefresh()
        {
            if (offset > 0)
                offset = 0;
            await commentPresenter.GetComment(slug, offset);
        }

        public async void OnLoadMoreRequested()
        {
            await commentPresenter.GetComment(slug, offset);
        }
    }
}