namespace TF.QR
{
    using System;
    using System.Runtime.CompilerServices;
    using ToolGood.ReadyGo.Attributes;

    [Serializable]
    public class DbBase
    {
        public DbBase()
        {
            this.CreateTime = DateTime.Now;
            this.IsDel = false;
        }

        public DateTime CreateTime { get; set; }

        [FieldLength(0x3e8)]
        public string Flags { get; set; }

        public long Id { get; set; }

        public bool IsDel { get; set; }

        [FieldLength(200)]
        public string Memo { get; set; }

        [FieldLength(0xfa0)]
        public string Tag { get; set; }
    }
}

