
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Square.Picasso;
using ZhihuFind.Droid.ViewModel;
using ZhihuFind.Droid.Presenter;
using ZhihuFind.Droid.UI.Shareds;
using ZhihuFind.Droid.UI.Views;

namespace ZhihuFind.Droid.UI.Activitys
{
    [Activity(MainLauncher = true, LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
    public class SplashActivity : BaseActivity, ISplashView
    {
        private ISplashPresenter splashPresenter;
        private Handler handler;

        private ImageView startImage;
        private TextView title;
        protected override int LayoutResource
        {
            get { return Resource.Layout.splash; }
        }
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            splashPresenter = new SplashPresenter(this);
            handler = new Handler();

            startImage = FindViewById<ImageView>(Resource.Id.startImage);
            title = FindViewById<TextView>(Resource.Id.title);
            var url = StartImageShared.GetImg(this);
            var text = StartImageShared.GetText(this);
            if (url != "")
            {
                var file = new Java.IO.File(System.IO.Path.Combine(this.CacheDir.Path, url));
                Picasso.With(this).Load(file).Into(startImage);
                title.Text = text;
            }
            else
            {
                Picasso.With(this).Load(Resource.Drawable.splash).Into(startImage);
            }
            await splashPresenter.GetStartImage();

            handler.PostDelayed(() =>
            {
                MainActivity.Start(this);
                this.Finish();
            }, 3000);
        }
        public void GetStartImageFail(string msg)
        {
        }

        public void GetStartImageSuccess(StartImageModel img)
        {
            var file = img.img.Split('/');
            var filename = file[file.Length - 1];
            if (!StartImageShared.GetImg(this).Equals(filename))
            {
                Intent intent = new Intent(this, typeof(Services.StartImageService));
                intent.PutExtra("url", img.img);
                intent.PutExtra("text", img.text);
                StartService(intent);
            }
        }
    }
}