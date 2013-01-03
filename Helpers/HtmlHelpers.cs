using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using iBoox.Models;

namespace iBoox.Helpers
{
    public static class HtmlHelpers
    {
        public static MvcHtmlString PageLinks(this HtmlHelper html, PagingInfo pagingInfo, Func<int, string> pageUrl)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 1; i <= pagingInfo.TotalPages; i++)
            {
                TagBuilder tag = new TagBuilder("a");
                tag.MergeAttribute("href", pageUrl(i));
                tag.InnerHtml = i.ToString();

                if (i == pagingInfo.CurrentPage)
                    tag.AddCssClass("selected");

                result.Append(tag.ToString());
                result.Append("&nbsp;");
            }

            return MvcHtmlString.Create(result.ToString());
        }

        public static MvcHtmlString TagLink(this HtmlHelper html, Tag tag, int max, Func<string, string> tagUrl)
        {
            int fontSize = 0;
            double part = (double)(tag.Count ?? 0) / max * 100;

            if (part >= 70) fontSize = 22;
            else if (part >= 60) fontSize = 20;
            else if (part >= 50) fontSize = 18;
            else if (part >= 40) fontSize = 16;
            else if (part >= 30) fontSize = 14;
            else if (part >= 25) fontSize = 13;
            else if (part >= 20) fontSize = 12;
            else if (part >= 15) fontSize = 11;
            else fontSize = 10;

            StringBuilder result = new StringBuilder();
            TagBuilder span = new TagBuilder("span");
            TagBuilder a = new TagBuilder("a");
            a.MergeAttribute("href", tagUrl(tag.TagID.ToString()));
            a.MergeAttribute("style", String.Format("font-size: {0}pt", fontSize));
            a.InnerHtml = tag.Title;
            span.InnerHtml = a.ToString();

            result.Append(span.ToString());
            result.Append("&nbsp;");

            span = new TagBuilder("span");
            span.MergeAttribute("style", String.Format("font-size: {0}pt", 9));
            span.InnerHtml = tag.Count.ToString();

            result.Append(span.ToString());
            result.Append("  ");

            return MvcHtmlString.Create(result.ToString());
        }
    }
}