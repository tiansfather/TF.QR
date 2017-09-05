using Senparc.Weixin;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.TenPayLibV3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace TF.QR.Controllers
{
    public class PayController : BaseController
    {
        private static TenPayV3Info _tenPayV3Info;

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            if (errors == SslPolicyErrors.None)
                return true;
            return false;
        }
        public static TenPayV3Info TenPayV3Info
        {
            get
            {
                if (_tenPayV3Info == null)
                {
                    _tenPayV3Info =
                        TenPayV3InfoCollection.Data[System.Configuration.ConfigurationManager.AppSettings["TenPayV3_MchId"]];
                }
                return _tenPayV3Info;
            }
        }
        /// <summary>
        /// 获取用户的OpenId
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(int buyid = 0, int hc = 0)
        {
            var returnUrl = string.Format("http://1.jn99.net/Pay/JsApi");
            var state = string.Format("{0}|{1}", buyid, hc);
            var url = OAuthApi.GetAuthorizeUrl(TenPayV3Info.AppId, returnUrl, state, OAuthScope.snsapi_userinfo);

            return Redirect(url);
        }

        public ActionResult JsApi(string code, string state)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Content("您拒绝了授权！");
            }

            if (!state.Contains("|"))
            {
                //这里的state其实是会暴露给客户端的，验证能力很弱，这里只是演示一下
                //实际上可以存任何想传递的数据，比如用户ID，并且需要结合例如下面的Session["OAuthAccessToken"]进行验证
                return Content("验证失败！请从正规途径进入！1001");
            }
            try
            {


                

                //通过，用code换取access_token
                var openIdResult = OAuthApi.GetAccessToken(TenPayV3Info.AppId, TenPayV3Info.AppSecret, code);
                if (openIdResult.errcode != ReturnCode.请求成功)
                {
                    return Content("错误：" + openIdResult.errmsg);
                }
                //获取产品信息
                var stateData = state.Split('|');
                int buyid = 0;
                DbBuy buy = null;
                if (int.TryParse(stateData[0], out buyid))
                {
                    buy = Config.Helper.SingleOrDefaultById<DbBuy>(buyid);
                    if (buy == null || buy.OpenID!=openIdResult.openid || buy.PayTime!=null)
                    {
                        return Content("商品信息不存在，或非法进入！1002");
                    }
                    ViewData["product"] = buy;
                }
                string sp_billno = Request["order_no"];
                if (string.IsNullOrEmpty(sp_billno))
                {
                    //生成订单10位序列号，此处用时间和随机数生成，商户根据自己调整，保证唯一
                    sp_billno = string.Format("{0}{1}{2}", TenPayV3Info.MchId/*10位*/, DateTime.Now.ToString("yyyyMMddHHmmss"),
                        TenPayV3Util.BuildRandomStr(6));
                }
                //else
                //{
                //    sp_billno = Request["order_no"];
                //}

                var timeStamp = TenPayV3Util.GetTimestamp();
                var nonceStr = TenPayV3Util.GetNoncestr();

                var body = buy == null ? "test" : buy.Product.ProductName;
                var price = buy == null ? 100 : (int)buy.ActualCost * 100;
                var xmlDataInfo = new TenPayV3UnifiedorderRequestData(TenPayV3Info.AppId, TenPayV3Info.MchId, body, sp_billno, price, Request.UserHostAddress, TenPayV3Info.TenPayV3Notify, TenPayV3Type.JSAPI, openIdResult.openid, TenPayV3Info.Key, nonceStr);

                var result = TenPayV3.Unifiedorder(xmlDataInfo);//调用统一订单接口
                //JsSdkUiPackage jsPackage = new JsSdkUiPackage(TenPayV3Info.AppId, timeStamp, nonceStr,);
                var package = string.Format("prepay_id={0}", result.prepay_id);

                buy.OrderNo = sp_billno;
                Config.Helper.Save(buy);

                ViewData["appId"] = TenPayV3Info.AppId;
                ViewData["timeStamp"] = timeStamp;
                ViewData["nonceStr"] = nonceStr;
                ViewData["package"] = package;
                ViewData["paySign"] = TenPayV3.GetJsPaySign(TenPayV3Info.AppId, timeStamp, nonceStr, package, TenPayV3Info.Key);

                return View(buy);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                msg += "<br>" + ex.StackTrace;
                msg += "<br>==Source==<br>" + ex.Source;

                if (ex.InnerException != null)
                {
                    msg += "<br>===InnerException===<br>" + ex.InnerException.Message;
                }
                return Content(msg);
            }
        }

        public ActionResult Callback()
        {

            ResponseHandler resHandler = new ResponseHandler(null);

            string return_code = resHandler.GetParameter("return_code");
            string return_msg = resHandler.GetParameter("return_msg");

            string res = null;

            resHandler.SetKey(TenPayV3Info.Key);
            //验证请求是否从微信发过来（安全）
            if (resHandler.IsTenpaySign())
            {
                res = "success";

                //正确的订单处理
                //商户在收到后台通知后根据通知ID向财付通发起验证确认，采用后台系统调用交互模式
                string notify_id = resHandler.GetParameter("notify_id");
                //取结果参数做业务处理
                string out_trade_no = resHandler.GetParameter("out_trade_no");
                //财付通订单号
                string transaction_id = resHandler.GetParameter("transaction_id");
                //金额,以分为单位
                string total_fee = resHandler.GetParameter("total_fee");
                //如果有使用折扣券，discount有值，total_fee+discount=原请求的total_fee
                string discount = resHandler.GetParameter("discount");
                //支付结果
                string result_code = resHandler.GetParameter("result_code");

                if (result_code == "SUCCESS")
                {
                    var buy = Config.Helper.CreateWhere<DbBuy>()
                        .Where(o => o.OrderNo == out_trade_no)
                        .SingleOrDefault();
                    if (buy != null)
                    {
                        buy.PayTime = DateTime.Now;
                        Config.Helper.Save(buy);
                    }
                }

                
            }
            else
            {
                res = "wrong";

                //错误的订单处理
            }

            var fileStream = System.IO.File.OpenWrite(Server.MapPath("~/1.txt"));
            fileStream.Write(Encoding.Default.GetBytes(res), 0, Encoding.Default.GetByteCount(res));
            fileStream.Close();

            string xml = string.Format(@"<xml>
   <return_code><![CDATA[{0}]]></return_code>
   <return_msg><![CDATA[{1}]]></return_msg>
</xml>", return_code, return_msg);

            return Content(xml, "text/xml");
        }

        [WeUser]
        public ActionResult Success(int buyid)
        {
            var buy = Config.Helper.SingleById<DbBuy>(buyid);

            var weuser = Config.Helper.CreateWhere<DbWeUser>()
                .Where(o => o.OpenID == buy.OpenID).SingleOrDefault();
            if (weuser != null)
            {
                buy.Mobile = weuser.Mobile;
                buy.RealName = weuser.RealName;
                buy.Address = weuser.Address;
                Config.Helper.Save(buy);
            }

            return View(buy);
        }
        [HttpPost,WeUser]
        public ActionResult Success(int buyid,FormCollection form)
        {
            using(var trans = Config.Helper.UseTransaction())
            {
                try
                {
                    var buy = Config.Helper.SingleById<DbBuy>(buyid);
                    if (Request.Cookies["openid"].Value != buy.OpenID)
                    {
                        throw new Exception("数据错误");
                    }
                    if (!string.IsNullOrEmpty(buy.RecommandMobile))
                    {
                        //return Error("请勿重复提交");
                        throw new Exception("请勿重复提交");
                    }
                    buy.Mobile = form["Mobile"];
                    buy.RealName = form["RealName"];
                    buy.Address = form["Address"];
                    //buy.RecommandMobile = form["RecommandMobile"];
                    //判断推荐人是否存在
                    //if (!string.IsNullOrEmpty(buy.RecommandMobile))
                    //{
                    //    var recommanduser = Config.Helper.CreateWhere<DbWeUser>()
                    //    .Where(o => o.Mobile == buy.RecommandMobile).FirstOrDefault();
                    //    if (recommanduser == null)
                    //    {
                    //        //return Error("推荐人不存在");
                    //        throw new Exception("推荐人不存在");
                    //    }
                    //    else
                    //    {
                    //        var recommand = new DbRecommand()
                    //        {
                    //            OpenID=buy.OpenID,
                    //            Mobile = buy.RecommandMobile,
                    //            Fee = buy.ActualCost * 0.1M,
                    //            WeName = buy.WeName
                    //        };
                    //        Config.Helper.Save(recommand);
                    //        recommanduser.Bonus += recommand.Fee;
                    //        Config.Helper.Save(recommanduser);
                    //    }
                    //}
                    
                    Config.Helper.Save(buy);
                    //加入会员表
                    var weuser = Config.Helper.CreateWhere<DbWeUser>()
                        .Where(o => o.OpenID == buy.OpenID)
                        .SingleOrDefault();
                    if (weuser == null)
                    {
                        //手机号是否已存在
                        if (Config.Helper.Exists<DbWeUser>("where mobile=@0", buy.Mobile))
                        {
                            //return Error("此手机号码已经被另一微信帐号使用");
                            throw new Exception("此手机号码已经被另一微信帐号使用");
                        }
                        weuser = new DbWeUser()
                        {
                            OpenID = buy.OpenID,
                            RealName = buy.RealName,
                            Mobile = buy.Mobile,
                            Address = buy.Address
                        };
                        Config.Helper.Save(weuser);
                    }
                    return Success("提交成功");
                }
                catch(Exception ex)
                {
                    trans.Abort();
                    return Error(ex.Message);
                }
            }
            
        }
    }
}
