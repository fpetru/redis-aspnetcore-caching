using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCache.Models
{
    public class TwitterItem
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string ProfileImage { get; set; }
        public string ProfileDescription { get; set; }
        public string Username { get; set; }
        public string ScreenName { get; set; }
    }
}
