using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using ZhihuFind.Droid.UI.Fragments;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using ZhihuFind.Droid.UI.Views;
using ZhihuFind.Droid.Presenter;
using BottomNavigationBar;
using BottomNavigationBar.Listeners;
using Com.Umeng.Analytics;
using Android.Support.Design.Widget;
using Android.Content;

namespace ZhihuFind.Droid.UI.Activitys
{
    [Activity(LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
    public class MainActivity : BaseActivity, IMainView, IOnMenuTabClickListener
    {
        private Toolbar toolbar;
        private BottomBar bottomBar;
        private int lastSelecteID;//上一次选中的menuItemId
        private FragmentManager fm;
        private ArticlesFragment articleaFragment;
        private DailysFragment dailysFragment;
        private IMainPresenter mainPresenter;

        public static void Start(Context context)
        {
            Intent intent = new Intent(context, typeof(MainActivity));
            context.StartActivity(intent);
        }
        protected override int LayoutResource
        {
            get { return Resource.Layout.Main; }
        }
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            mainPresenter = new MainPresenter(this);

            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            toolbar.Title = Resources.GetString(Resource.String.daily);
            SetSupportActionBar(toolbar);

            fm = SupportFragmentManager;

            bottomBar = BottomBar.AttachShy((CoordinatorLayout)FindViewById(Resource.Id.coordinatorLayout), FindViewById(Resource.Id.swipeRefreshLayout), bundle);

            bottomBar.UseFixedMode();

            bottomBar.SetItems(Resource.Menu.bottombar_menu);
            bottomBar.SetOnMenuTabClickListener(this);

            MobclickAgent.SetDebugMode(true);
            MobclickAgent.OpenActivityDurationTrack(false);
            MobclickAgent.SetScenarioType(this, MobclickAgent.EScenarioType.EUmNormal);
        }
        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            bottomBar.OnSaveInstanceState(outState);
        }
        public void OnMenuTabSelected(int menuItemId)
        {
            if (lastSelecteID > 0)
            {
                mainPresenter.HideNavigationBar(lastSelecteID);
            }
            mainPresenter.SwitchNavigationBar(menuItemId);
            lastSelecteID = menuItemId;
        }

        public void OnMenuTabReSelected(int menuItemId)
        {
        }

        public void SwitchDailys()
        {
            FragmentTransaction transaction = fm.BeginTransaction();
            if (dailysFragment == null)
            {
                dailysFragment = new DailysFragment();
                transaction.Add(Resource.Id.frameContent, dailysFragment).Commit();
            }
            else
            {
                transaction.Show(dailysFragment).Commit();
            }
            toolbar.Title = Resources.GetString(Resource.String.daily);
        }

        public void SwitchArticles()
        {
            FragmentTransaction transaction = fm.BeginTransaction();
            if (articleaFragment == null)
            {
                articleaFragment = new ArticlesFragment();
                transaction.Add(Resource.Id.frameContent, articleaFragment).Commit();
            }
            else
            {
                transaction.Show(articleaFragment).Commit();
            }
            toolbar.Title = Resources.GetString(Resource.String.article);
        }


        public void HideDailys()
        {
            FragmentTransaction transaction = fm.BeginTransaction();
            if (dailysFragment != null)
            {
                transaction.Hide(dailysFragment).Commit();
            }
        }

        public void HideArticles()
        {
            FragmentTransaction transaction = fm.BeginTransaction();
            if (articleaFragment != null)
            {
                transaction.Hide(articleaFragment).Commit();
            }
        }

    }
}

