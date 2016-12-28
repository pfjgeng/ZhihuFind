using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Com.Umeng.Socialize;
using Com.Umeng.Socialize.Bean;
using Com.Umeng.Socialize.Media;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using FFImageLoading.Work;
using Square.Picasso;
using System;
using ZhihuFind.Droid.ViewModel;
using ZhihuFind.Droid.Presenter;
using ZhihuFind.Droid.UI.Listeners;
using ZhihuFind.Droid.UI.Views;
using ZhihuFind.Droid.UI.Widgets;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace ZhihuFind.Droid.UI.Activitys
{
    [Activity(Label = "")]
    public class ArticleActivity : BaseActivity, View.IOnClickListener, IArticleView, SwipeRefreshLayout.IOnRefreshListener, ViewTreeObserver.IOnScrollChangedListener, AppBarLayout.IOnOffsetChangedListener, Toolbar.IOnMenuItemClickListener
    {
        private int slug;
        private Handler handler;
        private IArticlePresenter articlePresenter;

        private Toolbar toolbar;
        private CoordinatorLayout coordinatorLayout;
        private CollapsingToolbarLayout collapsingToolbar;
        private AppBarLayout appbar;
        private SwipeRefreshLayout swipeRefreshLayout;
        private NestedScrollView scrollView;
        private TextView toolbarTitle;
        private ImageView titleImage;
        private ImageViewAsync imgAvatar;
        private ImageView org;
        private TextView txtAuthor;
        private TextView txtTitle;
        private TextView txtBio;
        private TextView txtTime;
        private ArticleWebView articleContent;
        private TextView txtGood;
        private TextView txtComments;

        private string title = "";
        private ArticleModel article;
        public static void Start(Context context, string slug)
        {
            Intent intent = new Intent(context, typeof(ArticleActivity));
            intent.PutExtra("slug", Convert.ToInt32(slug));
            context.StartActivity(intent);
        }
        protected override int LayoutResource
        {
            get { return Resource.Layout.article; }
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            slug = Intent.GetIntExtra("slug", 0);
            handler = new Handler();
            articlePresenter = new ArticlePresenter(this);

            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            toolbar.SetNavigationIcon(Resource.Drawable.back_24dp);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            toolbar.SetNavigationOnClickListener(this);
            toolbar.SetOnMenuItemClickListener(this);

            coordinatorLayout = FindViewById<CoordinatorLayout>(Resource.Id.main_content);
            collapsingToolbar = FindViewById<CollapsingToolbarLayout>(Resource.Id.collapsingtoolbar);

            appbar = FindViewById<AppBarLayout>(Resource.Id.appbar);
            appbar.AddOnOffsetChangedListener(this);

            swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            swipeRefreshLayout.SetColorSchemeResources(Resource.Color.primary);
            swipeRefreshLayout.SetOnRefreshListener(this);
            scrollView = FindViewById<NestedScrollView>(Resource.Id.scrollView);
            scrollView.ViewTreeObserver.AddOnScrollChangedListener(this);

            toolbarTitle = FindViewById<TextView>(Resource.Id.toolbarTitle);
            titleImage = FindViewById<ImageView>(Resource.Id.titleImage);
            imgAvatar = FindViewById<ImageViewAsync>(Resource.Id.llAvatar);
            org = FindViewById<ImageView>(Resource.Id.org);
            txtAuthor = FindViewById<TextView>(Resource.Id.txtAuthor);
            txtTitle = FindViewById<TextView>(Resource.Id.txtTitle);
            txtBio = FindViewById<TextView>(Resource.Id.txtBio);
            articleContent = FindViewById<ArticleWebView>(Resource.Id.postContent);
            txtTime = FindViewById<TextView>(Resource.Id.txtTime);
            txtGood = FindViewById<TextView>(Resource.Id.txtGood);
            txtComments = FindViewById<TextView>(Resource.Id.txtComments);

            swipeRefreshLayout.Post(async () =>
            {
                await articlePresenter.GetClientArticle(slug);
            });
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.share, menu);
            return true;
        }
        public bool OnMenuItemClick(IMenuItem item)
        {
            if (article != null)
            {
                new ShareAction(this).SetDisplayList(SHARE_MEDIA.Weixin, SHARE_MEDIA.WeixinCircle, SHARE_MEDIA.WeixinFavorite)
                                 .WithTitle(article.Title)
                                 .WithText(article.Title)
                                 .WithMedia(new UMImage(this, article.TitleImage))
                                 .WithTargetUrl(article.Url)
                                 .SetCallback(new UMShare(this))
                                 .Open();
            }
            return true;
        }
        public void OnClick(View v)
        {
            this.Finish();
        }
        public async void OnRefresh()
        {
            await articlePresenter.GetServiceArticle(slug);
        }

        public void GetArticleFail(string msg)
        {
            if (swipeRefreshLayout.Refreshing)
            {
                swipeRefreshLayout.Refreshing = false;
            }
            Toast.MakeText(this, msg, ToastLength.Short).Show();
        }

        public async void GetServiceArticleSuccess(ArticleModel article)
        {
            if (swipeRefreshLayout.Refreshing)
            {
                swipeRefreshLayout.Refreshing = false;
            }
            title = article.Title;
            txtAuthor.Text = article.Author.Name;

            if (article.Author.IsOrg)
            {
                org.Visibility = ViewStates.Visible;
                org.SetImageResource(Resource.Drawable.identity);
            }
            else
            {
                if (article.Author.Badge != null)
                {
                    org.Visibility = ViewStates.Visible;
                    if (article.Author.Badge.Identity != null)
                    {
                        org.SetImageResource(Resource.Drawable.identity);
                    }
                    else if (article.Author.Badge.Best_answerer != null)
                    {
                        org.SetImageResource(Resource.Drawable.bestanswerer);
                    }
                }
                else
                {
                    org.Visibility = ViewStates.Gone;
                }
            }
            txtBio.Text = article.Author.Bio;
            if (this.article == null || this.article.Content != article.Content)
            {
                var content = "<h1>" + article.Title + "</h1>" + article.Content;

                articleContent.Settings.JavaScriptEnabled = true;
                articleContent.Settings.CacheMode = CacheModes.CacheElseNetwork;
                var jsInterface = new WebViewJSInterface(this);
                articleContent.SetWebViewClient(DailyWebViewClient.With(this));
                articleContent.AddJavascriptInterface(jsInterface, "openlistner");
                articleContent.LoadRenderedContent(content);
                jsInterface.CallFromPageReceived += delegate (object sender, WebViewJSInterface.CallFromPageReceivedEventArgs e)
                {
                    switch (e.Type)
                    {
                        case WebViewJSInterface.CallFromType.Image:
                            PhotoActivity.Start(this, e.Result.Split(','), e.Index);
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
            }
            txtTime.Text = "创建于：" + Convert.ToDateTime(article.PublishedTime).ToString("yyyy-MM-dd");

            if (article.LikesCount > 0)
                txtGood.Text = article.LikesCount.ToString();
            if (article.CommentsCount > 0)
            {
                txtComments.Text = article.CommentsCount.ToString();
                (txtComments.Parent as FrameLayout).Click += delegate
                {
                    ArticleCommentActivity.Start(this, slug);
                };
            }
            if (article.TitleImage != "")
            {
                Picasso.With(this)
                            .Load(article.TitleImage)
                           .Into(titleImage);
            }
            else
            {
                appbar.LayoutParameters = new CoordinatorLayout.LayoutParams(CoordinatorLayout.LayoutParams.MatchParent, toolbar.Height);
                toolbarTitle.Text = title;
            }
            var avatar = article.Author.Avatar.Template.Replace("{id}", article.Author.Avatar.Id);
            avatar = avatar.Replace("{size}", "l");
            await ImageService.Instance.LoadUrl(avatar)
                  .Retry(3, 200)
                  .DownSample(40, 40)
                  .Transform(new CircleTransformation())
                  .LoadingPlaceholder("ic_placeholder.png", ImageSource.ApplicationBundle)
                  .ErrorPlaceholder("ic_placeholder.png", ImageSource.ApplicationBundle)
                  .IntoAsync(imgAvatar);

            this.article = article;
        }

        public void GetClientArticleSuccess(ArticleModel article)
        {
            GetServiceArticleSuccess(article);
            if (article.UpdateTime != DateTime.MinValue && article.UpdateTime.AddMinutes(15) < DateTime.Now)
            {
                OnRefresh();
            }
        }
        public void OnScrollChanged()
        {
            swipeRefreshLayout.Enabled = scrollView.ScrollY == 0;
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
            else if (Math.Abs(verticalOffset) >= layout.TotalScrollRange)
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
    }
}