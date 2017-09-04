namespace TF.QR.Controllers
{
    using Senparc.Weixin;
    using Senparc.Weixin.Exceptions;
    using Senparc.Weixin.HttpUtility;
    using Senparc.Weixin.MP;
    using Senparc.Weixin.MP.AdvancedAPIs;
    using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
    using System;
    using System.Web.Mvc;
    using TF.QR;

    public class OAuth2Controller : BaseController
    {
        private string appId = TF.QR.Config.AppId;
        private string secret = TF.QR.Config.AppSecret;

        public ActionResult BaseCallback(string code, string state, string returnUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    return base.RedirectError("您拒绝了授权！");
                }
                if (state != (base.Session["State"] as string))
                {
                    return base.RedirectError("验证失败！请从正规途径进入！");
                }
                OAuthAccessTokenResult result = OAuthApi.GetAccessToken(this.appId, this.secret, code, "authorization_code");
                if (result.errcode != ReturnCode.请求成功)
                {
                    return base.RedirectError("错误：" + result.errmsg);
                }
                OAuthUserInfo model = OAuthApi.GetUserInfo(result.access_token, result.openid, Language.zh_CN);
                
                base.Session["OAuthAccessTokenStartTime"] = DateTime.Now;
                base.Session["OAuthAccessToken"] = result;
                base.SetCookie("openid", result.openid);
                base.SetCookie("wename", model.nickname);
                base.SetCookie("headimg", model.headimgurl);
                return this.Redirect(returnUrl);
            }catch(Exception ex)
            {
                return Content(ex.Message);
            }
            
            
        }

        public ActionResult Index(string returnUrl)
        {
            if ((base.Request.Cookies["openid"] != null) && !string.IsNullOrEmpty(base.Request.Cookies["openid"].Value))
            {
                return this.Redirect(returnUrl);
            }
            string state = "TF-" + DateTime.Now.Millisecond;
            base.Session["State"] = state;
            string url = OAuthApi.GetAuthorizeUrl(this.appId, "http://1.jn99.net/oauth2/BaseCallback?returnUrl=" + returnUrl.UrlEncode(), state, OAuthScope.snsapi_userinfo, "code", true);
            return this.Redirect(url);
        }

        public ActionResult TestReturnUrl()
        {
            string content = ("OAuthAccessTokenStartTime：" + base.Session["OAuthAccessTokenStartTime"]) + "<br /><br />此页面为returnUrl功能测试页面，可以进行刷新（或后退），不会得到code不可用的错误。<br />测试不带returnUrl效果，请" + string.Format("<a href=\"{0}\">点击这里</a>。", base.Url.Action("Index"));
            return base.Content(content);
        }

        public ActionResult UserInfoCallback(string code, string state, string returnUrl)
        {
            if (string.IsNullOrEmpty(code))
            {
                return base.Content("您拒绝了授权！");
            }
            if (state != (base.Session["State"] as string))
            {
                return base.Content("验证失败！请从正规途径进入！");
            }
            OAuthAccessTokenResult result = null;
            try
            {
                result = OAuthApi.GetAccessToken(this.appId, this.secret, code, "authorization_code");
            }
            catch (Exception exception)
            {
                return base.Content(exception.Message);
            }
            if (result.errcode != ReturnCode.请求成功)
            {
                return base.Content("错误：" + result.errmsg);
            }
            base.Session["OAuthAccessTokenStartTime"] = DateTime.Now;
            base.Session["OAuthAccessToken"] = result;
            try
            {
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return this.Redirect(returnUrl);
                }
                OAuthUserInfo model = OAuthApi.GetUserInfo(result.access_token, result.openid, Language.zh_CN);
                return base.View(model);
            }
            catch (ErrorJsonResultException exception2)
            {
                return base.Content(exception2.Message);
            }
        }
    }
}

