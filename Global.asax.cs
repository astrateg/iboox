using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace iBoox
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        // Т.к. реализована своя поддержка 404, этот метод не нужен
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Error404",
                "NotFound",
                new { controller = "Error", action = "NotFound" }
            );

            routes.MapRoute(
                "Error500",
                "ServerError",
                new { controller = "Error", action = "ServerError" }
            );

            routes.MapRoute(
                "BookThemeViewIndex",
                "books/themes/{themeID}",
                new { controller = "Book", action = "Index", search = (string)null, seriesID = (string)null, themeID = (string)null, authorID = (int?)null, tagID = (int?)null, page = 1 },
                new { themeID = "[a-zA-Z0-9\\-]+" }
            );

            routes.MapRoute(
                "BookThemeViewIndexPage",
                "books/themes/{themeID}/page{page}",
                new { controller = "Book", action = "Index", search = (string)null, seriesID = (string)null, themeID = (string)null, authorID = (int?)null, tagID = (int?)null },
                new { themeID = "[a-zA-Z0-9\\-]+", page = @"\d{1,9}" }
            );

            routes.MapRoute(
                "BookSeriesViewIndex",
                "books/series/{seriesID}",
                new { controller = "Book", action = "Index", search = (string)null, seriesID = (string)null, themeID = (string)null, authorID = (int?)null, tagID = (int?)null, page = 1 },
                new { seriesID = "[a-zA-Z0-9\\-]+" }
            );

            routes.MapRoute(
                "BookSeriesViewIndexPage",
                "books/series/{seriesID}/page{page}",
                new { controller = "Book", action = "Index", search = (string)null, seriesID = (string)null, themeID = (string)null, authorID = (int?)null, tagID = (int?)null },
                new { seriesID = "[a-zA-Z0-9\\-]+", page = @"\d{1,9}" }
            );

            routes.MapRoute(
                "BookAuthorViewIndex",
                "books/authors/{authorID}",
                new { controller = "Book", action = "Index", search = (string)null, seriesID = (string)null, themeID = (string)null, authorID = (int?)null, tagID = (int?)null, page = 1 },
                new { authorID = @"\d{1,9}" }   // Constraints: page must be numerical (тип int, max = 2 147 483 647 -> принимаем 9 знаков максимум, и то много)
            );

            routes.MapRoute(
                "BookAuthorViewIndexPage",
                "books/authors/{authorID}/page{page}",
                new { controller = "Book", action = "Index", search = (string)null, seriesID = (string)null, themeID = (string)null, authorID = (int?)null, tagID = (int?)null },
                new { authorID = @"\d{1,9}", page = @"\d{1,9}" }
            );

            routes.MapRoute(
                "BookTagViewIndex",
                "books/tags/{tagID}",
                new { controller = "Book", action = "Index", search = (string)null, seriesID = (string)null, themeID = (string)null, authorID = (int?)null, tagID = (int?)null, page = 1 },
                new { tagID = @"\d{1,9}" }
            );

            routes.MapRoute(
                "BookTagViewIndexPage",
                "books/tags/{tagID}/page{page}",
                new { controller = "Book", action = "Index", search = (string)null, seriesID = (string)null, themeID = (string)null, authorID = (int?)null, tagID = (int?)null },
                new { tagID = @"\d{1,9}" }
            );

            routes.MapRoute(
                "BookSearchViewIndex",
                "books/search/{search}",
                new { controller = "Book", action = "Index", search = (string)null, seriesID = (string)null, themeID = (string)null, authorID = (int?)null, tagID = (int?)null, page = 1 },
                new { search = "[a-zA-ZА-Яа-я 0-9\\-]+" }
            );

            routes.MapRoute(
                "BookSearchViewIndexPage",
                "books/search/{search}/page{page}",
                new { controller = "Book", action = "Index", search = (string)null, seriesID = (string)null, themeID = (string)null, authorID = (int?)null, tagID = (int?)null },
                new { search = "[a-zA-ZА-Яа-я 0-9\\-]+" }
            );

            routes.MapRoute(
                "BookViewPage",
                "books/book/{id}",
                new { controller = "Book", action = "ViewBook", id = (int?)null },
                new { id = @"\d{1,9}" }
            );

            routes.MapRoute(
                "BookIndex",
                "",
                new { controller = "Book", action = "Index", page = 1 }
            );

            routes.MapRoute(
                "BookIndexPage",
                "page{page}",
                new { controller = "Book", action = "Index" },
                new { page = @"\d{1,9}" }
            );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Book", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            //RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }
    }
}