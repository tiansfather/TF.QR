namespace Senparc.Weixin.MP.Sample.CommonService.Download
{
    using Senparc.Weixin.MP.AdvancedAPIs.QrCode;
    using System;
    using System.Runtime.CompilerServices;

    public class CodeRecord
    {
        public bool AllowDownload { get; set; }

        public bool IsWebVersion { get; set; }

        public string Key { get; set; }

        public int QrCodeId { get; set; }

        public CreateQrCodeResult QrCodeTicket { get; set; }

        public bool Used { get; set; }

        public string Version { get; set; }
    }
}

