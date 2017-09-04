namespace TF.QR
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Web;
    using ToolGood.ReadyGo.Attributes;

    public class DbQRInfo : DbBase
    {
        public static void Export(List<DbQRInfo> data)
        {            
            File.WriteAllText(HttpContext.Current.Server.MapPath("/编码信息.txt"), "");
            File.AppendAllLines(HttpContext.Current.Server.MapPath("/编码信息.txt"), from o in data select "http://1.jn99.net/" + o.Code);
        }

        public DateTime? ActivateDate { get; set; }

        [FieldLength(200)]
        public string Address { get; set; }

        public int? BindAdministrator { get; set; }

        public DateTime? BirthDay { get; set; }

        [Ignore]
        public bool CanBeAcessed
        {
            get
            {
                DbUser currentUser = Config.GetCurrentUser();
                if (currentUser != null)
                {
                    if (currentUser.DbRoleType == DbRoleType.Administrator)
                    {
                        goto Label_003B;
                    }
                    int? bindAdministrator = this.BindAdministrator;
                    long id = currentUser.Id;
                    if ((((long) bindAdministrator.GetValueOrDefault()) == id) && bindAdministrator.HasValue)
                    {
                        goto Label_003B;
                    }
                }
                return false;
            Label_003B:
                return true;
            }
        }

        [FieldLength(50)]
        public string Code { get; set; }

        [FieldLength(50)]
        public string ContactMobile { get; set; }

        [FieldLength(50)]
        public string ContactMobile2 { get; set; }

        [FieldLength(50)]
        public string ContactName { get; set; }

        [FieldLength(50)]
        public string ContactName2 { get; set; }

        public DateTime? ExpireDate { get; set; }

        [FieldLength(100)]
        public string OpenID { get; set; }

        [FieldLength(100)]
        public string Photo { get; set; }

        [FieldLength(50)]
        public string RealName { get; set; }

        [FieldLength(50)]
        public string Sex { get; set; }

        [FieldLength(200)]
        public string Tip { get; set; }

        [FieldLength(10)]
        public string VerifyCode { get; set; }
    }
}

