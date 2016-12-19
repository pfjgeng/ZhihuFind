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
    public class DailyCommentActivity : BaseActivity, View.IOnClickListener, IDailyCommentView, IOnLoadMoreListener, SwipeRefreshLayout.IOnRefreshListener
    {
        private string id;
        private Handler handler;
        private IDailyCommentPresenter commentPresenter;

        private Toolbar toolbar;
        private SwipeRefreshLayout swipeRefreshLayout;
        private RecyclerView recyclerView;
        private DailyCommentsAdapter adapter;
        private View notLoadingView;
        public static void Start(Context context, string id)
        {
            Intent intent = new Intent(context, typeof(DailyCommentActivity));
            intent.PutExtra("id", id);
            context.StartActivity(intent);
        }
        protected override int LayoutResource
        {
            get { return Resource.Layout.comment; }
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            id = Intent.GetStringExtra("id");
            handler = new Handler();
            commentPresenter = new DailyCommentPresenter(this);

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
            adapter = new DailyCommentsAdapter();
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
            if (swipeRefreshLayout.Refreshing)
            {
                swipeRefreshLayout.Refreshing = false;
            }
            Toast.MakeText(this, msg, ToastLength.Short).Show();
        }

        public void GetCommentSuccess(List<DailyCommentModel> comments)
        {
            if (swipeRefreshLayout.Refreshing)
            {
                swipeRefreshLayout.Refreshing = false;
            }
            adapter.NewData(comments);
            adapter.RemoveAllFooterView();
        }

        public async void OnRefresh()
        {
            await commentPresenter.GetComment(id);
        }

        public void OnLoadMoreRequested()
        {
            handler.Post(() =>
            {
                adapter.LoadComplete();
                if (notLoadingView == null)
                {
                    notLoadingView = LayoutInflater.Inflate(Resource.Layout.recyclerview_notloading, (ViewGroup)recyclerView.Parent, false);
                }
                adapter.RemoveAllFooterView();
                adapter.AddFooterView(notLoadingView);
            });
        }
    }
}