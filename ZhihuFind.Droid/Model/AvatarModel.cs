using SQLite.Net.Attributes;

namespace ZhihuFind.Droid.Model
{
    public class AvatarModel
    {
        [PrimaryKey, Indexed]
        public string AuthorSlug { get; set; }
        public string Id { get; set; }
        public string Template { get; set; }
    }
}