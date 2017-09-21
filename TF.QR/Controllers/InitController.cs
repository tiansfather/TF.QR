namespace TF.QR.Controllers
{
    using System.Web.Mvc;
    using TF.QR;

    public class InitController : Controller
    {
        public ActionResult Index()
        {
            Config.Helper.TableHelper.TryCreateTable<DbUser>();
            Config.Helper.TableHelper.TryCreateTable<DbQRInfo>();
            Config.Helper.TableHelper.TryCreateTable<DbConfig>();
            Config.Helper.TableHelper.TryCreateTable<DbProduct>();
            Config.Helper.TableHelper.TryCreateTable<DbBuy>();
            Config.Helper.TableHelper.TryCreateTable<DbBuySupporter>();
            Config.Helper.TableHelper.TryCreateTable<DbWeUser>();
            Config.Helper.TableHelper.TryCreateTable<DbRecommand>();
            Config.Helper.TableHelper.TryCreateTable<DbCashHistory>();
            return base.Content("OK");
        }

        public ActionResult Info()
        {
            var openid = "ooZJguE850-mSlAN8P6TiTCVU2Y0";
            var accessToken = Senparc.Weixin.MP.Containers.AccessTokenContainer.TryGetAccessToken(Config.AppId, Config.AppSecret);
            var userinfo = Senparc.Weixin.MP.AdvancedAPIs.UserApi.Info(accessToken, openid);
            return Content(accessToken + "-" + Newtonsoft.Json.JsonConvert.SerializeObject(userinfo));
        }
    }
}

