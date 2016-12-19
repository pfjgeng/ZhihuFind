using Android.OS;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Util;
using Square.Picasso;
using System;
using System.Collections.Generic;
using ZhihuFind.Droid.ViewModel;
using ZhihuFind.Droid.Presenter;
using ZhihuFind.Droid.UI.Activitys;
using ZhihuFind.Droid.UI.Adapters;
using ZhihuFind.Droid.UI.Listeners;
using ZhihuFind.Droid.UI.Views;
using Fragment = Android.Support.V4.App.Fragment;

namespace ZhihuFind.Droid.UI.Fragments
{
    public class DailysFragment : Fragment, IDailysView, IOnLoadMoreListener, SwipeRefreshLayout.IOnRefreshListener
    {
        private Handler handler;
        private SwipeRefreshLayout swipeRefreshLayout;
        private RecyclerView recyclerView;
        private DailysAdapter adapter;
        private string date = null;
        private IDailysPresenter dailysPresenter;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            handler = new Handler();
            dailysPresenter = new DailysPresenter(this);
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            return inflater.Inflate(Resource.Layout.fragment_dailys, container, false);
        }
        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            swipeRefreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            swipeRefreshLayout.SetColorSchemeResources(Resource.Color.primary);
            swipeRefreshLayout.SetOnRefreshListener(this);

            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView);
            var manager = new LinearLayoutManager(this.Activity);
            recyclerView.SetLayoutManager(manager);

            adapter = new DailysAdapter();
            adapter.OnLoadMoreListener = this;

            recyclerView.SetAdapter(adapter);
            recyclerView.Post(() =>
            {
                swipeRefreshLayout.Refreshing = true;
                OnRefresh();
            });
        }
        public override async void OnResume()
        {
            base.OnResume();
            if (this.date == null)
            {
                await dailysPresenter.GetClientDailys();
            }
        }
        public async void OnLoadMoreRequested()
        {
            await dailysPresenter.GetServiceDailys(date);
        }
        public async void OnRefresh()
        {
            if (date != null)
                date = null;
            await dailysPresenter.GetServiceDailys();
        }
        public void GetServiceDailysFail(string msg)
        {
            if (date != null)
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
        public void GetServiceTopDailysSuccess(List<TopDailysModel> lists)
        {
            adapter.ChangeHaderView(lists);
            recyclerView.SmoothScrollToPosition(0);
        }
        public void GetServiceDailysSuccess(string date, List<DailysModel> lists)
        {
            handler.Post(() =>
            {
                lists[0].Date = date;
                if (this.date == null)
                {
                    if (swipeRefreshLayout.Refreshing)
                    {
                        swipeRefreshLayout.Refreshing = false;
                    }
                    adapter.NewData(lists);
                    adapter.RemoveAllFooterView();
                }
                else
                {
                    adapter.AddData(lists);
                }
                this.date = date;
            });
        }
        public void GetClientTopDailysSuccess(List<TopDailysModel> lists)
        {
            adapter.AddHader(lists);
            recyclerView.SmoothScrollToPosition(0);
        }
        public void GetClientDailysSuccess(string date, List<DailysModel> lists)
        {
            adapter.NewData(lists);
            adapter.RemoveAllFooterView();
        }
    }
}