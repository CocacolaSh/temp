using System.Drawing;

namespace Ocean.Core.ImageMaker.ImageDraw
{
    public class Segmentation
    {
        /// <summary>
        /// 切割单个
        /// </summary>
        /// <param name="segImg">源图片</param>
        /// <param name="drawPoint">起始坐标</param>
        /// <param name="draw_width">宽度</param>
        /// <param name="draw_height">高度</param>
        /// <returns></returns>
        public static Bitmap GetSegmentation(Image sourceImage, Point drawPoint, int draw_width, int draw_height)
        {
            if (draw_width == 0 || draw_height == 0)
            {
                return null;
            }
            Bitmap img = new Bitmap(draw_width, draw_height);
            Graphics g = Graphics.FromImage(img);
            g.DrawImage(sourceImage, new Rectangle(0, 0, draw_width, draw_height),
                new Rectangle(drawPoint.X, drawPoint.Y, draw_width, draw_height), GraphicsUnit.Pixel);
            g.Dispose();
            return img;
        }
    }
}
