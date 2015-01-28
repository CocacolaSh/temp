using System.Drawing;

namespace Ocean.Core.ImageMaker.FileNameConfig
{
    public class LayerInfo
    {
        #region 构造函数

        /// <summary>
        /// 新建配置
        /// </summary>
        public LayerInfo() { }
        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="ConfigString"></param>
        /// <param name="SrcImage"></param>
        public LayerInfo(string ConfigString, Image SrcImage, int y, bool noSeg)
        {
            ReadConfig(ConfigString, y);
            SegmentationLayerImage(SrcImage, noSeg);
        }
        #endregion
        #region 变量
        /// <summary>
        /// 截取出来的图层图像
        /// </summary>
        private Bitmap layerImage;
        /// <summary>
        /// 图片的大小
        /// </summary>
        private Size image_Size;
        #endregion
        #region 属性
        /// <summary>
        /// 是否改变图层的颜色 
        /// </summary>
        public bool IsChangeColor { get; set; }
        /// <summary>
        /// 是否添加背景色使其不透明
        /// </summary>
        public bool IsAddBackGround { get; set; }
        /// <summary>
        /// 截取出来的图层图像
        /// </summary>
        public Bitmap LayerImage
        {
            get { return layerImage; }
            set
            {
                layerImage = value;
                image_Size.Width = layerImage.Width;
                image_Size.Height = layerImage.Height;
            }
        }
        /// <summary>
        /// 截取起始坐标
        /// </summary>
        public Point Start_Point { get; set; }
        /// <summary>
        /// 图片的大小
        /// </summary>
        public Size Image_Size
        {
            get
            {
                return image_Size;
            }
        }

        #endregion
        #region 读取
        /// <summary>
        /// 从字符串读取配置
        /// </summary>
        /// <param name="configString"></param>
        public void ReadConfig(string configString, int y)
        {
            Start_Point = new Point(0, y);
            string[] configList = configString.Split('-');
            //Start_Point = new Point(int.Parse(configList[0]), int.Parse(configList[1]));
            image_Size = new Size(int.Parse(configList[0]), int.Parse(configList[1]));
            IsChangeColor = (configList[2] == "1");
            IsAddBackGround = (configList[3] == "1");
        }
        /// <summary>
        /// 截取出图层图像
        /// </summary>
        public void SegmentationLayerImage(Image sourceImage, bool noSeg)
        {
            if (noSeg)
            {
                LayerImage = new Bitmap(sourceImage);
            }
            else
            {
                LayerImage = ImageDraw.Segmentation.GetSegmentation(sourceImage, Start_Point, Image_Size.Width, Image_Size.Height);
            }
        }
        #endregion
        #region 写入
        /// <summary>
        /// 转换成配置字符串
        /// </summary>
        /// <returns></returns>
        public string WriteConfig()
        {
            return string.Format("{0}-{1}-{2}-{3}",
                //Start_Point.X,
                //Start_Point.Y,
                Image_Size.Width,
                Image_Size.Height,
                (IsChangeColor) ? 1 : 0,
                (IsAddBackGround) ? 1 : 0
                );
        }
        #endregion
    }
}
