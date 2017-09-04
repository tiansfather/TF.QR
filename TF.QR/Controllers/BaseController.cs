namespace TF.QR
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Mvc;
    using ToolGood.ReadyGo;

    public class BaseController : Controller
    {
        public void ClearCookie(string name)
        {
            Response.Cookies.Remove(name);
        }

        protected ActionResult Error(string msg)
        {
            var data = new {
                errCode = -1,
                errMsg = msg
            };
            return base.Json(data);
        }

        protected ActionResult Error404()
        {
            return new HttpStatusCodeResult(0x194);
        }

        protected ActionResult Error404(string msg)
        {
            return new HttpStatusCodeResult(0x194, msg);
        }

        [NonAction]
        public ActionResult File(string fileName)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary[".323"] = "text/h323";
            dictionary[".acx"] = "application/internet-property-stream";
            dictionary[".ai"] = "application/postscript";
            dictionary[".aif"] = "audio/x-aiff";
            dictionary[".aifc"] = "audio/x-aiff";
            dictionary[".aiff"] = "audio/x-aiff";
            dictionary[".asf"] = "video/x-ms-asf";
            dictionary[".asr"] = "video/x-ms-asf";
            dictionary[".asx"] = "video/x-ms-asf";
            dictionary[".au"] = "audio/basic";
            dictionary[".avi"] = "video/x-msvideo";
            dictionary[".axs"] = "application/olescript";
            dictionary[".bas"] = "text/plain";
            dictionary[".bcpio"] = "application/x-bcpio";
            dictionary[".bin"] = "application/octet-stream";
            dictionary[".bmp"] = "image/bmp";
            dictionary[".c"] = "text/plain";
            dictionary[".cat"] = "application/vnd.ms-pkiseccat";
            dictionary[".cdf"] = "application/x-cdf";
            dictionary[".cer"] = "application/x-x509-ca-cert";
            dictionary[".class"] = "application/octet-stream";
            dictionary[".clp"] = "application/x-msclip";
            dictionary[".cmx"] = "image/x-cmx";
            dictionary[".cod"] = "image/cis-cod";
            dictionary[".cpio"] = "application/x-cpio";
            dictionary[".crd"] = "application/x-mscardfile";
            dictionary[".crl"] = "application/pkix-crl";
            dictionary[".crt"] = "application/x-x509-ca-cert";
            dictionary[".csh"] = "application/x-csh";
            dictionary[".css"] = "text/css";
            dictionary[".dcr"] = "application/x-director";
            dictionary[".der"] = "application/x-x509-ca-cert";
            dictionary[".dir"] = "application/x-director";
            dictionary[".dll"] = "application/x-msdownload";
            dictionary[".dms"] = "application/octet-stream";
            dictionary[".doc"] = "application/msword";
            dictionary[".dot"] = "application/msword";
            dictionary[".dvi"] = "application/x-dvi";
            dictionary[".dxr"] = "application/x-director";
            dictionary[".eps"] = "application/postscript";
            dictionary[".etx"] = "text/x-setext";
            dictionary[".evy"] = "application/envoy";
            dictionary[".exe"] = "application/octet-stream";
            dictionary[".fif"] = "application/fractals";
            dictionary[".flr"] = "x-world/x-vrml";
            dictionary[".gif"] = "image/gif";
            dictionary[".gtar"] = "application/x-gtar";
            dictionary[".gz"] = "application/x-gzip";
            dictionary[".h"] = "text/plain";
            dictionary[".hdf"] = "application/x-hdf";
            dictionary[".hlp"] = "application/winhlp";
            dictionary[".hqx"] = "application/mac-binhex40";
            dictionary[".hta"] = "application/hta";
            dictionary[".htc"] = "text/x-component";
            dictionary[".htm"] = "text/html";
            dictionary[".html"] = "text/html";
            dictionary[".htt"] = "text/webviewhtml";
            dictionary[".ico"] = "image/x-icon";
            dictionary[".ief"] = "image/ief";
            dictionary[".iii"] = "application/x-iphone";
            dictionary[".ins"] = "application/x-internet-signup";
            dictionary[".isp"] = "application/x-internet-signup";
            dictionary[".jfif"] = "image/pipeg";
            dictionary[".jpe"] = "image/jpeg";
            dictionary[".jpeg"] = "image/jpeg";
            dictionary[".jpg"] = "image/jpeg";
            dictionary[".js"] = "application/x-javascript";
            dictionary[".latex"] = "application/x-latex";
            dictionary[".lha"] = "application/octet-stream";
            dictionary[".lsf"] = "video/x-la-asf";
            dictionary[".lsx"] = "video/x-la-asf";
            dictionary[".lzh"] = "application/octet-stream";
            dictionary[".m13"] = "application/x-msmediaview";
            dictionary[".m14"] = "application/x-msmediaview";
            dictionary[".m3u"] = "audio/x-mpegurl";
            dictionary[".man"] = "application/x-troff-man";
            dictionary[".mdb"] = "application/x-msaccess";
            dictionary[".me"] = "application/x-troff-me";
            dictionary[".mht"] = "message/rfc822";
            dictionary[".mhtml"] = "message/rfc822";
            dictionary[".mid"] = "audio/mid";
            dictionary[".mny"] = "application/x-msmoney";
            dictionary[".mov"] = "video/quicktime";
            dictionary[".movie"] = "video/x-sgi-movie";
            dictionary[".mp2"] = "video/mpeg";
            dictionary[".mp3"] = "audio/mpeg";
            dictionary[".mpa"] = "video/mpeg";
            dictionary[".mpe"] = "video/mpeg";
            dictionary[".mpeg"] = "video/mpeg";
            dictionary[".mpg"] = "video/mpeg";
            dictionary[".mpp"] = "application/vnd.ms-project";
            dictionary[".mpv2"] = "video/mpeg";
            dictionary[".ms"] = "application/x-troff-ms";
            dictionary[".mvb"] = "application/x-msmediaview";
            dictionary[".nws"] = "message/rfc822";
            dictionary[".oda"] = "application/oda";
            dictionary[".p10"] = "application/pkcs10";
            dictionary[".p12"] = "application/x-pkcs12";
            dictionary[".p7b"] = "application/x-pkcs7-certificates";
            dictionary[".p7c"] = "application/x-pkcs7-mime";
            dictionary[".p7m"] = "application/x-pkcs7-mime";
            dictionary[".p7r"] = "application/x-pkcs7-certreqresp";
            dictionary[".p7s"] = "application/x-pkcs7-signature";
            dictionary[".pbm"] = "image/x-portable-bitmap";
            dictionary[".pdf"] = "application/pdf";
            dictionary[".pfx"] = "application/x-pkcs12";
            dictionary[".pgm"] = "image/x-portable-graymap";
            dictionary[".pko"] = "application/ynd.ms-pkipko";
            dictionary[".pma"] = "application/x-perfmon";
            dictionary[".pmc"] = "application/x-perfmon";
            dictionary[".pml"] = "application/x-perfmon";
            dictionary[".pmr"] = "application/x-perfmon";
            dictionary[".pmw"] = "application/x-perfmon";
            dictionary[".pnm"] = "image/x-portable-anymap";
            dictionary[".pot,"] = "application/vnd.ms-powerpoint";
            dictionary[".ppm"] = "image/x-portable-pixmap";
            dictionary[".pps"] = "application/vnd.ms-powerpoint";
            dictionary[".ppt"] = "application/vnd.ms-powerpoint";
            dictionary[".prf"] = "application/pics-rules";
            dictionary[".ps"] = "application/postscript";
            dictionary[".pub"] = "application/x-mspublisher";
            dictionary[".qt"] = "video/quicktime";
            dictionary[".ra"] = "audio/x-pn-realaudio";
            dictionary[".ram"] = "audio/x-pn-realaudio";
            dictionary[".ras"] = "image/x-cmu-raster";
            dictionary[".rgb"] = "image/x-rgb";
            dictionary[".rmi"] = "audio/mid";
            dictionary[".roff"] = "application/x-troff";
            dictionary[".rtf"] = "application/rtf";
            dictionary[".rtx"] = "text/richtext";
            dictionary[".scd"] = "application/x-msschedule";
            dictionary[".sct"] = "text/scriptlet";
            dictionary[".setpay"] = "application/set-payment-initiation";
            dictionary[".setreg"] = "application/set-registration-initiation";
            dictionary[".sh"] = "application/x-sh";
            dictionary[".shar"] = "application/x-shar";
            dictionary[".sit"] = "application/x-stuffit";
            dictionary[".snd"] = "audio/basic";
            dictionary[".spc"] = "application/x-pkcs7-certificates";
            dictionary[".spl"] = "application/futuresplash";
            dictionary[".src"] = "application/x-wais-source";
            dictionary[".sst"] = "application/vnd.ms-pkicertstore";
            dictionary[".stl"] = "application/vnd.ms-pkistl";
            dictionary[".stm"] = "text/html";
            dictionary[".svg"] = "image/svg+xml";
            dictionary[".sv4cpio"] = "application/x-sv4cpio";
            dictionary[".sv4crc"] = "application/x-sv4crc";
            dictionary[".swf"] = "application/x-shockwave-flash";
            dictionary[".t"] = "application/x-troff";
            dictionary[".tar"] = "application/x-tar";
            dictionary[".tcl"] = "application/x-tcl";
            dictionary[".tex"] = "application/x-tex";
            dictionary[".texi"] = "application/x-texinfo";
            dictionary[".texinfo"] = "application/x-texinfo";
            dictionary[".tgz"] = "application/x-compressed";
            dictionary[".tif"] = "image/tiff";
            dictionary[".tiff"] = "image/tiff";
            dictionary[".tr"] = "application/x-troff";
            dictionary[".trm"] = "application/x-msterminal";
            dictionary[".tsv"] = "text/tab-separated-values";
            dictionary[".txt"] = "text/plain";
            dictionary[".uls"] = "text/iuls";
            dictionary[".ustar"] = "application/x-ustar";
            dictionary[".vcf"] = "text/x-vcard";
            dictionary[".vrml"] = "x-world/x-vrml";
            dictionary[".wav"] = "audio/x-wav";
            dictionary[".wcm"] = "application/vnd.ms-works";
            dictionary[".wdb"] = "application/vnd.ms-works";
            dictionary[".wks"] = "application/vnd.ms-works";
            dictionary[".wmf"] = "application/x-msmetafile";
            dictionary[".wps"] = "application/vnd.ms-works";
            dictionary[".wri"] = "application/x-mswrite";
            dictionary[".wrl"] = "x-world/x-vrml";
            dictionary[".wrz"] = "x-world/x-vrml";
            dictionary[".xaf"] = "x-world/x-vrml";
            dictionary[".xbm"] = "image/x-xbitmap";
            dictionary[".xla"] = "application/vnd.ms-excel";
            dictionary[".xlc"] = "application/vnd.ms-excel";
            dictionary[".xlm"] = "application/vnd.ms-excel";
            dictionary[".xls"] = "application/vnd.ms-excel";
            dictionary[".xlt"] = "application/vnd.ms-excel";
            dictionary[".xlw"] = "application/vnd.ms-excel";
            dictionary[".xof"] = "x-world/x-vrml";
            dictionary[".xpm"] = "image/x-xpixmap";
            dictionary[".xwd"] = "image/x-xwindowdump";
            dictionary[".z"] = "application/x-compress";
            dictionary[".zip"] = "application/zip";
            dictionary[".mp4"] = "video/mp4";
            dictionary[".ogg"] = "video/ogg";
            dictionary[".webm"] = "video/webm";
            dictionary[".doc"] = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            dictionary[".xlsx"] = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            dictionary[".woff"] = "application/x-font-woff";
            dictionary[".svg"] = "image/svg+xml";
            string fileDownloadName = Path.GetFileName(fileName);
            string extension = Path.GetExtension(fileName);
            string str3 = "";
            if (dictionary.TryGetValue(extension, out str3))
            {
                return this.File(fileName, str3, fileDownloadName);
            }
            return this.File(fileName, "application/octet-stream", fileDownloadName);
        }

        protected string GetSession(string key)
        {
            if (base.Session[key] == null)
            {
                return string.Empty;
            }
            return base.Session[key].ToString();
        }

        protected ActionResult GridJson<T>(Page<T> objs)
        {
            var data = new {
                rows = objs.Items,
                results = objs.TotalItems
            };
            return base.Json(data);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            Exception exception = filterContext.Exception;
            if (exception is ArgumentException)
            {
                ArgumentException exception2 = exception as ArgumentException;
                var type = new {
                    errCode = -1,
                    errMsg = exception2.Message
                };
                JsonResult result = new JsonResult {
                    Data = type
                };
                filterContext.Result = result;
                filterContext.ExceptionHandled = true;
            }
            else
            {
                var type2 = new {
                    errCode = -1,
                    errMsg = exception.Message
                };
                JsonResult result2 = new JsonResult {
                    Data = type2
                };
                filterContext.Result = result2;
                filterContext.ExceptionHandled = true;
            }
        }

        protected ActionResult RedirectError(string info)
        {
            return this.Redirect("/Page/Error?info=" + info);
        }

        protected void SetCookie(string cookiename, string cookievalue)
        {
            this.SetCookie(cookiename, cookievalue, DateTime.Now.AddDays(30.0));
        }

        protected void SetCookie(string cookiename, string cookievalue, DateTime expires)
        {
            HttpCookie cookie = new HttpCookie(cookiename) {
                Value = cookievalue,
                Expires = expires
            };
            Response.Cookies.Add(cookie);
        }

        protected ActionResult Success()
        {
            var data = new {
                errCode = 0,
                errMsg = ""
            };
            return base.Json(data);
        }

        protected ActionResult Success(object obj)
        {
            var data = new {
                errCode = 0,
                errMsg = obj.ToString(),
                item = obj
            };
            return base.Json(data);
        }

        protected void VerifyDateTime(string name, DateTime num, DateTime min, DateTime max)
        {
            if (num < min)
            {
                throw new ArgumentException(name + "不能小于" + min.ToString("yyyy-MM-dd HH:mm"));
            }
            if (num > max)
            {
                throw new ArgumentException(name + "不能大于" + max.ToString("yyyy-MM-dd HH:mm"));
            }
        }

        protected void VerifyEmail(string name, string str)
        {
            Regex regex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            if (!regex.IsMatch(str))
            {
                throw new ArgumentException(name + "非Eamil格式");
            }
        }

        protected void VerifyInt(string name, string num)
        {
            Regex regex = new Regex(@"^-?(\d+)$");
            if (!regex.IsMatch(num))
            {
                throw new ArgumentException(name + "不能小于不是整数");
            }
        }

        protected void VerifyInt(string name, string num, int min, int max)
        {
            Regex regex = new Regex(@"^-?(\d+)$");
            if (!regex.IsMatch(num))
            {
                throw new ArgumentException(name + "不能小于不是整数");
            }
            int num2 = int.Parse(num);
            if (num2 < min)
            {
                throw new ArgumentException(name + "不能小于" + min);
            }
            if (num2 > max)
            {
                throw new ArgumentException(name + "不能大于" + max);
            }
        }

        protected void VerifyLength(string name, string str, int max)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentException(name + "不能为空");
            }
            if (str.Length > max)
            {
                throw new ArgumentException(name + "字符大于" + max);
            }
        }

        protected void VerifyLength(string name, string str, int min, int max)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentException(name + "不能为空");
            }
            if (str.Length < min)
            {
                throw new ArgumentException(name + "字符小于" + min);
            }
            if (str.Length > max)
            {
                throw new ArgumentException(name + "字符大于" + max);
            }
        }

        protected void VerifyNotNull<T>(string name, List<T> str)
        {
            if (str == null)
            {
                throw new ArgumentException(name + "不能为空");
            }
            if (str.Count == 0)
            {
                throw new ArgumentException(name + "不能为空");
            }
        }

        protected void VerifyNotNull(string name, string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentException(name + "不能为空");
            }
        }

        protected void VerifyNotNull<T>(string name, T[] str)
        {
            if (str == null)
            {
                throw new ArgumentException(name + "不能为空");
            }
            if (str.Length == 0)
            {
                throw new ArgumentException(name + "不能为空");
            }
        }

        protected void VerifyNumber(string name, string num)
        {
            Regex regex = new Regex(@"^-?(\d+|\d+\.\d+)$");
            if (!regex.IsMatch(num))
            {
                throw new ArgumentException(name + "不能小于不是数字");
            }
        }

        protected void VerifyNumber(string name, decimal num, decimal min, decimal max)
        {
            if (num < min)
            {
                throw new ArgumentException(name + "不能小于" + min);
            }
            if (num > max)
            {
                throw new ArgumentException(name + "不能大于" + max);
            }
        }

        protected void VerifyNumber(string name, double num, double min, double max)
        {
            if (num < min)
            {
                throw new ArgumentException(name + "不能小于" + min);
            }
            if (num > max)
            {
                throw new ArgumentException(name + "不能大于" + max);
            }
        }

        protected void VerifyNumber(string name, short num, short min, short max)
        {
            if (num < min)
            {
                throw new ArgumentException(name + "不能小于" + min);
            }
            if (num > max)
            {
                throw new ArgumentException(name + "不能大于" + max);
            }
        }

        protected void VerifyNumber(string name, int num, int min, int max)
        {
            if (num < min)
            {
                throw new ArgumentException(name + "不能小于" + min);
            }
            if (num > max)
            {
                throw new ArgumentException(name + "不能大于" + max);
            }
        }

        protected void VerifyNumber(string name, long num, long min, long max)
        {
            if (num < min)
            {
                throw new ArgumentException(name + "不能小于" + min);
            }
            if (num > max)
            {
                throw new ArgumentException(name + "不能大于" + max);
            }
        }

        protected void VerifyNumber(string name, float num, float min, float max)
        {
            if (num < min)
            {
                throw new ArgumentException(name + "不能小于" + min);
            }
            if (num > max)
            {
                throw new ArgumentException(name + "不能大于" + max);
            }
        }

        protected void VerifyNumber(string name, ushort num, ushort min, ushort max)
        {
            if (num < min)
            {
                throw new ArgumentException(name + "不能小于" + min);
            }
            if (num > max)
            {
                throw new ArgumentException(name + "不能大于" + max);
            }
        }

        protected void VerifyNumber(string name, uint num, uint min, uint max)
        {
            if (num < min)
            {
                throw new ArgumentException(name + "不能小于" + min);
            }
            if (num > max)
            {
                throw new ArgumentException(name + "不能大于" + max);
            }
        }

        protected void VerifyNumber(string name, ulong num, ulong min, ulong max)
        {
            if (num < min)
            {
                throw new ArgumentException(name + "不能小于" + min);
            }
            if (num > max)
            {
                throw new ArgumentException(name + "不能大于" + max);
            }
        }

        protected void VerifyUserName(string name, string str)
        {
            Regex regex = new Regex("^[a-zA-Z][a-zA-Z0-9_]{4,20}$");
            if (!regex.IsMatch(name))
            {
                throw new ArgumentException(name + "不符合要求");
            }
        }
    }
}

