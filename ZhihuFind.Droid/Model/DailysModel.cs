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
    public class DailysModel
    {
        [PrimaryKey, Indexed]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Ga_prefix { get; set; }
        public string Date { get; set; }
    }
}