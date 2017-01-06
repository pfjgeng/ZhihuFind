using System;
using System.Collections.Generic;
using System.Text;

namespace ZhihuFind.IOS.Classes
{
    public class GrowItem
    {
        public string ImageName { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public GrowItem()
        {
        }

        public GrowItem(string imageName, string title, string description)
        {
            // Initialize
            this.ImageName = imageName;
            this.Title = title;
            this.Description = description;
        }
    }
}
