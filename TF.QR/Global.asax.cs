using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.MP.TenPayLib;
using Senparc.Weixin.MP.TenPayLibV3;
using Senparc.Weixin.Threads;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace TF.QR
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        

        private void Application_End(object sender, EventArgs e)
        {
            Thread.Sleep(0x3e8);
            string requestUriString = "http://localhost";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUriString);
            ((HttpWebResponse)request.GetResponse()).GetResponseStream();
        }

        private void Application_Error(object sender, EventArgs e)
        {
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            System.Timers.Timer source = new System.Timers.Timer(3600000.0);
            source.Elapsed += new ElapsedEventHandler(this.myTimer_Elapsed);
            source.Enabled = true;
            source.AutoReset = true;
            this.myTimer_Elapsed(source, null);

            /* 微信配置开始
               * 
               * 建议按照以下顺序进行注册，尤其须将缓存放在第一位！
               */

            RegisterWeixinCache();      //注册分布式缓存（按需，如果需要，必须放在第一个）
            ConfigWeixinTraceLog();     //配置微信跟踪日志（按需）
            RegisterWeixinThreads();    //激活微信缓存及队列线程（必须）
            RegisterSenparcWeixin();    //注册Demo所用微信公众号的账号信息（按需）
            RegisterWeixinPay();        //注册微信支付（按需）

            /* 微信配置结束 */
        }

        private void myTimer_Elapsed(object source, ElapsedEventArgs e)
        {
            Task.Factory.StartNew(() => Config.AutoShrinkAndBackUp());
        }

        private void Session_End(object sender, EventArgs e)
        {
        }

        private void Session_Start(object sender, EventArgs e)
        {
            base.Session.Timeout = 600;
        }

        /// <summary>
        /// 自定义缓存策略
        /// </summary>
        private void RegisterWeixinCache()
        {
            //如果留空，默认为localhost（默认端口）

            #region  Redis配置
            //var redisConfiguration = System.Configuration.ConfigurationManager.AppSettings["Cache_Redis_Configuration"];
            //RedisManager.ConfigurationOption = redisConfiguration;

            //////如果不执行下面的注册过程，则默认使用本地缓存

            //if (!string.IsNullOrEmpty(redisConfiguration) && redisConfiguration != "Redis配置")
            //{
            //    CacheStrategyFactory.RegisterObjectCacheStrategy(() => RedisObjectCacheStrategy.Instance);//Redis
            //}
            #endregion

            #region Memcached 配置

            //var memcachedConfig = new Dictionary<string, int>()
            //{
            //    { "localhost",9101 }
            //};
            //MemcachedObjectCacheStrategy.RegisterServerList(memcachedConfig);

            #endregion

            //CacheStrategyFactory.RegisterContainerCacheStrategy(() => MemcachedContainerStrategy.Instance);//Memcached
        }

        /// <summary>
        /// 注册WebSocket模块（可用于小程序或独立WebSocket应用）
        /// </summary>
        private void RegisterWebSocket()
        {
#if NET45
            Senparc.WebSocket.WebSocketConfig.RegisterRoutes(RouteTable.Routes);
            Senparc.WebSocket.WebSocketConfig.RegisterMessageHandler<CustomWebSocketMessageHandler>();
#endif
        }

        /// <summary>
        /// 激活微信缓存
        /// </summary>
        private void RegisterWeixinThreads()
        {
            ThreadUtility.Register();
        }

        /// <summary>
        /// 注册Demo所用微信公众号的账号信息
        /// </summary>
        private void RegisterSenparcWeixin()
        {
            //注册公众号
            AccessTokenContainer.Register(
                System.Configuration.ConfigurationManager.AppSettings["WeixinAppId"],
                System.Configuration.ConfigurationManager.AppSettings["WeixinAppSecret"],
                "公众号");
        }
        

        /// <summary>
        /// 注册微信支付
        /// </summary>
        private void RegisterWeixinPay()
        {
            //提供微信支付信息
            //var weixinPay_PartnerId = System.Configuration.ConfigurationManager.AppSettings["WeixinPay_PartnerId"];
            //var weixinPay_Key = System.Configuration.ConfigurationManager.AppSettings["WeixinPay_Key"];
            //var weixinPay_AppId = System.Configuration.ConfigurationManager.AppSettings["WeixinPay_AppId"];
            //var weixinPay_AppKey = System.Configuration.ConfigurationManager.AppSettings["WeixinPay_AppKey"];
            //var weixinPay_TenpayNotify = System.Configuration.ConfigurationManager.AppSettings["WeixinPay_TenpayNotify"];

            var tenPayV3_MchId = System.Configuration.ConfigurationManager.AppSettings["TenPayV3_MchId"];
            var tenPayV3_Key = System.Configuration.ConfigurationManager.AppSettings["TenPayV3_Key"];
            var tenPayV3_AppId = System.Configuration.ConfigurationManager.AppSettings["TenPayV3_AppId"];
            var tenPayV3_AppSecret = System.Configuration.ConfigurationManager.AppSettings["TenPayV3_AppSecret"];
            var tenPayV3_TenpayNotify = System.Configuration.ConfigurationManager.AppSettings["TenPayV3_TenpayNotify"];

            //var weixinPayInfo = new TenPayInfo(weixinPay_PartnerId, weixinPay_Key, weixinPay_AppId, weixinPay_AppKey, weixinPay_TenpayNotify);
            //TenPayInfoCollection.Register(weixinPayInfo);
            var tenPayV3Info = new TenPayV3Info(tenPayV3_AppId, tenPayV3_AppSecret, tenPayV3_MchId, tenPayV3_Key,
                                                tenPayV3_TenpayNotify);
            TenPayV3InfoCollection.Register(tenPayV3Info);
        }

        

        /// <summary>
        /// 配置微信跟踪日志
        /// </summary>
        private void ConfigWeixinTraceLog()
        {
            ////这里设为Debug状态时，/App_Data/WeixinTraceLog/目录下会生成日志文件记录所有的API请求日志，正式发布版本建议关闭
            //Senparc.Weixin.Config.IsDebug = true;
            //Senparc.Weixin.WeixinTrace.SendCustomLog("系统日志", "系统启动");//只在Senparc.Weixin.Config.IsDebug = true的情况下生效

            ////自定义日志记录回调
            //Senparc.Weixin.WeixinTrace.OnLogFunc = () =>
            //{
            //    //加入每次触发Log后需要执行的代码
            //};

            ////当发生基于WeixinException的异常时触发
            //Senparc.Weixin.WeixinTrace.OnWeixinExceptionFunc = ex =>
            //{
            //    //加入每次触发WeixinExceptionLog后需要执行的代码

            //    //发送模板消息给管理员
            //    var eventService = new EventService();
            //    eventService.ConfigOnWeixinExceptionFunc(ex);
            //};
        }
    }
}