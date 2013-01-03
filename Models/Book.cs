using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iBoox.Models
{
    public partial class Book
    {
        public double RatingAvg { 
            get 
            {
                if (this.Votes > 0)
                    return (double)this.TotalRating / (double)this.Votes;
                else
                    return 0;
            } 
        }

        public double RatingDecAvg { 
            get 
            {
                if (this.Votes > 0)
                    return Math.Round(this.RatingAvg, 1);
                else
                    return 0;
            }
        }

        public int RatingIntAvg {
            get
            {
                if (this.Votes > 0)
                    return (int)Math.Round(this.RatingAvg);
                else
                    return 0;
            }
        }
    }
}