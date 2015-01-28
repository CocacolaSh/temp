using System.Drawing;
using System.Drawing.Imaging;

namespace Ocean.Core.ImageMaker.ImageDraw
{
    /// <summary>
    /// 创建不透明背景图片
    /// </summary>
    public class CreateWhiteBackGround
    {
        /// <summary>
        /// 用指针操作 创建白色不透明背景图片（快）
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static Bitmap CreateBackGround_Pointer(Image image)
        {
            Color background = Color.White;
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
