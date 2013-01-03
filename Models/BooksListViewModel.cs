using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iBoox.Models
{
    public class BooksListViewModel
    {
        public IEnumerable<Book> Books { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}