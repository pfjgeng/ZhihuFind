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
using Android.Graphics;

namespace PhontView
{
    public class Info
    {
        // 内部图片在整个手机界面的位置
        RectF mRect = new RectF();

        // 控件在窗口的位置
        RectF mImgRect = new RectF();

        RectF mWidgetRect = new RectF();

        RectF mBaseRect = new RectF();

        PointF mScreenCenter = new PointF();

        float mScale;

        float mDegrees;

        ImageView.ScaleType mScaleType;

        public Info(RectF rect, RectF img, RectF widget, RectF b, PointF screenCenter, float scale, float degrees, ImageView.ScaleType scaleType)
        {
            mRect.Set(rect);
            mImgRect.Set(img);
            mWidgetRect.Set(widget);
            mScale = scale;
            mScaleType = scaleType;
            mDegrees = degrees;
            mBaseRect.Set(b);
            mScreenCenter.Set(screenCenter);
        }
    }
}