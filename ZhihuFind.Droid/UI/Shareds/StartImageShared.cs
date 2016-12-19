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
using ZhihuFind.Droid.ViewModel;

namespace ZhihuFind.Droid.UI.Shareds
{
    public class StartImageShared
    {
        private const string Tag = "StartImageShared";

        private const string KeyImg = "img";
        private const string KeyText = "text";
        public static void Update(Context context, StartImageModel user)
        {
            BaseShared.With(context, Tag).SetString(KeyImg, user.img);
            BaseShared.With(context, Tag).SetString(KeyText, user.text);
        }
        public static string GetImg(Context context)
        {
            return BaseShared.With(context, Tag).GetString(KeyImg, "");
        }
        public static string GetText(Context context)
        {
            return BaseShared.With(context, Tag).GetString(KeyText, "");
        }
    }
}