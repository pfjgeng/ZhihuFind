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
using UK.CO.Senab.Photoview;
using Android.Graphics.Drawables;
using Square.Picasso;
using Android.Support.V4.View;
using ZhihuFind.Droid.UI.Widgets;
using ZhihuFind.Droid.UI.Adapters;

namespace ZhihuFind.Droid.UI.Activitys
{
    [Activity(Theme = "@style/PhotoTheme")]
    public class PhotoActivity : BaseActivity, ViewPager.IOnPageChangeListener
    {
        private string[] urls;
        private int index;
        private int count;
        private ViewPager viewpager;
        private TextView txtIndex;
        private TextView txtCount;
        public static void Start(Context context, string[] urls, int index)
        {
            Intent intent = new Intent(context, typeof(PhotoActivity));
            intent.PutExtra("urls", urls);
            intent.PutExtra("index", index);
            context.StartActivity(intent);
        }
        protected override int LayoutResource
        {
            get { return Resource.Layout.photo; }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            urls = Intent.GetStringArrayExtra("urls");
            index = Intent.GetIntExtra("index", 0);
            count = urls.Length;

            viewpager = FindViewById<HackyViewPager>(Resource.Id.viewpager);
            viewpager.OffscreenPageLimit = count;

            txtIndex = FindViewById<TextView>(Resource.Id.index);
            txtCount = FindViewById<TextView>(Resource.Id.count);
            txtCount.Text = count.ToString();

            viewpager.Adapter = new PhotoAdapter(this, urls.ToList());
            viewpager.AddOnPageChangeListener(this);
        }
        public void OnPageScrollStateChanged(int state)
        {
        }

        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {
        }

        public void OnPageSelected(int position)
        {
            txtIndex.Text = (position + 1).ToString();
        }
    }
}