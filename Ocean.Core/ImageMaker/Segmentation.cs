using System.Drawing;

namespace Ocean.Core.Common.ImageMaker
{
    public class Segmentation
    {
        /// <summary>
        /// 切割图片
        /// </summary>
        /// <param name="segImg">原图</param>
        /// <param name="drawPoint">起始坐标</param>
        /// <param name="draw_width">切割的宽度</param>
        /// <param name="draw_height">切割的高度</param>
        /// <returns></returns>
        public static Bitmap Segmentation_Image(Bitmap sourceImage, Point startPoint, Size segSize)
        {
            Bitmap drawImage = new Bitmap(segSize.Width, segSize.Height);
            Graphics draw_g = Graphics.FromImage(drawImage);
            draw_g.DrawImage(sourceImage, new Rectangle(0, 0, segSize.Width, segSize.Height),
                //下面这行是指定原图的截取起始点及大小的
                new Rectangle(startPoint.X, startPoint.Y, segSize.Width, segSize.Height), GraphicsUnit.Pixel);
            //释放资源
            draw_g.Dispose();
            return drawImage;
        }
    }
}
