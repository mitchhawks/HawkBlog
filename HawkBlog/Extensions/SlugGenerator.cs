using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HawkBlog.Extensions
{
    public class SlugGenerator
    {
        public string GenPostUrl(int year, int month, string postSlug)
        {
            string url = "/Blog/" + year + "/" + month + "/" + postSlug;
            return url;
        }
    }
}
