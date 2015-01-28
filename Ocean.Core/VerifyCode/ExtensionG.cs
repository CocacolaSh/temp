using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Ocean.Core.VerifyCode
{
    internal static class ExtensionG
    {
        readonly static StringFormat measureSF;

        static ExtensionG()
        {
            measureSF = StringFormat.GenericTypographic;
            measureSF.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
        }

        /// <summary>
        /// 在制定位置画出制定样式的可调间距的文字
        /// </summary>
        /// <param name="g">Graphics</param>
        /// <param name="s">要画的文字</param>
        /// <param name="font">字体</param>
        /// <param name="brush">刷子</param>
        /// <param name="x">x坐标</param>
        /// <param name="y">y坐标</param>
        /// <param name="spacing">间距，可以是负值</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void DrawStringA(this Graphics g, string s, Font font, Brush brush, float x, float y, float spacing)
        {
            foreach (char c in s)
            {
                g.DrawString(c.ToString(), font, brush, x, y);
                x += g.MeasureStringA(c.ToString(), font).Width + spacing;
            }
        }

        public static SizeF MeasureStringA(this Graphics g, string text, Font font)
        {
            return g.MeasureString(text, font, 0, measureSF);
        }
    }
}