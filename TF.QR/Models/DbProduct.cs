using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToolGood.ReadyGo.Attributes;

namespace TF.QR
{
    public class DbProduct : DbBase
    {
        [FieldLength(50)]
        public string ProductName { get; set; }
        [FieldLength(500)]
        public string Photos { get; set; }
        public decimal Cost { get; set; }
        [FieldLength(50)]
        public string Discription { get; set; }
    }
}