namespace TF.QR
{
    using System;
    using System.Runtime.CompilerServices;

    public class DbWeUser : DbBase
    {
        public string IDCard { get; set; }

        public string Mobile { get; set; }

        public string OpenID { get; set; }

        public string RealName { get; set; }

        public decimal Bonus { get; set; }

        public string Address { get; set; }
    }
}

