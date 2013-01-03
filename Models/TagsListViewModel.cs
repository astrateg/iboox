using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iBoox.Models
{
    public class TagsListViewModel
    {
        public IEnumerable<Tag> Tags { get; set; }
        public int TagsMaxCount { get; set; }
    }
}