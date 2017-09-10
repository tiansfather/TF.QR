namespace TF.QR.Controllers
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Web.Mvc;
    using TF.QR;
    using ToolGood.ReadyGo;

    public class DataController : BaseController
    {
        [HttpPost]
        public ActionResult FileUpload(string filename, string filecontent)
        {
            string path = string.Format("/upload/{0}/{1}/{2}/", DateTime.Now.Year, DateTime.Now.ToString("MM"), DateTime.Now.ToString("dd"));
            try
            {
                Directory.CreateDirectory(base.Server.MapPath(path));
            }
            catch
            {
            }
            Guid guid = Guid.NewGuid();
            string str2 = path + guid + Path.GetExtension(filename);
            Config.Base64ToImg(filecontent.Substring(filecontent.IndexOf(",") + 1), str2);
            return base.Success(str2);
        }

        #region 编码信息
        [User]
        public ActionResult QRStorage()
        {
            ParameterExpression expression5;
            long num = long.Parse(base.Request.QueryString["page"]);
            string str = base.Request.QueryString["activate"];
            string str2 = base.Request.QueryString["expired"];
            string searchkey = base.Request.QueryString["searchkey"];
            DbUser user = Config.GetCurrentUser();
            Page<DbQRInfo> page = Config.Helper.CreateWhere<DbQRInfo>()
                .IfTrue((user.DbRoleType != DbRoleType.Administrator)).Where(o => o.BindAdministrator == user.Id)
                .IfTrue((str == "1")).Where(o => o.ActivateDate != null)
                .IfTrue((str == "-1")).Where(o => o.ActivateDate == null)
                .IfTrue((str2 == "1")).Where(o => (o.ExpireDate != null) && (o.ExpireDate < Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))))
                //.IfTrue((str2 == "-1")).Where(Expression.Lambda<Func<DbQRInfo, bool>>(Expression.OrElse(Expression.Equal(Expression.Property(expression5 = Expression.Parameter(typeof(DbQRInfo), "o"), (MethodInfo) methodof(DbQRInfo.get_ExpireDate)), Expression.Convert(Expression.Constant(null, typeof(DateTime?)), typeof(DateTime?)), false, (MethodInfo) methodof(DateTime.op_Equality)), Expression.GreaterThan(Expression.Property(expression5, (MethodInfo) methodof(DbQRInfo.get_ExpireDate)), Expression.Convert(Expression.Call(null, (MethodInfo) methodof(Convert.ToDateTime), new Expression[] { Expression.Call(Expression.Property(null, (MethodInfo) methodof(DateTime.get_Now)), (MethodInfo) methodof(DateTime.ToString), new Expression[] { Expression.Constant("yyyy-MM-dd", typeof(string)) }) }), typeof(DateTime?)), false, (MethodInfo) methodof(DateTime.op_GreaterThan))), new ParameterExpression[] { expression5 }))
                .IfSet(searchkey).Where(o => ((((((o.Code.Contains(searchkey) || o.RealName.Contains(searchkey)) || o.ContactName.Contains(searchkey)) || o.ContactMobile.Contains(searchkey)) || o.ContactName2.Contains(searchkey)) || o.ContactMobile2.Contains(searchkey)) || o.Address.Contains(searchkey)) || o.Tip.Contains(searchkey)).OrderBy<DateTime>(o => o.CreateTime, OrderType.Desc).Page(num, 20L, null);
            IsoDateTimeConverter converter = new IsoDateTimeConverter
            {
                DateTimeFormat = "yyyy-MM-dd"
            };
            return base.Content(JsonConvert.SerializeObject(page, new JsonConverter[] { converter }));
        }

        [User, HttpPost]
        public ActionResult QRStorageActivate(string ids)
        {
            List<DbQRInfo> source = Config.Helper.CreateWhere<DbQRInfo>().AddWhereSql("id in ("+ids+")").Select(null);
            if (source.Exists(o => !o.CanBeAcessed))
            {
                return base.Error("没有操作权限");
            }
            source.AsParallel<DbQRInfo>().ForAll<DbQRInfo>(delegate (DbQRInfo o) {
                if (!o.ActivateDate.HasValue)
                {
                    o.ActivateDate = new DateTime?(DateTime.Now);
                }
                else
                {
                    o.ActivateDate = null;
                }
                Config.Helper.Save(o);
            });
            return base.Success("提交成功");
        }

        [HttpPost, User]
        public ActionResult QRStorageDelete(int id)
        {
            DbQRInfo poco = Config.Helper.SingleOrDefaultById<DbQRInfo>(id);
            if (poco == null)
            {
                return base.Error("信息不存在");
            }
            if (!poco.CanBeAcessed)
            {
                return base.Error("无权限操作");
            }
            Config.Helper.Delete<DbQRInfo>(poco);
            return base.Success("删除成功");
        }

        [HttpPost, User]
        public ActionResult QRStorageSubmit(DbQRInfo info)
        {
            if (string.IsNullOrEmpty(info.Code))
            {
                return base.Error("编码不能为空");
            }
            if ((info.Id == 0L) && Config.Helper.Exists<DbQRInfo>("where code=@0", new object[] { info.Code }))
            {
                return base.Error("相同编码已存在");
            }
            if (!info.CanBeAcessed)
            {
                return base.Error("无权限操作");
            }
            Config.Helper.Save(info);
            return base.Success("提交成功");
        }
        #endregion

        #region 订单信息
        [User]
        public ActionResult Orders()
        {
            long num = long.Parse(base.Request.QueryString["page"]);
            var payed = Request.QueryString["payed"];
            string searchkey = base.Request.QueryString["searchkey"];
            DbUser user = Config.GetCurrentUser();
            Page<DbBuy> page = Config.Helper.CreateWhere<DbBuy>()
                .Where(o=>!o.IsDel)
                .IfTrue(payed=="1").Where(o=>o.PayTime!=null)
                .IfTrue(payed=="-1").Where(o=>o.PayTime==null)
                .IfSet(searchkey).Where(o => o.Mobile.Contains(searchkey) ||o.Address.Contains(searchkey) || o.RealName.Contains(searchkey)).OrderBy(o => o.CreateTime, OrderType.Desc).Page(num, 20L, null);
            IsoDateTimeConverter converter = new IsoDateTimeConverter
            {
                DateTimeFormat = "yyyy-MM-dd"
            };
            return base.Content(JsonConvert.SerializeObject(page, new JsonConverter[] { converter }));
        }
        [HttpPost,User]
        public ActionResult OrdersDelete(int id)
        {
            var order = Config.Helper.SingleOrDefaultById<DbBuy>(id);
            if (order == null)
            {
                return base.Error("信息不存在");
            }
            Config.Helper.Delete<DbBuy>(order);
            return base.Success("删除成功");
        }
        #endregion

        #region 会员信息
        [User]
        public ActionResult WeUsers()
        {
            long num = long.Parse(base.Request.QueryString["page"]);
            string searchkey = base.Request.QueryString["searchkey"];
            DbUser user = Config.GetCurrentUser();
            Page<DbWeUser> page = Config.Helper.CreateWhere<DbWeUser>()
                .Where(o => !o.IsDel)
                .IfSet(searchkey).Where(o => o.Mobile.Contains(searchkey) || o.Address.Contains(searchkey) || o.RealName.Contains(searchkey)).OrderBy(o => o.CreateTime, OrderType.Desc).Page(num, 20L, null);
            IsoDateTimeConverter converter = new IsoDateTimeConverter
            {
                DateTimeFormat = "yyyy-MM-dd"
            };
            return base.Content(JsonConvert.SerializeObject(page, new JsonConverter[] { converter }));
        }
        #endregion

        #region 推荐信息
        [User]
        public ActionResult Recommand()
        {
            long num = long.Parse(base.Request.QueryString["page"]);
            string searchkey = base.Request.QueryString["searchkey"];
            DbUser user = Config.GetCurrentUser();
            Page<DbRecommand> page = Config.Helper.CreateWhere<DbRecommand>()
                .Where(o => !o.IsDel)
                .IfSet(searchkey).Where(o => o.Mobile.Contains(searchkey) || o.WeName.Contains(searchkey)).OrderBy(o => o.CreateTime, OrderType.Desc).Page(num, 20L, null);
            IsoDateTimeConverter converter = new IsoDateTimeConverter
            {
                DateTimeFormat = "yyyy-MM-dd"
            };
            return base.Content(JsonConvert.SerializeObject(page, new JsonConverter[] { converter }));
        }
        #endregion
        #region 提现信息
        [User]
        public ActionResult Cash()
        {
            try
            {
                long num = long.Parse(base.Request.QueryString["page"]);
                var status = Request.QueryString["status"];
                string searchkey = base.Request.QueryString["searchkey"];
                DbUser user = Config.GetCurrentUser();
                Page<DbCashHistory> page = Config.Helper.CreateWhere<DbCashHistory>()
                    .Where(o => !o.IsDel)
                    .IfTrue(status == "2").Where(o => o.Status == DbCashHistory.CashStatus.Cashed)
                    .IfTrue(status == "1").Where(o => o.Status == DbCashHistory.CashStatus.Created)
                    .IfSet(searchkey).AddWhereSql("openid in(select openid from dbweuser where mobile like '%" + searchkey + "%')")
                    .OrderBy(o => o.CreateTime, OrderType.Desc).Page(num, 20L, null);
                IsoDateTimeConverter converter = new IsoDateTimeConverter
                {
                    DateTimeFormat = "yyyy-MM-dd"
                };
                var result = new
                {
                    page.CurrentPage,
                    page.ItemsPerPage,
                    page.TotalItems,
                    page.TotalPages,
                    Items = page.Items.Select(o =>
                    {
                        var weuser = Config.Helper.FirstOrDefault<DbWeUser>("where openid=@0", o.OpenID);
                        return new {o.Id, o.CreateTime, o.Fee, o.Status, Mobile = weuser.Mobile, Name = weuser.RealName };
                    })
                };
                return base.Content(JsonConvert.SerializeObject(result, new JsonConverter[] { converter }));
            }catch(Exception ex)
            {
                return Content(ex.Message);
            }
            
        }
        [User]
        public ActionResult CashStatus(int id)
        {
            using (var trans=Config.Helper.UseTransaction())
            {
                try
                {
                    var record = Config.Helper.SingleById<DbCashHistory>(id);
                    if (record.Status == DbCashHistory.CashStatus.Cashed)
                    {
                        return Error("此记录已被处理");
                    }
                    else
                    {
                        var weuser = Config.Helper.CreateWhere<DbWeUser>().Where(o => o.OpenID == record.OpenID).Single();
                        weuser.Bonus -= record.Fee;
                        Config.Helper.Update(weuser);
                        record.Status = DbCashHistory.CashStatus.Cashed;
                        Config.Helper.Update(record);
                    }
                    return Success("提交成功");
                }catch(Exception ex)
                {
                    trans.Abort();
                    return Error(ex.Message);
                }
            }
            
        }
        #endregion

        public ActionResult ThumbFile(string filepath, int w)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                return null;
            }
            MemoryStream stream = Config.ThumbImageToStream(base.Server.MapPath(filepath), w, w);
            return base.File(stream.ToArray(), "image/jpeg");
        }
    }
}

