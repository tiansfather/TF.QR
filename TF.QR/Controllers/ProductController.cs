﻿using Senparc.Weixin.MP.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TF.QR.Controllers
{
    public class ProductController : BaseController
    {
        /// <summary>
        /// 产品列表
        /// </summary>
        /// <returns></returns>
        public ActionResult List()
        {
            var products = Config.Helper.CreateWhere<DbProduct>().Select();
            return View(products);
        }

        public ActionResult Index(int id)
        {
            JsSdkUiPackage package = JSSDKHelper.GetJsSdkUiPackage(TF.QR.Config.AppId, TF.QR.Config.AppSecret, base.Request.Url.AbsoluteUri);
            base.ViewData["jssdk"] = package;
            var product = Config.Helper.SingleById<DbProduct>(id);
            return View(product);
        }
        [WeUser]
        public ActionResult GenBuy(int id)
        {
            try
            {
                var openid = Request.Cookies["openid"].Value;
                var accessToken = Senparc.Weixin.MP.Containers.AccessTokenContainer.TryGetAccessToken(Config.AppId, Config.AppSecret);
                var userinfo = Senparc.Weixin.MP.AdvancedAPIs.UserApi.Info(accessToken, openid);
                var buy = Config.Helper.CreateWhere<DbBuy>()
                    .Where(o => o.ProductID == id && o.OpenID == openid && o.PayTime == null)
                    .FirstOrDefault();
                if (buy == null)
                {
                    buy = new DbBuy()
                    {
                        OpenID = openid,
                        ProductID = id,
                        WeName = userinfo.nickname,
                        ActualCost=Config.Helper.SingleById<DbProduct>(id).Cost
                    };
                    Config.Helper.Save(buy);
                }

                return RedirectToAction("ForBuy",new { id=buy.Id});
            }
            catch(Exception ex)
            {
                return Content(ex.Message);
            }            
        }
        [WeUser]
        public ActionResult ForBuy(int id)
        {
            JsSdkUiPackage package = JSSDKHelper.GetJsSdkUiPackage(TF.QR.Config.AppId, TF.QR.Config.AppSecret, base.Request.Url.AbsoluteUri);
            base.ViewData["jssdk"] = package;
            var buy = Config.Helper.SingleById<DbBuy>(id);
            return View(buy);
        }

        [WeUser,HttpPost]
        public ActionResult Support(int id)
        {
            using (var trans=Config.Helper.UseTransaction())
            {
                try
                {
                    var openid = Request.Cookies["openid"].Value;
                    //var accessToken = Senparc.Weixin.MP.Containers.AccessTokenContainer.TryGetAccessToken(Config.AppId, Config.AppSecret);
                    //var userinfo = Senparc.Weixin.MP.AdvancedAPIs.UserApi.Info(accessToken, openid);
                    var buy = Config.Helper.SingleById<DbBuy>(id);
                    if (buy.Supporters.Exists(o => o.SupporterOpenID == openid))
                    {
                        throw new Exception("您已支持过此次购买");
                    }
                    else
                    {
                        var support = new DbBuySupporter()
                        {
                            SupporterOpenID = openid,
                            SupporterImg =Request.Cookies["headimg"].Value,
                            SupporterWeName = Request.Cookies["wename"].Value,
                            BuyID = id
                        };
                        Config.Helper.Save(support);
                        buy.ActualCost = buy.Product.Cost * Convert.ToDecimal(Math.Pow(0.98, buy.Supporters.Count));
                        Config.Helper.Save(buy);
                    }
                    return Success("提交成功");
                }
                catch (Exception ex)
                {
                    trans.Abort();
                    return Error(ex.Message);
                }
            }
            
        }
    }
}
