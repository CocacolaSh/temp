using System.Drawing;
using System.Drawing.Imaging;

namespace Ocean.Core.ImageMaker.ImageDraw
{
    public class ImageIsTransparent
    {
        /// <summary>
        /// 用指针操作 判断一个图片是否是完全透明的（快）
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static bool IsTransparent(Image image)
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
                        //这里判断透明度是否大于零（非全透明）
                        if (((int)color_p[3]) > 0)
                        {
                            return false;
                        }
                        color_p += BPP;
                    } // x
                    color_p += offset;
                } // y
            }
            bitMap.UnlockBits(bitmapData);

            return true;
        }

    }
}
