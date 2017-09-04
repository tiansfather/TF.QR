namespace TF.Common
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public static class Md5
    {
        public static string GetFileMd5(FileInfo fileinfo)
        {
            FileStream inputStream = new FileStream(fileinfo.FullName, FileMode.Open, FileAccess.Read);
            MD5 md = new MD5CryptoServiceProvider();
            byte[] buffer = md.ComputeHash(inputStream);
            md.Dispose();
            inputStream.Close();
            return BitConverter.ToString(buffer).Replace("-", "");
        }

        public static string GetFileMd5(string filePath)
        {
            FileStream inputStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            MD5 md = new MD5CryptoServiceProvider();
            byte[] buffer = md.ComputeHash(inputStream);
            md.Dispose();
            inputStream.Close();
            return BitConverter.ToString(buffer).Replace("-", "");
        }

        public static byte[] GetMd5Bytes(byte[] bs)
        {
            MD5 md = new MD5CryptoServiceProvider();
            byte[] buffer = md.ComputeHash(bs);
            md.Dispose();
            return buffer;
        }

        public static byte[] GetMd5Bytes(string str)
        {
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            byte[] buffer = provider.ComputeHash(Encoding.UTF8.GetBytes(str));
            provider.Dispose();
            return buffer;
        }

        public static string GetMd5String(byte[] bs)
        {
            MD5 md = new MD5CryptoServiceProvider();
            byte[] buffer = md.ComputeHash(bs);
            md.Dispose();
            return BitConverter.ToString(buffer).Replace("-", "");
        }

        public static string GetMd5String(string str)
        {
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            byte[] buffer = provider.ComputeHash(Encoding.UTF8.GetBytes(str));
            provider.Dispose();
            return BitConverter.ToString(buffer).Replace("-", "");
        }

        public static string GetQQEncrypt(string password, string verifyCode)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(password);
            MD5 md = MD5.Create();
            bytes = md.ComputeHash(bytes);
            bytes = md.ComputeHash(bytes);
            string s = BitConverter.ToString(md.ComputeHash(bytes)).Replace("-", "").ToUpper() + verifyCode;
            bytes = Encoding.ASCII.GetBytes(s);
            bytes = md.ComputeHash(bytes);
            md.Dispose();
            return BitConverter.ToString(bytes).Replace("-", "").ToUpper();
        }
    }
}

