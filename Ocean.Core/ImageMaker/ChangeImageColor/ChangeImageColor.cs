using System.Drawing;

namespace Ocean.Core.ImageMaker
{
    public class ChangeImageColor
    {
        /// <summary>
        /// 创建改变颜色后的图片
        /// </summary>
        /// <param name="folderPath">图片的文件夹路径</param>
        /// <param name="fileName">图片标识名</param>
        /// <param name="changeColor">要改变的颜色</param>
        /// <returns></returns>
        public static Bitmap CreateChangeColorImage(string folderPath, string fileName, Color changeColor)
        {
            try
            {
                return MergeImageChangeColor(GetAllLayerInfo(folderPath, fileName), changeColor);
            }
            catch//只要发生异常就不返回图片
            {
                return null;
            }
        }
        /// <summary>
        /// 合并同时改变图片颜色
        /// </summary>
        /// <param name="allLayerinfo"></param>
        /// <param name="changeColor"></param>
        /// <returns></returns>
        private static Bitmap MergeImageChangeColor(FileNameConfig.AllLayerInfo allLayerinfo, Color changeColor)
        {
            if (allLayerinfo.LayerList.Count > 0)
            {
                //创建一个空画布
                Bitmap drawImage = new Bitmap(allLayerinfo.LayerList[0].LayerImage.Width, allLayerinfo.LayerList[0].LayerImage.Height);
                Graphics g = Graphics.FromImage(drawImage);
                foreach (FileNameConfig.LayerInfo layerInfo in allLayerinfo.LayerList)
                {
                    Bitmap drawLayerImage = layerInfo.LayerImage;
                    if (layerInfo.IsAddBackGround)//是否添加背景
                    {
                        g.DrawImage(ImageDraw.CreateWhiteBackGround.CreateBackGround_Pointer(drawLayerImage), new Point(0, 0));
                    }
                    if (layerInfo.IsChangeColor)//是否改变图片颜色
                    {
                        drawLayerImage = ImageDraw.ChangeImageColor.ChangeImageColor_Pointer(drawLayerImage, changeColor);
                    }
                    g.DrawImage(drawLayerImage, new Point(0, 0));
                }
                g.Dispose();
                return drawImage;
            }
            return null;
        }
        /// <summary>
        /// 获得指定目录中的指定 图片 路径
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static FileNameConfig.AllLayerInfo GetAllLayerInfo(string folderPath, string fileName)
        {
            string filePath = FileNameConfig.GetFormatFileNamePath.GetFilePath(folderPath, fileName);
            return new FileNameConfig.AllLayerInfo(filePath, ImageDraw.ImageRead.LoadImage_Bitmap(filePath));
        }
    }
}
