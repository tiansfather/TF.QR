namespace TF.QR.Controllers
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using TF.Common;
    using TF.QR;
    using ToolGood.ReadyGo;
    using System.Collections.Generic;
    public class MyController : BaseController
    {
        [User]
        public ActionResult Export()
        {
            ParameterExpression expression5;
            string str = base.Request.QueryString["activate"];
            string str2 = base.Request.QueryString["expired"];
            string searchkey = base.Request.QueryString["searchkey"];
            DbUser user = Config.GetCurrentUser();
            var list=Config.Helper.CreateWhere<DbQRInfo>()
                .IfTrue(user.DbRoleType != DbRoleType.Administrator).Where(o => o.BindAdministrator == user.Id)
                .IfTrue((str == "1")).Where(o => o.ActivateDate != null)
                .IfTrue((str == "-1")).Where(o => o.ActivateDate == null)
                .IfTrue((str2 == "1")).Where(o => (o.ExpireDate != null) && (o.ExpireDate < Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))))
                //.IfTrue((str2 == "-1")).Where(Expression.Lambda<Func<DbQRInfo, bool>>(Expression.OrElse(Expression.Equal(Expression.Property(expression5 = Expression.Parameter(typeof(DbQRInfo), "o"), (MethodInfo)methodof(DbQRInfo.get_ExpireDate)), Expression.Convert(Expression.Constant(null, typeof(DateTime?)), typeof(DateTime?)), false, (MethodInfo)methodof(DateTime.op_Equality)), Expression.GreaterThan(Expression.Property(expression5, (MethodInfo)methodof(DbQRInfo.get_ExpireDate)), Expression.Convert(Expression.Call(null, (MethodInfo)methodof(Convert.ToDateTime), new Expression[] { Expression.Call(Expression.Property(null, (MethodInfo)methodof(DateTime.get_Now)), (MethodInfo)methodof(DateTime.ToString), new Expression[] { Expression.Constant("yyyy-MM-dd", typeof(string)) }) }), typeof(DateTime?)), false, (MethodInfo)methodof(DateTime.op_GreaterThan))), new ParameterExpression[] { expression5 }))
                .IfSet(searchkey).Where(o => ((((((o.Code.Contains(searchkey) || o.RealName.Contains(searchkey)) || o.ContactName.Contains(searchkey)) || o.ContactMobile.Contains(searchkey)) || o.ContactName2.Contains(searchkey)) || o.ContactMobile2.Contains(searchkey)) || o.Address.Contains(searchkey)) || o.Tip.Contains(searchkey)).OrderBy<DateTime>(o => o.CreateTime, OrderType.Desc).Select(null);
            DbQRInfo.Export(list);
            return base.File(base.Server.MapPath("/编码信息.txt"));
        }

        [User]
        public ActionResult Index()
        {
            return RedirectToAction("Orders");
            //return base.View();
        }

        public ActionResult Login()
        {
            base.Session["user"] = null;
            return base.View();
        }

        [HttpPost]
        public ActionResult Login(string user, string pwd)
        {
            SqlHelper helper = Config.Helper;
            pwd = Md5.GetMd5String(pwd);
            DbUser user2 = helper.CreateWhere<DbUser>().Where(q => !q.IsDel).Where(q => q.Enable).Where(q => q.UserName == user).Where(q => q.PassWord == pwd).FirstOrDefault(null);
            if (user2 == null)
            {
                return base.Error("用户名或密码错误！");
            }
            base.Session["user"] = user2;
            return base.Success();
        }

        [User]
        public ActionResult QRGenerate()
        {
            return base.View();
        }

        [HttpPost, User]
        public ActionResult QRGenerate(int gennumber)
        {
            var codeList = new List<string>();
            Parallel.For(0, gennumber, delegate (int o) {
                codeList.Add(Config.GenCode());
            });
            //生成文件
            System.IO.File.WriteAllText(Server.MapPath("/code.txt"), "");
            foreach(var code in codeList)
            {
                System.IO.File.AppendAllLines(Server.MapPath("/code.txt"), new string[] { code });
            }
            return base.Success("生成成功");
        }

        [User]
        public ActionResult QRStorage()
        {
            return base.View();
        }
        [User]
        public ActionResult Product()
        {
            var products = Config.Helper.CreateWhere<DbProduct>().Select();
            return View(products);
        }
        [User]
        public ActionResult ProductAdd()
        {
            return View();
        }
        [User, HttpPost]
        [ValidateInput(false)]
        public ActionResult ProductAdd(DbProduct product)
        {
            Config.Helper.Insert(product);
            return Success("提交成功");
        }
        [User]
        public ActionResult ProductEdit(int id)
        {
            var product = Config.Helper.SingleById<DbProduct>(id);
            return View(product);
        }
        [User,HttpPost]
        [ValidateInput(false)]
        public ActionResult ProductEdit(DbProduct product)
        {
            Config.Helper.Save(product);
            return Success("提交成功");
        }
        [User,HttpPost]
        public ActionResult ProductDel(int id)
        {
            Config.Helper.DeleteById<DbProduct>(id);
            return Success("OK");
        }
        [User]
        public ActionResult Orders()
        {
            return View();
        }
        [User]
        public ActionResult WeUsers()
        {
            return View();
        }
        [User]
        public ActionResult Recommand()
        {
            return View();
        }
        [User]
        public ActionResult Cash()
        {
            return View();
        }
    }
}

