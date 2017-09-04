namespace Senparc.Weixin.MP.Sample.CommonService.Download
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Xml.Linq;

    public class ConfigHelper
    {
        private HttpContextBase _context;
        public static Dictionary<string, CodeRecord> CodeCollection = new Dictionary<string, CodeRecord>(StringComparer.OrdinalIgnoreCase);
        public static object Lock = new object();

        public ConfigHelper(HttpContextBase context)
        {
            this._context = context;
        }

        public string Download(string version, bool isWebVersion)
        {
            lock (Lock)
            {
                Config config = this.GetConfig();
                config.DownloadCount++;
                this.Save(config);
            }
            return this._context.Server.MapPath(string.Format("~/App_Data/Document/Files/Senparc.Weixin{0}-v{1}.rar", isWebVersion ? "-Web" : "", version));
        }

        public Config GetConfig()
        {
            XDocument xDocument = this.GetXDocument();
            return new Config { 
                QrCodeId = int.Parse(xDocument.Root.Element("QrCodeId").Value),
                DownloadCount = int.Parse(xDocument.Root.Element("DownloadCount").Value),
                Versions = (from z in xDocument.Root.Element("Versions").Elements("Version") select z.Value).ToList<string>(),
                WebVersions = (from z in xDocument.Root.Element("WebVersions").Elements("Version") select z.Value).ToList<string>()
            };
        }

        private string GetDatabaseFilePath()
        {
            return this._context.Server.MapPath("~/App_Data/Document/Config.xml");
        }

        public int GetQrCodeId()
        {
            lock (Lock)
            {
                Config config = this.GetConfig();
                config.QrCodeId++;
                this.Save(config);
                return config.QrCodeId;
            }
        }

        private XDocument GetXDocument()
        {
            return XDocument.Load(this.GetDatabaseFilePath());
        }

        public void Save(Config config)
        {
            XDocument xDocument = this.GetXDocument();
            xDocument.Root.Element("QrCodeId").Value = config.QrCodeId.ToString();
            xDocument.Root.Element("DownloadCount").Value = config.DownloadCount.ToString();
            xDocument.Root.Element("Versions").Elements().Remove<XElement>();
            foreach (string str in config.Versions)
            {
                xDocument.Root.Element("Versions").Add(new XElement("Version", str));
            }
            xDocument.Save(this.GetDatabaseFilePath());
        }
    }
}

