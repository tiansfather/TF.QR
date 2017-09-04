namespace Senparc.Weixin.MP.Sample.CommonService.Download
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class Config
    {
        public int DownloadCount { get; set; }

        public int QrCodeId { get; set; }

        public List<string> Versions { get; set; }

        public List<string> WebVersions { get; set; }
    }
}

