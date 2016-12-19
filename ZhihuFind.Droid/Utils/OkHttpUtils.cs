using System;
using System.Collections.Generic;
using Square.OkHttp;
using Java.Util.Concurrent;
using Java.Net;
using System.Threading.Tasks;

namespace ZhihuFind.Droid.Utils
{
    public class OkHttpUtils
    {
        private const string Tag = "OkHttpUtils";
        private OkHttpClient okHttpClient;

        public OkHttpUtils()
        {
            okHttpClient = new OkHttpClient();
            okHttpClient.SetConnectTimeout(10, TimeUnit.Seconds);
            okHttpClient.SetWriteTimeout(10, TimeUnit.Seconds);
            okHttpClient.SetReadTimeout(30, TimeUnit.Seconds);
            okHttpClient.SetCookieHandler(new CookieManager(null, CookiePolicy.AcceptOriginalServer));
        }

        private static OkHttpUtils instance;
        public static OkHttpUtils Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (typeof(OkHttpUtils))
                    {
                        if (instance == null)
                        {
                            instance = new OkHttpUtils();
                        }
                    }
                }
                return instance;
            }
        }
        public void Get(string url, Action<Response> onResponse, Action<Request, Java.IO.IOException> onFailure)
        {
            Request request = new Request.Builder()
                .Url(url)
                .Build();
            okHttpClient.NewCall(request).Enqueue(onResponse, onFailure);
        }
        public async Task<string> GetAsyn(string url)
        {
            Request request = new Request.Builder()
                   .Url(url)
                   .Build();
            Response response = await okHttpClient.NewCall(request).ExecuteAsync();
            return await response.Body().StringAsync();
        }
        public void Post(string url, List<Param> param, Action<Response> onResponse, Action<Request, Java.IO.IOException> onFailure)
        {
            FormEncodingBuilder builder = new FormEncodingBuilder();
            foreach (var item in param)
            {
                builder.Add(item.Key, item.Value);
            }
            RequestBody requestBody = builder.Build();
            Request request = new Request.Builder().Url(url).Post(requestBody).Build();
            okHttpClient.NewCall(request).Enqueue(onResponse, onFailure);
        }
        public class Param
        {
            public string Key { get; set; }
            public string Value { get; set; }

            public Param(string key, string value)
            {
                this.Key = key;
                this.Value = value;
            }
        }
    }
}