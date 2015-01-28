using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using Ocean.Core.Common;

namespace Ocean.Core.ImageMaker
{
    public class DrawFont
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
        /// 绘制距离
        /// </summary>
        const int distance_1 = 2;
        const int distance_2 = 4;
        const int distance_3 = 6;
        /// <summary>
        /// 背景的颜色
        /// </summary>
        static Color shadowColor = Color.FromArgb(150, 150, 150);
        /// <summary>
        /// 阴影效果样式
        /// </summary>
        private enum ShadowStyle
        {
            /// <summary>
            /// 无效果
            /// </summary>
            None,
            /// <summary>
            /// 左上阴影距离1
            /// </summary>
            LeftTop1,
            /// <summary>
            /// 左上阴影距离2
            /// </summary>
            LeftTop2,
            /// <summary>
            /// 左上阴影距离3
            /// </summary>
            LeftTop3,
            /// <summary>
            /// 左下阴影距离1
            /// </summary>
            LeftBottom1,
            /// <summary>
            /// 左下阴影距离2
            /// </summary>
            LeftBottom2,
            /// <summary>
            /// 左下阴影距离3
            /// </summary>
            LeftBottom3,
            /// <summary>
            /// 右上阴影距离1
            /// </summary>
            RightTop1,
            /// <summary>
            /// 右上阴影距离2
            /// </summary>
            RightTop2,
            /// <summary>
            /// 右上阴影距离3
            /// </summary>
            RightTop3,
            /// <summary>
            /// 右下阴影距离1
            /// </summary>
            RightBottom1,
            /// <summary>
            /// 右下阴影距离2
            /// </summary>
            RightBottom2,
            /// <summary>
            /// 右下阴影距离3
            /// </summary>
            RightBottom3,
            /// <summary>
            /// 左上立体阴影距离1
            /// </summary>
            LeftTop3D1,
            /// <summary>
            /// 左上立体阴影距离2
            /// </summary>
            LeftTop3D2,
            /// <summary>
            /// 左上立体阴影距离3
            /// </summary>
            LeftTop3D3,
            /// <summary>
            /// 左下立体阴影距离1
            /// </summary>
            LeftBottom3D1,
            /// <summary>
            /// 左下立体阴影距离2
            /// </summary>
            LeftBottom3D2,
            /// <summary>
            /// 左下立体阴影距离3
            /// </summary>
            LeftBottom3D3,
            /// <summary>
            /// 右上立体阴影距离1
            /// </summary>
            RightTop3D1,
            /// <summary>
            /// 右上立体阴影距离2
            /// </summary>
            RightTop3D2,
            /// <summary>
            /// 右上立体阴影距离3
            /// </summary>
            RightTop3D3,
            /// <summary>
            /// 右下立体阴影距离1
            /// </summary>
            RightBottom3D1,
            /// <summary>
            /// 右下立体阴影距离2
            /// </summary>
            RightBottom3D2,
            /// <summary>
            /// 右下立体阴影距离3
            /// </summary>
            RightBottom3D3
        }
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
        private static Bitmap DrawShadow(string drawtxt, Font drawFont, int w, int h, Point startPoint, int x, int y, int distance, bool is3D)
        {
            Bitmap bit = new Bitmap(w, h);
            Graphics g = Graphics.FromImage(bit);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            if (is3D)
            {
                for (int i = 1; i <= distance; i++)
                {
                    Point p = new Point(startPoint.X + i * x, startPoint.Y + i * y);
                    g.DrawString(drawtxt, drawFont, new SolidBrush(shadowColor), p);
                }
            }
            else
            {
                Point p = new Point(startPoint.X + distance * x, startPoint.Y + distance * y);
                g.DrawString(drawtxt, drawFont, new SolidBrush(shadowColor), p);
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
        private static Bitmap DrawShadowImage(string drawtxt, Font drawFont, int w, int h, Point startPoint, ShadowStyle shadowStyle)
        {
            int x = 0;
            int y = 0;
            int distance = 0;
            bool is3D = false;

            switch (shadowStyle)
            {
                case ShadowStyle.None:
                    return null;
                case ShadowStyle.LeftTop1:
                    x = -1;
                    y = -1;
                    distance = distance_1;
                    break;
                case ShadowStyle.LeftTop2:
                    x = -1;
                    y = -1;
                    distance = distance_2;
                    break;
                case ShadowStyle.LeftTop3:
                    x = -1;
                    y = -1;
                    distance = distance_3;
                    break;

                case ShadowStyle.LeftBottom1:
                    x = -1;
                    y = 1;
                    distance = distance_1;
                    break;
                case ShadowStyle.LeftBottom2:
                    x = -1;
                    y = 1;
                    distance = distance_2;
                    break;
                case ShadowStyle.LeftBottom3:
                    x = -1;
                    y = 1;
                    distance = distance_3;
                    break;

                case ShadowStyle.RightTop1:
                    x = 1;
                    y = -1;
                    distance = distance_1;
                    break;
                case ShadowStyle.RightTop2:
                    x = 1;
                    y = -1;
                    distance = distance_2;
                    break;
                case ShadowStyle.RightTop3:
                    x = 1;
                    y = -1;
                    distance = distance_3;
                    break;

                case ShadowStyle.RightBottom1:
                    x = 1;
                    y = 1;
                    distance = distance_1;
                    break;
                case ShadowStyle.RightBottom2:
                    x = 1;
                    y = 1;
                    distance = distance_2;
                    break;
                case ShadowStyle.RightBottom3:
                    x = 1;
                    y = 1;
                    distance = distance_3;
                    break;

                case ShadowStyle.LeftTop3D1:
                    x = -1;
                    y = -1;
                    distance = distance_1;
                    is3D = true;
                    break;
                case ShadowStyle.LeftTop3D2:
                    x = -1;
                    y = -1;
                    distance = distance_2;
                    is3D = true;
                    break;
                case ShadowStyle.LeftTop3D3:
                    x = -1;
                    y = -1;
                    distance = distance_3;
                    is3D = true;
                    break;

                case ShadowStyle.LeftBottom3D1:
                    x = -1;
                    y = 1;
                    distance = distance_1;
                    is3D = true;
                    break;
                case ShadowStyle.LeftBottom3D2:
                    x = -1;
                    y = 1;
                    distance = distance_2;
                    is3D = true;
                    break;
                case ShadowStyle.LeftBottom3D3:
                    x = -1;
                    y = 1;
                    distance = distance_3;
                    is3D = true;
                    break;

                case ShadowStyle.RightTop3D1:
                    x = 1;
                    y = -1;
                    distance = distance_1;
                    is3D = true;
                    break;
                case ShadowStyle.RightTop3D2:
                    x = 1;
                    y = -1;
                    distance = distance_2;
                    is3D = true;
                    break;
                case ShadowStyle.RightTop3D3:
                    x = 1;
                    y = -1;
                    distance = distance_3;
                    is3D = true;
                    break;

                case ShadowStyle.RightBottom3D1:
                    x = 1;
                    y = 1;
                    distance = distance_1;
                    is3D = true;
                    break;
                case ShadowStyle.RightBottom3D2:
                    x = 1;
                    y = 1;
                    distance = distance_2;
                    is3D = true;
                    break;
                case ShadowStyle.RightBottom3D3:
                    x = 1;
                    y = 1;
                    distance = distance_3;
                    is3D = true;
                    break;
                default:
                    break;
            }

            return DrawShadow(drawtxt, drawFont, w, h, startPoint, x, y, distance, is3D);
        }
        #endregion

        #region 绘制特效文字图片
        /// <summary>
        /// 绘制特效文字图片
        /// </summary>
        /// <param name="drawStr">绘制的字符串</param>
        /// <param name="configStr">参数字符串(01260_image1_shade2_rtd2)</param>
        /// <returns></returns>
        public static Bitmap DrawSpeciallyFont(string drawStr, string configStr)
        {
            try
            {
                string[] configs = configStr.Split('_');
                if (configs.Length != 4)
                {
                    return null;
                }
                //请求参数字符串格式  01260_image1_shade2_rtd2
                return DrawSpeciallyFont(drawStr, configs[0], configs[1], configs[2], GetShadowStyle(configs[3]));
            }
            catch
            {
                return null;
            }
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
        private static Bitmap DrawSpeciallyFont(string drawStr, string fontStyleSrt, string drawFontBrushSrt, string drawBackBrushStr, ShadowStyle shadowStyle)
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
            if (shadowStyle != ShadowStyle.None)
            {
                Bitmap shadowImage = DrawShadowImage(drawStr, drawFont, img_w, img_h, sPoint, shadowStyle);
                if (shadowImage != null)
                {
                    g.DrawImage(shadowImage, new Point(0, 0));
                }
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
            //color121212 , shade0 ,image0 
            string[] arge = new string[2];
            arge[0] = brushStr.Substring(0, 5);
            arge[1] = brushStr.Substring(5);
            string brushSrt = arge[1];
            switch (arge[0])
            {
                case "color":
                    return GetColorBrush(brushSrt);
                case "shade":
                    return GetShadeBrush(brushSrt, w_distance, h_distance);
                case "image":
                    return GetImageBrush(brushSrt);
                default:
                    return null;
            }
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
            try
            {
                int colorNum = int.Parse(colorStr);
                int[] rgb = new int[3];
                //超过颜色的临界值
                if (colorNum > 16777215)
                {
                    return null;
                }
                int temp;
                for (int index = 5; index >= 0; index--)
                {
                    temp = Convert.ToInt32(Math.Pow(2, index * 4));
                    rgb[index / 2] += (colorNum / temp) * Convert.ToInt32(Math.Pow(2, index % 2 * 4));
                    colorNum = colorNum % temp;
                }
                return new SolidBrush(Color.FromArgb(rgb[2], rgb[1], rgb[0]));
            }
            catch
            {
                return null;
            }
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
            Point s_point = new Point(0, 0);
            Point e_point = new Point(0, 0);
            Color s_color = Color.FromArgb(0, 0, 0);
            Color e_color = Color.FromArgb(255, 255, 255);

            ShadeStyle shadeStyle = (ShadeStyle)int.Parse(shadeStr);
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
            return new LinearGradientBrush(s_point, e_point, s_color, e_color);
        }
        /// <summary>
        /// 获取图案笔刷
        /// </summary>
        /// <param name="imgStr">图案值</param>
        /// <returns></returns>
        private static Brush GetImageBrush(string imgStr)
        {
            //加个判断imgStr的代码
            string imgpath = string.Format("/widgets/specialfont/ShadeImages/ShadeImage_{0}.jpg", imgStr);
            Image image = ImageDraw.ImageRead.LoadImage_Bitmap(WebHelper.MapPaths(imgpath));
            return new TextureBrush(image);
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

        #region 投影样式获取
        /// <summary>
        /// 通过字符串获取投影样式
        /// </summary>
        /// <param name="shadowStr"></param>
        /// <returns></returns>
        private static ShadowStyle GetShadowStyle(string shadowStr)
        {
            if (shadowStr.Length != 4)
            {
                return ShadowStyle.None;
            }
            shadowStr = shadowStr.ToLower();
            string styleStr = "";
            switch (shadowStr[0])
            {
                case 'l':
                    styleStr += "Left";
                    break;
                case 'r':
                    styleStr += "Right";
                    break;
                default:
                    return ShadowStyle.None;
            }
            switch (shadowStr[1])
            {
                case 't':
                    styleStr += "Top";
                    break;
                case 'b':
                    styleStr += "Bottom";
                    break;
                default:
                    return ShadowStyle.None;
            }
            switch (shadowStr[2])
            {
                case 'n':
                    break;
                case 'd':
                    styleStr += "3D";
                    break;
                default:
                    return ShadowStyle.None;
            }
            if (shadowStr[3] != '1' && shadowStr[3] != '2' && shadowStr[3] != '3')
            {
                return ShadowStyle.None;
            }
            styleStr += shadowStr[3];
            try
            {
                return (DrawFont.ShadowStyle)Enum.Parse(typeof(DrawFont.ShadowStyle), styleStr);
            }
            catch (Exception)
            {
                return ShadowStyle.None;
            }
        }
        #endregion
    }
}
