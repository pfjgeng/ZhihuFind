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
using Android.Support.V7.Widget;
using ZhihuFind.Droid.ViewModel;
using ZhihuFind.Droid.UI.Listeners;
using Java.Lang;
using Square.Picasso;
using ZhihuFind.Droid.UI.Widgets;
using ZhihuFind.Droid.UI.Activitys;
using ZhihuFind.Droid.Utils;
using FFImageLoading.Views;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Work;


namespace ZhihuFind.Droid.UI.Adapters
{
    public class ArticlesAdapter : RecyclerView.Adapter, View.IOnClickListener
    {
        public const int LoadingView = 0x00000111;
        public const int FooterView = 0x00000222;
        private Context context;
        protected LayoutInflater layoutInflater;
        private LinearLayout footerLayout;
        private LinearLayout copyFooterLayout;
        private View loadMoreFailedView;

        private bool loadingMoreEnable;

        public List<ArticleModel> List;
        public IOnLoadMoreListener OnLoadMoreListener;

        public ArticlesAdapter()
        {
            List = new List<ArticleModel>();
        }
        public override int ItemCount
        {
            get
            {
                var count = 0;
                if (List.Count > 0)
                {
                    count = List.Count + 1;
                }
                else
                {
                    if (footerLayout != null)
                    {
                        count = 1;
                    }
                }
                return count;
            }
        }
        public override int GetItemViewType(int position)
        {
            if (List.Count == 0 || position == List.Count)
            {
                if (footerLayout == null)
                {
                    return LoadingView;
                }
                else
                {
                    return FooterView;
                }
            }
            return base.GetItemViewType(position);
        }

        public override async void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            int viewType = viewHolder.ItemViewType;

            switch (viewType)
            {
                case LoadingView:
                    AddLoadMore(viewHolder);
                    break;
                case FooterView:
                    break;
                default:
                    var item = (ItemViewHolder)viewHolder;
                    var model = List[position];
                    item.ItemView.Tag = model.Slug;
                    item.ItemView.SetOnClickListener(this);

                    item.title.Text = model.Title;
                    if (model.Author != null)
                    {
                        item.name.Text = model.Author.Name;
                        if (model.Author.IsOrg)
                        {
                            item.org.Visibility = ViewStates.Visible;
                            item.org.SetImageResource(Resource.Drawable.identity);
                        }
                        else
                        {
                            if (model.Author.Badge != null)
                            {
                                item.org.Visibility = ViewStates.Visible;
                                if (model.Author.Badge.Identity != null)
                                {
                                    item.org.SetImageResource(Resource.Drawable.identity);
                                }
                                else if (model.Author.Badge.Best_answerer != null)
                                {
                                    item.org.SetImageResource(Resource.Drawable.bestanswerer);
                                }
                                else
                                {
                                    item.org.Visibility = ViewStates.Gone;
                                }
                            }
                            else
                            {
                                item.org.Visibility = ViewStates.Gone;
                            }
                        }
                        if (model.Author.Avatar != null && model.Author.Avatar.Template != null && model.Author.Avatar.Id != null)
                        {
                            var avatar = model.Author.Avatar.Template.Replace("{id}", model.Author.Avatar.Id);
                            avatar = avatar.Replace("{size}", "s");
                            try
                            {
                                await ImageService.Instance.LoadUrl(avatar)
                                      .Retry(3, 200)
                                      .DownSample(40, 40)
                                      .Transform(new CircleTransformation())
                                      .LoadingPlaceholder("ic_placeholder.png", ImageSource.ApplicationBundle)
                                      .ErrorPlaceholder("ic_placeholder.png", ImageSource.ApplicationBundle)
                                      .IntoAsync(item.avatar);
                            }
                            catch (System.Exception)
                            {

                            }
                        }
                    }
                    item.summary.Text = HtmlUtils.ReplaceHtmlTag(model.Content, 500);
                    item.time.Text = DateTimeUtils.CommonTime(Convert.ToDateTime(model.PublishedTime));

                    if (model.TitleImage == "")
                    {
                        item.titleImage.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        item.titleImage.Visibility = ViewStates.Visible;
                        try
                        {
                            await ImageService.Instance.LoadUrl(model.TitleImage)
                              .Retry(3, 200)
                              .DownSample(700, 200)
                              .LoadingPlaceholder("ic_placeholder.png", ImageSource.ApplicationBundle)
                              .ErrorPlaceholder("ic_placeholder.png", ImageSource.ApplicationBundle)
                              .IntoAsync(item.titleImage);
                        }
                        catch (System.Exception)
                        {

                        }
                    }
                    break;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            this.context = parent.Context;
            this.layoutInflater = LayoutInflater.From(context);
            switch (viewType)
            {
                case LoadingView:
                    return new LoadingViewHolder(layoutInflater.Inflate(Resource.Layout.recyclerview_loadmore, parent, false));
                case FooterView:
                    return new FooterViewHolder(footerLayout);
                default:
                    return new ItemViewHolder(layoutInflater.Inflate(Resource.Layout.articles_item, parent, false));
            }
        }
        public class ItemViewHolder : RecyclerView.ViewHolder
        {
            public ImageViewAsync avatar { get; set; }
            public TextView name { get; set; }
            public ImageViewAsync titleImage { get; set; }
            public ImageView org { get; set; }
            public TextView title { get; set; }
            public TextView summary { get; set; }
            public TextView time { get; set; }
            public ItemViewHolder(View view)
                : base(view)
            {
                avatar = view.FindViewById<ImageViewAsync>(Resource.Id.llAvatar);
                name = view.FindViewById<TextView>(Resource.Id.txtName);
                titleImage = view.FindViewById<ImageViewAsync>(Resource.Id.titleImage);
                org = view.FindViewById<ImageView>(Resource.Id.org);
                title = view.FindViewById<TextView>(Resource.Id.txtTitle);
                summary = view.FindViewById<TextView>(Resource.Id.txtSummary);
                time = view.FindViewById<TextView>(Resource.Id.txtTime);
            }
        }
        public class LoadingViewHolder : RecyclerView.ViewHolder
        {
            public LoadingViewHolder(View view)
                : base(view)
            {
            }
        }
        public class FooterViewHolder : RecyclerView.ViewHolder
        {
            public FooterViewHolder(View view)
                : base(view)
            {
            }
        }
        public void NewData(List<ArticleModel> list)
        {
            this.List = list;
            if (loadMoreFailedView != null)
            {
                RemoveFooterView(loadMoreFailedView);
            }
            NotifyDataSetChanged();
        }
        public void Remove(int position)
        {
            List.RemoveAt(position);
            NotifyItemRemoved(position);
        }
        public void Add(int position, ArticleModel item)
        {
            List.Insert(position, item);
            NotifyItemInserted(position);
        }
        public void AddData(List<ArticleModel> newList)
        {
            loadingMoreEnable = false;
            this.List.AddRange(newList);
            NotifyItemRangeChanged(List.Count - newList.Count, newList.Count);
        }
        public void AddFooterView(View footer)
        {
            AddFooterView(footer, -1);
        }
        public void AddFooterView(View footer, int index)
        {
            if (footerLayout == null)
            {
                if (copyFooterLayout == null)
                {
                    footerLayout = new LinearLayout(footer.Context);
                    footerLayout.Orientation = Orientation.Vertical;
                    footerLayout.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                    copyFooterLayout = footerLayout;
                }
                else
                {
                    footerLayout = copyFooterLayout;
                }
            }
            index = index >= footerLayout.ChildCount ? -1 : index;
            footerLayout.AddView(footer, index);
            this.NotifyItemChanged(ItemCount);
        }
        public void RemoveFooterView(View footer)
        {
            if (footerLayout == null) return;

            footerLayout.RemoveView(footer);
            if (footerLayout.ChildCount == 0)
            {
                footerLayout = null;
            }
            this.NotifyDataSetChanged();
        }
        public void RemoveAllFooterView()
        {
            if (footerLayout == null) return;

            footerLayout.RemoveAllViews();
            footerLayout = null;
        }
        public void ShowLoadMoreFailedView()
        {
            LoadComplete();
            if (loadMoreFailedView == null)
            {
                loadMoreFailedView = layoutInflater.Inflate(Resource.Layout.recyclerview_loadmore_failed, null);
                loadMoreFailedView.Click += delegate
                {
                    RemoveFooterView(loadMoreFailedView);
                };
            }
            AddFooterView(loadMoreFailedView);
        }
        public void LoadComplete()
        {
            loadingMoreEnable = false;
            this.NotifyItemChanged(ItemCount);
        }

        private void AddLoadMore(RecyclerView.ViewHolder holder)
        {
            if (!loadingMoreEnable)
            {
                loadingMoreEnable = true;
                OnLoadMoreListener.OnLoadMoreRequested();
            }
        }
        public void OnClick(View v)
        {
            if (v.Tag != null)
            {
                ArticleActivity.Start(context, v.Tag.ToString());
            }
        }
    }
}