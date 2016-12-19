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
    public class DailyCommentModel
    {
        public string author { get; set; }
        public int id { get; set; }
        public string content { get; set; }
        public int likes { get; set; }
        public int time { get; set; }
        public string avatar { get; set; }
    }
}