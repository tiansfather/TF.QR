namespace TF.QR.Controllers
{
    using Senparc.Weixin.MP.AdvancedAPIs;
    using Senparc.Weixin.MP.Containers;
    using Senparc.Weixin.MP.Helpers;
    using System;
    using System.IO;
    using System.Web.Mvc;
    using TF.QR;

    public class ShowController : BaseController
    {
        [WeUser]
        public ActionResult Bind(string code)
        {
            base.ViewData["code"] = code;
            return base.View();
        }

        [WeUser, HttpPost]
        public ActionResult Bind(string code, string verifycode)
        {
            DbQRInfo poco = TF.QR.Config.Helper.CreateWhere<DbQRInfo>().Where(o => o.Code == code).FirstOrDefault(null);
            if (poco == null)
            {
                return base.Error("编码错误");
            }
            if (!string.IsNullOrEmpty(poco.OpenID))
            {
                return base.Error("此编码已经被绑定");
            }
            if (poco.VerifyCode == verifycode)
            {
                poco.OpenID = base.Request.Cookies["openid"].Value;
                TF.QR.Config.Helper.Save(poco);
                return base.Success("设置成功");
            }
            return base.Error("验证码不正确");
        }

        [WeUser]
        public ActionResult Edit(string code)
        {
            try
            {
                string openid = base.Request.Cookies["openid"].Value;
                DbQRInfo model = TF.QR.Config.Helper.CreateWhere<DbQRInfo>().Where(o => o.Code == code).Where(o => o.OpenID == openid).FirstOrDefault(null);
                if (model == null)
                {
                    return base.RedirectError("对不起，无权修改此编码信息");
                }
                JsSdkUiPackage package = JSSDKHelper.GetJsSdkUiPackage(TF.QR.Config.AppId, TF.QR.Config.AppSecret, base.Request.Url.AbsoluteUri);
                base.ViewData["jssdk"] = package;
                return base.View(model);
            }
            catch (Exception exception)
            {
                return base.RedirectError(exception.Message);
            }
        }

        [WeUser, HttpPost]
        public ActionResult Edit(DbQRInfo info)
        {
            DbQRInfo poco = TF.QR.Config.Helper.SingleById<DbQRInfo>(info.Id);
            if (poco.OpenID != base.Request.Cookies["openid"].Value)
            {
                return base.Error("没有操作权限");
            }
            string str = base.Request.Form["mediaid"];
            if (!string.IsNullOrEmpty(str))
            {
                string accessTokenOrAppId = AccessTokenContainer.TryGetAccessToken(TF.QR.Config.AppId, TF.QR.Config.AppSecret, false);
                string path = string.Concat(new object[] { "/upload/", DateTime.Now.Year, "/", DateTime.Now.Month.ToString("D2"), "/", DateTime.Now.Day.ToString("D2"), "/" });
                if (!Directory.Exists(base.Server.MapPath(path)))
                {
                    Directory.CreateDirectory(base.Server.MapPath(path));
                }
                Guid guid = Guid.NewGuid();
                string str4 = path + guid + ".jpg";
                FileStream stream = new FileStream(base.Server.MapPath(str4), FileMode.Create);
                MediaApi.Get(accessTokenOrAppId, str, stream);
                stream.Close();
                poco.Photo = str4;
            }
            poco.RealName = info.RealName;
            poco.Sex = info.Sex;
            poco.BirthDay = info.BirthDay;
            poco.Address = info.Address;
            poco.ContactName = info.ContactName;
            poco.ContactMobile = info.ContactMobile;
            poco.ContactName2 = info.ContactName2;
            poco.ContactMobile2 = info.ContactMobile2;
            poco.Tip = info.Tip;
            TF.QR.Config.Helper.Save(poco);
            return base.Success("提交成功");
        }

        public ActionResult Index(string code)
        {
            DbQRInfo model = TF.QR.Config.Helper.CreateWhere<DbQRInfo>().Where(o => o.Code == code).SingleOrDefault(null);
            if (model == null)
            {
                return base.RedirectError("未找到数据");
            }
            if (!model.ActivateDate.HasValue)
            {
                return base.RedirectError("此编码尚未被激活,编码:"+code);
            }
            if (model.ExpireDate.HasValue)
            {
                DateTime now = DateTime.Now;
                DateTime? expireDate = model.ExpireDate;
                if (expireDate.HasValue ? (now > expireDate.GetValueOrDefault()) : false)
                {
                    return base.RedirectError("此编码已经过期");
                }
            }
            return base.View(model);
        }

        [WeUser]
        public ActionResult VerifyEdit(string code)
        {
            DbQRInfo info = TF.QR.Config.Helper.CreateWhere<DbQRInfo>().Where(o => o.Code == code).FirstOrDefault(null);
            if (info == null)
            {
                return base.RedirectError("编码信息错误");
            }
            if (string.IsNullOrEmpty(info.OpenID))
            {
                return base.RedirectToAction("Bind", new { code = code });
            }
            return base.RedirectToAction("Edit", new { code = code });
        }

        /// <summary>
        /// 我的推荐金
        /// </summary>
        /// <returns></returns>
        [WeUser]
        public ActionResult Bonus()
        {
            var openid = Request.Cookies["openid"].Value;
            var weuser = Config.Helper.CreateWhere<DbWeUser>().Where(o => o.OpenID == openid).SingleOrDefault();
            var recommands = Config.Helper.CreateWhere<DbRecommand>().Where(o => o.Mobile == weuser.Mobile).OrderBy(o=>o.CreateTime,ToolGood.ReadyGo.OrderType.Desc).SkipTake(0,10);
            string accessTokenOrAppId = AccessTokenContainer.TryGetAccessToken(Config.AppId, Config.AppSecret, false);
            foreach(var obj in recommands)
            {
                var user = Senparc.Weixin.MP.AdvancedAPIs.UserApi.Info(accessTokenOrAppId, obj.OpenID);
                obj.HeadImg = user.headimgurl;
            }
            ViewData["weuser"] = weuser;
            return View(recommands);
        }
    }
}

