using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Ocean.Core.ImageMaker
{
    public class ChangeImageAlpha
    {
        /// <summary>
        /// 改变图像透明度（真透明）
        /// </summary>
        /// <param name="img">所要转变的图像</param>
        /// <param name="alpha">透明度，最大为1，最小为0</param>
        /// <returns>改变后的图像</returns>
        public static Bitmap VitrificationImage(Image img, float alpha)
        {
            if (img == null)
            {
                return null;
            }
            Bitmap _newImg = new Bitmap(img.Width, img.Height);
            using (Graphics _g = Graphics.FromImage(_newImg))
            {
                using (ImageAttributes _imageAttrs = new ImageAttributes())
                {
                    _imageAttrs.SetColorMatrix(new ColorMatrix(CreateAlphaMatrix(alpha)));
                    _g.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height),
                                1, 1, img.Width, img.Height, GraphicsUnit.Pixel, _imageAttrs);
                }
            }
            return _newImg;
        }
        /// <summary>
        /// 创建用于改变图像透明度的颜色矩阵
        /// </summary>
        /// <param name="alpha">所要设置的透明度</param>
        /// <returns>创建用于改变图像透明度的颜色矩阵</returns>
        private static float[][] CreateAlphaMatrix(float alpha)
        {
            if (alpha > 1)
            {
                alpha = 1;
            }
            if (alpha < 0)
            {
                alpha = 0;
            }
            float[][] _matrix =
            { 
                new float[] {1, 0, 0, 0, 0},
                new float[] {0, 1, 0, 0, 0},
                new float[] {0, 0, 1, 0, 0},
                new float[] {0, 0, 0, alpha, 0},
                new float[] {0, 0, 0, 0, 1}
            };
            return _matrix;
        }
    }
}
