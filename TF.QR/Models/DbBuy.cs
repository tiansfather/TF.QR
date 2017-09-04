using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToolGood.ReadyGo.Attributes;

namespace TF.QR
{
    public class DbBuy:DbBase
    {
        public int ProductID { get; set; }
        public string OpenID { get; set; }
        public string WeName { get; set; }
        public decimal ActualCost { get; set; }
        public DateTime? PayTime { get; set; }
        public string OrderNo { get; set; }
        public string RealName { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public string RecommandMobile { get; set; }
        [Ignore]
        public List<DbBuySupporter> Supporters => Config.Helper.CreateWhere<DbBuySupporter>()
                    .Where(o => o.BuyID == this.Id)
                    .Select();

        [Ignore]
        public DbProduct Product => Config.Helper.SingleById<DbProduct>(ProductID);
    }

    public class DbBuySupporter : DbBase
    {
        public int BuyID { get; set; }
        public string SupporterOpenID { get; set; }
        public string SupporterWeName { get; set; }
        public string SupporterImg { get; set; }
    }
}