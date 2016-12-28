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
using Android.Util;

namespace ZhihuFind.Droid.UI.Widgets
{
    public class HackyViewPager : ViewPager
    {
        public HackyViewPager(Context context) : base(context)
        {

        }

        public HackyViewPager(Context context, IAttributeSet attrs) : base(context, attrs)
        {

        }
        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            try
            {
                return base.OnInterceptTouchEvent(ev);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}