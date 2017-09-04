namespace TF.QR
{
    using System;
    using System.Web;
    using System.Web.Mvc;

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class WeUserAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.Cookies["openid"] == null)
            {
                var url = "/OAuth2/Index?returnUrl=" + HttpUtility.UrlEncode(filterContext.HttpContext.Request.RawUrl);
                filterContext.HttpContext.Response.Redirect(url);
                    //filterContext.HttpContext.Response.Redirect("/Page/Error?info={0}".Fmt(new object[] { "状态超时,请通过菜单重新获取链接进入" }));
            }
        }
    }
}

