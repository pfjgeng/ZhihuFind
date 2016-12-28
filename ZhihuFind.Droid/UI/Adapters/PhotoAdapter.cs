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
using Java.Lang;
using Square.Picasso;
using UK.CO.Senab.Photoview;

namespace ZhihuFind.Droid.UI.Adapters
{
    public class PhotoAdapter : PagerAdapter
    {
        private Context context;
        private Handler handler;
        private List<string> srcList;
        private PhotoViewAttacher attacher;
        public PhotoAdapter(Context context, List<string> srcList)
        {
            this.context = context;
            this.srcList = srcList;
            this.handler = new Handler();
        }
        public override int Count
        {
            get
            {
                return srcList.Count;
            }
        }

        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
        {
            View view = LayoutInflater.From(context).Inflate(Resource.Layout.photo_item, container, false);
            handler.Post(() =>
            {
                PhotoView image = view.FindViewById<PhotoView>(Resource.Id.photo);
                Picasso.With(context).Load(srcList[position]).Into(image);
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
    }
}