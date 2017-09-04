namespace TF.QR
{
    using System;
    using System.Configuration;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Configuration;
    using ToolGood.ReadyGo;
    using ToolGood.ReadyGo.Internals;

    public static class Config
    {
        private static SqlHelper _helper;
        public static readonly string AppId = WebConfigurationManager.AppSettings["WeixinAppId"];
        public static readonly string AppSecret = WebConfigurationManager.AppSettings["WeixinAppSecret"];
        public static readonly string EncodingAESKey = WebConfigurationManager.AppSettings["WeixinEncodingAESKey"];
        private static object lockobj = new object();
        public static readonly string Token = WebConfigurationManager.AppSettings["WeixinToken"];

        public static void AutoShrinkAndBackUp()
        {
            string configValue = GetConfigValue("LastBackUpDate");
            bool flag = false;
            if (string.IsNullOrEmpty(configValue))
            {
                flag = true;
            }
            else
            {
                TimeSpan span = (TimeSpan) (DateTime.Now - Convert.ToDateTime(configValue));
                if (span.TotalDays > 30.0)
                {
                    flag = true;
                }
            }
            string backupPath = GetBackupPath();
            if (flag)
            {
                Helper.Execute("exec p_shrinkdb ", new object[0]);
                string sql = "exec p_backupdb @bkpath='" + backupPath + @"',@bkfname='tfqr_\DATE\_db.bak',@bktype='DB',@appendfile=0";
                Helper.Execute(sql, new object[0]);
                UpdateConfig("LastBackUpDate", DateTime.Now.ToString("yyyy-MM-dd"));
            }
            DirectoryInfo info = new DirectoryInfo(backupPath);
            foreach (FileInfo info2 in (from o in info.GetFiles()
                where ((TimeSpan) (DateTime.Now - o.CreationTime)).TotalDays > 30.0
                select o).ToList<FileInfo>())
            {
                info2.Delete();
            }
        }

        public static void Base64ToImg(string Base64String, string Filename)
        {
            FileStream stream = new FileStream(HttpContext.Current.Server.MapPath(Filename), FileMode.Create, FileAccess.Write);
            byte[] buffer = Convert.FromBase64String(Base64String);
            stream.Write(buffer, 0, buffer.Length);
            stream.Close();
        }

        public static string GenCode()
        {
            string str = getStr(false, 4);
            bool flag = false;
            lock (lockobj)
            {
                flag = Helper.Exists<DbQRInfo>("where code='" + str + "'", new object[0]);
                if (!flag)
                {
                    DbQRInfo poco = new DbQRInfo {
                        Code = str
                    };
                    Helper.Insert<DbQRInfo>(poco);
                }
            }
            if (flag)
            {
                return GenCode();
            }
            return str;
        }

        public static string GetBackupPath()
        {
            string path = HttpRuntime.AppDomainAppPath.ToString() + @"\Data\Backup\";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        public static string GetConfigValue(string key)
        {
            DbConfig config = Helper.CreateWhere<DbConfig>().Where(o => o.ConfigKey == key).FirstOrDefault(null);
            if (config != null)
            {
                return config.ConfigValue;
            }
            return "";
        }

        public static DbUser GetCurrentUser()
        {
            return (HttpContext.Current.Session["user"] as DbUser);
        }

        public static string getStr(bool b, int n)
        {
            string str = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            if (b)
            {
                str = str + "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
            }
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < n; i++)
            {
                builder.Append(str.Substring(random.Next(0, str.Length), 1));
            }
            return builder.ToString();
        }

        public static MemoryStream ThumbImageToStream(string SourceFile, int intThumbWidth, int intThumbHeight)
        {
            MemoryStream stream2;
            MemoryStream stream = new MemoryStream();
            try
            {
                using (Image image = Image.FromFile(SourceFile))
                {
                    int num3;
                    int num4;
                    int width = image.Width;
                    int height = image.Height;
                    if ((width <= intThumbWidth) && (height <= intThumbHeight))
                    {
                        image.Save(stream, ImageFormat.Png);
                        return stream;
                    }
                    if ((width / height) <= (intThumbWidth / intThumbHeight))
                    {
                        num3 = (intThumbHeight * width) / height;
                        num4 = intThumbHeight;
                    }
                    else
                    {
                        num3 = intThumbWidth;
                        num4 = (intThumbWidth * height) / width;
                    }
                    image.GetThumbnailImage(num3, num4, null, IntPtr.Zero).Save(stream, ImageFormat.Png);
                    stream2 = stream;
                }
            }
            catch (Exception)
            {
                stream2 = stream;
            }
            return stream2;
        }

        public static void UpdateConfig(string key, string value)
        {
            DbConfig poco = Helper.CreateWhere<DbConfig>().Where(o => o.ConfigKey == key).FirstOrDefault(null);
            if (poco != null)
            {
                poco.ConfigValue = value;
                Helper.Save(poco);
            }
            else
            {
                poco = new DbConfig {
                    ConfigKey = key,
                    ConfigValue = value
                };
                Helper.Insert<DbConfig>(poco);
            }
        }

        public static SqlHelper Helper
        {
            get
            {
                if (_helper == null)
                {
                    _helper = SqlHelperFactory.OpenFormConnStr("conn", SqlType.None);
                }
                return _helper;
            }
        }
    }
}

