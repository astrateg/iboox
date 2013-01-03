using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;

namespace iBoox.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult NotFound(string message)
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            ViewBag.Message = message;
            return View();
        }

        public ActionResult ServerError()
        {
            if (User.IsInRole("Admin"))
            {
                //Exception exception = Server.GetLastError();
                //ViewBag.Exception = exception.Message;
            }
            else
            {
                Response.TrySkipIisCustomErrors = true;
            }
            return View();
        }

    }
}
