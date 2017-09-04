namespace TF.QR.Controllers
{
    using System;
    using System.Web.Mvc;

    public class AdminLTEController : Controller
    {
        public ActionResult _Footer()
        {
            return base.PartialView();
        }

        public ActionResult _Header()
        {
            return base.PartialView();
        }

        public ActionResult _HeaderUserInfo()
        {
            return base.PartialView();
        }

        public ActionResult _MenuPath(object menuPath)
        {
            base.TempData["_MenuPath"] = menuPath;
            return base.PartialView();
        }

        public ActionResult _Sidebar()
        {
            return base.PartialView();
        }

        public ActionResult _SidebarControl()
        {
            return base.PartialView();
        }

        public ActionResult _SidebarMenu()
        {
            return base.PartialView();
        }
    }
}

