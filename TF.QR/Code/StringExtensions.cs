namespace System
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;

    public static class StringExtensions
    {
        public static string Fmt(this string s, params object[] args)
        {
            if (!s.IsNotSet())
            {
                return string.Format(s, args);
            }
            return null;
        }

        public static bool IsImageName(this string inputString)
        {
            if ((!inputString.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase) && !inputString.EndsWith(".gif", StringComparison.InvariantCultureIgnoreCase)) && (!inputString.EndsWith(".jpeg", StringComparison.InvariantCultureIgnoreCase) && !inputString.EndsWith(".jpg", StringComparison.InvariantCultureIgnoreCase)))
            {
                return inputString.EndsWith(".bmp", StringComparison.InvariantCultureIgnoreCase);
            }
            return true;
        }

        public static bool IsMatch(this string s, string pattern)
        {
            if (s == null)
            {
                return false;
            }
            return Regex.IsMatch(s, pattern);
        }

        public static bool IsNotSet(this string inputString)
        {
            return string.IsNullOrWhiteSpace(inputString);
        }

        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        public static bool IsSet(this string inputString)
        {
            return !string.IsNullOrWhiteSpace(inputString);
        }

        public static string Join<T>(this IEnumerable<T> list, string s)
        {
            return string.Join<T>(s, list);
        }

        public static string Join<T>(this string s, IEnumerable<T> args)
        {
            return string.Join<T>(s, args);
        }

        public static string Join(this string s, params object[] args)
        {
            return string.Join(s, args);
        }

        public static System.Text.RegularExpressions.Match Match(this string s, string pattern)
        {
            return Regex.Match(s, pattern);
        }

        public static MatchCollection Matches(this string s, string pattern)
        {
            return Regex.Matches(s, pattern);
        }

        public static string StringToHexBytes(this string inputString)
        {
            string str = string.Empty;
            if (inputString.IsNotSet())
            {
                return str;
            }
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            bytes = provider.ComputeHash(bytes);
            StringBuilder builder = new StringBuilder();
            foreach (byte num in bytes)
            {
                builder.Append(num.ToString("x2").ToLower());
            }
            return builder.ToString();
        }

        public static byte[] ToBytes(this string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        public static string ToJsString(this string str)
        {
            if (str.IsSet())
            {
                str = str.Replace(@"\", @"\\");
                str = str.Replace("'", @"\'");
                str = str.Replace("\r", @"\r");
                str = str.Replace("\n", @"\n");
                str = str.Replace("\"", "\\\"");
            }
            return str;
        }

        public static string ToMd5Hash(this string value)
        {
            if (value.IsNotSet())
            {
                return value;
            }
            using (MD5 md = new MD5CryptoServiceProvider())
            {
                byte[] bytes = Encoding.Default.GetBytes(value);
                return BitConverter.ToString(md.ComputeHash(bytes)).Replace("-", string.Empty);
            }
        }

        public static string TruncateMiddle(this string input, int limit)
        {
            if (input.IsNotSet())
            {
                return null;
            }
            string str = input;
            if ((str.Length <= limit) || (limit <= 0))
            {
                return str;
            }
            int length = (limit / 2) - ("...".Length / 2);
            int num2 = (limit - length) - ("...".Length / 2);
            if (((length + num2) + "...".Length) < limit)
            {
                num2++;
            }
            else if (((length + num2) + "...".Length) > limit)
            {
                num2--;
            }
            return (input.Substring(0, length) + "..." + input.Substring(input.Length - num2, num2));
        }
    }
}

