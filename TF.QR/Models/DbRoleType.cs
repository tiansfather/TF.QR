namespace TF.QR
{
    using System;
    using System.ComponentModel;

    public enum DbRoleType
    {
        [Description("管理员")]
        Administrator = 0,
        [Description("分管理员")]
        SubManager = 1
    }
}

