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
using SQLite.Net.Attributes;

namespace ZhihuFind.Droid.Model
{
    public class DailyExtraModel
    {
        [PrimaryKey, Indexed]
        public int Id { get; set; }
        public int long_comments { get; set; }
        public int popularity { get; set; }
        public int short_comments { get; set; }
        public int comments { get; set; }
    }
}