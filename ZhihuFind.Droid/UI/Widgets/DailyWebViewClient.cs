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
using Android.Graphics;

namespace ZhihuFind.Droid.UI.Widgets
{
    public class DailyWebViewClient : WebViewClient
    {
        private volatile static DailyWebViewClient singleton;

        public static DailyWebViewClient With(Context context)
        {
            if (singleton == null)
            {
                lock (typeof(DailyWebViewClient))
                {
                    if (singleton == null)
                    {
                        singleton = new DailyWebViewClient(context);
                    }
                }
            }
            return singleton;
        }

        private Context context;

        private DailyWebViewClient(Context context)
        {
            this.context = context.ApplicationContext;
        }
        public override WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request)
        {
            return base.ShouldInterceptRequest(view, request);
        }
        public override void OnPageFinished(WebView view, string url)
        {
            base.OnPageFinished(view, url);
            view.LoadUrl("javascript:(function(){" +
                                    "var imgs = document.getElementsByTagName(\"img\"); "+
                                    "var srcs=new Array();" +
                                            "for(var i=0;i<imgs.length;i++)  " +
                                    "{"
                                        +
                                        "if(imgs[i].getAttribute('class')=='content-image') srcs.push(imgs[i].src);"
                                            + "    imgs[i].onclick=function()  " +
                                    "    {  "
                                            + "        openlistner.OpenImage(srcs.toString(),i);  " +
                                    "    };  " +
                                    "};" + "var as = document.getElementsByTagName(\"a\"); " +
                                            "for(var i=0;i<as.length;i++)  " +
                                    "{"
                                            + "    as[i].onclick=function()  " +
                                    "    {  "
                                            + "        openlistner.OpenHref(this.href); return false; " +
                                    "    }  " +
                                    "}" +
                                    "})()");
        }
        public override void OnPageStarted(WebView view, string url, Bitmap favicon)
        {
            base.OnPageStarted(view, url, favicon);
        }
    }
}