using System.Drawing;
using System;
using System.Text;
using System.Drawing.Imaging;
using Ocean.Core.Plugins.Bak;
using Ocean.Core.Plugins.FTP;
using System.IO;
using Ocean.Core.Configuration;
using Ocean.Core.Plugins.DFS;
using System.Drawing.Drawing2D;
using System.Web;
using Ocean.Core.Plugins.Upload;
using Ocean.Core.Utility;

namespace Ocean.Core.Common.ImageMaker
{
    public class Thumbnail
    {
        /// <summary>
        /// 压缩图片（按原图比例）
        /// </summary>
        /// <param name="sourceImage">原图</param>
        /// <param name="max_Width">压缩后的最大宽度</param>
        /// <param name="max_Height">压缩后的最大高度</param>
        /// <returns></returns>
        public static Bitmap Thumbnail_Image(Bitmap sourceImage, int max_Width, int max_Height)
        {
            float WH_Scale = (float)sourceImage.Width / (float)sourceImage.Height;
            int height = (int)((float)max_Width / WH_Scale);
            int width = (int)((float)max_Height * WH_Scale);
            //判断用那个最大值超比例生成的宽高不会超过两个设定的最大值
            if (height < max_Height)
            {
                width = max_Width;
            }
            else
            {
                height = max_Height;
            }
            if (sourceImage.Width < width)//判断压缩后的图片是否会超过原图，超过就用原图
            {
                //width = sourceImage.Width;
                //height = sourceImage.Height;
                return sourceImage;
            }
            //创建画布 绘制区域
            Bitmap drawImage = new Bitmap(width, height);
            Graphics draw_g = Graphics.FromImage(drawImage);
            //压缩图片
            draw_g.DrawImage(sourceImage, 0, 0, width, height);
            //释放资源
            draw_g.Dispose();

            return drawImage;
        }
        /// <summary>
        /// 缩放图片（按指定的宽高）
        /// </summary>
        /// <param name="sourceImage">原图</param>
        /// <param name="NewSize">新的大小</param>
        /// <returns></returns>
        public static Bitmap Thumbnail_Image(Bitmap sourceImage, Size NewSize)
        {
            //创建画布 绘制区域
            Bitmap drawImage = new Bitmap(NewSize.Width, NewSize.Height);
            Graphics draw_g = Graphics.FromImage(drawImage);
            //压缩图片
            draw_g.DrawImage(sourceImage, 0, 0, drawImage.Width, drawImage.Height);
            //释放资源
            draw_g.Dispose();

            return drawImage;
        }



        #region common
        private BaseConfigInfo config { get { return BaseConfigs.GetConfig(); } }
        private Image srcImage;
        private string srcFileName;

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="FileName">原始图片路径</param>
        public bool SetImage(string FileName)
        {
            srcFileName = FileHelper.GetMapPath(FileName);
            try
            {
                srcImage = Image.FromFile(srcFileName);
            }
            catch
            {
                return false;
            }
            return true;

        }

        /// <summary>
        /// 回调
        /// </summary>
        /// <returns></returns>
        public bool ThumbnailCallback()
        {
            return false;
        }

        /// <summary>
        /// 生成缩略图,返回缩略图的Image对象
        /// </summary>
        /// <param name="Width">缩略图宽度</param>
        /// <param name="Height">缩略图高度</param>
        /// <returns>缩略图的Image对象</returns>
        public Image GetImage(int Width, int Height)
        {
            Image img;
            Image.GetThumbnailImageAbort callb = new Image.GetThumbnailImageAbort(ThumbnailCallback);
            img = srcImage.GetThumbnailImage(Width, Height, callb, IntPtr.Zero);
            return img;
        }

        /// <summary>
        /// 保存缩略图
        /// </summary>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        public void SaveThumbnailImage(int Width, int Height, UpFileEntity upFileEntity)
        {
            switch (Path.GetExtension(srcFileName).ToLower())
            {
                case ".png":
                    SaveImage(Width, Height, ImageFormat.Png, upFileEntity);
                    break;
                case ".gif":
                    SaveImage(Width, Height, ImageFormat.Gif, upFileEntity);
                    break;
                default:
                    SaveImage(Width, Height, ImageFormat.Jpeg, upFileEntity);
                    break;
            }
        }

        /// <summary>
        /// 生成缩略图并保存
        /// </summary>
        /// <param name="Width">缩略图的宽度</param>
        /// <param name="Height">缩略图的高度</param>
        /// <param name="imgformat">保存的图像格式</param>
        /// <returns>缩略图的Image对象</returns>
        public void SaveImage(int Width, int Height, ImageFormat imgformat, UpFileEntity upFileEntity)
        {
            if (imgformat != ImageFormat.Gif && (srcImage.Width > Width) || (srcImage.Height > Height))
            {
                Image img;
                Image.GetThumbnailImageAbort callb = new Image.GetThumbnailImageAbort(ThumbnailCallback);
                img = srcImage.GetThumbnailImage(Width, Height, callb, IntPtr.Zero);
                srcImage.Dispose();

                #region 加入FTP｜分布式-vebin.h:2012.12.12
                SaveThumb(img, imgformat, upFileEntity);
                #endregion
                //img.Save(srcFileName, imgformat);
                //img.Dispose;
            }
        }

        #region Helper

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="image">Image 对象</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="ici">指定格式的编解码参数</param>
        private static void SaveImage(Image image, string savePath, ImageCodecInfo ici, UpFileEntity upfileEntity)
        {
            //设置 原图片 对象的 EncoderParameters 对象
            EncoderParameters parameters = new EncoderParameters(1);
            parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, ((long)100));

            #region 加入FTP｜分布式-vebin.h:2012.12.12
            SaveThumb(image, savePath, ici, parameters, upfileEntity);
            #endregion
            //image.Save(savePath, ici, parameters);
            //parameters.Dispose();
        }

        /// <summary>
        /// 获取图像编码解码器的所有相关信息
        /// </summary>
        /// <param name="mimeType">包含编码解码器的多用途网际邮件扩充协议 (MIME) 类型的字符串</param>
        /// <returns>返回图像编码解码器的所有相关信息</returns>
        private static ImageCodecInfo GetCodecInfo(string mimeType)
        {
            ImageCodecInfo[] CodecInfo = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo ici in CodecInfo)
            {
                if (ici.MimeType == mimeType)
                    return ici;
            }
            return null;
        }

        /// <summary>
        /// 计算新尺寸
        /// </summary>
        /// <param name="width">原始宽度</param>
        /// <param name="height">原始高度</param>
        /// <param name="maxWidth">最大新宽度</param>
        /// <param name="maxHeight">最大新高度</param>
        /// <returns></returns>
        private static Size ResizeImage(int width, int height, int maxWidth, int maxHeight)
        {
            decimal MAX_WIDTH = (decimal)maxWidth;
            decimal MAX_HEIGHT = (decimal)maxHeight;
            decimal ASPECT_RATIO = MAX_WIDTH / MAX_HEIGHT;

            int newWidth, newHeight;
            decimal originalWidth = (decimal)width;
            decimal originalHeight = (decimal)height;

            if (originalWidth > MAX_WIDTH || originalHeight > MAX_HEIGHT)
            {
                decimal factor;
                // determine the largest factor 
                if (originalWidth / originalHeight > ASPECT_RATIO)
                {
                    factor = originalWidth / MAX_WIDTH;
                    newWidth = Convert.ToInt32(originalWidth / factor);
                    newHeight = Convert.ToInt32(originalHeight / factor);
                }
                else
                {
                    factor = originalHeight / MAX_HEIGHT;
                    newWidth = Convert.ToInt32(originalWidth / factor);
                    newHeight = Convert.ToInt32(originalHeight / factor);
                }
            }
            else
            {
                newWidth = width;
                newHeight = height;
            }
            return new Size(newWidth, newHeight);
        }

        /// <summary>
        /// 得到图片格式
        /// </summary>
        /// <param name="name">文件名称</param>
        /// <returns></returns>
        public static ImageFormat GetFormat(string name)
        {
            string ext = name.Substring(name.LastIndexOf(".") + 1);
            switch (ext.ToLower())
            {
                case "jpg":
                case "jpeg":
                    return ImageFormat.Jpeg;
                case "bmp":
                    return ImageFormat.Bmp;
                case "png":
                    return ImageFormat.Png;
                case "gif":
                    return ImageFormat.Gif;
                default:
                    return ImageFormat.Jpeg;
            }
        }
        #endregion

        /// <summary>
        /// 制作小正方形
        /// </summary>
        /// <param name="fileName">原图的文件路径</param>
        /// <param name="newFileName">新地址</param>
        /// <param name="newSize">长度或宽度</param>
        public static void MakeSquareImage(string fileName, string newFileName, int newSize, UpFileEntity upFileEntity)
        {
            Image image = Image.FromFile(fileName);

            int i = 0;
            int width = image.Width;
            int height = image.Height;
            if (width > height)
                i = height;
            else
                i = width;
            Bitmap b = new Bitmap(newSize, newSize);

            try
            {
                Graphics g = Graphics.FromImage(b);
                g.InterpolationMode = InterpolationMode.High;
                g.SmoothingMode = SmoothingMode.HighQuality;

                //清除整个绘图面并以透明背景色填充
                g.Clear(Color.Transparent);
                if (width < height)
                    g.DrawImage(image, new Rectangle(0, 0, newSize, newSize), new Rectangle(0, (height - width) / 2, width, width), GraphicsUnit.Pixel);
                else
                    g.DrawImage(image, new Rectangle(0, 0, newSize, newSize), new Rectangle((width - height) / 2, 0, height, height), GraphicsUnit.Pixel);

                #region 加入FTP｜分布式-vebin.h:2012.12.12
                SaveImage(b, newFileName, GetCodecInfo("image/" + GetFormat(fileName).ToString().ToLower()), upFileEntity);
                #endregion
            }
            finally
            {
                image.Dispose();
                //b.Dispose();
            }
        }

        /// <summary>
        /// 制作缩略图-filename
        /// </summary>
        /// <param name="fileName">原图路径</param>
        /// <param name="newFileName">新图路径</param>
        /// <param name="maxWidth">最大宽度</param>
        /// <param name="maxHeight">最大高度</param>
        /// <param name="attachId">附件</param>
        public static void MakeThumbnailImage(string fileName, string newFileName, int maxWidth, int maxHeight, bool HighQutity, UpFileEntity upFileEntity, bool isDispose = true)
        {
            Image original = Image.FromFile(fileName, true);
            Size _newSize = ResizeImage(original.Width, original.Height, maxWidth, maxHeight);
            System.Drawing.Image hb = new System.Drawing.Bitmap(_newSize.Width, _newSize.Height);//创建图片对象       
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(hb);//创建画板并加载空白图像 
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;//设置保真模式为高度保真 
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.Clear(Color.White);
            g.DrawImage(original, new Rectangle(0, 0, _newSize.Width, _newSize.Height), 0, 0, original.Width, original.Height, GraphicsUnit.Pixel);//开始画图    

            #region 加入FTP｜分布式-vebin.h:2012.12.12
            if (!string.IsNullOrEmpty(newFileName))
            {
                SaveThumb(hb, newFileName, GetFormat(newFileName), upFileEntity);
            }
            else
            {
                SaveThumb(hb, upFileEntity.Mast_File_Name.Replace(upFileEntity.File_Ext, upFileEntity.Prefix_Name + upFileEntity.File_Ext), GetFormat(upFileEntity.Mast_File_Name), upFileEntity);
            }
            #endregion
            //hb.Save(newFileName, GetFormat("jpg"));
            g.Dispose();
            //hb.Dispose();
            if (isDispose)
            {
                original.Dispose();
            }
        }
        public static void MakeThumbnailImage(string fileName, int maxWidth, int maxHeight, UpFileEntity upFileEntity, bool isDispose = true)
        {
            MakeThumbnailImage(fileName, null, maxWidth, maxHeight, upFileEntity, isDispose);
        }
        public static void MakeThumbnailImage(string fileName, string newFileName, int maxWidth, int maxHeight, UpFileEntity upFileEntity, bool isDispose = true)
        {
            MakeThumbnailImage(fileName, newFileName, maxWidth, maxHeight, true, upFileEntity, isDispose);
        }
        public static void MakeThumbnailImage(string fileName, int maxWidth, int maxHeight, bool HighQutity, UpFileEntity upFileEntity, bool isDispose = true)
        {
            MakeThumbnailImage(fileName, null, maxWidth, maxHeight, HighQutity, upFileEntity, isDispose);
        }

        /// <summary>
        /// 制作缩略图-postfile
        /// </summary>
        /// <param name="postFile"></param>
        /// <param name="newFileName"></param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        /// <param name="HighQutity"></param>
        /// <param name="upFileEntity"></param>
        public static void MakeThumbnailImage(HttpPostedFileBase postFile, string newFileName, int maxWidth, int maxHeight, bool HighQutity, UpFileEntity upFileEntity, bool isDispose = true)
        {
            Image img = Image.FromStream(postFile.InputStream);
            MakeThumbnailImage(img, newFileName, maxWidth, maxHeight, HighQutity, upFileEntity, isDispose);
            //img.Dispose();
        }
        public static void MakeThumbnailImage(HttpPostedFileBase postFile, int maxWidth, int maxHeight, UpFileEntity upFileEntity, bool isDispose = true)
        {
            MakeThumbnailImage(postFile, null, maxWidth, maxHeight, upFileEntity, isDispose);
        }
        public static void MakeThumbnailImage(HttpPostedFileBase postFile, string newFileName, int maxWidth, int maxHeight, UpFileEntity upFileEntity, bool isDispose = true)
        {
            MakeThumbnailImage(postFile, newFileName, maxWidth, maxHeight, true, upFileEntity, isDispose);
        }
        public static void MakeThumbnailImage(HttpPostedFileBase postFile, int maxWidth, int maxHeight, bool HighQutity, UpFileEntity upFileEntity, bool isDispose = true)
        {
            MakeThumbnailImage(postFile, null, maxWidth, maxHeight, HighQutity, upFileEntity, isDispose);
        }


        /// <summary>
        /// 制作缩略图-image
        /// </summary>
        /// <param name="img">原图</param>
        /// <param name="newFileName">新图路径</param>
        /// <param name="maxWidth">最大宽度</param>
        /// <param name="maxHeight">最大高度</param>
        public static void MakeThumbnailImage(Image img, string newFileName, int maxWidth, int maxHeight, bool HighQutity, UpFileEntity upFileEntity, bool isDispose = true)
        {
            Image original = img;
            Size _newSize = ResizeImage(original.Width, original.Height, maxWidth, maxHeight);
            System.Drawing.Image hb = new System.Drawing.Bitmap(_newSize.Width, _newSize.Height);//创建图片对象       
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(hb);//创建画板并加载空白图像 
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;//设置保真模式为高度保真 
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.Clear(Color.White);
            g.DrawImage(original, new Rectangle(0, 0, _newSize.Width, _newSize.Height), 0, 0, original.Width, original.Height, GraphicsUnit.Pixel);//开始画图    

            #region 加入FTP｜分布式-vebin.h:2012.12.12
            if (!string.IsNullOrEmpty(newFileName))
            {
                SaveThumb(hb, newFileName, GetFormat(newFileName), upFileEntity);
            }
            else
            {
                SaveThumb(hb, upFileEntity.Mast_File_Name.Replace(upFileEntity.File_Ext, upFileEntity.Prefix_Name + upFileEntity.File_Ext), GetFormat(upFileEntity.Mast_File_Name), upFileEntity);
            }
            #endregion
            //hb.Save(newFileName, GetFormat("jpg"));
            g.Dispose();
            //hb.Dispose();
            if (isDispose)
            {
                original.Dispose();
            }
        }
        public static void MakeThumbnailImage(Image img, int maxWidth, int maxHeight, UpFileEntity upFileEntity, bool isDispose = true)
        {
            MakeThumbnailImage(img, null, maxWidth, maxHeight, upFileEntity, isDispose);
        }
        public static void MakeThumbnailImage(Image img, string newFileName, int maxWidth, int maxHeight, UpFileEntity upFileEntity, bool isDispose = true)
        {
            MakeThumbnailImage(img, newFileName, maxWidth, maxHeight, true, upFileEntity, isDispose);
        }
        public static void MakeThumbnailImage(Image img, int maxWidth, int maxHeight, bool HighQutity, UpFileEntity upFileEntity, bool isDispose = true)
        {
            MakeThumbnailImage(img, null, maxWidth, maxHeight, HighQutity, upFileEntity, isDispose);
        }

        #region 加入FTP|分布式-vebin.h:2012.12.12
        private static void SaveThumb(Image image, string newFileName, ImageCodecInfo ici, EncoderParameters parameters, UpFileEntity upFileEntity)
        {
            BaseConfigInfo config = BaseConfigs.GetConfig();
            string fileName = Path.GetFileName(newFileName);
            if (config.DistributeFileSystem == 1 && upFileEntity.DistributeFileSystem == 1)
            {
                if (!string.IsNullOrEmpty(upFileEntity.GroupName))
                {
                    byte[] contentBytes = GetByteImage(image, ici, parameters);
                    DFSProvider.CurrentDFS.UploadSlaveFile(contentBytes, upFileEntity.Mast_File_Name, upFileEntity.Prefix_Name, upFileEntity.File_Ext.Replace(".", ""));
                }
            }
            else
            {
                if (config.FTPEnable == 1 && upFileEntity.FTPEnable == 1)
                {
                    if (FTPs.IsLocalhost(upFileEntity.FTPIdent))//区分本地或FTP
                    {
                        image.Save(newFileName, ici, parameters);
                    }
                    else
                    {
                        string fileExt = Path.GetExtension(newFileName);
                        string dir = Path.GetDirectoryName(config.FTPTempPath + GetFtpPath("", fileExt, config.FTPPathFormat, upFileEntity));
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        string savePath = dir + "/" + fileName;
                        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                        stopwatch.Start();

                        image.Save(savePath, ici, parameters);//存入临时文件夹
                        FTPs.UpLoadFile(Path.GetDirectoryName(newFileName), savePath, upFileEntity, upFileEntity.FTPIdent);//-FTP不需要传入文件名
                        stopwatch.Stop();
                        long minsecond = stopwatch.ElapsedMilliseconds;
                    }
                }
                else
                {
                    image.Save(newFileName, ici, parameters);
                }
            }
            image.Dispose();
            parameters.Dispose();
            #region 写入备份记录-通过接口
            if (upFileEntity.AttachID > 0)
            {
                fileName = fileName.ToLower().Replace(upFileEntity.File_Ext.ToLower(), "");
                string[] fileNameArr = fileName.Split('!');
                string[] sites;
                if (fileNameArr.Length > 1)
                {
                    sites = fileNameArr[1].Split('_');
                }
                else
                {
                    sites = fileName.Split('_');
                }
                if (sites.Length == 3)
                {
                    if (!string.IsNullOrEmpty(config.BakClassName))
                    {
                        UploadBakProvider.GetInstance(config.BakClassName).Update(upFileEntity.AttachID, sites[1] + "_" + sites[2]);
                    }
                }
            }
            #endregion
        }
        private static void SaveThumb(Image img, string newFileName, ImageFormat imgFormat, UpFileEntity upFileEntity)
        {
            BaseConfigInfo config = BaseConfigs.GetConfig();
            string fileName = Path.GetFileName(newFileName);
            if (config.DistributeFileSystem == 1 && upFileEntity.DistributeFileSystem == 1)
            {
                if (!string.IsNullOrEmpty(upFileEntity.GroupName))
                {
                    byte[] contentBytes = GetByteImage(img, imgFormat);
                    DFSProvider.CurrentDFS.UploadSlaveFile(contentBytes, upFileEntity.Mast_File_Name, upFileEntity.Prefix_Name, upFileEntity.File_Ext.Replace(".", ""));
                }
            }
            else
            {
                if (config.FTPEnable == 1 && upFileEntity.FTPEnable == 1)
                {
                    if (FTPs.IsLocalhost(upFileEntity.FTPIdent))//区分本地或FTP
                    {
                        img.Save(newFileName, imgFormat);
                    }
                    else
                    {
                        string fileExt = Path.GetExtension(newFileName);
                        string dir = Path.GetDirectoryName(config.FTPTempPath + GetFtpPath("", fileExt, config.FTPPathFormat, upFileEntity));
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        string savePath = dir + "/" + fileName;
                        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                        stopwatch.Start();
                        img.Save(savePath, imgFormat);//存入临时文件夹
                        FTPs.UpLoadFile(Path.GetDirectoryName(newFileName), savePath, upFileEntity, upFileEntity.FTPIdent);//-FTP不需要传入文件名
                        stopwatch.Stop();
                        long minsecond = stopwatch.ElapsedMilliseconds;
                    }
                }
                else
                {
                    img.Save(newFileName, imgFormat);
                }
            }
            img.Dispose();
            #region 写入备份记录-通过接口
            if (upFileEntity.AttachID > 0)
            {
                fileName = fileName.ToLower().Replace(upFileEntity.File_Ext.ToLower(), "");
                string[] fileNameArr = fileName.Split('!');
                string[] sites;
                if (fileNameArr.Length > 1)
                {
                    sites = fileNameArr[1].Split('_');
                }
                else
                {
                    sites = fileName.Split('_');
                }
                if (sites.Length == 3)
                {
                    if (!string.IsNullOrEmpty(config.BakClassName))
                    {
                        UploadBakProvider.GetInstance(config.BakClassName).Update(upFileEntity.AttachID, sites[1] + "_" + sites[2]);
                    }
                }
            }
            #endregion
        }
        private void SaveThumb(Image img, ImageFormat imgFormat, UpFileEntity upFileEntity)
        {
            SaveThumb(img, srcFileName, imgFormat, upFileEntity);
        }
        /// <summary>
        ///获取FTP存放方式
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileExtName"></param>
        /// <param name="pathFormat"></param>
        /// <returns></returns>
        private static string GetFtpPath(string path, string fileExtName, int pathFormat, UpFileEntity upfileEntity)
        {
            StringBuilder saveDir = new StringBuilder("");
            //附件保存方式 0=按年/月/日存入不同目录 1=按年/月/日/存入不同目录 2=按存入不同目录 3=按文件类型存入不同目录
            if (string.IsNullOrEmpty(upfileEntity.PathFormat))
            {
                switch (pathFormat)
                {
                    case 0:
                        {
                            if (!string.IsNullOrEmpty(path))
                            {
                                saveDir.Append(path);
                                saveDir.Append("/");
                                //saveDir.Append(Path.DirectorySeparatorChar);
                            }
                            break;
                        }
                    case 1:
                        {
                            saveDir.Append(DateTime.Now.ToString("yyyy"));
                            saveDir.Append("/");
                            saveDir.Append(DateTime.Now.ToString("MM"));
                            saveDir.Append("/");
                            saveDir.Append(DateTime.Now.ToString("dd"));
                            saveDir.Append("/");
                            if (!string.IsNullOrEmpty(path))
                            {
                                saveDir.Append(path);
                                saveDir.Append("/");
                                //saveDir.Append(Path.DirectorySeparatorChar);
                            }
                            break;
                        }
                    case 2:
                        {
                            saveDir.Append(path);
                            saveDir.Append("/");
                            //saveDir.Append(Path.DirectorySeparatorChar);
                            break;
                        }
                    case 3:
                        {
                            saveDir.Append(fileExtName);
                            saveDir.Append("/");
                            //saveDir.Append(Path.DirectorySeparatorChar);
                            break;
                        }
                    default:
                        {
                            saveDir.Append(DateTime.Now.ToString("yyyy"));
                            saveDir.Append("/");
                            saveDir.Append(DateTime.Now.ToString("MM"));
                            saveDir.Append("/");
                            saveDir.Append(DateTime.Now.ToString("dd"));
                            saveDir.Append("/");
                            break;
                        }
                }
            }
            else
            {
                saveDir.Append(upfileEntity.PathFormat);
            }
            return saveDir.ToString();
        }
        private static byte[] GetByteImage(Image img, ImageFormat imgFormat)
        {

            byte[] bt = null;
            if (!img.Equals(null))
            {
                using (MemoryStream mostream = new MemoryStream())
                {
                    Bitmap bmp = new Bitmap(img);
                    bmp.Save(mostream, imgFormat);//将图像以指定的格式存入缓存内存流
                    bt = new byte[mostream.Length];
                    mostream.Position = 0;//设置留的初始位置
                    mostream.Read(bt, 0, bt.Length);
                }
            }
            return bt;
        }
        private static byte[] GetByteImage(Image img, ImageCodecInfo ici, EncoderParameters parameters)
        {
            byte[] bt = null;
            if (!img.Equals(null))
            {
                using (MemoryStream mostream = new MemoryStream())
                {
                    Bitmap bmp = new Bitmap(img);
                    bmp.Save(mostream, ici, parameters);//将图像以指定的格式存入缓存内存流
                    bt = new byte[mostream.Length];
                    mostream.Position = 0;//设置留的初始位置
                    mostream.Read(bt, 0, bt.Length);
                }
            }
            return bt;
        }
        #endregion
        #endregion
    }
}
