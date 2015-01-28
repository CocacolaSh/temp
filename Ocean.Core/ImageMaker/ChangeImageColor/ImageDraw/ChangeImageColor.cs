using System.Drawing;
using System.Drawing.Imaging;

namespace Ocean.Core.ImageMaker.ImageDraw
{
    /// <summary>
    /// 改变图片颜色
    /// </summary>
    public class ChangeImageColor
    {
        /// <summary>
        /// 用指针进行颜色改变（快）
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="changeColor"></param>
        /// <returns></returns>
        public static Bitmap ChangeImageColor_Pointer(Bitmap bitmap, Color changeColor)
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
    }
}
