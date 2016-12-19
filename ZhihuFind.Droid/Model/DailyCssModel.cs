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
    public class DailyCssModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public int DailyId { get; set; }
        public string css { get; set; }
    }
}