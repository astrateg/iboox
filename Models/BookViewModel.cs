using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iBoox.Models
{
    public class BookViewModel
    {
        public Book Book { get; set; }
        public string SecondISBN { get; set; }
        public string SecondCover { get; set; }
    }
}