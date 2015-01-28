using System.Drawing;

namespace Ocean.Core.ImageMaker.ImageDraw
{
    /// <summary>
    /// 图片加载类
    /// </summary>
    public class ImageRead
    {
        /// <summary>
        /// 加载图片Bitmap
        /// </summary>
        /// <param name="imgPath"></param>
        /// <returns></returns>
        public static Bitmap LoadImage_Bitmap(string imgPath)
        {
            try
            {
                Image img = Image.FromFile(imgPath);
                Bitmap returnBitmap = new Bitmap(img);
                //释放读取的图片
                img.Dispose();
                return new Bitmap(returnBitmap);
            }
            catch
            {
                return null;
            }
        }
    }
}
