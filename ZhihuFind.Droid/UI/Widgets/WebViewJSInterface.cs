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

namespace ZhihuFind.Droid.UI.Widgets
{
    public class WebViewJSInterface : Java.Lang.Object
    {
        Context context { get; set; }

        public WebViewJSInterface(Context context)
        {
            this.context = context;
        }

        [Java.Interop.Export]
        [JavascriptInterface]
        public void OpenImage(string srcs, int index)
        {
            EventHandler<CallFromPageReceivedEventArgs> handler = CallFromPageReceived;

            if (null != handler)
            {
                handler(this, new CallFromPageReceivedEventArgs
                {
                    Type = CallFromType.Image,
                    Result = srcs,
                    Index = index
                });
            }
        }
        [Java.Interop.Export]
        [JavascriptInterface]
        public void OpenHref(string href)
        {
            EventHandler<CallFromPageReceivedEventArgs> handler = CallFromPageReceived;

            if (null != handler)
            {
                handler(this, new CallFromPageReceivedEventArgs
                {
                    Type = CallFromType.Href,
                    Result = href,
                    Index = 0
                });
            }
        }

        public event EventHandler<CallFromPageReceivedEventArgs> CallFromPageReceived;
        public enum CallFromType
        {
            Image,
            Href
        }
        public class CallFromPageReceivedEventArgs : EventArgs
        {
            public CallFromType Type { get; set; }
            public string Result { get; set; }
            public int Index { get; set; }
        }
    }
}