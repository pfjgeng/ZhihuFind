using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Com.Umeng.Analytics;
using Com.Umeng.Socialize;
using Com.Umeng.Socialize.Bean;
using Com.Umeng.Socialize.Media;
using Square.Picasso;
using System;
using ZhihuFind.Droid.ViewModel;
using ZhihuFind.Droid.Presenter;
using ZhihuFind.Droid.UI.Listeners;
using ZhihuFind.Droid.UI.Views;
using ZhihuFind.Droid.UI.Widgets;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Java.Lang;
using Android.Runtime;
using Android.Webkit;

namespace ZhihuFind.Droid.UI.Activitys
{
    [Activity(Label = "")]
    public class DailyActivity : BaseActivity, View.IOnClickListener, IDailyView, SwipeRefreshLayout.IOnRefreshListener, ViewTreeObserver.IOnScrollChangedListener, AppBarLayout.IOnOffsetChangedListener, Toolbar.IOnMenuItemClickListener
    {
        private int id;
        private Handler handler;
        private IDailyPresenter dailyPresenter;

        private Toolbar toolbar;
        private CoordinatorLayout coordinatorLayout;
        private CollapsingToolbarLayout collapsingToolbar;
        private AppBarLayout appbar;
        private SwipeRefreshLayout swipeRefreshLayout;
        private NestedScrollView scrollView;
        private TextView toolbarTitle;
        private ImageView titleImage;
        private TextView txtTitle;
        private TextView txtAuthor;
        private DailyWebView body;
        private TextView txtGood;
        private TextView txtComments;
        private string title = "";
        private DailyModel daily;
        public static void Start(Context context, string id)
        {
            Intent intent = new Intent(context, typeof(DailyActivity));
            intent.PutExtra("id", Convert.ToInt32(id));
            context.StartActivity(intent);
        }
        protected override int LayoutResource
        {
            get { return Resource.Layout.daily; }
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            id = Intent.GetIntExtra("id", 0);
            handler = new Handler();
            dailyPresenter = new DailyPresenter(this);

            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            toolbar.SetNavigationIcon(Resource.Drawable.back_24dp);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            toolbar.SetNavigationOnClickListener(this);
            toolbar.SetOnMenuItemClickListener(this);

            coordinatorLayout = FindViewById<CoordinatorLayout>(Resource.Id.main_content);
            collapsingToolbar = FindViewById<CollapsingToolbarLayout>(Resource.Id.collapsingtoolbar);
            collapsingToolbar.SetTitle("");
            appbar = FindViewById<AppBarLayout>(Resource.Id.appbar);
            appbar.AddOnOffsetChangedListener(this);

            swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            swipeRefreshLayout.SetColorSchemeResources(Resource.Color.primary);
            swipeRefreshLayout.SetOnRefreshListener(this);
            scrollView = FindViewById<NestedScrollView>(Resource.Id.scrollView);
            scrollView.ViewTreeObserver.AddOnScrollChangedListener(this);

            toolbarTitle = FindViewById<TextView>(Resource.Id.toolbarTitle);
            txtTitle = FindViewById<TextView>(Resource.Id.txtTitle);
            txtAuthor = FindViewById<TextView>(Resource.Id.txtAuthor);
            titleImage = FindViewById<ImageView>(Resource.Id.titleImage);
            body = FindViewById<DailyWebView>(Resource.Id.body);
            txtGood = FindViewById<TextView>(Resource.Id.txtGood);
            txtComments = FindViewById<TextView>(Resource.Id.txtComments);

            swipeRefreshLayout.Post(async () =>
            {
                await dailyPresenter.GetClientDaily(id);
                await dailyPresenter.GetClientDailyExtra(id);
            });
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.share, menu);
            return true;
        }
        public bool OnMenuItemClick(IMenuItem item)
        {
            if (daily != null)
            {
                new ShareAction(this).SetDisplayList(SHARE_MEDIA.Weixin, SHARE_MEDIA.WeixinCircle, SHARE_MEDIA.WeixinFavorite)
                                 .WithTitle(daily.title)
                                 .WithText(daily.title)
                                 .WithMedia(new UMImage(this, daily.image))
                                 .WithTargetUrl(daily.share_url)
                                 .SetCallback(new UMShare(this))
                                 .Open();
            }
            return true;
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            UMShareAPI.Get(this).OnActivityResult(requestCode, (int)resultCode, data);
        }
        public async void OnRefresh()
        {
            await dailyPresenter.GetServiceDaily(id);
            await dailyPresenter.GetServiceDailyExtra(id);
        }

        public void GetServiceDailyFail(string msg)
        {
            if (swipeRefreshLayout.Refreshing)
            {
                swipeRefreshLayout.Refreshing = false;
            }
            Toast.MakeText(this, msg, ToastLength.Short).Show();
        }

        public void GetServiceDailySuccess(DailyModel daily)
        {
            if (swipeRefreshLayout.Refreshing)
            {
                swipeRefreshLayout.Refreshing = false;
            }
            if (daily != null && daily.id > 0)
            {
                this.daily = daily;
                title = daily.title;
                txtTitle.Text = daily.title;
                txtAuthor.Text = daily.image_source;
                body.Settings.JavaScriptEnabled = true;
                body.Settings.DomStorageEnabled = true;
                var jsInterface = new WebViewJSInterface(this);
                body.SetWebViewClient(DailyWebViewClient.With(this));
                body.AddJavascriptInterface(jsInterface, "openlistner");
                body.LoadRenderedContent(daily.body.Replace("img-place-holder", "img-place-holder1"));
                jsInterface.CallFromPageReceived += delegate (object sender, WebViewJSInterface.CallFromPageReceivedEventArgs e)
                {
                    switch (e.Type)
                    {
                        case WebViewJSInterface.CallFromType.Image:
                            PhotoActivity.Start(this, e.Result.Split(','),e.Index);
                            break;
                        case WebViewJSInterface.CallFromType.Href:
                            Intent intent = new Intent();
                            intent.SetAction("android.intent.action.VIEW");
                            intent.SetData(Android.Net.Uri.Parse(e.Result));
                            intent.SetClassName("com.android.browser", "com.android.browser.BrowserActivity");
                            StartActivity(intent);
                            break;
                    }
                };
                if (daily.image != "")
                {
                    Picasso.With(this).Load(daily.image).Into(titleImage);
                }
                else
                {
                    appbar.LayoutParameters = new CoordinatorLayout.LayoutParams(CoordinatorLayout.LayoutParams.MatchParent, toolbar.Height);
                    toolbarTitle.Text = title;
                }
            }
        }
        public void GetClientDailySuccess(DailyModel daily)
        {
            GetServiceDailySuccess(daily);
            if (daily.updatetime.AddMinutes(15) < DateTime.Now)
            {
                swipeRefreshLayout.Refreshing = true;
                OnRefresh();
            }
        }

        public void GetDailyExtraFail(string msg)
        {
        }
        public void GetDailyExtraSuccess(DailyExtraModel extra)
        {
            if (extra.popularity > 0)
                txtGood.Text = extra.popularity.ToString();
            if (extra.comments > 0)
            {
                txtComments.Text = extra.comments.ToString();
                (txtComments.Parent as FrameLayout).Click += delegate
                {
                    DailyCommentActivity.Start(this, id.ToString());
                };
            }
        }
        public void OnClick(View v)
        {
            this.Finish();
        }
        public void OnOffsetChanged(AppBarLayout layout, int verticalOffset)
        {
            if (verticalOffset == 0)
            {
                if (CollapsingState != CollapsingToolbarLayoutState.Expanded)
                {
                    CollapsingState = CollapsingToolbarLayoutState.Expanded;//修改状态标记为展开
                    toolbarTitle.Text = "";//设置title不显示
                }
            }
            else if (System.Math.Abs(verticalOffset) >= layout.TotalScrollRange)
            {
                if (CollapsingState != CollapsingToolbarLayoutState.Collapsed)
                {
                    toolbarTitle.Text = title;//设置title显示
                    CollapsingState = CollapsingToolbarLayoutState.Collapsed;//修改状态标记为折叠
                }
            }
            else
            {
                if (CollapsingState != CollapsingToolbarLayoutState.Internediate)
                {
                    CollapsingState = CollapsingToolbarLayoutState.Internediate;//修改状态标记为中间
                    toolbarTitle.Text = "";//设置title不显示
                }
            }
        }

        private CollapsingToolbarLayoutState CollapsingState;

        private enum CollapsingToolbarLayoutState
        {
            Expanded,
            Collapsed,
            Internediate
        }

        public void OnScrollChanged()
        {
            swipeRefreshLayout.Enabled = scrollView.ScrollY == 0;
        }
    }
}