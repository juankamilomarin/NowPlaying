using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NowPlaying.Controllers
{
    public class HomeController : Controller
    {
        #region Methods
        public ActionResult Index()
        {
            return View();
        }

        #endregion
    }
}