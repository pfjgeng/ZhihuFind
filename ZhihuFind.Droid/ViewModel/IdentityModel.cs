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
    public class IdentityModel
    {
        /// <summary>
        /// 官方帐号
        /// </summary>
        public BestAnswererModel Identity { get; set; }
        /// <summary>
        /// 优秀回答者
        /// </summary>
        public BestAnswererModel Best_answerer { get; set; }
    }
}