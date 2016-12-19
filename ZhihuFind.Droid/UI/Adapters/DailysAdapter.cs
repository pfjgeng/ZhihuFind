using Android.Content;
using Android.OS;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Views;
using FFImageLoading.Work;
using Java.Util;
using Square.Picasso;
using System.Collections.Generic;
using ZhihuFind.Droid.ViewModel;
using ZhihuFind.Droid.UI.Activitys;
using ZhihuFind.Droid.UI.Listeners;
using System;
using FFImageLoading.Transformations;

namespace ZhihuFind.Droid.UI.Adapters
{
    public class DailysAdapter : RecyclerView.Adapter, View.IOnClickListener
    {
        public const int LoadingView = 0x00000111;
        public const int FooterView = 0x00000222;
        public const int HaderView = 0x00000333;
        private Context context;
        protected LayoutInflater layoutInflater;
        private LinearLayout footerLayout;
        private LinearLayout copyFooterLayout;
        private View loadMoreFailedView;

        private bool loadingMoreEnable;
        public IOnLoadMoreListener OnLoadMoreListener;

        public List<DailysModel> List { get; set; }
        public List<TopDailysModel> TopDailys { get; set; }
        private List<ImageView> dotList;
        private BannerAdapter bannerAdapter;

        private int bannerPosition = 0;
        private int FakeBannerSize = 100;
        private int DefaultBannerSize = 5;
        private bool isUserTouched = false;
        private Timer mTimer;

        private Handler handler;


        public DailysAdapter()
        {
            List = new List<DailysModel>();
            TopDailys = new List<TopDailysModel>();
            mTimer = new Timer();
            dotList = new List<ImageView>();
            handler = new Handler();
        }
        public override int ItemCount
        {
            get
            {
                var count = 0;
                if (TopDailys.Count > 0)
                {
                    count = 1;
                }
                if (List.Count > 0)
                {
                    count += List.Count + 1;
                }
                else
                {
                    if (footerLayout != null)
                    {
                        count += 1;
                    }
                }
                return count;
            }
        }
        public override int GetItemViewType(int position)
        {
            if (position == 0 && TopDailys.Count > 0)
            {
                return HaderView;
            }
            if (List.Count == 0 || position == List.Count + (TopDailys.Count > 0 ? 1 : 0))
            {
                if (footerLayout == null)
                {
                    return LoadingView;
                }
                else
                {
                    return FooterView;
                }
            }
            return base.GetItemViewType(position);
        }

        public override async void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            int viewType = viewHolder.ItemViewType;

            switch (viewType)
            {
                case HaderView:
                    AddHaderView(viewHolder);
                    break;
                case LoadingView:
                    AddLoadMore(viewHolder);
                    break;
                case FooterView:
                    break;
                default:
                    var item = (ItemViewHolder)viewHolder;

                    var model = List[TopDailys.Count > 0 ? position - 1 : position];

                    item.ItemView.Tag = model.Id;
                    item.ItemView.SetOnClickListener(this);

                    item.title.Text = model.Title;
                    if (model.extra != null)
                    {
                        item.good.Text = model.extra.popularity + " 赞 · " + model.extra.comments + " 评论";
                    }
                    if (model.Date != null)
                    {
                        item.date.Visibility = ViewStates.Visible;
                        var d = Convert.ToDateTime(model.Date.Substring(0, 4) + "/" + model.Date.Substring(4, 2) + "/" + model.Date.Substring(6, 2));
                        item.date.Text = model.Date == DateTime.Now.ToString("yyyyMMdd") ? "今日热闻" : d.ToString("MM月dd日");
                    }
                    else
                    {
                        item.date.Visibility = ViewStates.Gone;
                    }
                    if (model.Images.Count > 0)
                    {
                        item.titleImage.Visibility = ViewStates.Visible;
                        try
                        {
                            await ImageService.Instance.LoadUrl(model.Images[0])
                                  .Retry(3, 200)
                                  .DownSample(140, 140)
                                  .Transform(new RoundedTransformation(20))
                                  .LoadingPlaceholder("ic_placeholder.png", ImageSource.ApplicationBundle)
                                  .ErrorPlaceholder("ic_placeholder.png", ImageSource.ApplicationBundle)
                                  .IntoAsync(item.titleImage);
                        }
                        catch (System.Exception)
                        {

                        }
                    }
                    else
                    {
                        item.titleImage.Visibility = ViewStates.Gone;
                    }
                    break;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            this.context = parent.Context;
            this.layoutInflater = LayoutInflater.From(context);
            switch (viewType)
            {
                case HaderView:
                    return new HaderViewHolder(layoutInflater.Inflate(Resource.Layout.banner, parent, false));
                case LoadingView:
                    return new LoadingViewHolder(layoutInflater.Inflate(Resource.Layout.recyclerview_loadmore, parent, false));
                case FooterView:
                    return new FooterViewHolder(footerLayout);
                default:
                    return new ItemViewHolder(layoutInflater.Inflate(Resource.Layout.dailys_item, parent, false));
            }
        }
        public class HaderViewHolder : RecyclerView.ViewHolder
        {
            public ViewPager viewPager { get; set; }
            public TextView title { get; set; }
            public LinearLayout layoutDotlist { get; set; }
            public HaderViewHolder(View view)
                : base(view)
            {
                viewPager = view.FindViewById<ViewPager>(Resource.Id.viewpager);
                title = view.FindViewById<TextView>(Resource.Id.title);
                layoutDotlist = view.FindViewById<LinearLayout>(Resource.Id.layoutDotlist);
            }
        }
        public class ItemViewHolder : RecyclerView.ViewHolder
        {
            public TextView date { get; set; }
            public ImageViewAsync titleImage { get; set; }
            public TextView title { get; set; }
            public TextView good { get; set; }
            public ItemViewHolder(View view)
                : base(view)
            {
                date = view.FindViewById<TextView>(Resource.Id.txtDate);
                titleImage = view.FindViewById<ImageViewAsync>(Resource.Id.titleImage);
                title = view.FindViewById<TextView>(Resource.Id.txtTitle);
                good = view.FindViewById<TextView>(Resource.Id.txtGood);
            }
        }
        public class LoadingViewHolder : RecyclerView.ViewHolder
        {
            public LoadingViewHolder(View view)
                : base(view)
            {
            }
        }
        public class FooterViewHolder : RecyclerView.ViewHolder
        {
            public FooterViewHolder(View view)
                : base(view)
            {
            }
        }
        public void NewData(List<DailysModel> list)
        {
            this.List = list;
            if (loadMoreFailedView != null)
            {
                RemoveFooterView(loadMoreFailedView);
            }
            NotifyDataSetChanged();
        }
        public void Remove(int position)
        {
            List.RemoveAt(position);
            NotifyItemRemoved(position);
        }
        public void AddData(List<DailysModel> newList)
        {
            loadingMoreEnable = false;
            this.List.AddRange(newList);
            NotifyItemRangeChanged(List.Count - newList.Count + (TopDailys.Count > 0 ? 1 : 0), newList.Count);
        }
        public void AddHader(List<TopDailysModel> lists)
        {
            TopDailys = lists;
            NotifyItemInserted(0);
        }
        public void ChangeHaderView(List<TopDailysModel> lists)
        {
            TopDailys = lists;
            NotifyItemChanged(0);
        }
        public void AddFooterView(View footer)
        {
            AddFooterView(footer, -1);
        }
        public void AddFooterView(View footer, int index)
        {
            if (footerLayout == null)
            {
                if (copyFooterLayout == null)
                {
                    footerLayout = new LinearLayout(footer.Context);
                    footerLayout.Orientation = Orientation.Vertical;
                    footerLayout.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                    copyFooterLayout = footerLayout;
                }
                else
                {
                    footerLayout = copyFooterLayout;
                }
            }
            index = index >= footerLayout.ChildCount ? -1 : index;
            footerLayout.AddView(footer, index);
            this.NotifyItemChanged(ItemCount);
        }
        public void RemoveFooterView(View footer)
        {
            if (footerLayout == null) return;

            footerLayout.RemoveView(footer);
            if (footerLayout.ChildCount == 0)
            {
                footerLayout = null;
            }
            this.NotifyDataSetChanged();
        }
        public void RemoveAllFooterView()
        {
            if (footerLayout == null) return;

            footerLayout.RemoveAllViews();
            footerLayout = null;
        }
        public void ShowLoadMoreFailedView()
        {
            LoadComplete();
            if (loadMoreFailedView == null)
            {
                loadMoreFailedView = layoutInflater.Inflate(Resource.Layout.recyclerview_loadmore_failed, null);
                loadMoreFailedView.Click += delegate
                {
                    RemoveFooterView(loadMoreFailedView);
                };
            }
            AddFooterView(loadMoreFailedView);
        }
        public void LoadComplete()
        {
            loadingMoreEnable = false;
            this.NotifyItemChanged(ItemCount);
        }
        private void AddHaderView(RecyclerView.ViewHolder holder)
        {
            var item = (HaderViewHolder)holder;
            if (item.layoutDotlist.ChildCount == 0)
            {
                for (int i = 0; i < DefaultBannerSize; i++)
                {
                    var img = new ImageView(context);
                    var parame = new LinearLayout.LayoutParams(context.Resources.GetDimensionPixelSize(Resource.Dimension.dot_width), context.Resources.GetDimensionPixelSize(Resource.Dimension.dot_height));
                    parame.SetMargins(0, 0, context.Resources.GetDimensionPixelSize(Resource.Dimension.dot_marginRight), 0);
                    img.LayoutParameters = parame;
                    img.SetImageResource(Resource.Drawable.dot_normal);
                    dotList.Add(img);
                    item.layoutDotlist.AddView(img);
                }
                bannerAdapter = new BannerAdapter(context, handler, TopDailys, item.viewPager, ref bannerPosition, FakeBannerSize, DefaultBannerSize);
                handler.Post(() =>
                {
                    item.viewPager.AddOnPageChangeListener(new BannerPageChange(handler, TopDailys, dotList, item.layoutDotlist, item.title, ref bannerPosition, DefaultBannerSize));
                });
                item.viewPager.Adapter = bannerAdapter;

                (item.layoutDotlist.GetChildAt(0) as ImageView).SetImageResource(Resource.Drawable.dot_focused);
                item.title.Text = TopDailys[0].Title;

                var timer = new BannerTimerTask(context, handler, item.viewPager, isUserTouched, ref bannerPosition, FakeBannerSize, DefaultBannerSize);
                mTimer.Schedule(timer, 5000, 5000);
            }
        }
        private void AddLoadMore(RecyclerView.ViewHolder holder)
        {
            if (!loadingMoreEnable)
            {
                loadingMoreEnable = true;
                OnLoadMoreListener.OnLoadMoreRequested();
            }
        }
        public void OnClick(View v)
        {
            if (v.Tag != null)
            {
                DailyActivity.Start(context, v.Tag.ToString());
            }
        }

    }
    public class BannerPageChange : Java.Lang.Object, ViewPager.IOnPageChangeListener
    {
        private Handler handler;
        private List<TopDailysModel> topDailys;
        private List<ImageView> dotList;
        private TextView title;
        private LinearLayout layoutDotlist;
        private int bannerPosition;
        private int DefaultBannerSize;

        public BannerPageChange(Handler handler, List<TopDailysModel> topDailys, List<ImageView> dotList, LinearLayout layoutDotlist, TextView title, ref int bannerPosition, int DefaultBannerSize)
        {
            this.handler = handler;
            this.topDailys = topDailys;
            this.dotList = dotList;
            this.layoutDotlist = layoutDotlist;
            this.title = title;
            this.bannerPosition = bannerPosition;
            this.DefaultBannerSize = DefaultBannerSize;
        }
        public void OnPageScrollStateChanged(int state)
        {
        }

        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {
        }

        public void OnPageSelected(int position)
        {
            bannerPosition = position;
            position %= DefaultBannerSize;
            handler.Post(() =>
            {
                for (int i = 0; i < layoutDotlist.ChildCount; i++)
                {
                    (layoutDotlist.GetChildAt(i) as ImageView).SetImageResource(Resource.Drawable.dot_normal);
                }
           (layoutDotlist.GetChildAt(position) as ImageView).SetImageResource(Resource.Drawable.dot_focused);
                title.Text = topDailys[position].Title;
            });
        }
    }
    public class BannerTimerTask : TimerTask
    {
        private Context context;
        private Handler handler;
        private ViewPager viewPager;
        private bool isUserTouched;
        private int bannerPosition;
        private int FakeBannerSize;
        private int DefaultBannerSize;

        public BannerTimerTask(Context context, Handler handler, ViewPager viewPager, bool isUserTouched, ref int bannerPosition, int FakeBannerSize, int DefaultBannerSize)
        {
            this.context = context;
            this.handler = handler;
            this.viewPager = viewPager;
            this.isUserTouched = isUserTouched;
            this.bannerPosition = bannerPosition;
            this.FakeBannerSize = FakeBannerSize;
            this.DefaultBannerSize = DefaultBannerSize;
        }
        public override void Run()
        {
            if (!isUserTouched)
            {
                bannerPosition = (bannerPosition + 1) % FakeBannerSize;
                handler.Post(() =>
                {
                    if (bannerPosition == FakeBannerSize - 1)
                    {
                        viewPager.SetCurrentItem(DefaultBannerSize - 1, true);
                    }
                    else
                    {
                        viewPager.SetCurrentItem(bannerPosition, true);
                    }
                });
            }
        }
    }
}