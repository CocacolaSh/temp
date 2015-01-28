using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using Ocean.Core.Common;

namespace Ocean.Core.ImageMaker
{
    public class DrawFontImage
    {
        #region 一些值和枚举
        /// <summary>
        /// 字体列表
        /// </summary>
        static List<string> fontFamilyList = new List<string>() { 
            "微软雅黑",
            "方正舒体",
            "方正琥珀简体",
            "方正华隶简体",
            "方正黄草简体",
            "方正超粗黑简体",
            "汉仪醒示体简",
            "汉仪雪峰体简",
            "汉仪篆书繁",
            "华文彩云",
            "华文细黑", 
            "华文新魏",
            "华文行楷",
            "隶书",
            "宋体", 
            "楷体",
            "幼圆",
            "仿宋",
        };
        /// <summary>
        /// 渐变效果样式
        /// </summary>
        private enum ShadeStyle
        {
            /// <summary>
            /// 上到下
            /// </summary>
            Top = 0,
            /// <summary>
            /// 下到上
            /// </summary>
            Bottom = 1,
            /// <summary>
            /// 左到右
            /// </summary>
            Left = 2,
            /// <summary>
            /// 右到左
            /// </summary>
            Right = 3,
            /// <summary>
            /// 右上到左下
            /// </summary>
            LeftBottom = 4,
            /// <summary>
            /// 左上到右下
            /// </summary>
            RightBottom = 5
        }
        #endregion

        #region 绘制投影
        /// <summary>
        /// 绘制阴影
        /// </summary>
        /// <param name="drawtxt">字符串</param>
        /// <param name="drawFont">字体</param>
        /// <param name="w">图片宽度</param>
        /// <param name="h">图片高度</param>
        /// <param name="startPoint">绘制起始点</param>
        /// <param name="x">偏移X</param>
        /// <param name="y">偏移Y</param>
        /// <param name="distance">偏移距离</param>
        /// <param name="is3D">阴影是否是立体的</param>
        /// <returns></returns>
        private static Bitmap DrawShadow(string drawtxt, Font drawFont, int w, int h, Point startPoint, int x, int y, int distance, Color backColor, bool is3D)
        {
            Bitmap bit = new Bitmap(w, h);
            Graphics g = Graphics.FromImage(bit);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            if (is3D)
            {
                for (int i = 1; i <= distance; i++)
                {
                    Point p = new Point(startPoint.X + i * x, startPoint.Y + i * y);
                    g.DrawString(drawtxt, drawFont, new SolidBrush(backColor), p);
                }
            }
            else
            {
                Point p = new Point(startPoint.X + distance * x, startPoint.Y + distance * y);
                g.DrawString(drawtxt, drawFont, new SolidBrush(backColor), p);
            }
            g.Dispose();
            return bit;
        }
        /// <summary>
        /// 绘制阴影图案
        /// </summary>
        /// <param name="drawtxt">字符串</param>
        /// <param name="drawFont">字体</param>
        /// <param name="w">图片宽度</param>
        /// <param name="h">图片高度</param>
        /// <param name="startPoint">绘制起始点</param>
        /// <param name="x">偏移X</param>
        /// <param name="y">偏移Y</param>
        /// <param name="distance">偏移距离</param>
        /// <returns></returns>
        private static Bitmap DrawShadowImage(string drawtxt, Font drawFont, int w, int h, Point startPoint, string shadowStr)
        {
            int x = 0;
            int y = 0;
            bool is3D = false;
            Color backColor = new Color();
            int distance = 1;

            string[] arges = shadowStr.Split('_');
            if (arges.Length != 3)
            {
                return null;
            }
            if (arges[0].Length != 5)
            {
                return null;
            }

            distance = int.Parse(arges[0][3].ToString() + arges[0][4].ToString());
            backColor = GetColor(arges[1], arges[2]);

            switch (arges[0][0])
            {
                case 'l':
                    x = -1;
                    break;
                case 'r':
                    x = 1;
                    break;
                default:
                    break;
            }
            switch (arges[0][1])
            {
                case 't':
                    y = -1;
                    break;
                case 'b':
                    y = 1;
                    break;
                default:
                    break;
            }
            switch (arges[0][2])
            {
                case 'd':
                    is3D = true;
                    break;
                default:
                    break;
            }


            return DrawShadow(drawtxt, drawFont, w, h, startPoint, x, y, distance, backColor, is3D);
        }
        #endregion

        #region 字体获取
        /// <summary>
        /// 获取字体
        /// </summary>
        /// <param name="fontStr"></param>
        /// <returns></returns>
        private static Font GetFont(string fontStr)
        {
            if (fontStr.Length != 5)
            {
                return null;
            }
            int fontFamilyStyle = int.Parse(fontStr.Substring(0, 2));
            int fontSize = int.Parse(fontStr.Substring(2, 2));
            int fontStyle = int.Parse(fontStr.Substring(4, 1));
            string fontFamily = fontFamilyList[0];
            if (fontFamilyStyle < fontFamilyList.Count)
            {
                fontFamily = fontFamilyList[fontFamilyStyle];
            }
            return new Font(fontFamily, fontSize, (FontStyle)fontStyle, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        }
        #endregion

        #region 获取颜色
        /// <summary>
        /// 获取颜色
        /// </summary>
        /// <param name="colorStr">颜色值</param>
        /// <returns></returns>
        public static Color GetColor(string colorStr)
        {
            try
            {
                int colorNum = int.Parse(colorStr);
                int[] rgb = new int[3];
                //超过颜色的临界值
                if (colorNum > 16777215)
                {
                    return Color.Transparent;
                }
                int temp;
                for (int index = 5; index >= 0; index--)
                {
                    temp = Convert.ToInt32(Math.Pow(2, index * 4));
                    rgb[index / 2] += (colorNum / temp) * Convert.ToInt32(Math.Pow(2, index % 2 * 4));
                    colorNum = colorNum % temp;
                }
                return Color.FromArgb(rgb[2], rgb[1], rgb[0]);
            }
            catch
            {
                return Color.Transparent;
            }
        }
        /// <summary>
        /// 获取颜色
        /// </summary>
        /// <param name="colorStr">颜色值</param>
        /// <param name="alphaStr">透明度</param>
        /// <returns></returns>
        public static Color GetColor(string colorStr, string alphaStr)
        {
            float alpha = float.Parse(alphaStr);
            alpha = alpha * 0.01f * 255f;
            return Color.FromArgb((int)alpha, GetColor(colorStr));
        }
        #endregion

        #region 各种笔刷获取
        /// <summary>
        /// 获取颜色笔刷
        /// </summary>
        /// <param name="colorStr">颜色值</param>
        /// <returns></returns>
        private static Brush GetColorBrush(string colorStr)
        {
            string[] arges = colorStr.Split('_');
            if (arges.Length != 2)
            {
                return null;
            }
            return new SolidBrush(GetColor(arges[1]));
        }
        /// <summary>
        /// 获取渐变笔刷
        /// </summary>
        /// <param name="shadeStr">渐变类型</param>
        /// <param name="w_distance">宽度距离</param>
        /// <param name="h_distance">高度距离</param>
        /// <returns></returns>
        private static Brush GetShadeBrush(string shadeStr, int w_distance, int h_distance)
        {
            //shadeStr  =  shade_1_111111_100_222222_100
            Point s_point = new Point(0, 0);
            Point e_point = new Point(0, 0);
            Color s_color = Color.FromArgb(0, 0, 0);
            Color e_color = Color.FromArgb(255, 255, 255);

            string[] arges = shadeStr.Split('_');
            if (arges.Length != 6)
            {
                return null;
            }

            ShadeStyle shadeStyle = (ShadeStyle)int.Parse(arges[1]);
            switch (shadeStyle)
            {
                case ShadeStyle.Top:
                    e_point = new Point(0, h_distance);
                    break;
                case ShadeStyle.Bottom:
                    s_point = new Point(0, h_distance);
                    break;
                case ShadeStyle.Left:
                    e_point = new Point(w_distance, 0);
                    break;
                case ShadeStyle.Right:
                    s_point = new Point(w_distance, 0);
                    break;
                case ShadeStyle.LeftBottom:
                    e_point = new Point(w_distance, h_distance);
                    break;
                case ShadeStyle.RightBottom:
                    s_point = new Point(w_distance, h_distance);
                    break;
                default:
                    return null;
            }
            //将颜色值及透明度转成对象
            s_color = GetColor(arges[2], arges[3]);
            e_color = GetColor(arges[4], arges[5]);

            return new LinearGradientBrush(s_point, e_point, s_color, e_color);
        }
        /// <summary>
        /// 获取图案笔刷
        /// </summary>
        /// <param name="imgStr">图案值</param>
        /// <returns></returns>
        private static Brush GetImageBrush(string imgStr)
        {
            //image_01_100
            float alpha = 0;
            string[] arges = imgStr.Split('_');
            if (arges.Length != 3)
            {
                return null;
            }
            alpha = float.Parse(arges[2]) * 0.01f;
            string imgpath = string.Format("/widgets/specialfont/ShadeImages/ShadeImage_{0}.jpg", arges[1]);
            Image image = Ocean.Core.ImageMaker.ImageDraw.ImageRead.LoadImage_Bitmap(WebHelper.MapPaths(imgpath));
            if (alpha < 1)
            {
                image = ImageMaker.ChangeImageAlpha.VitrificationImage(image, alpha);
            }
            if (image == null)
            {
                return null;
            }
            return new TextureBrush(image);
        }
        #endregion

        #region 绘制特效文字图片
        /// <summary>
        /// 绘制特效文字图片
        /// </summary>
        /// <param name="drawStr">绘制的字符串</param>
        /// <param name="configStr">参数字符串(01660|s_3_511111_100_1005222_100|i_00_50|rtd08_0221212_10)</param>
        /// <returns></returns>
        public static Bitmap DrawSpeciallyFont(string drawStr, string configStr)
        {
            string[] configs = configStr.Split('|');
            if (configs.Length != 4)
            {
                return null;
            }
            //请求参数字符串格式  01260_image1_shade2_rtd2
            //01260|shade_1_111111_100_222222_100|color_121212|rtd01_121212
            return DrawSpeciallyFont(drawStr, configs[0], configs[1], configs[2], configs[3]);
        }
        /// <summary>
        /// 绘制特效文字图片
        /// </summary>
        /// <param name="drawStr">绘制的字符串</param>
        /// <param name="drawFont">绘制的字体</param>
        /// <param name="drawFontBrush">字体的笔刷</param>
        /// <param name="drawBackBrush">背景的笔刷</param>
        /// <param name="shadowImage">投影的类型</param>
        /// <returns></returns>
        private static Bitmap DrawSpeciallyFont(string drawStr, string fontStyleSrt, string drawFontBrushSrt, string drawBackBrushStr, string shadowStyle)
        {
            //绘制的字体
            Font drawFont = GetFont(fontStyleSrt);
            if (drawFont == null)
            {
                return null;
            }
            #region 获取绘制字符串的大小
            Bitmap temp_bit = new Bitmap(1, 1);
            Graphics temp_g = Graphics.FromImage(temp_bit);
            SizeF bitsize = temp_g.MeasureString(drawStr, drawFont);
            //释放资源
            temp_bit.Dispose();
            temp_g.Dispose();
            #endregion
            int img_w = (int)bitsize.Width;
            int img_h = (int)bitsize.Height;
            #region 对文字的位置进行调整
            int adjust_w = (int)(bitsize.Height * 0.00F);//R
            int adjust_h = (int)(bitsize.Height * 0.06F);
            if (drawFont.Italic)
            {
                adjust_w = (int)(bitsize.Height * 0.10F);//I
                adjust_h = (int)(bitsize.Height * 0.06F);
            }
            Point sPoint = new Point(-adjust_w, adjust_h);//绘制文字的起始点
            #endregion
            Bitmap fontimg = new Bitmap(img_w, img_h);
            Graphics g = Graphics.FromImage(fontimg);
            //字体笔刷
            Brush drawFontBrush = GetDrawBrush(drawFontBrushSrt, img_w, img_h);
            //背景笔刷
            Brush drawBackBrush = GetDrawBrush(drawBackBrushStr, img_w, img_h);
            //设置绘制文字的抗锯齿效果
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            #region 绘制背景
            if (drawBackBrush != null)
            {
                g.FillRectangle(drawBackBrush, 0, 0, img_w, img_h);
                drawBackBrush.Dispose();
            }
            #endregion
            #region 绘制阴影效果
            Bitmap shadowImage = DrawShadowImage(drawStr, drawFont, img_w, img_h, sPoint, shadowStyle);
            if (shadowImage != null)
            {
                g.DrawImage(shadowImage, new Point(0, 0));
            }
            #endregion
            //绘制字符串
            if (drawFontBrush != null)
            {
                g.DrawString(drawStr, drawFont, drawFontBrush, sPoint);
                drawFontBrush.Dispose();
            }
            //释放资源
            g.Dispose();

            return fontimg;
        }
        /// <summary>
        /// 获取绘制的笔刷
        /// </summary>
        /// <param name="brushStr">笔刷参数</param>
        /// <param name="w_distance">宽度距离</param>
        /// <param name="h_distance">高度距离</param>
        /// <returns></returns>
        private static Brush GetDrawBrush(string brushStr, int w_distance, int h_distance)
        {
            //c_121212 , s_1_111111_100_222222_100 ,i_01_100 
            switch (brushStr.Substring(0, 1))
            {
                case "c"://color
                    return GetColorBrush(brushStr);
                case "s"://shade
                    return GetShadeBrush(brushStr, w_distance, h_distance);
                case "i"://image
                    return GetImageBrush(brushStr);
                default:
                    return null;
            }
        }

        #endregion
    }
}
