using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToolGood.ReadyGo.Attributes;

namespace TF.QR
{
    public class DbRecommand:DbBase
    {
        [FieldLength(50)]
        public string Mobile { get; set; }
        [FieldLength(50)]
        public string OpenID { get; set; }
        [FieldLength(50)]
        public string WeName { get; set; }
        public decimal Fee { get; set; }
    }
}