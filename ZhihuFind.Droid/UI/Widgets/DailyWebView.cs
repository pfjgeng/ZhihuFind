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
using Android.Webkit;
using Android.Util;
using Android.Views.InputMethods;

namespace ZhihuFind.Droid.UI.Widgets
{
    public class DailyWebView : WebView
    {
        private const string DailyCSS = "file:///android_asset/daily.css";
        private const string HtmlBegin = "" +
            "<!DOCTYPE html>\n" +
            "<html>\n";
        private const string HeadBegin = "" +
            "<head>\n" +
            "<meta charset=\"UTF-8\">\n" +
            "<meta name=\"viewport\" content=\"width=device-width,initial-scale=1,maximum-scale=1\">\n";
        private const string HeadlEnd = "" +
                "</head>\n" +
                "<body>\n";
        private const string HtmlEnd = "" +
                "</body>\n" +
                "</html>";

        public DailyWebView(Context context)
            : base(context)
        {
        }
        public DailyWebView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
        }
        
        public void LoadRenderedContent(string body)
        {
            var data = HtmlBegin + HeadBegin;
            data += "<link type=\"text/css\" rel=\"stylesheet\" href=\"" + DailyCSS + "\">\n";
            data += HeadlEnd + body + "\n" + HtmlEnd;
            LoadDataWithBaseURL(null, data, "text/html", "utf-8", null);
        }
    }
}