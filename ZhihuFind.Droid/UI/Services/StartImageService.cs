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
using Android.Media;
using Square.Picasso;
using Android.Graphics;
using Android.Graphics.Drawables;
using System.IO;
using ZhihuFind.Droid.UI.Shareds;
using ZhihuFind.Droid.ViewModel;

namespace ZhihuFind.Droid.UI.Services
{
    [Service]
    public class StartImageService : Service, ITarget
    {
        private string url = "";
        private string text = "";
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            url = intent.GetStringExtra("url");
            text = intent.GetStringExtra("text");
            if (url != "")
            {
                Picasso.With(this).Load(url).Into(this);
            }
            return base.OnStartCommand(intent, flags, startId);
        }
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public void OnBitmapFailed(Drawable p0)
        {
        }

        public void OnBitmapLoaded(Bitmap bitmap, Picasso.LoadedFrom p1)
        {
            var file = url.Split('/');
            var filename = file[file.Length - 1];
            if (!StartImageShared.GetImg(this).Equals(filename))
            {
                FileStream fstr = null;
                var path = this.CacheDir.Path;
                try
                {
                    fstr = new FileStream(System.IO.Path.Combine(path, filename), FileMode.OpenOrCreate, FileAccess.Write);

                    bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, fstr);

                    StartImageShared.Update(this, new StartImageModel() { img = filename, text = text });
                }
                catch (Exception e)
                {
                    File.Delete(System.IO.Path.Combine(path, filename));
                }
                finally
                {
                    if (fstr != null)
                    {
                        fstr.Flush();
                        fstr.Close();
                    }
                }
            }
            else
            {
                OnDestroy();
            }
        }

        public void OnPrepareLoad(Drawable p0)
        {
        }
    }
}