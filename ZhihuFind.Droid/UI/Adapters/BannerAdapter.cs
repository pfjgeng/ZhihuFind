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
using Android.Support.V4.View;
using ZhihuFind.Droid.UI.Activitys;
using Square.Picasso;
using ZhihuFind.Droid.ViewModel;

namespace ZhihuFind.Droid.UI.Adapters
{
    public class BannerAdapter : PagerAdapter, View.IOnClickListener
    {
        private Context context;
        private Handler handler;
        private ViewPager viewPager;
        private int bannerPosition;
        private int FakeBannerSize;
        private int DefaultBannerSize;
        private List<TopDailysModel> topDailys;

        public BannerAdapter(Context context, Handler handler, List<TopDailysModel> topDailys, ViewPager viewPager, ref int bannerPosition, int FakeBannerSize, int DefaultBannerSize)
        {
            this.context = context;
            this.handler = handler;
            this.topDailys = topDailys;
            this.viewPager = viewPager;
            this.bannerPosition = bannerPosition;
            this.FakeBannerSize = FakeBannerSize;
            this.DefaultBannerSize = DefaultBannerSize;
        }
        public override int Count
        {
            get
            {
                return FakeBannerSize;
            }
        }
        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
        {
            View view = LayoutInflater.From(context).Inflate(Resource.Layout.banner_item, container, false);
            handler.Post(() =>
            {
                position %= DefaultBannerSize;
                view.Tag = topDailys[position].Id;
                view.SetOnClickListener(this);
                ImageView image = view.FindViewById<ImageView>(Resource.Id.image);

                Picasso.With(context)
                            .Load(topDailys[position].Image)
                            .Placeholder(Resource.Drawable.ic_placeholder)
                            .Error(Resource.Drawable.ic_placeholder)
                            .Into(image);
                container.AddView(view);
            });
            return view;
        }
        public override bool IsViewFromObject(View view, Java.Lang.Object @object)
        {
            return view == @object;
        }
        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object @object)
        {
            container.RemoveView((View)@object);
        }
        public override void FinishUpdate(ViewGroup container)
        {
            int position = viewPager.CurrentItem;
            if (position == 0)
            {
                position = DefaultBannerSize;
                viewPager.SetCurrentItem(position, true);
            }
            else if (position == FakeBannerSize - 1)
            {
                position = DefaultBannerSize - 1;
                viewPager.SetCurrentItem(position, true);
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
}