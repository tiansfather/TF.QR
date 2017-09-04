namespace TF.QR
{
    using System;
    using System.Web.Mvc;

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class UserAttribute : FilterAttribute, IAuthorizationFilter
    {
        public const string LoginUrl = "/My/Login";

        private void GotoLogin(AuthorizationContext filterContext)
        {
            if (filterContext.RequestContext.HttpContext.Request.HttpMethod.ToUpper() == "GET")
            {
                ContentResult result = new ContentResult {
                    Content = string.Format("<script>top.location.href = '{0}';</script>", "/My/Login"),
                    ContentType = "text/html"
                };
                filterContext.Result = result;
            }
            else
            {
                JsonResult result2 = new JsonResult {
                    Data = new { 
                        ok = false,
                        jumpUrl = "/My/Login",
                        errCode = -1,
                        errMsg = "超时，请重新登入."
                    }
                };
                filterContext.Result = result2;
            }
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (TF.QR.Config.GetCurrentUser() == null)
            {
                this.GotoLogin(filterContext);
            }
        }
    }
}

