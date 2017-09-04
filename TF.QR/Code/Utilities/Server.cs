namespace Senparc.Weixin.MP.Sample.CommonService.Utilities
{
    using System;
    using System.IO;
    using System.Web;

    public static class Server
    {
        private static string _appDomainAppPath;

        public static string GetMapPath(string virtualPath)
        {
            if (virtualPath == null)
            {
                return "";
            }
            if (virtualPath.StartsWith("~/"))
            {
                return virtualPath.Replace("~/", AppDomainAppPath).Replace("/", @"\");
            }
            return Path.Combine(AppDomainAppPath, virtualPath.Replace("/", @"\"));
        }

        public static string AppDomainAppPath
        {
            get
            {
                if (_appDomainAppPath == null)
                {
                    _appDomainAppPath = HttpRuntime.AppDomainAppPath;
                }
                return _appDomainAppPath;
            }
            set
            {
                _appDomainAppPath = value;
            }
        }

        public static System.Web.HttpContext HttpContext
        {
            get
            {
                System.Web.HttpContext current = System.Web.HttpContext.Current;
                if (current == null)
                {
                    HttpRequest request = new HttpRequest("Default.aspx", "http://sdk.weixin.senparc.com/default.aspx", null);
                    StringWriter writer = new StringWriter();
                    HttpResponse response = new HttpResponse(writer);
                    current = new System.Web.HttpContext(request, response);
                }
                return current;
            }
        }
    }
}

