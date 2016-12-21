using Android.Util;
using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Platform.XamarinAndroid;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ZhihuFind.Droid.Utils
{
    public class SQLiteUtils
    {
        public class Database : SQLiteAsyncConnection
        {
            public Database(string path) : base(new Func<SQLiteConnectionWithLock>(() => new SQLiteConnectionWithLock(new SQLitePlatformAndroid(), new SQLiteConnectionString(path, storeDateTimeAsTicks: false))))
            {
                CreateTableAsyn();
            }
            public async void CreateTableAsyn()
            {
                await CreateTableAsync<Model.DailysImagesModel>().ContinueWith((results) =>
                {
                    Log.Error("CreateTable", "Create DailysImagesModel Table Success");
                });
                await instance.CreateTableAsync<Model.DailyExtraModel>().ContinueWith((results) =>
                {
                    Log.Error("CreateTable", "Create DailyExtraModel Table Success");
                });
                await instance.CreateTableAsync<Model.DailysModel>().ContinueWith((results) =>
                {
                    Log.Error("CreateTable", "Create DailysModel Table Success");
                });
                await instance.CreateTableAsync<Model.TopDailysModel>().ContinueWith((results) =>
                {
                    Log.Error("CreateTable", "Create TopDailysModel Table Success");
                });
                await instance.CreateTableAsync<Model.DailyModel>().ContinueWith((results) =>
                {
                    Log.Error("CreateTable", "Create DailyModel Table Success");
                });
                await instance.CreateTableAsync<Model.DailyJsModel>().ContinueWith((results) =>
                {
                    Log.Error("CreateTable", "Create DailyJsModel Table Success");
                });
                await instance.CreateTableAsync<Model.DailyCssModel>().ContinueWith((results) =>
                {
                    Log.Error("CreateTable", "Create DailyCssModel Table Success");
                });

                await instance.CreateTableAsync<Model.ArticleModel>().ContinueWith((results) =>
                {
                    Log.Error("CreateTable", "Create ArticleModel Table Success");
                });
                await instance.CreateTableAsync<Model.AuthorModel>().ContinueWith((results) =>
                {
                    Log.Error("CreateTable", "Create AuthorModel Table Success");
                });
                await instance.CreateTableAsync<Model.AvatarModel>().ContinueWith((results) =>
                {
                    Log.Error("CreateTable", "Create AvatarModel Table Success");
                });
            }
            #region DailysModel
            public async Task<List<ViewModel.DailysModel>> QueryAllDailys()
            {
                var vDailys = new List<ViewModel.DailysModel>();

                var dailys = await Table<Model.DailysModel>().ToListAsync();
                foreach (var item in dailys)
                {
                    var extras = await Table<Model.DailyExtraModel>().Where(d => d.Id == item.Id).FirstOrDefaultAsync();

                    var images = await Table<Model.DailysImagesModel>().Where(d => d.DailyId == item.Id).ToListAsync();
                    var vImages = new List<string>();
                    foreach (var i in images)
                    {
                        vImages.Add(i.Images);
                    }
                    vDailys.Add(new ViewModel.DailysModel()
                    {
                        Id = item.Id,
                        Date = item.Date,
                        Ga_prefix = item.Ga_prefix,
                        Title = item.Title,
                        Images = vImages,
                        extra = new ViewModel.DailyExtraModel()
                        {
                            comments = extras.comments,
                            long_comments = extras.long_comments,
                            popularity = extras.popularity,
                            short_comments = extras.short_comments
                        }
                    });
                }
                return vDailys;
            }
            public async Task DeleteAllDailys()
            {
                await DeleteAllAsync<Model.DailysImagesModel>().ContinueWith(async (results) =>
                 {
                     await DeleteAllAsync<Model.DailysModel>();
                 });
            }
            public async Task UpdateAllDailys(List<ViewModel.DailysModel> lists)
            {
                foreach (var item in lists)
                {
                    //添加标题图
                    await DeleteAsync<Model.DailysImagesModel>(item.Id).ContinueWith(async (results) =>
                    {
                        foreach (var img in item.Images)
                        {
                            await InsertAsync(new Model.DailysImagesModel { DailyId = item.Id, Images = img });
                        }
                    });
                    //添加其他信息
                    if (item.extra != null)
                    {
                        await UpdateDailyExtra(item.extra);
                    }
                    //主信息
                    await DeleteAsync<Model.DailysModel>(item.Id).ContinueWith(async (results) =>
                     {
                         await InsertAsync(new Model.DailysModel
                         {
                             Id = item.Id,
                             Date = item.Date,
                             Ga_prefix = item.Ga_prefix,
                             Title = item.Title
                         });
                     });
                }
            }
            #endregion

            #region TopDailysModel
            public async Task<List<ViewModel.TopDailysModel>> QueryAllTopDailys()
            {
                var lists = new List<ViewModel.TopDailysModel>();
                var dailys = await Table<Model.TopDailysModel>().ToListAsync();
                foreach (var item in dailys)
                {
                    lists.Add(new ViewModel.TopDailysModel()
                    {
                        Id = item.Id,
                        Ga_prefix = item.Ga_prefix,
                        Title = item.Title,
                        Image = item.Image,
                        Type = item.Type
                    });
                }
                return lists;
            }
            public async Task DeleteAllTopDailys()
            {
                await DeleteAllAsync<Model.TopDailysModel>();
            }
            public async Task UpdateAllTopDailys(List<ViewModel.TopDailysModel> lists)
            {
                foreach (var item in lists)
                {
                    await InsertAsync(new Model.TopDailysModel
                    {
                        Id = item.Id,
                        Ga_prefix = item.Ga_prefix,
                        Title = item.Title,
                        Image = item.Image,
                        Type = item.Type
                    });
                }
            }
            #endregion

            #region DailyModel
            public async Task<ViewModel.DailyModel> QueryDaily(int id)
            {
                var vDaily = new ViewModel.DailyModel();

                var daily = await Table<Model.DailyModel>().Where(d => d.Id == id).FirstOrDefaultAsync();
                if (daily != null)
                {
                    vDaily = new ViewModel.DailyModel()
                    {
                        id = daily.Id,
                        body = daily.body,
                        ga_prefix = daily.ga_prefix,
                        image = daily.image,
                        image_source = daily.image_source,
                        share_url = daily.share_url,
                        title = daily.title,
                        type = daily.type,
                        updatetime = daily.updatetime,
                        css = new List<string>(),
                        js = new List<string>()
                    };
                    foreach (var item in await Table<Model.DailyCssModel>().Where(d => d.DailyId == id).ToListAsync())
                    {
                        vDaily.css.Add(item.css);
                    }
                    foreach (var item in await Table<Model.DailyJsModel>().Where(d => d.DailyId == id).ToListAsync())
                    {
                        vDaily.js.Add(item.js);
                    }
                }
                return vDaily;
            }
            public async Task DeleteDaily(int id)
            {
                await DeleteAsync<Model.DailyModel>(id).ContinueWith(async (results) =>
                {
                    foreach (var item in await Table<Model.DailyJsModel>().Where(d => d.DailyId == id).ToListAsync())
                    {
                        await DeleteAsync<Model.DailyJsModel>(item.Id);
                    }
                    foreach (var item in await Table<Model.DailyCssModel>().Where(d => d.DailyId == id).ToListAsync())
                    {
                        await DeleteAsync<Model.DailyCssModel>(item.Id);
                    }
                });
            }
            public async Task UpdateDaily(ViewModel.DailyModel item)
            {
                await DeleteAsync<Model.DailyModel>(item.id).ContinueWith(async (results) =>
                {
                    await InsertAsync(new Model.DailyModel
                    {
                        Id = item.id,
                        body = item.body,
                        ga_prefix = item.ga_prefix,
                        image = item.image,
                        image_source = item.image_source,
                        share_url = item.share_url,
                        title = item.title,
                        type = item.type,
                        updatetime = DateTime.Now
                    });
                });

                foreach (var js in await Table<Model.DailyJsModel>().Where(d => d.DailyId == item.id).ToListAsync())
                {
                    await DeleteAsync<Model.DailyJsModel>(js.Id);
                }
                foreach (var js in item.js)
                {
                    await InsertAsync(new Model.DailyJsModel
                    {
                        DailyId = item.id,
                        js = js
                    });
                }
                foreach (var css in await Table<Model.DailyCssModel>().Where(d => d.DailyId == item.id).ToListAsync())
                {
                    await DeleteAsync<Model.DailyCssModel>(css.Id);
                }
                foreach (var css in item.css)
                {
                    await InsertAsync(new Model.DailyCssModel
                    {
                        DailyId = item.id,
                        css = css
                    });
                }
            }
            #endregion

            #region DailyExtraModel
            public async Task<ViewModel.DailyExtraModel> QueryDailyExtra(int id)
            {
                var vExtra = new ViewModel.DailyExtraModel();

                var extra = await Table<Model.DailyExtraModel>().Where(d => d.Id == id).FirstOrDefaultAsync();
                if (extra != null)
                {
                    vExtra = new ViewModel.DailyExtraModel()
                    {
                        id = extra.Id,
                        comments = extra.comments,
                        long_comments = extra.long_comments,
                        popularity = extra.popularity,
                        short_comments = extra.short_comments
                    };
                }
                return vExtra;
            }
            public async Task DeleteDailyExtra(int id)
            {
                await DeleteAsync<Model.DailyExtraModel>(id);
            }
            public async Task UpdateDailyExtra(ViewModel.DailyExtraModel item)
            {
                await DeleteAsync<Model.DailyExtraModel>(item.id).ContinueWith(async (results) =>
                {
                    await InsertAsync(new Model.DailyExtraModel
                    {
                        Id = item.id,
                        comments = item.comments,
                        long_comments = item.long_comments,
                        popularity = item.popularity,
                        short_comments = item.short_comments
                    });
                });
            }
            #endregion

            #region ArticleModel
            public async Task<ViewModel.ArticleModel> QueryArticle(int slug)
            {
                var vArticle = new ViewModel.ArticleModel();

                var article = await Table<Model.ArticleModel>().Where(d => d.Slug == slug).FirstOrDefaultAsync();
                if (article != null)
                {
                    vArticle = new ViewModel.ArticleModel()
                    {
                        Slug = article.Slug,
                        CommentsCount = article.CommentsCount,
                        Content = article.Content,
                        LikesCount = article.LikesCount,
                        PublishedTime = article.PublishedTime,
                        Title = article.Title,
                        TitleImage = article.TitleImage,
                        Url = article.Url,
                        UpdateTime = article.UpdateTime,
                        Author = await QueryAuthor(article.AuthorSlug)
                    };
                }
                return vArticle;
            }
            public async Task<List<ViewModel.ArticleModel>> QueryArticles(int limit)
            {
                var vArticles = new List<ViewModel.ArticleModel>();

                var articles = await Table<Model.ArticleModel>().OrderByDescending(a => a.LikesCount).Skip(0).Take(limit).ToListAsync();
                if (articles != null)
                {
                    foreach (var article in articles)
                    {
                        vArticles.Add(new ViewModel.ArticleModel()
                        {
                            Slug = article.Slug,
                            CommentsCount = article.CommentsCount,
                            Content = article.Content,
                            LikesCount = article.LikesCount,
                            PublishedTime = article.PublishedTime,
                            Title = article.Title,
                            TitleImage = article.TitleImage,
                            Url = article.Url,
                            UpdateTime = article.UpdateTime,
                            Author = await QueryAuthor(article.AuthorSlug)
                        });
                    }
                }
                return vArticles;
            }
            public async Task UpdateArticle(ViewModel.ArticleModel item)
            {
                var author = item.Author;
                await DeleteAsync<Model.AuthorModel>(item.Slug).ContinueWith(async (results) =>
                {
                    await InsertAsync(new Model.AuthorModel
                    {
                        Slug = author.Slug,
                        Bio = author.Bio,
                        Description = author.Description,
                        Hash = author.Hash,
                        IsOrg = author.IsOrg,
                        Name = author.Name,
                        ProfileUrl = author.ProfileUrl,
                        Uid = author.Uid,
                        Best_answererIdDescription = author.Badge != null ? author.Badge.Best_answerer.Description : null,
                        IdentityDescription = author.Badge != null ? author.Badge.Identity.Description : null
                    });
                });

                await DeleteAsync<Model.AvatarModel>(item.Slug).ContinueWith(async (results1) =>
                {
                    await InsertAsync(new Model.AvatarModel
                    {
                        AuthorSlug = author.Slug,
                        Id = author.Avatar.Id,
                        Template = author.Avatar.Template,
                    });
                });

                await DeleteAsync<Model.ArticleModel>(item.Slug).ContinueWith(async (results) =>
                {
                    await InsertAsync(new Model.ArticleModel
                    {
                        CommentsCount = item.CommentsCount,
                        Content = item.Content,
                        LikesCount = item.LikesCount,
                        PublishedTime = item.PublishedTime,
                        Slug = item.Slug,
                        Title = item.Title,
                        TitleImage = item.TitleImage,
                        Url = item.Url,
                        AuthorSlug = item.Author.Slug,
                        UpdateTime = item.UpdateTime
                    });
                });
            }
            public async Task UpdateArticles(List<ViewModel.ArticleModel> lists)
            {
                foreach (var item in lists)
                {
                    item.UpdateTime = DateTime.MinValue;
                    await UpdateArticle(item);
                }
            }
            public async Task DeleteArticle(int slug)
            {
                await DeleteAsync<Model.ArticleModel>(slug);
            }
            #endregion

            #region AuthorModel
            public async Task<ViewModel.AuthorModel> QueryAuthor(string slug)
            {
                var vAuthor = new ViewModel.AuthorModel();

                var author = await Table<Model.AuthorModel>().Where(d => d.Slug == slug).FirstOrDefaultAsync();
                if (author != null)
                {
                    vAuthor = new ViewModel.AuthorModel()
                    {
                        Bio = author.Bio,
                        Slug = author.Slug,
                        Description = author.Description,
                        Hash = author.Hash,
                        IsOrg = author.IsOrg,
                        Name = author.Name,
                        ProfileUrl = author.ProfileUrl,
                        Uid = author.Uid,
                        Avatar = await QueryAvatar(author.Slug),
                        Badge = new ViewModel.IdentityModel()
                        {
                            Best_answerer = author.Best_answererIdDescription != null ? new ViewModel.BestAnswererModel()
                            {
                                Description = author.Best_answererIdDescription,
                                Topics = new List<int>()
                            } : null,
                            Identity = author.IdentityDescription != null ? new ViewModel.BestAnswererModel()
                            {
                                Description = author.IdentityDescription,
                                Topics = new List<int>()
                            } : null
                        }
                    };
                }
                return vAuthor;
            }
            public async Task UpdateAuthor(ViewModel.AuthorModel item)
            {
                await DeleteAsync<Model.AuthorModel>(item.Slug).ContinueWith(async (results) =>
                {
                    await InsertAsync(new Model.AuthorModel
                    {
                        Slug = item.Slug,
                        Bio = item.Bio,
                        Description = item.Description,
                        Hash = item.Hash,
                        IsOrg = item.IsOrg,
                        Name = item.Name,
                        ProfileUrl = item.ProfileUrl,
                        Uid = item.Uid,
                        Best_answererIdDescription = item.Badge != null ? item.Badge.Best_answerer.Description : null,
                        IdentityDescription = item.Badge != null ? item.Badge.Identity.Description : null
                    });
                });

                await DeleteAsync<Model.AvatarModel>(item.Slug).ContinueWith(async (results1) =>
                {
                    await InsertAsync(new Model.AvatarModel
                    {
                        AuthorSlug = item.Slug,
                        Id = item.Avatar.Id,
                        Template = item.Avatar.Template,
                    });
                });

            }
            #endregion

            #region AvatarModel
            public async Task<ViewModel.AvatarModel> QueryAvatar(string slug)
            {
                var vAvatar = new ViewModel.AvatarModel();

                var avatar = await Table<Model.AvatarModel>().Where(d => d.AuthorSlug == slug).FirstOrDefaultAsync();
                if (avatar != null)
                {
                    vAvatar = new ViewModel.AvatarModel()
                    {
                        Id = avatar.Id,
                        Template = avatar.Template
                    };
                }
                return vAvatar;
            }
            #endregion
        }

        private static Database instance;
        public static Database Instance()
        {
            if (instance == null)
            {
                string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "zhihufind.db");
                instance = new Database(dbPath);
            }
            return instance;
        }
    }
}