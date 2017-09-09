using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToolGood.ReadyGo.Attributes;

namespace TF.QR
{
    public class DbCashHistory:DbBase
    {
        [FieldLength(100)]
        public string OpenID { get; set; }
        public decimal Fee { get; set; }
        public CashStatus Status { get; set; }

        public enum CashStatus
        {
            Created=1,
            Cashed=2
        }
    }
}