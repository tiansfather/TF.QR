namespace TF.QR
{
    using System;
    using System.Runtime.CompilerServices;
    using ToolGood.ReadyGo.Attributes;

    public class DbConfig
    {
        [FieldLength(50)]
        public string ConfigKey { get; set; }

        [FieldLength(50)]
        public string ConfigValue { get; set; }

        public int Id { get; set; }
    }
}

