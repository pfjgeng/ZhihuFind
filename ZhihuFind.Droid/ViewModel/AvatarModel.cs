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

namespace ZhihuFind.Droid.ViewModel
{
    public class AvatarModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// UrlÆ´½ÓÄ£°æ
        /// </summary>
        public string Template { get; set; }
    }
}