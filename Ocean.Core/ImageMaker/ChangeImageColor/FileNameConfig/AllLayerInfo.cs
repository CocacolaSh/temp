using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Ocean.Core.ImageMaker.FileNameConfig
{
    public class AllLayerInfo
    {
        #region 构造函数
        /// <summary>
        /// 新建配置
        /// </summary>
        public AllLayerInfo() { }
        /// <summary>
        /// 读取配置
        /// </summary>
        public AllLayerInfo(string fileName, Image srcImage)
        {
            ReadConfig(GetFileConfigString(fileName), srcImage);
            SourceImage = new Bitmap(srcImage);
        }
        #endregion
        #region 变量
        /// <summary>
        /// 图层列表
        /// </summary>
        private List<LayerInfo> layerList = new List<LayerInfo>();
        #endregion
        #region 属性
        /// <summary>
        /// 图层列表
        /// </summary>
        public List<LayerInfo> LayerList
        {
            get
            {
                return layerList;
            }
            set
            {
                layerList = value;
            }
        }
        /// <summary>
        /// 源图片
        /// </summary>
        public Bitmap SourceImage { get; set; }

        #endregion
        #region 读取
        /// <summary>
        /// 读取图层列表
        /// </summary>
        public void ReadConfig(string configString, Image srcImage)
        {
            if (configString.ToLower() == "noseg")//当图片不用分割时直接赋值
            {
                LayerList.Add(new LayerInfo() { LayerImage = new Bitmap(srcImage), IsChangeColor = true, IsAddBackGround = true });
                return;
            }

            string[] configStingList = configString.Split('.');
            if (configStingList.Length == 1)//当图层配置只有一个图层时
            {
                LayerList.Add(new LayerInfo(configStingList[0], srcImage, 0, true));
            }
            else
            {
                for (int i = 0; i < configStingList.Length; i++)
                {
                    if (i == 0)
                    {
                        LayerList.Add(new LayerInfo(configStingList[i], srcImage, 0, false));
                    }
                    else
                    {
                        LayerList.Add(new LayerInfo(configStingList[i], srcImage, LayerList[i - 1].Start_Point.Y + LayerList[i - 1].Image_Size.Height, false));
                    }
                }
            }
        }
        /// <summary>
        /// 获取文件名中的配置字符串
        /// </summary>
        /// <param name="imageFileName"></param>
        /// <returns></returns>
        public string GetFileConfigString(string imageFileName)
        {
            FileInfo fileinfo = new FileInfo(imageFileName);
            string configString = fileinfo.Name;
            configString = configString.Substring(0, configString.LastIndexOf('.'));
            return configString.Substring(0, configString.LastIndexOf('.'));
        }
        #endregion
        #region 写入

        /// <summary>
        /// 合并所有图层
        /// </summary>
        /// <returns></returns>
        public Bitmap CreateMergeLayerImage(string imageFormatName, string SaveFolder)
        {
            RemoveIsTransparentImage();

            if (LayerList.Count == 0)
            {
                return null;
            }

            Bitmap mergeImage = new Bitmap(1, 1);
            string configString = "";

            if (LayerList.Count == 1)
            {
                mergeImage = LayerList[0].LayerImage;
                configString += "noseg.";
                configString += imageFormatName;
                configString += ".png";
            }
            else
            {

                int h = 0;
                int w = 0;
                foreach (LayerInfo layer in LayerList)
                {
                    if (layer.LayerImage.Width > w)
                    {
                        w = layer.LayerImage.Width;
                    }
                    h += layer.LayerImage.Height;
                }
                mergeImage = new Bitmap(w, h);

                Graphics g = Graphics.FromImage(mergeImage);
                Point startPoint = new Point(0, 0);
                foreach (LayerInfo layer in LayerList)
                {
                    g.DrawImage(layer.LayerImage, startPoint);
                    layer.Start_Point = startPoint;
                    startPoint = new Point(startPoint.X, startPoint.Y + layer.LayerImage.Height);
                }

                g.Dispose();

                foreach (LayerInfo item in LayerList)
                {
                    configString += item.WriteConfig() + ".";
                }
                configString += imageFormatName;
                configString += ".png";
            }
            SaveImage(mergeImage, SaveFolder + "\\" + configString);

            return mergeImage;
        }

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="image"></param>
        /// <param name="savePath"></param>
        private void SaveImage(Bitmap image, string savePath)
        {
            Bitmap savaImage = new Bitmap(image);
            savaImage.Save(savePath, System.Drawing.Imaging.ImageFormat.Png);
        }
        /// <summary>
        /// 移除完全透明的图片
        /// </summary>
        private void RemoveIsTransparentImage()
        {
            for (int i = 0; i < LayerList.Count; i++)
            {
                if (ImageDraw.ImageIsTransparent.IsTransparent(LayerList[i].LayerImage))
                {
                    LayerList.RemoveAt(i);
                    i--;
                }
            }
        }
        #endregion
    }
}
