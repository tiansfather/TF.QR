namespace TF.QR.Controllers
{
    using Newtonsoft.Json;
    using Senparc.Weixin.MP;
    using Senparc.Weixin.MP.AdvancedAPIs;
    using Senparc.Weixin.MP.Containers;
    using Senparc.Weixin.MP.Entities.Request;
    using Senparc.Weixin.MP.MvcExtension;
    using Senparc.Weixin.MP.Sample.CommonService.CustomMessageHandler;
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Web.Mvc;
    using TF.QR;

    public class WeiXinController : BaseController
    {
        private readonly Func<string> _getRandomFileName = () => (DateTime.Now.ToString("yyyyMMdd-HHmmss") + Guid.NewGuid().ToString("n").Substring(0, 6));
        public static readonly string AppId = TF.QR.Config.AppId;
        public static readonly string AppSecret = TF.QR.Config.AppSecret;
        public static readonly string EncodingAESKey = TF.QR.Config.EncodingAESKey;
        public static readonly string Token = TF.QR.Config.Token;


        [HttpGet, ActionName("Index")]
        public ActionResult Get(PostModel postModel, string echostr)
        {
            if (CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Token))
            {
                return base.Content(echostr);
            }
            return base.Content("failed:" + postModel.Signature + "," + CheckSignature.GetSignature(postModel.Timestamp, postModel.Nonce, Token) + "。如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
        }

        [ActionName("Index"), HttpPost]
        public ActionResult MiniPost(PostModel postModel)
        {
            if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Token))
            {
                return new WeixinResult("参数错误！");
            }
            postModel.Token = Token;
            postModel.EncodingAESKey = EncodingAESKey;
            postModel.AppId = AppId;
            Senparc.Weixin.MP.Sample.CommonService.CustomMessageHandler.CustomMessageHandler messageHandlerDocument = new Senparc.Weixin.MP.Sample.CommonService.CustomMessageHandler.CustomMessageHandler(base.Request.InputStream, postModel, 10);
            messageHandlerDocument.Execute();
            return new FixWeixinBugWeixinResult(messageHandlerDocument);
        }

        public ActionResult R(string openid)
        {
            
            base.SetCookie("openid", openid);
            return this.Redirect("/Service");
        }

        [WeUser]
        public ActionResult UploadFile(string serverId, string filename)
        {
            string accessTokenOrAppId = AccessTokenContainer.TryGetAccessToken(AppId, AppSecret, false);
            string path = string.Concat(new object[] { "/upload/", DateTime.Now.Year, "/", DateTime.Now.Month.ToString("D2"), "/", DateTime.Now.Day.ToString("D2"), "/" });
            if (!Directory.Exists(base.Server.MapPath(path)))
            {
                Directory.CreateDirectory(base.Server.MapPath(path));
            }
            Guid guid = Guid.NewGuid();
            string str3 = path + guid + ".jpg";
            FileStream stream = new FileStream(base.Server.MapPath(str3), FileMode.Create);
            MediaApi.Get(accessTokenOrAppId, serverId, stream);
            stream.Close();
            return base.Content(JsonConvert.SerializeObject(new { path=str3}));
        }
    }
}

