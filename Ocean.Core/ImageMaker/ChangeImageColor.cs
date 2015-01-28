using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Ocean.Core.Common
{
    public class ChangeImageColor
    {
        /// <summary>
        /// 改变图片颜色 并且创建白色背景
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="changeColor"></param>
        /// <returns></returns>
        public static Bitmap ChangeImageColorAndCreateWhiteBackGroud(Image bitmap, Color changeColor)
        {
            Bitmap newImage = new Bitmap(CreateBackGround_Pointer(bitmap, Color.White));
            Graphics g = Graphics.FromImage(newImage);
            g.DrawImage(ChangeImageColor_Pointer(bitmap, changeColor), new Point(0, 0));

            g.Dispose();
            return newImage;
        }
        /// <summary>
        /// 用指针进行颜色改变（快）
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="changeColor"></param>
        /// <returns></returns>
        private static Bitmap ChangeImageColor_Pointer(Image bitmap, Color changeColor)
        {
            // 每像素字节数 BytesPerPixel
            const int BPP = 4;
            Bitmap bitMap = new Bitmap(bitmap);
            int width = bitMap.Width;
            int height = bitMap.Height;

            BitmapData bitmapData = bitMap.LockBits(new Rectangle(0, 0, width, height),
              ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            unsafe
            {
                byte* color_p = (byte*)bitmapData.Scan0;
                int offset = bitmapData.Stride - width * BPP;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        color_p[0] = (byte)changeColor.B;
                        color_p[1] = (byte)changeColor.G;
                        color_p[2] = (byte)changeColor.R;
                        color_p += BPP;
                    } // x
                    color_p += offset;
                } // y
            }
            bitMap.UnlockBits(bitmapData);

            return bitMap;
        }

        /// <summary>
        /// 用指针操作 创建不透明背景图片（快）
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static Bitmap CreateBackGround_Pointer(Image image, Color background)
        {
            // 每像素字节数 BytesPerPixel
            const int BPP = 4;
            Bitmap bitMap = new Bitmap(image);
            int width = bitMap.Width;
            int height = bitMap.Height;

            BitmapData bitmapData = bitMap.LockBits(new Rectangle(0, 0, width, height),
              ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            unsafe
            {
                byte* color_p = (byte*)bitmapData.Scan0;
                int offset = bitmapData.Stride - width * BPP;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        color_p[0] = (byte)background.B;
                        color_p[1] = (byte)background.G;
                        color_p[2] = (byte)background.R;
                        //这里判断透明度是否大于零（非全透明）
                        if (((int)color_p[3]) > 0)
                        {
                            color_p[3] = (byte)background.A;
                        }
                        color_p += BPP;
                    } // x
                    color_p += offset;
                } // y
            }
            bitMap.UnlockBits(bitmapData);

            return bitMap;
        }
    }
}