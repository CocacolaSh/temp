using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Ocean.Core.VerifyCode
{
    public class VerifyCode
    {
        readonly static char[] characterTable = 
        {   
            '1','2','3','4','5','6','7','8','9',
            'A'    ,'C','D',    'F',
            'H',    'J','K','L'    ,'N',
                    'P',    'R','S','T',
            'U','V',        'X','Y','Z',
        };
        readonly static Random rnd = new Random();
        readonly static double[,] waveMatrix;
        readonly static Size waveMatrixSize;
        readonly static Color[] colors = new Color[] { Color.Red, Color.Green, Color.SlateGray, Color.DarkKhaki };
        readonly static Bitmap oneMap = new Bitmap(1, 1);
        readonly static Graphics oneG = Graphics.FromImage(oneMap);
        readonly static Color emptyColor = Color.FromArgb(0, 0, 0, 0);

        static VerifyCode()
        {
            waveMatrixSize = new Size(500, 500);
            waveMatrix = new double[waveMatrixSize.Width, waveMatrixSize.Height];

            for(int i = 0;i < waveMatrixSize.Width;i++)
            {
                for(int j = 0;j < waveMatrixSize.Height;j++)
                {
                    waveMatrix[i, j] = (Math.Sin((i + j) * Math.PI / 70.0) + 1.0) / 2.0;
                }
            }
        }

        public static string NextCode(int length)
        {
            string checkCode = string.Empty;
            for(int i = 0;i < length;i++)
            {
                checkCode += characterTable[rnd.Next(characterTable.Length)];
            }
            return checkCode;
        }

        public static Bitmap NextImage(int length)
        {
            return NextImage(NextCode(length));
        }

        public static Bitmap NextImage(int length, Size size)
        {
            return NextImage(NextCode(length), size);
        }

        public static Bitmap NextImage(string code)
        {
            return NextImage(code, colors[rnd.Next(colors.Length)], null, 40, 15);
        }

        public static byte[] GetImage(string code)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            Bitmap image = NextImage(code);
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] bImage = ms.GetBuffer();
            ms.Close();
            ms = null;
            image.Dispose();
            image = null;
            return bImage;
        }

        public static Bitmap NextImage(string code, Size size)
        {
            return new Bitmap(NextImage(code), size);
        }

        public static Bitmap NextImage(string code, Color color, Image background, int emSize, int range)
        {
            if(emSize < 15)
                throw new ArgumentOutOfRangeException("emSize", "emSize<15");
            if(range < 1)
                throw new ArgumentOutOfRangeException("range", "range<1");

            Font font = new Font("Calibri", emSize, (FontStyle.Bold | FontStyle.Italic));
            Pen pen = new Pen(color);
            Size fontSize = oneG.MeasureStringA(code, font).ToSize();
            float spacing = (float)-Math.Sqrt(1.4 * emSize);

            fontSize.Width += range + (int)(spacing * (code.Length - 3));
            fontSize.Height += range;

            Bitmap bmp = new Bitmap(fontSize.Width, fontSize.Height);

            using(Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawStringA(code, font, pen.Brush, 0f, 0f, spacing);
                pen.Width = 2;
                g.DrawCurve(pen, new Point[] {
                    new Point(0, rnd.Next(fontSize.Height)),
                    new Point(fontSize.Width >> 1, rnd.Next(fontSize.Height >> 2, fontSize.Height >> 1)),
                    new Point(fontSize.Width , rnd.Next(fontSize.Height)) });
            }

            bmp = WaveBitmap(bmp, range);
            if(background != null)
                bmp = DrawBackground(bmp, background);
            return bmp;
        }

        private static Bitmap WaveBitmap(Bitmap org, int range)
        {
            Bitmap bmp = new Bitmap(org.Width, org.Height);

            int xi = rnd.Next(waveMatrixSize.Width - org.Width);
            int xj = rnd.Next(waveMatrixSize.Height - org.Height);
            int yi = rnd.Next(waveMatrixSize.Width - org.Width);
            int yj = rnd.Next(waveMatrixSize.Height - org.Height);

            for(int i = 0;i < org.Width;i++, xi++, yi++, xj -= org.Height, yj -= org.Height)
            {
                for(int j = 0;j < org.Height;j++, xj++, yj++)
                {
                    int nx = i - (int)(waveMatrix[xi, xj] * range);
                    int ny = j - (int)(waveMatrix[yi, yj] * range);
                    if(0 <= nx && 0 <= ny)
                    {
                        Color c = org.GetPixel(nx, ny);
                        if(c != emptyColor)
                            bmp.SetPixel(i, j, c);
                    }
                }
            }
            return bmp;
        }

        private static Bitmap DrawBackground(Bitmap org, Image background)
        {
            Bitmap bmp = new Bitmap(background, org.Size);
            using(Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawImage(org, 0, 0);
            }
            return bmp;
        }
    }
}