namespace TF.QR.Controllers
{
    using System;
    using System.Web.Mvc;

    public class PageController : Controller
    {
        public ActionResult Error(string info)
        {
            base.ViewData["info"] = info;
            return base.View();
        }
        public ActionResult Success(string info)
        {
            base.ViewData["info"] = info;
            return base.View();
        }
    }
}

