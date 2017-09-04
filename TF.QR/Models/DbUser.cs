namespace TF.QR
{
    using System;
    using System.Runtime.CompilerServices;
    using ToolGood.ReadyGo.Attributes;

    public class DbUser : DbBase
    {
        public DbUser()
        {
            this.Enable = true;
        }

        public TF.QR.DbRoleType DbRoleType { get; set; }

        public bool Enable { get; set; }

        [FieldLength(50)]
        public string PassWord { get; set; }

        [FieldLength(50)]
        public string TrueName { get; set; }

        [FieldLength(50)]
        public string UserName { get; set; }
    }
}

