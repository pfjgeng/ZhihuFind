using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Square.Picasso;

namespace ZhihuFind.Droid.UI.Widgets
{
    public class CircleTransform : Java.Lang.Object, ITransformation
    {
        public string Key
        {
            get
            {
                return "CircleTransform()";
            }
        }

        public Bitmap Transform(Bitmap source)
        {
            int size = Math.Min(source.Width, source.Height);

            int x = (source.Width - size) / 2;
            int y = (source.Height - size) / 2;

            Bitmap squaredBitmap = Bitmap.CreateBitmap(source, x, y, size, size);
            if (squaredBitmap != source)
            {
                source.Recycle();          //回收垃圾
            }

            Bitmap bitmap = Bitmap.CreateBitmap(size, size, source.GetConfig());

            try
            {
                Canvas canvas = new Canvas(bitmap);
                Paint paint = new Paint();
                BitmapShader shader = new BitmapShader(squaredBitmap,
                BitmapShader.TileMode.Clamp, BitmapShader.TileMode.Clamp);//定义一个渲染器
                paint.SetShader(shader);//设置渲染器
                paint.AntiAlias = true;

                float r = size / 2f;
                canvas.DrawCircle(r, r, r, paint);//绘制图形

                squaredBitmap.Recycle();
            }
            catch
            {
            }
            return bitmap;
        }
    }
}