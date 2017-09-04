namespace TF.QR
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Runtime.InteropServices;

    public class QR
    {
        public static byte[] CreateValidateGraphic(out string Code, int CodeLength, int Width, int Height, int FontSize)
        {
            byte[] buffer;
            string str = string.Empty;
            Color[] colorArray = new Color[] { Color.Black, Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Brown, Color.Brown, Color.DarkBlue };
            string[] strArray = new string[] { "Times New Roman", "MS Mincho", "Book Antiqua", "Gungsuh", "PMingLiU", "Impact" };
            char[] chArray = new char[] { 
                '2', '3', '4', '5', '6', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J',
                'K', 'L', 'M', 'N', 'P', 'R', 'S', 'T', 'W', 'X', 'Y'
            };
            Random random = new Random();
            Bitmap image = null;
            Graphics graphics = null;
            int num = 0;
            Point point = new Point();
            Point point2 = new Point();
            string familyName = null;
            Font font = null;
            Color color = new Color();
            for (num = 0; num <= (CodeLength - 1); num++)
            {
                str = str + chArray[random.Next(chArray.Length)];
            }
            image = new Bitmap(Width, Height);
            graphics = Graphics.FromImage(image);
            graphics.Clear(Color.White);
            try
            {
                for (num = 0; num <= 4; num++)
                {
                    point.X = random.Next(Width);
                    point.Y = random.Next(Height);
                    point2.X = random.Next(Width);
                    point2.Y = random.Next(Height);
                    color = colorArray[random.Next(colorArray.Length)];
                    graphics.DrawLine(new Pen(color), point, point2);
                }
                float num2 = 0f;
                float x = 0f;
                float y = 0f;
                if (CodeLength != 0)
                {
                    num2 = ((Width - (FontSize * CodeLength)) - 10) / CodeLength;
                }
                for (num = 0; num <= (str.Length - 1); num++)
                {
                    familyName = strArray[random.Next(strArray.Length)];
                    font = new Font(familyName, (float) FontSize, FontStyle.Italic);
                    color = colorArray[random.Next(colorArray.Length)];
                    y = ((Height - font.Height) / 2) + 2;
                    x = (Convert.ToSingle(num) * FontSize) + ((num + 1) * num2);
                    graphics.DrawString(str[num].ToString(), font, new SolidBrush(color), x, y);
                }
                for (int i = 0; i <= 30; i++)
                {
                    int num6 = random.Next(image.Width);
                    int num7 = random.Next(image.Height);
                    Color color2 = colorArray[random.Next(colorArray.Length)];
                    image.SetPixel(num6, num7, color2);
                }
                Code = str;
                MemoryStream stream = new MemoryStream();
                image.Save(stream, ImageFormat.Jpeg);
                buffer = stream.ToArray();
            }
            finally
            {
                graphics.Dispose();
            }
            return buffer;
        }
    }
}

