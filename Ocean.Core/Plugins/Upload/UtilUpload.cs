using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core.Configuration;
using Ocean.Core.Common;
using System.IO;
using System.Drawing;
using System.Web;
using Ocean.Core.Plugins.FTP;
using System.Drawing.Imaging;
using Ocean.Core.Plugins.Security;
using Ocean.Core.Plugins.DFS;
using Ocean.Core.Plugins.Bak;
using System.Collections;
using System.Data;
using Ocean.Core.Utility;
using Ocean.Core.Common.ImageMaker;

namespace Ocean.Core.Plugins.Upload.Common
{
    public class UtilUpload : IUpload
    {
        private static BaseConfigInfo config;
        public static BaseConfigInfo Config
        {
            get
            {
                if (config == null)
                {
                    config = BaseConfigs.GetConfig();
                }
                return config;
            }
            set { config = value; }
        }
        private static IStringEncrypt encrypt;
        public static IStringEncrypt Encrypt
        {
            get
            {
                if (encrypt == null)
                {
                    encrypt = new SimpleStringEncrypt();
                }
                return encrypt;
            }
            set { encrypt = value; }
        }
        private Random randomer;

        /// <summary>
        /// 随机数
        /// </summary>
        protected Random Randomer
        {
            get
            {
                if (randomer == null)
                {
                    randomer = new Random(unchecked((int)DateTime.Now.Ticks));
                }
                return randomer;
            }
            set { randomer = value; }
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="uploadFile"></param>
        /// <param name="upfileEntity"></param>
        /// <returns></returns>
        public AttachmentInfo UploadFile(HttpPostedFileBase uploadFile, UpFileEntity upfileEntity)
        {
            AttachmentInfo attach = new AttachmentInfo();
            int nFileLen = uploadFile.ContentLength;
            string fileextname = Path.GetExtension(uploadFile.FileName).ToLower();
            upfileEntity.File_Ext = fileextname;
            string savePath = GetSavePath(nFileLen, uploadFile.FileName, upfileEntity, attach);
            
            if (StringHelper.InArray(Path.GetExtension(uploadFile.FileName).ToLower(), upfileEntity.AllowType))
            {
                if (uploadFile.ContentLength / 1024 > upfileEntity.Size)
                {
                    attach.Error = "<li>照片大小不能超过" + (upfileEntity.Size/1024.0) + "M</li>";
                    return attach;
                }
                else
                {
                    //上传图片文件jpg,gif
                    try
                    {
                        string extension = string.IsNullOrEmpty(savePath) ? fileextname : Path.GetExtension(savePath);// 
                        attach.FileType = extension;
                        attach.FileSize = nFileLen / 1024;
                        if (StringHelper.InArray(Path.GetExtension(uploadFile.FileName).ToLower(), ".jpg,.jpeg,.gif,.bmp,.png"))
                        {
                            Image img = Image.FromStream(uploadFile.InputStream);
                            if (upfileEntity.MaxHeight > 0 && img.Height > upfileEntity.MaxHeight)
                            {
                                attach.Error += "<li>图片高度不得超过" + upfileEntity.MaxHeight.ToString() + "</li>";
                                return attach;
                            }
                            else if (upfileEntity.Height > 0 && (img.Height > (upfileEntity.Height + 5) || img.Height < (upfileEntity.Height - 5)))
                            {
                                attach.Error += "<li>请上传图片高度为" + upfileEntity.Height.ToString() + "的图片</li>";
                                return attach;
                            }
                            if (upfileEntity.MaxWidth > 0 && img.Width > upfileEntity.MaxWidth)
                            {
                                attach.Error += "<li>图片宽度不得超过" + upfileEntity.MaxWidth.ToString() + "</li>";
                                return attach;
                            }
                            else if (upfileEntity.Width > 0 && (img.Width > (upfileEntity.Width + 5) || img.Width < (upfileEntity.Width - 5)))
                            {
                                attach.Error += "<li>请上传图片宽度为" + upfileEntity.Width.ToString() + "的图片</li>";
                                return attach;
                            }
                            string saveFullPath = string.IsNullOrEmpty(savePath)?"":savePath.Replace(extension, "!" + img.Width.ToString() + "x" + img.Height.ToString() + extension);
                            attach.Width = img.Width;
                            attach.Height = img.Height;
                            //if (Path.GetExtension(uploadFile.FileName).ToLower() == ".gif")
                            //{
                            //    SaveFile(uploadFile, savePath, upfileEntity, attach);
                            //}
                            //else
                            //{
                            //    SaveImage(img, saveFullPath, GetImgFormat(extension), upfileEntity, attach, false);
                            //}
                            if (upfileEntity.ThumbnailSizes == null || upfileEntity.ThumbnailSizes.Length == 0)
                            {
                                attach.Thumbnail_Sizes = "";
                            }
                            else {
                                attach.Thumbnail_Sizes = upfileEntity.ThumbnailSizes.Aggregate((i, j) => i + "," + j).Trim(',');
                            }
                            SaveImage(img, saveFullPath, GetImgFormat(extension), upfileEntity, attach, false);
                            //img.Save(sSavePath + sFilename.Replace(extension, "!" + img.Width.ToString() + "x" + img.Height.ToString() + extension));

                            if (upfileEntity.ThumbnailSizes != null && upfileEntity.ThumbnailSizes.Length > 0)
                            {
                                for (int i = 0; i < upfileEntity.ThumbnailSizes.Length; i++)
                                {
                                    upfileEntity.Prefix_Name = "_" + upfileEntity.ThumbnailSizes[i];
                                    MakeThumbnailImage(img, saveFullPath, upfileEntity.ThumbnailSizes[i], upfileEntity,(i == upfileEntity.ThumbnailSizes.Length - 1) ? true : false);
                                    //string[] thumbnailSize = upfileEntity.ThumbnailSizes[i].Split('_');
                                    //if (thumbnailSize.Length == 2)
                                    //{
                                    //    int thumbnailWidth = TypeConverter.StrToInt(thumbnailSize[0], 0);
                                    //    int thumbnailHeight = TypeConverter.StrToInt(thumbnailSize[1], 0);
                                    //    if (thumbnailWidth > 0 && thumbnailHeight > 0)
                                    //    {
                                    //        //缩略
                                    //        Thumbnail.MakeThumbnailImage(img, saveFullPath.Replace(extension, "!" + img.Width.ToString() + "x" + img.Height.ToString() + "_" + upfileEntity.ThumbnailSizes[i] + extension), thumbnailWidth, thumbnailHeight, true, upfileEntity);
                                    //    }
                                    //}
                                }
                                img.Dispose();
                            }
                        }
                        else
                        {
                            SaveFile(uploadFile, savePath, upfileEntity, attach);
                        }
                        if (upfileEntity.ftpclient != null && upfileEntity.IsClose)
                        {
                            upfileEntity.ftpclient.Disconnect();
                            upfileEntity.ftpclient = null;
                        }
                    }
                    catch (ArgumentException errArgument)
                    {
                        File.Delete(savePath);
                        attach.Error += errArgument.Message;
                        attach.FileName = "";
                    }
                }
            }
            else //上传除图片文件以外的全部文件
            {
                attach.Error = "<li>请上传指定的文件类型[.jpg,.jpeg,.gif,.bmp,.png]</li>";
                return attach;
            }
            return attach;
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="uploadFile"></param>
        /// <param name="upfileEntity"></param>
        /// <returns></returns>
        public AttachmentInfo UploadFile(HttpPostedFile uploadFile, UpFileEntity upfileEntity)
        {
            AttachmentInfo attach = new AttachmentInfo();
            int nFileLen = uploadFile.ContentLength;
            string fileextname = Path.GetExtension(uploadFile.FileName).ToLower();
            upfileEntity.File_Ext = fileextname;
            string savePath = GetSavePath(nFileLen, uploadFile.FileName, upfileEntity, attach);

            if (StringHelper.InArray(Path.GetExtension(uploadFile.FileName).ToLower(), upfileEntity.AllowType))
            {
                if (uploadFile.ContentLength / 1024 > upfileEntity.Size)
                {
                    attach.Error = "<li>照片大小不能超过" + (upfileEntity.Size / 1024.0) + "M</li>";
                    return attach;
                }
                else
                {
                    //上传图片文件jpg,gif
                    try
                    {
                        string extension = string.IsNullOrEmpty(savePath) ? fileextname : Path.GetExtension(savePath);// 
                        attach.FileType = extension;
                        attach.FileSize = nFileLen / 1024;
                        if (StringHelper.InArray(Path.GetExtension(uploadFile.FileName).ToLower(), ".jpg,.jpeg,.gif,.bmp,.png"))
                        {
                            Image img = Image.FromStream(uploadFile.InputStream);
                            if (upfileEntity.MaxHeight > 0 && img.Height > upfileEntity.MaxHeight)
                            {
                                attach.Error += "<li>图片高度不得超过" + upfileEntity.MaxHeight.ToString() + "</li>";
                                return attach;
                            }
                            else if (upfileEntity.Height > 0 && (img.Height > (upfileEntity.Height + 5) || img.Height < (upfileEntity.Height - 5)))
                            {
                                attach.Error += "<li>请上传图片高度为" + upfileEntity.Height.ToString() + "的图片</li>";
                                return attach;
                            }
                            if (upfileEntity.MaxWidth > 0 && img.Width > upfileEntity.MaxWidth)
                            {
                                attach.Error += "<li>图片宽度不得超过" + upfileEntity.MaxWidth.ToString() + "</li>";
                                return attach;
                            }
                            else if (upfileEntity.Width > 0 && (img.Width > (upfileEntity.Width + 5) || img.Width < (upfileEntity.Width - 5)))
                            {
                                attach.Error += "<li>请上传图片宽度为" + upfileEntity.Width.ToString() + "的图片</li>";
                                return attach;
                            }
                            string saveFullPath = string.IsNullOrEmpty(savePath) ? "" : savePath.Replace(extension, "!" + img.Width.ToString() + "x" + img.Height.ToString() + extension);
                            attach.Width = img.Width;
                            attach.Height = img.Height;
                            //if (Path.GetExtension(uploadFile.FileName).ToLower() == ".gif")
                            //{
                            //    SaveFile(uploadFile, savePath, upfileEntity, attach);
                            //}
                            //else
                            //{
                            //    SaveImage(img, saveFullPath, GetImgFormat(extension), upfileEntity, attach, false);
                            //}
                            if (upfileEntity.ThumbnailSizes == null || upfileEntity.ThumbnailSizes.Length == 0)
                            {
                                attach.Thumbnail_Sizes = "";
                            }
                            else
                            {
                                attach.Thumbnail_Sizes = upfileEntity.ThumbnailSizes.Aggregate((i, j) => i + "," + j).Trim(',');
                            }
                            SaveImage(img, saveFullPath, GetImgFormat(extension), upfileEntity, attach, false);
                            //img.Save(sSavePath + sFilename.Replace(extension, "!" + img.Width.ToString() + "x" + img.Height.ToString() + extension));

                            if (upfileEntity.ThumbnailSizes != null && upfileEntity.ThumbnailSizes.Length > 0)
                            {
                                for (int i = 0; i < upfileEntity.ThumbnailSizes.Length; i++)
                                {
                                    upfileEntity.Prefix_Name = "_" + upfileEntity.ThumbnailSizes[i];
                                    MakeThumbnailImage(img, saveFullPath, upfileEntity.ThumbnailSizes[i], upfileEntity, (i == upfileEntity.ThumbnailSizes.Length - 1) ? true : false);
                                    //string[] thumbnailSize = upfileEntity.ThumbnailSizes[i].Split('_');
                                    //if (thumbnailSize.Length == 2)
                                    //{
                                    //    int thumbnailWidth = TypeConverter.StrToInt(thumbnailSize[0], 0);
                                    //    int thumbnailHeight = TypeConverter.StrToInt(thumbnailSize[1], 0);
                                    //    if (thumbnailWidth > 0 && thumbnailHeight > 0)
                                    //    {
                                    //        //缩略
                                    //        Thumbnail.MakeThumbnailImage(img, saveFullPath.Replace(extension, "!" + img.Width.ToString() + "x" + img.Height.ToString() + "_" + upfileEntity.ThumbnailSizes[i] + extension), thumbnailWidth, thumbnailHeight, true, upfileEntity);
                                    //    }
                                    //}
                                }
                                img.Dispose();
                            }
                        }
                        else
                        {
                            SaveFile(uploadFile, savePath, upfileEntity, attach);
                        }
                        if (upfileEntity.ftpclient != null)
                        {
                            upfileEntity.ftpclient.Disconnect();
                            upfileEntity.ftpclient = null;
                        }
                    }
                    catch (ArgumentException errArgument)
                    {
                        File.Delete(savePath);
                        attach.Error += errArgument.Message;
                        attach.FileName = "";
                    }
                }
            }
            else //上传除图片文件以外的全部文件
            {
                attach.Error = "<li>请上传指定的文件类型[.jpg,.jpeg,.gif,.bmp,.png]</li>";
                return attach;
            }
            return attach;
        }

        public AttachmentInfo[] UploadFiles(IList<System.Web.HttpPostedFileBase> uploadFiles, UpFileEntity upfileEntity)
        {
            upfileEntity.IsClose = false;
            string[] tmp = StringHelper.SplitString(upfileEntity.AllowType, ",");
            string[] allowFileExtName = new string[tmp.Length];
            int[] maxSize = new int[tmp.Length];


            for (int i = 0; i < tmp.Length; i++)
            {
                allowFileExtName[i] = StringHelper.CutString(tmp[i], 0, tmp[i].LastIndexOf(","));
                maxSize[i] = TypeConverter.StrToInt(StringHelper.CutString(tmp[i], tmp[i].LastIndexOf(",") + 1), 0);
            }

            int saveFileCount = 0;
            int fCount = uploadFiles.Count;

            for (int i = 0; i < fCount; i++)
            {
                if (uploadFiles[i]!=null && !uploadFiles[i].FileName.Equals("") && HttpContext.Current.Request.Files.AllKeys[i].StartsWith(upfileEntity.Key))
                {
                    saveFileCount++;
                }
            }

            AttachmentInfo[] attachmentInfo = saveFileCount > 0 ? new AttachmentInfo[saveFileCount] : null;
            if (saveFileCount > upfileEntity.MaxAllowFileCount)
                return attachmentInfo;

            saveFileCount = 0;


            for (int i = 0; i < fCount; i++)
            {
                if (!uploadFiles[i].FileName.Equals("") && HttpContext.Current.Request.Files.AllKeys[i].StartsWith(upfileEntity.Key))
                {
                    attachmentInfo[i] = UploadFile(uploadFiles[i], upfileEntity);
                   
                }
            }
            if (upfileEntity.ftpclient != null)
            {
                upfileEntity.ftpclient.Disconnect();
                upfileEntity.ftpclient = null;
            }
            return attachmentInfo;
        }

        public AttachmentInfo UploadImage(int contentLength,string clientFileName,Image img, UpFileEntity upfileEntity)
        {
            AttachmentInfo attach = new AttachmentInfo();
            int nFileLen = contentLength;
            string fileextname = Path.GetExtension(clientFileName).ToLower();
            upfileEntity.File_Ext = fileextname;
            string savePath = GetSavePath(nFileLen, clientFileName, upfileEntity, attach);

            if (StringHelper.InArray(Path.GetExtension(clientFileName).ToLower(), upfileEntity.AllowType))
            {
                if (contentLength / 1024 > upfileEntity.Size)
                {
                    attach.Error = "<li>请上传指定文件大小的文件</li>";
                    return attach;
                }
                else
                {
                    //上传图片文件jpg,gif
                    try
                    {
                        string extension = Path.GetExtension(savePath);// 
                        attach.FileType = extension;
                        attach.FileSize = nFileLen / 1024;
                        if (StringHelper.InArray(Path.GetExtension(clientFileName).ToLower(), ".jpg,.jpeg,.gif,.bmp,.png"))
                        {
                            if (upfileEntity.MaxHeight > 0 && img.Height > upfileEntity.MaxHeight)
                            {
                                attach.Error += "<li>图片高度不得超过" + upfileEntity.MaxHeight.ToString() + "</li>";
                                return attach;
                            }
                            else if (upfileEntity.Height > 0 && (img.Height > (upfileEntity.Height + 5) || img.Height < (upfileEntity.Height - 5)))
                            {
                                attach.Error += "<li>请上传图片高度为" + upfileEntity.Height.ToString() + "的图片</li>";
                                return attach;
                            }
                            if (upfileEntity.MaxWidth > 0 && img.Width > upfileEntity.MaxWidth)
                            {
                                attach.Error += "<li>图片宽度不得超过" + upfileEntity.MaxWidth.ToString() + "</li>";
                                return attach;
                            }
                            else if (upfileEntity.Width > 0 && (img.Width > (upfileEntity.Width + 5) || img.Width < (upfileEntity.Width - 5)))
                            {
                                attach.Error += "<li>请上传图片宽度为" + upfileEntity.Width.ToString() + "的图片</li>";
                                return attach;
                            }
                            string saveFullPath = string.IsNullOrEmpty(savePath) ? "" : savePath.Replace(extension, "!" + img.Width.ToString() + "x" + img.Height.ToString() + extension);
                            attach.Width = img.Width;
                            attach.Height = img.Height;


                            if (upfileEntity.ThumbnailSizes == null || upfileEntity.ThumbnailSizes.Length == 0)
                            {
                                attach.Thumbnail_Sizes = "";
                            }
                            else
                            {
                                attach.Thumbnail_Sizes = upfileEntity.ThumbnailSizes.Aggregate((i, j) => i + "," + j).Trim(',');
                            }
                                SaveImage(img, saveFullPath, GetImgFormat(extension), upfileEntity, attach, false);
                            //img.Save(sSavePath + sFilename.Replace(extension, "!" + img.Width.ToString() + "x" + img.Height.ToString() + extension));
                            if (upfileEntity.ThumbnailSizes != null && upfileEntity.ThumbnailSizes.Length > 0)
                            {
                                for (int i = 0; i < upfileEntity.ThumbnailSizes.Length; i++)
                                {
                                    upfileEntity.Prefix_Name = "_" + upfileEntity.ThumbnailSizes[i];
                                    MakeThumbnailImage(img, saveFullPath, upfileEntity.ThumbnailSizes[i], upfileEntity, (i == upfileEntity.ThumbnailSizes.Length - 1) ? true : false);
                                }
                            }
                                img.Dispose();
                        }
                        else
                        {
                            SaveImage(img, savePath, GetImgFormat(extension), upfileEntity, attach);
                        }
                        if (upfileEntity.ftpclient != null)
                        {
                            upfileEntity.ftpclient.Disconnect();
                            upfileEntity.ftpclient = null;
                        }
                    }
                    catch (ArgumentException errArgument)
                    {
                        File.Delete(savePath);
                        attach.Error += errArgument.Message;
                        attach.FileName = "";
                    }
                }
            }
            else //上传除图片文件以外的全部文件
            {
                attach.Error = "<li>请上传指定的文件类型</li>";
                return attach;
            }
            return attach;
        }

        #region 水印
        /// <summary>
        /// 加图片水印
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="watermarkFilename">水印文件名</param>
        /// <param name="watermarkStatus">图片水印位置</param>
        public static void AddImageSignPic(string dir, string oriFileName, string formatName, string extension, string watermarkFilename, int watermarkStatus, int quality, int watermarkTransparency)
        {
            Image img = Image.FromFile(dir + oriFileName.Replace(extension, formatName + extension));
            Graphics g = Graphics.FromImage(img);
            //设置高质量插值法
            //g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            Image watermark = new Bitmap(watermarkFilename);

            if (watermark.Height >= img.Height || watermark.Width >= img.Width)
                return;

            ImageAttributes imageAttributes = new ImageAttributes();
            ColorMap colorMap = new ColorMap();

            colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
            colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
            ColorMap[] remapTable = { colorMap };

            imageAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

            float transparency = 0.5F;
            if (watermarkTransparency >= 1 && watermarkTransparency <= 10)
                transparency = (watermarkTransparency / 10.0F);


            float[][] colorMatrixElements = {
												new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
												new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
												new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
												new float[] {0.0f,  0.0f,  0.0f,  transparency, 0.0f},
												new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
											};

            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);

            imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            int xpos = 0;
            int ypos = 0;

            switch (watermarkStatus)
            {
                case 1:
                    xpos = (int)(img.Width * (float).01);
                    ypos = (int)(img.Height * (float).01);
                    break;
                case 2:
                    xpos = (int)((img.Width * (float).50) - (watermark.Width / 2));
                    ypos = (int)(img.Height * (float).01);
                    break;
                case 3:
                    xpos = (int)((img.Width * (float).99) - (watermark.Width));
                    ypos = (int)(img.Height * (float).01);
                    break;
                case 4:
                    xpos = (int)(img.Width * (float).01);
                    ypos = (int)((img.Height * (float).50) - (watermark.Height / 2));
                    break;
                case 5:
                    xpos = (int)((img.Width * (float).50) - (watermark.Width / 2));
                    ypos = (int)((img.Height * (float).50) - (watermark.Height / 2));
                    break;
                case 6:
                    xpos = (int)((img.Width * (float).99) - (watermark.Width));
                    ypos = (int)((img.Height * (float).50) - (watermark.Height / 2));
                    break;
                case 7:
                    xpos = (int)(img.Width * (float).01);
                    ypos = (int)((img.Height * (float).99) - watermark.Height);
                    break;
                case 8:
                    xpos = (int)((img.Width * (float).50) - (watermark.Width / 2));
                    ypos = (int)((img.Height * (float).99) - watermark.Height);
                    break;
                case 9:
                    xpos = (int)((img.Width * (float).99) - (watermark.Width));
                    ypos = (int)((img.Height * (float).99) - watermark.Height);
                    break;
            }

            g.DrawImage(watermark, new Rectangle(xpos, ypos, watermark.Width, watermark.Height), 0, 0, watermark.Width, watermark.Height, GraphicsUnit.Pixel, imageAttributes);

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo ici = null;
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.MimeType.IndexOf("jpeg") > -1)
                    ici = codec;
            }
            EncoderParameters encoderParams = new EncoderParameters();
            long[] qualityParam = new long[1];
            if (quality < 0 || quality > 100)
                quality = 80;

            qualityParam[0] = quality;

            EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qualityParam);
            encoderParams.Param[0] = encoderParam;

            if (ici != null)
                img.Save(dir + oriFileName.Replace(extension, "-WM" + formatName + extension), ici, encoderParams);
            else
                img.Save(dir + oriFileName.Replace(extension, "-WM" + formatName + extension));

            g.Dispose();
            img.Dispose();
            watermark.Dispose();
            imageAttributes.Dispose();
        }

        /// <summary>
        /// 增加文字水印
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="watermarkText">水印文字</param>
        /// <param name="watermarkStatus">图片水印位置</param>
        public static void AddImageSignText(string dir, string oriFileName, string formatName, string extension, string watermarkText, int watermarkStatus, int quality, string fontname, int fontsize)
        {
            Image img = Image.FromFile(dir + oriFileName.Replace(extension, formatName + extension));
            Graphics g = Graphics.FromImage(img);
            Font drawFont = new Font(fontname, fontsize, FontStyle.Regular, GraphicsUnit.Pixel);
            SizeF crSize;
            crSize = g.MeasureString(watermarkText, drawFont);

            float xpos = 0;
            float ypos = 0;

            switch (watermarkStatus)
            {
                case 1:
                    xpos = (float)img.Width * (float).01;
                    ypos = (float)img.Height * (float).01;
                    break;
                case 2:
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = (float)img.Height * (float).01;
                    break;
                case 3:
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = (float)img.Height * (float).01;
                    break;
                case 4:
                    xpos = (float)img.Width * (float).01;
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case 5:
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case 6:
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case 7:
                    xpos = (float)img.Width * (float).01;
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
                case 8:
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
                case 9:
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
            }

            g.DrawString(watermarkText, drawFont, new SolidBrush(Color.White), xpos + 1, ypos + 1);
            g.DrawString(watermarkText, drawFont, new SolidBrush(Color.Black), xpos, ypos);

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo ici = null;
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.MimeType.IndexOf("jpeg") > -1)
                    ici = codec;
            }
            EncoderParameters encoderParams = new EncoderParameters();
            long[] qualityParam = new long[1];
            if (quality < 0 || quality > 100)
                quality = 80;

            qualityParam[0] = quality;

            EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qualityParam);
            encoderParams.Param[0] = encoderParam;

            if (ici != null)
                img.Save(dir + oriFileName.Replace(extension, "-WM" + formatName + extension), ici, encoderParams);
            else
                img.Save(dir + oriFileName.Replace(extension, "-WM" + formatName + extension));

            g.Dispose();
            img.Dispose();
        }
        #endregion

        #region 操作
        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="saveDir"></param>
        /// <param name="thumbnailSizes"></param>
        /// <returns></returns>
        public string MakeThumbnailImage(string fileName, string thumbnailSizes, UpFileEntity upfileEntity, bool isDispose = true)
        {
            return MakeThumbnailImage(fileName, null, thumbnailSizes, upfileEntity, isDispose);
        }

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="saveDir"></param>
        /// <param name="thumbnailSizes"></param>
        /// <returns></returns>
        public string MakeThumbnailImage(string fileName, string savePath, string thumbnailSizes, UpFileEntity upfileEntity, bool isDispose = true)
        {
            try
            {
                string extension = Path.GetExtension(savePath);
                string[] thumbnailSize = thumbnailSizes.Split('_');
                int thumbnailWidth = TypeConverter.StrToInt(thumbnailSize[0], 0);
                int thumbnailHeight = TypeConverter.StrToInt(thumbnailSize[1], 0);
                if (thumbnailWidth > 0 && thumbnailHeight > 0)
                {
                    string thumbnailFilePath = "";
                    if (!string.IsNullOrEmpty(savePath))
                    {
                        thumbnailFilePath = savePath.Replace(extension, "_" + thumbnailSizes + extension);
                    }
                    else
                    {
                        thumbnailFilePath = upfileEntity.Mast_File_Name;//.Replace(extension, "_" + thumbnailSizes + extension);
                    }
                    Thumbnail.MakeThumbnailImage(fileName, thumbnailFilePath, thumbnailWidth, thumbnailHeight, upfileEntity, isDispose);
                    return thumbnailFilePath;
                }
                return "";
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="saveDir"></param>
        /// <param name="thumbnailSizes"></param>
        /// <returns></returns>
        public string MakeThumbnailImage(Image img, string savePath, string thumbnailSizes, UpFileEntity upfileEntity,bool isDispose=true)
        {
            try
            {
                string extension = Path.GetExtension(savePath);
                string[] thumbnailSize = thumbnailSizes.Split('_');
                int thumbnailWidth = TypeConverter.StrToInt(thumbnailSize[0], 0);
                int thumbnailHeight = TypeConverter.StrToInt(thumbnailSize[1], 0);
                if (thumbnailWidth > 0 && thumbnailHeight > 0)
                {
                    string thumbnailFilePath;
                    if (!string.IsNullOrEmpty(savePath))
                    {
                        thumbnailFilePath = savePath.Replace(extension, "_" + thumbnailSizes + extension);
                    }
                    else
                    {
                        thumbnailFilePath = upfileEntity.Mast_File_Name;//.Replace(extension, "_" + thumbnailSizes + extension);
                    }
                    Thumbnail.MakeThumbnailImage(img, thumbnailFilePath, thumbnailWidth, thumbnailHeight, upfileEntity, isDispose);
                    return thumbnailFilePath;
                }
                return "";
            }
            catch
            {
                return "";
            }
        }

        public string MakeThumbnailImage(Image img, string thumbnailSizes, UpFileEntity upfileEntity, bool isDispose = true)
        {
            return MakeThumbnailImage(img, null, thumbnailSizes, upfileEntity, isDispose);
        }
        #region 删除

        /// <summary>
        /// 文件删除
        /// </summary>
        /// <param name="saveDir">中间目录</param>
        /// <param name="fileName">文件地址</param>
        /// <param name="ident">FTP唯一标识[-1为本地，0-当前配置，>0相应的配置</param>
        /// <param name="groupName">组名</param>
        /// <returns></returns>
        public int DeleteImage(string saveDir, string fileName, int ident,string groupName)
        {
            try
            {
                if (ident == -1 && groupName == "none")
                {
                    return DeleteImage(saveDir, fileName);
                }
                else if (ident != -1)
                {
                    return DeleteImage(saveDir, fileName, ident);
                }
                else
                {
                    return DeleteImage(saveDir, fileName, groupName);
                }
            }
            catch { return 0; }
        }

        /// <summary>
        /// 文件删除
        /// </summary>
        /// <param name="saveDir">中间目录</param>
        /// <param name="fileName">文件地址</param>
        /// <param name="ident">FTP唯一标识[-1为本地，0-当前配置，>0相应的配置</param>
        /// <returns></returns>
        public int DeleteImage(string saveDir, string fileName, int ident)
        {
            string[] filenames = fileName.Split(',');
            if (filenames.Length == 1)
            {
                if (ident == -1)
                {
                    string filePath = GetDeleteDirectory(ident, saveDir, fileName);
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    string filePath = GetDeleteDirectory(ident, saveDir, fileName);
                    if (FTPs.IsLocalhost(ident))
                    {
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        FTPs.DeleteFile(filePath, fileName, ident);
                        return 1;
                    }
                }
            }
            else {
                return DeleteImages(saveDir, filenames, ident);
            }
        }

        public int DeleteImages(string saveDir, string[] fileNames, int ident)
        {
            if (ident == -1)
            {
                int result = 0;
                foreach (string fileName in fileNames)
                {
                    string filePath = GetDeleteDirectory(ident, saveDir, fileName);
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        result++;
                    }
                }
                return result;
            }
            else
            {
                if (FTPs.IsLocalhost(ident))
                {
                    int result = 0;
                    foreach (string fileName in fileNames)
                    {
                        string filePath = GetDeleteDirectory(ident, saveDir, fileName);
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                            result++;
                        }
                    }
                    return result;
                }
                else
                {
                    string filePath = GetDeleteDirectory(ident, saveDir, fileNames[0]);
                    FTPs.DeleteFiles(fileNames, filePath, ident);
                    return 1;
                }
            }
        }

        /// <summary>
        /// 文件删除
        /// </summary>
        /// <param name="saveDir">中间目录</param>
        /// <param name="fileName">文件地址</param>
        /// <param name="groupName">组名</param>
        /// <returns></returns>
        public int DeleteImage(string saveDir, string fileName, string groupName)
        {
            string path = GetDeleteDirectory(groupName,fileName);
            try
            {
                IDFS dfs = DFSProvider.Instrance(groupName);
                bool result = dfs.DeleteFile(path);

                #region 接口调用-备份删除

                #endregion

                return result ? 1 : 0;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="saveDir"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public int DeleteImage(string saveDir, string fileName)
        {
            return DeleteImage(saveDir, fileName, -1);//删除本地
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="saveDir">文件目录</param>
        /// <param name="fileName">文件名</param>
        /// <param name="ident">FTP唯一标识</param>
        /// <returns></returns>
        public int DeleteImage(string fileName, bool isAddImagesFilename, int ident,string groupName)
        {
            if (ident == -1 && groupName == "none")
            {
                return DeleteImage(fileName, isAddImagesFilename);
            }
            else if (ident != -1)
            {
                return DeleteImage(fileName,isAddImagesFilename, ident);
            }
            else
            {
                return DeleteImage(fileName,isAddImagesFilename, groupName);
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="saveDir">文件目录</param>
        /// <param name="fileName">文件名</param>
        /// <param name="ident">FTP唯一标识</param>
        /// <returns></returns>
        public int DeleteImage(string fileName, bool isAddImagesFilename, int ident)
        {
            string[] filenames = fileName.Split(',');
            if (filenames.Length == 1)
            {
                if (ident == -1)
                {
                    string filePath = GetDeleteDirectory(isAddImagesFilename) + fileName;
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {

                    string filePath = GetDeleteDirectory(ident, "", fileName);
                    if (FTPs.IsLocalhost(ident))
                    {
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        FTPs.DeleteFile(filePath, fileName, ident);
                        return 1;
                    }
                }
            }
            else {
                return DeleteImages(filenames, isAddImagesFilename, ident);
            }
        }

        public int DeleteImages(string[] fileNames, bool isAddImagesFilename, int ident)
        {
            try
            {
                if (ident == -1)
                {
                    int result = 0;
                    foreach (string fileName in fileNames)
                    {
                        string filePath = GetDeleteDirectory(isAddImagesFilename) + fileName;
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                            result++;
                        }
                    }
                    return result;
                }
                else
                {
                    if (FTPs.IsLocalhost(ident))
                    {
                        int result = 0;
                        foreach (string fileName in fileNames)
                        {
                            string filePath = GetDeleteDirectory(ident, "", fileName);
                            if (File.Exists(filePath))
                            {
                                File.Delete(filePath);
                                result++;
                            }
                        }
                        return result;
                    }
                    else
                    {
                        string filePath = GetDeleteDirectory(ident, "", fileNames[0]);
                        FTPs.DeleteFiles(fileNames, filePath, ident);
                        return 1;
                    }
                }
            }
            catch { return 0; }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="saveDir">文件目录</param>
        /// <param name="fileName">文件名</param>
        /// <param name="groupName">文件系统组名</param>
        /// <returns></returns>
        public int DeleteImage(string fileName, bool isAddImagesFilename, string groupName)
        {
            try
            {
                string path = GetDeleteDirectory(groupName, fileName);
                IDFS dfs = DFSProvider.Instrance(groupName);
                dfs.DeleteFile(path);

                #region 接口调用-备份删除

                #endregion

                return 1;
            }
            catch {
                return 0;
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="saveDir"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public int DeleteImage(string fileName, bool isAddImagesFilename)
        {
            return DeleteImage(fileName, isAddImagesFilename, -1);//删除本地
        }

        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="saveDir"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public int DeleteImage(string fileName, int ident, string groupName)
        {
            if (ident == -1 && groupName == "none")
            {
                return DeleteImage(fileName);
            }
            else if (ident != -1)
            {
                return DeleteImage(fileName, ident);
            }
            else
            {
                return DeleteImage(fileName, groupName, true);
            }
        }

        // /// <summary>
        ///// 删除图片
        ///// </summary>
        ///// <param name="saveDir"></param>
        ///// <param name="fileName"></param>
        ///// <returns></returns>
        //public int DeleteImage(string[] fileName, int ident, string groupName)
        //{

        //}

        public int DeleteImage(string fileName)
        {
            return DeleteImage(fileName, true, -1);
        }

        public int DeleteImage(string fileName, int ident)
        {
            return DeleteImage(fileName, true, ident);
        }

        public int DeleteImage(string fileName, string groupName, bool isGroup)
        {
            return DeleteImage(fileName, true, groupName);
        }

        /// <summary>
        /// 删除原始图+缩略图
        /// </summary>
        /// <param name="saveDir"></param>
        /// <param name="fileName"></param>
        /// <param name="thumbnailSizes"></param>
        /// <returns></returns>
        public int DeleteImage(string saveDir, string fileName, string[] thumbnailSizes,int ident,string groupName)
        {
            if (ident == -1 && groupName == "none")
            {
                return DeleteImage(saveDir,fileName,thumbnailSizes);
            }
            else if (ident != -1)
            {
                return DeleteImage(saveDir, fileName, thumbnailSizes, ident);
            }
            else
            {
                return DeleteImage(saveDir, fileName, thumbnailSizes,groupName);
            }
        }

        public int DeleteImage(string saveDir, string fileName, string[] thumbnailSizes)
        {
            int count = DeleteImage(saveDir, fileName, -1);
            foreach (string photo in thumbnailSizes)
            {
                count += DeleteImage(saveDir, GetOriThumbnailPath(saveDir, fileName, photo), -1);
            }
            return count;
        }

        public int DeleteImage(string saveDir, string fileName, string[] thumbnailSizes, int ident)
        {
            int count = DeleteImage(saveDir, fileName, ident);
            foreach (string photo in thumbnailSizes)
            {
                count += DeleteImage(saveDir, GetOriThumbnailPath(saveDir, fileName, photo), ident);
            }
            return count;
        }

        public int DeleteImage(string saveDir, string fileName, string[] thumbnailSizes, string groupName)
        {
            int count = DeleteImage(saveDir, fileName, groupName);
            foreach (string photo in thumbnailSizes)
            {
                count += DeleteImage(saveDir, GetOriThumbnailPath(saveDir, fileName, photo), groupName);
            }
            return count;
        }

        /// <summary>
        /// 删除原始图+缩略图
        /// </summary>
        /// <param name="saveDir"></param>
        /// <param name="fileName"></param>
        /// <param name="thumbnailSizes"></param>
        /// <returns></returns>
        public int DeleteImage(string fileName, string[] thumbnailSizes,int ident,string groupName)
        {
            if (ident == -1 && groupName == "none")
            {
                return DeleteImage(fileName, thumbnailSizes);
            }
            else if (ident != -1)
            {
                return DeleteImage(fileName, thumbnailSizes, ident);
            }
            else
            {
                return DeleteImage(fileName, thumbnailSizes, groupName);
            }
        }

        public int DeleteImage(string fileName, string[] thumbnailSizes)
        {
            return DeleteImage(fileName, thumbnailSizes, true, -1);
        }

        public int DeleteImage(string fileName, string[] thumbnailSizes, int ident)
        {
            return DeleteImage(fileName, thumbnailSizes, true, ident);
        }

        public int DeleteImage(string fileName, string[] thumbnailSizes, string groupName)
        {
            return DeleteImage(fileName, thumbnailSizes, true, groupName);
        }

        /// <summary>
        /// 删除原始图+缩略图
        /// </summary>
        /// <param name="saveDir"></param>
        /// <param name="fileName"></param>
        /// <param name="thumbnailSizes"></param>
        /// <returns></returns>
        public int DeleteImage(string fileName, string[] thumbnailSizes,bool isAddImagesfileName, int ident, string groupName)
        {
            if (ident == -1 && groupName == "none")
            {
                return DeleteImage(fileName, thumbnailSizes,isAddImagesfileName);
            }
            else if (ident != -1)
            {
                return DeleteImage(fileName, thumbnailSizes,isAddImagesfileName, ident);
            }
            else
            {
                return DeleteImage(fileName, thumbnailSizes,isAddImagesfileName, groupName);
            }
        }

        public int DeleteImage(string fileName, string[] thumbnailSizes, bool isAddImagesFilename)
        {
            int count =0;
            if (isAddImagesFilename)
                count = DeleteImage(fileName, -1);
            else
                count = DeleteImage(fileName, isAddImagesFilename, -1);
            
            foreach (string photo in thumbnailSizes)
            {
                count += DeleteImage(GetOriThumbnailPath(fileName, photo), isAddImagesFilename, -1);
            }
            return count;
        }

        public int DeleteImage(string fileName, string[] thumbnailSizes, bool isAddImagesFilename, int ident)
        {
            int count = 0;
            if (isAddImagesFilename)
                count = DeleteImage(fileName, ident);
            else
                count = DeleteImage(fileName, isAddImagesFilename, ident);

            foreach (string photo in thumbnailSizes)
            {
                count += DeleteImage(GetOriThumbnailPath(fileName, photo), isAddImagesFilename, ident);
            }
            return count;
        }

        public int DeleteImage(string fileName, string[] thumbnailSizes, bool isAddImagesFilename, string groupName)
        {
            int count = 0;
            if (isAddImagesFilename)
                count = DeleteImage(fileName, groupName,true);
            else
                count = DeleteImage(fileName, isAddImagesFilename, groupName);

            foreach (string photo in thumbnailSizes)
            {
                count += DeleteImage(GetOriThumbnailPath(fileName, photo), isAddImagesFilename, groupName);
            }
            return count;
        }

        /// <summary>
        /// 通过原始图地址获取缩略图地址（不带域，主机头-如：2011/1/11/***.jpg)
        /// </summary>
        /// <param name="saveDir"></param>
        /// <param name="fileName"></param>
        /// <param name="sizeStr">100_100</param>
        /// <returns></returns>
        public static string GetNoJudgeThumbnailPath(string saveDir, string fileName, string sizeStr, string thumbPrefix = "_")
        {
            string extenstion = Path.GetExtension(fileName).ToLower();
            if (string.IsNullOrEmpty(sizeStr))
            {
                return fileName;
            }
            else
            {
                if (string.IsNullOrEmpty(extenstion))
                {
                    return "";
                }
                return fileName.Replace(extenstion, (thumbPrefix + sizeStr + extenstion));
            }

        }

        public static string GetNoJudgeThumbnailPath(string filePath, string sizeStr)
        {
            string extenstion = Path.GetExtension(filePath).ToLower();

            if (string.IsNullOrEmpty(sizeStr))
            {
                return filePath;
            }
            else
            {
                if (string.IsNullOrEmpty(extenstion))
                {
                    return "";
                }
                return filePath.Replace(extenstion, ("_" + sizeStr + extenstion));
            }
        }

        /// <summary>
        /// 通过原始图地址获取缩略图地址（不带域，主机头-如：2011/1/11/***.jpg)
        /// </summary>
        /// <param name="saveDir"></param>
        /// <param name="fileName"></param>
        /// <param name="sizeStr">100_100</param>
        /// <returns></returns>
        public static string GetOriThumbnailPath(string saveDir, string fileName, string sizeStr,string thumbPrefix="_")
        {
            string filePath = fileName.StartsWith("http://") ? fileName : ((saveDir??"") + fileName.ToLower());
            string extenstion = Path.GetExtension(filePath).ToLower();
            if (filePath.IndexOf("!") < 0)
            {
                return filePath;
            }
            string[] pathArr =  filePath.Replace(extenstion, "").Split('!')[1].Split('x');;
            
            int width = TypeConverter.StrToInt(pathArr[0]);
            int height = TypeConverter.StrToInt(pathArr[1]);


            if (string.IsNullOrEmpty(sizeStr))
            {
                return filePath;
            }
            else
            {
                int minWidth = 132;
                int minHeight = 132;
                bool isCheckMin = false;
                if (sizeStr.IndexOf("_") > 0)
                {
                    string[] minSize = sizeStr.Split('_');
                    minWidth = TypeConverter.StrToInt(minSize[0]);
                    minHeight = TypeConverter.StrToInt(minSize[1]);
                    if (width > minWidth && height > minHeight)
                    {
                        isCheckMin = true;
                    }
                }
                else
                {
                    isCheckMin = true;
                }
                if (string.IsNullOrEmpty(extenstion))
                {
                    return "";
                }
                if (isCheckMin)
                {
                    return filePath.Replace(extenstion, ("_" + sizeStr + extenstion));
                }
                else
                {
                    return filePath;
                }
            }
        }

        public static string GetOriThumbnailPath(string filePath, string sizeStr)
        {
            string extenstion = Path.GetExtension(filePath).ToLower();
            if (filePath.IndexOf("!") < 0)
            {
                return filePath;
            }
            string[] pathArr = filePath.Replace(extenstion, "").Split('!')[1].Split('x'); ;

            int width = TypeConverter.StrToInt(pathArr[0]);
            int height = TypeConverter.StrToInt(pathArr[1]);

            if (string.IsNullOrEmpty(sizeStr))
            {
                return filePath;
            }
            else
            {
                int minWidth = 132;
                int minHeight = 132;
                bool isCheckMin = false;
                if (!extenstion.Equals(".gif"))
                {
                    if (sizeStr.IndexOf("_") > 0)
                    {
                        string[] minSize = sizeStr.Split('_');
                        minWidth = TypeConverter.StrToInt(minSize[0]);
                        minHeight = TypeConverter.StrToInt(minSize[1]);
                        if (width > minWidth && height > minHeight)
                        {
                            isCheckMin = true;
                        }
                    }
                    else
                    {
                        isCheckMin = true;
                    }
                }
                if (string.IsNullOrEmpty(extenstion))
                {
                    return "";
                }
                if (isCheckMin)
                {
                    return filePath.Replace(extenstion, ("_" + sizeStr + extenstion));
                }
                else
                {
                    return filePath;
                }
            }
        }

        /// <summary>
        /// 获取缩略图地址(带域、主机头-如：http://images.juxiangke.com/)
        /// </summary>
        /// <param name="saveDir"></param>
        /// <param name="fileName"></param>
        /// <param name="sizeStr"></param>
        /// <returns></returns>
        public static string GetThumbnailPath(string saveDir, string fileName, string sizeStr, string thumbPrefix = "_")
        {
            string extenstion = Path.GetExtension(fileName).ToLower();

            if (!fileName.StartsWith("http://"))
            {

                string filePath = Config.WebImagesUrl + saveDir + fileName.ToLower();
                if (extenstion.Equals(".gif"))
                {
                    return filePath;
                }
                if (string.IsNullOrEmpty(sizeStr))
                {
                    return filePath;
                }
                else
                {
                    if (string.IsNullOrEmpty(extenstion))
                    {
                        return "";
                    }
                    return filePath.Replace(extenstion, (thumbPrefix + sizeStr + extenstion));

                }
            }
            else {
                if (extenstion.Equals(".gif"))
                {
                    return fileName;
                }
                if (string.IsNullOrEmpty(sizeStr))
                {
                    return fileName;
                }
                else
                {
                    if (string.IsNullOrEmpty(extenstion))
                    {
                        return "";
                    }
                    return fileName.Replace(extenstion, (thumbPrefix + sizeStr + extenstion));
                }
            }
        }

        /// <summary>
        /// 从缩略图路径获取原图
        /// </summary>
        /// <param name="thumbnailPath">缩略图路径</param>
        /// <param name="sizeStr">缩略图大小</param>
        /// <param name="saveDir">中间路径</param>
        /// <param name="isUp">是否加入主机头完整地址</param>
        /// <returns></returns>
        public static string GetOriginalPath(string saveDir, string thumbnailPath, string sizeStr, bool isUp, string thumbPrefix = "_")
        {
            if (isUp)
            {
                if (thumbnailPath.StartsWith("http://"))
                {
                    thumbnailPath = thumbnailPath.Replace("http://", "");
                    thumbnailPath = thumbnailPath.Substring(thumbnailPath.IndexOf("/"));
                    return thumbnailPath.Replace(saveDir, "").Replace((thumbPrefix + sizeStr), "");
                }
                else
                {
                    string imgPath = Config.WebImagesUrl + saveDir;
                    return thumbnailPath.Replace(imgPath, "").Replace((thumbPrefix + sizeStr), "");
                }
            }
            else
            {
                return thumbnailPath.Replace((thumbPrefix + sizeStr), "");
            }
        }

        /// <summary>
        /// 从缩略图路径获取原图
        /// </summary>
        /// <param name="thumbnailPath">缩略图路径</param>
        /// <param name="sizeStr">缩略图大小</param>
        /// <param name="saveDir">中间路径</param>
        /// <param name="isUp">是否加入主机头完整地址</param>
        /// <returns></returns>
        public static string GetOriginalPath(string thumbnailPath, string sizeStr, bool isUp, string thumbPrefix = "_")
        {
            if (isUp)
            {
                if (thumbnailPath.StartsWith("http://"))
                {
                    thumbnailPath = thumbnailPath.Replace("http://", "");
                    thumbnailPath = thumbnailPath.Substring(thumbnailPath.IndexOf("/"));
                    return thumbnailPath.Replace((thumbPrefix + sizeStr), "");
                }
                else
                {
                string imgPath = Config.WebImagesUrl;
                return thumbnailPath.Replace(imgPath, "").Replace((thumbPrefix + sizeStr), "");
                }
            }
            else
            {
                return thumbnailPath.Replace((thumbPrefix + sizeStr), "");
            }
        }

        /// <summary>
        /// 从缩略图路径获取缩略图地址
        /// </summary>
        /// <param name="saveDir">中间路径</param>
        /// <param name="thumbnailPath">缩略图路径</param>
        /// <param name="sizeStr">缩略图大小</param>
        /// <param name="otherSizeStr">其他缩略图大小</param>
        /// <param name="isUp">是否加入主机头完整地址</param>
        /// <returns></returns>
        public static string GetOriginalPath(string saveDir, string thumbnailPath, string sizeStr, string otherSizeStr, bool isUp, string thumbPrefix = "_")
        {

            if (isUp)
            {
                if (thumbnailPath.StartsWith("http://"))
                {
                    thumbnailPath = thumbnailPath.Replace("http://", "");
                    thumbnailPath = thumbnailPath.Substring(thumbnailPath.IndexOf("/"));
                    return thumbnailPath.Replace(saveDir, "").Replace((thumbPrefix + sizeStr), (thumbPrefix + otherSizeStr));
                }
                else
                {
                    string imgPath = Config.WebImagesUrl + saveDir;
                    return thumbnailPath.Replace(imgPath, "").Replace((thumbPrefix + sizeStr), (thumbPrefix + otherSizeStr));
                }
            }
            else
            {
                return thumbnailPath.Replace((thumbPrefix + sizeStr), (thumbPrefix + otherSizeStr));
            }
        }

        /// <summary>
        /// 通过原文件判断是否存在缩略图，不存在返回原图
        /// </summary>
        /// <param name="saveDir">中间路径</param>
        /// <param name="filePath">原文件路径</param>
        /// <param name="bakFilePath">分布式文件系统时-备份的路径</param>
        /// <param name="thumbnailSize">缩略图大小格式</param>
        /// <param name="minThumbnailSize">缩略图压缩最小范围格式</param>
        /// <param name="isUp">是否返回完整路径[本地文件系统]</param>
        /// <returns></returns>
        public static string GetJudgeThumbnailPath(string saveDir, string filePath, string bakFilePath, string thumbnailSize, string minThumbnailSize, bool isUp = true, string thumbPrefix = "_")
        {
            string extenstion = Path.GetExtension(filePath).ToLower();
            string[] pathArr = null;
            if (filePath.IndexOf("!") > 0)
            {
                pathArr = filePath.Replace(extenstion, "").Split('!')[1].Split('x');
            }
            else
            {
                if (bakFilePath.IndexOf("!") < 0)
                {
                    return filePath;
                }
                pathArr = bakFilePath.Replace(extenstion, "").Split('!')[1].Split('x');
            }
            int width = TypeConverter.StrToInt(pathArr[0]);
            int height = TypeConverter.StrToInt(pathArr[1]);

            int minWidth = 132;
            int minHeight = 132;

            if (!string.IsNullOrEmpty(minThumbnailSize))
            {
                string[] minSize = minThumbnailSize.Split('_');
                minWidth = TypeConverter.StrToInt(minSize[0]);
                minHeight = TypeConverter.StrToInt(minSize[1]);
            }


            if (!filePath.StartsWith("http://"))
            {
                if (isUp)
                {
                    filePath = Config.WebImagesUrl + saveDir + filePath.ToLower();
                }
                if (extenstion.Equals(".gif"))
                {
                    return filePath;
                }
                if (string.IsNullOrEmpty(thumbnailSize))
                {
                    return filePath;
                }
                else
                {
                    if (string.IsNullOrEmpty(extenstion))
                    {
                        return "";
                    }
                    if (width > minWidth && height > minHeight)
                    {
                        return filePath.Replace(extenstion, (thumbPrefix + thumbnailSize + extenstion));
                    }
                    else
                    {
                        return filePath;
                    }
                }
            }
            else
            {
                if (extenstion.Equals(".gif"))
                {
                    return filePath;
                }
                if (string.IsNullOrEmpty(thumbnailSize))
                {
                    return filePath;
                }
                else
                {
                    if (string.IsNullOrEmpty(extenstion))
                    {
                        return "";
                    }
                    if (width > minWidth && height > minHeight)
                    {

                        return filePath.Replace(extenstion, (thumbPrefix + thumbnailSize + extenstion));
                    }
                    else
                    {
                        return filePath;
                    }
                }
            }
        }

        public static string GetJudgeThumbnailPath(string saveDir, string filePath, string bakFilePath, string thumbnailSize, bool isUp)
        {
            return GetJudgeThumbnailPath(saveDir, filePath, bakFilePath, thumbnailSize,"", isUp);
        }

        public static string GetJudgeThumbnailPath(string filePath, string bakFilePath, string thumbnailSize, bool isUp)
        {
            return GetJudgeThumbnailPath("", filePath, bakFilePath, thumbnailSize, isUp);
        }

        public static string GetJudgeThumbnailPath(string filePath, string bakFilePath, string thumbnailSize)
        {
            return GetJudgeThumbnailPath("", filePath, bakFilePath, thumbnailSize, true);
        }

        public static string GetJudgeThumbnailPath(string filePath, string bakFilePath, string thumbnailSize, string minThumbnailSize)
        {
            return GetJudgeThumbnailPath("", filePath, bakFilePath, thumbnailSize, minThumbnailSize, true);
        }
        #endregion
        #endregion

        #region 根据地址参数-获取图片

        public string GetNoJudgeThumbPath(string saveDir, string fileName, string sizeStr, string thumbPrefix = "_")
        {
            return GetNoJudgeThumbnailPath(saveDir, fileName, sizeStr, thumbPrefix);
        }

        public string GetNoJudgeThumbPath(string filePath, string sizeStr)
        {
            return GetNoJudgeThumbnailPath(filePath, sizeStr);
        }


        public string GetOriThumbPath(string saveDir, string fileName, string sizeStr, string thumbPrefix = "_")
        {
            return GetOriThumbnailPath(saveDir, fileName, sizeStr, thumbPrefix);
        }

        public string GetOriThumbPath(string filePath, string sizeStr, string thumbPrefix = "_")
        {
            return GetOriThumbnailPath(filePath, sizeStr, thumbPrefix);
        }

        public string GetThumbPath(string saveDir, string fileName, string sizeStr, string thumbPrefix = "_")
        {
            return GetThumbnailPath(saveDir, fileName, sizeStr, thumbPrefix);
        }

        public string GetOriPath(string saveDir, string thumbnailPath, string sizeStr, bool isUp, string thumbPrefix = "_")
        {
            return GetOriginalPath(saveDir, thumbnailPath, sizeStr, isUp, thumbPrefix);
        }

        public string GetOriPath(string thumbnailPath, string sizeStr, bool isUp, string thumbPrefix = "_")
        {
            return GetOriginalPath(thumbnailPath, sizeStr, isUp, thumbPrefix);
        }

        public string GetOriPath(string saveDir, string thumbnailPath, string sizeStr, string otherSizeStr, bool isUp, string thumbPrefix = "_")
        {
            return GetOriginalPath(saveDir, thumbnailPath, sizeStr, otherSizeStr, isUp, thumbPrefix);
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 获得附件存放目录
        /// </summary>
        /// <param name="forumid"></param>
        /// <param name="Config"></param>
        /// <param name="fileExtName"></param>
        /// <returns></returns>
        private string GetAttachmentPathFormat(string path, string fileExtName, UpFileEntity upFileEntity)
        {
            StringBuilder saveDir = new StringBuilder("");
            if (string.IsNullOrEmpty(upFileEntity.PathFormat))
            {
                switch (Config.AttachSave)
                {
                    case 0:
                        {
                            if (path != "")
                            {
                                saveDir.Append(path);
                                saveDir.Append("/");
                                //saveDir.Append(Path.DirectorySeparatorChar);
                            }
                            break;
                        }
                    //附件保存方式 0=按年/月/日存入不同目录 1=按年/月/日/存入不同目录 2=按存入不同目录 3=按文件类型存入不同目录
                    case 1:
                        {
                            saveDir.Append(DateTime.Now.ToString("yyyy"));
                            saveDir.Append("/");
                            saveDir.Append(DateTime.Now.ToString("MM"));
                            saveDir.Append("/");
                            saveDir.Append(DateTime.Now.ToString("dd"));
                            saveDir.Append("/");
                            if (path != "")
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
                saveDir.Append(upFileEntity.PathFormat);
            }
            return saveDir.ToString();
        }

        /// <summary>
        ///获取FTP存放方式
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileExtName"></param>
        /// <param name="pathFormat"></param>
        /// <returns></returns>
        private string GetFtpPathFormat(string path, string fileExtName, int pathFormat,UpFileEntity upfileEntity)
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
            else {
                saveDir.Append(upfileEntity.PathFormat);
            }
            return saveDir.ToString();
        }

        /// <summary>
        /// 获取分布式存储的存储方式
        /// </summary>
        /// <param name="pathFormat"></param>
        /// <returns></returns>
        private string GetDFSPathFormat(int pathFormat)
        {
            switch(pathFormat)
            {
                case 0:
                    {
                        return "";//使用系统自带
                    }
                case 1://重写规则1-未定
                    {
                        return "";
                    }
                default:
                    {
                        return "";//使用系统自带
                    }
            }
        }
        /// <summary>
        /// 获取分布式文件系统返回的文件名[带http://xxxxxx:xx/filename]
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>不带主机头的文件名</returns>
        private string GetDFSFileName(string fileName)
        {
            return fileName.Substring(fileName.IndexOf('/')+1);
        }
        /// <summary>
        /// 得到图片格式
        /// </summary>
        /// <param name="name">文件名称</param>
        /// <returns></returns>
        private static ImageFormat GetImgFormat(string name)
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

        #region 存储=加入FTP|分布式-vebin.h:2012.12.14
        /// <summary>
        /// 图片保存
        /// </summary>
        /// <param name="img">图片</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="imgFormat">图像格式</param>
        /// <param name="upFileEntity">上传限制</param>
        private void SaveImage(Image img, string savePath, ImageFormat imgFormat, UpFileEntity upFileEntity,AttachmentInfo attach,bool isDispose=true)
        {
            if (Config.DistributeFileSystem == 1 && upFileEntity.DistributeFileSystem == 1)
            {
                if (!string.IsNullOrEmpty(upFileEntity.GroupName))
                {
                    byte[] contentBytes = GetByteImage(img, imgFormat);
                    IDFS dfs = DFSProvider.Instrance(upFileEntity.GroupName);
                    string fileName = dfs.UploadFile(contentBytes, upFileEntity.File_Ext.Replace(".", ""));
                    upFileEntity.Mast_File_Name = fileName;//主文件
                    //设置远程URL址址
                    attach.RemoteUrl = attach.RemoteUrl + fileName;// GetDFSFileName(fileName);
                    attach.RemoteUrl2 = attach.RemoteUrl + fileName;// GetDFSFileName(fileName);
                    
                    #region 写入备份记录-通过接口
                    attach.Is_Bak = 1;
                    #endregion
                }
            }
            else
            {
                if (Config.FTPEnable == 1&&upFileEntity.FTPEnable==1)
                {
                    if (FTPs.IsLocalhost(upFileEntity.FTPIdent))//区分本地或FTP
                    {
                        img.Save(savePath, imgFormat);
                    }
                    else
                    {
                        string FileName = Path.GetFileName(savePath);
                        string fileExt = Path.GetExtension(savePath);
                        string dir = Path.GetDirectoryName(Config.FTPTempPath + GetFtpPathFormat("", fileExt, Config.FTPPathFormat, upFileEntity));
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        string saveTempPath = dir +"/"+ FileName;
                        img.Save(saveTempPath, imgFormat);//存入临时文件夹
                        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                        stopwatch.Start();
                        FTPs.UpLoadFile(Path.GetDirectoryName(savePath), saveTempPath, upFileEntity, upFileEntity.FTPIdent);//Ftp不需要再加文件名
                        stopwatch.Stop();
                        long minsecond = stopwatch.ElapsedMilliseconds;
                    }
                }
                else
                {
                    img.Save(savePath, imgFormat);
                }
                attach.FileName = attach.FileName.ToLower().Replace(upFileEntity.File_Ext.ToLower(), "!" + img.Width.ToString() + "x" + img.Height.ToString() + upFileEntity.File_Ext.ToLower());
                SetRemoteFileName(attach);
                SetMasterFileName(upFileEntity, savePath);
            }
            if (isDispose)
            {
                img.Dispose();
            }

            #region 备分记录
            attach.Date = DateTime.Now;
            attach.Bak_Status = 0;
            attach.Quote_Times = 0;
            attach.Site_ID = upFileEntity.Site_ID;
            attach.User_ID = upFileEntity.User_ID;
            attach.User_No = upFileEntity.User_No;
            if (!string.IsNullOrEmpty(config.BakClassName))
            {
                upFileEntity.AttachID = UploadBakProvider.GetInstance(Config.BakClassName).Create(attach);
            }
            #endregion
        }
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="postFile">文件上传</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="upFileEntity"></param>
        private void SaveFile(HttpPostedFileBase postFile, string savePath, UpFileEntity upFileEntity,AttachmentInfo attach)
        {
            if (Config.DistributeFileSystem == 1 && upFileEntity.DistributeFileSystem == 1)
            {
                if (!string.IsNullOrEmpty(upFileEntity.GroupName))
                {
                    byte[] contentBytes = GetBytePostFile(postFile);
                    IDFS dfs = DFSProvider.Instrance(upFileEntity.GroupName);
                    string fileName = dfs.UploadFile(contentBytes, upFileEntity.File_Ext.Replace(".", ""));
                    upFileEntity.Mast_File_Name = fileName;//主文件
                    //设置远程URL址址
                    attach.RemoteUrl = attach.RemoteUrl + fileName; //GetDFSFileName(fileName);
                    attach.RemoteUrl2 = attach.RemoteUrl + fileName;//GetDFSFileName(fileName);
                    #region 写入备份记录-通过接口
                    attach.Is_Bak = 1;
                    #endregion
                }
            }
            else
            {
                if (Config.FTPEnable == 1&&upFileEntity.FTPEnable==1)
                {
                    if (FTPs.IsLocalhost(upFileEntity.FTPIdent))//区分本地或FTP
                    {
                        postFile.SaveAs(savePath);
                    }
                    else
                    {
                        string FileName = Path.GetFileName(savePath);
                        string fileExt = Path.GetExtension(savePath);
                        string dir = Path.GetDirectoryName(Config.FTPTempPath +GetFtpPathFormat("", fileExt, Config.FTPPathFormat, upFileEntity));
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        string saveTempPath = dir + "/" + FileName;
                        postFile.SaveAs(saveTempPath);//存入临时文件夹
                        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                        stopwatch.Start();
                        FTPs.UpLoadFile(Path.GetDirectoryName(savePath), saveTempPath,upFileEntity, upFileEntity.FTPIdent);//-FTP不需要加文件名
                        stopwatch.Stop();
                        long minsecond = stopwatch.ElapsedMilliseconds;
                    }
                }
                else
                {
                    postFile.SaveAs(savePath);
                }
                SetRemoteFileName(attach);
            }
            #region 备分记录
            attach.Name = "原图";
            attach.Date = DateTime.Now;
            attach.Bak_Status = 0;
            attach.Quote_Times = 0;
            attach.Site_ID = upFileEntity.Site_ID;
            attach.User_ID = upFileEntity.User_ID;
            attach.User_No = upFileEntity.User_No;
            if (!string.IsNullOrEmpty(config.BakClassName))
            {
                upFileEntity.AttachID = UploadBakProvider.GetInstance(Config.BakClassName).Create(attach);
            }
            #endregion
        }
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="postFile">文件上传</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="upFileEntity"></param>
        private void SaveFile(HttpPostedFile postFile, string savePath, UpFileEntity upFileEntity, AttachmentInfo attach)
        {
            if (Config.DistributeFileSystem == 1 && upFileEntity.DistributeFileSystem == 1)
            {
                if (!string.IsNullOrEmpty(upFileEntity.GroupName))
                {
                    byte[] contentBytes = GetBytePostFile(postFile);
                    IDFS dfs = DFSProvider.Instrance(upFileEntity.GroupName);
                    string fileName = dfs.UploadFile(contentBytes, upFileEntity.File_Ext.Replace(".", ""));
                    upFileEntity.Mast_File_Name = fileName;//主文件
                    //设置远程URL址址
                    attach.RemoteUrl = attach.RemoteUrl + fileName; //GetDFSFileName(fileName);
                    attach.RemoteUrl2 = attach.RemoteUrl + fileName;//GetDFSFileName(fileName);
                    #region 写入备份记录-通过接口
                    attach.Is_Bak = 1;
                    #endregion
                }
            }
            else
            {
                if (Config.FTPEnable == 1 && upFileEntity.FTPEnable == 1)
                {
                    if (FTPs.IsLocalhost(upFileEntity.FTPIdent))//区分本地或FTP
                    {
                        postFile.SaveAs(savePath);
                    }
                    else
                    {
                        string FileName = Path.GetFileName(savePath);
                        string fileExt = Path.GetExtension(savePath);
                        string dir = Path.GetDirectoryName(Config.FTPTempPath + GetFtpPathFormat("", fileExt, Config.FTPPathFormat, upFileEntity));//GetSavePath(postFile.ContentLength, FileName, upFileEntity, attach)
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        string saveTempPath = dir + "/" + FileName; //Config.FTPTempPath + GetSavePath(0, FileName, upFileEntity, attach);
                        postFile.SaveAs(saveTempPath);//存入临时文件夹
                        FTPs.UpLoadFile(Path.GetDirectoryName(savePath), saveTempPath,upFileEntity, upFileEntity.FTPIdent);//-FTP不需要加文件名
                    }
                }
                else
                {
                    postFile.SaveAs(savePath);
                }
                SetRemoteFileName(attach);
            }
            #region 备分记录
            attach.Name = "原图";
            attach.Date = DateTime.Now;
            attach.Bak_Status = 0;
            attach.Quote_Times = 0;
            attach.Site_ID = upFileEntity.Site_ID;
            attach.User_ID = upFileEntity.User_ID;
            attach.User_No = upFileEntity.User_No;
            if (!string.IsNullOrEmpty(config.BakClassName))
            {
                upFileEntity.AttachID = UploadBakProvider.GetInstance(Config.BakClassName).Create(attach);
            }
            #endregion
        }
        /// <summary>
        /// 图像转为byte数据
        /// </summary>
        /// <param name="img">图像</param>
        /// <param name="imgFormat">图像格式</param>
        /// <returns></returns>
        private static byte[] GetByteImage(Image img, ImageFormat imgFormat)
        {

            byte[] bt = null;
            if (!img.Equals(null))
            {
                using (MemoryStream mostream = new MemoryStream())
                {
                    img.Save(mostream, imgFormat);//将图像以指定的格式存入缓存内存流
                    bt = new byte[mostream.Length];
                    mostream.Position = 0;//设置留的初始位置
                    mostream.Read(bt, 0, bt.Length);
                }
            }
            return bt;
        }

        /// <summary>
        /// 上传流转为byte[]
        /// </summary>
        /// <param name="img">图像</param>
        /// <param name="imgFormat">图像格式</param>
        /// <returns></returns>
        private static byte[] GetBytePostFile(HttpPostedFileBase postFile)
        {
            // 把 Stream 转换成 byte[] 
            byte[] bytes = new byte[postFile.InputStream.Length];
            using (Stream stream = new MemoryStream(bytes))
            {
                stream.Read(bytes, 0, bytes.Length);
                // 设置当前流的位置为流的开始 
                stream.Seek(0, SeekOrigin.Begin);
            }
            return bytes;
        }
        /// <summary>
        /// 上传流转为byte[]
        /// </summary>
        /// <param name="img">图像</param>
        /// <param name="imgFormat">图像格式</param>
        /// <returns></returns>
        private static byte[] GetBytePostFile(HttpPostedFile postFile)
        {
            // 把 Stream 转换成 byte[] 
            byte[] bytes = new byte[postFile.InputStream.Length];
            using (Stream stream = new MemoryStream(bytes))
            {
                stream.Read(bytes, 0, bytes.Length);
                // 设置当前流的位置为流的开始 
                stream.Seek(0, SeekOrigin.Begin);
            }
            return bytes;
        }
        /// <summary>
        /// 图像转为byte数据
        /// </summary>
        /// <param name="img">图像</param>
        /// <param name="ici">图像的编码、解码器</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
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


        /// <summary>
        /// 获取删除的根目录
        /// </summary>
        /// <returns></returns>
        private static string GetDeleteDirectory(bool isAddImagesFolder=false)
        {
            if (isAddImagesFolder)
            {
                return FileHelper.Root();
            }
            else
            {
                return FileHelper.DirRoot() + "/images/";
            }
        }
        /// <summary>
        /// 获取删除的根目录
        /// </summary>
        /// <param name="ident">FTP唯一标识</param>
        /// <returns></returns>
        private static string GetDeleteDirectory(int ident,string saveDir,string remoteUrl)
        {
            FTPConfigInfo ftpConfig = FTPConfigs.GetFTP(ident);
            if (!string.IsNullOrEmpty(remoteUrl))
            {
                remoteUrl = remoteUrl.Replace(ftpConfig.Remoteurl, "");
            }
            if (FTPs.IsLocalhost(ident))//区分本地或FTP
            {
                return ftpConfig.Uploadpath + saveDir + remoteUrl;
            }
            else
            {
                return ftpConfig.Uploadpath + saveDir + remoteUrl;
            }
        }
        /// <summary>
        /// 获取删除目录
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        private static string GetDeleteDirectory(string groupName,string remoteUrl)
        {
            FastDFSConfigInfo dfsConfig = FastDFSConfigs.GetFastDFS(groupName);
            string dir=string.Empty;
            if (!string.IsNullOrEmpty(remoteUrl))
            {
                //如何remoteurl不换的情况下
                dir = remoteUrl.Replace(dfsConfig.Remoteurl,"").Replace(dfsConfig.Groupname + "/","");
            }
            return dir;
        }
        /// <summary>
        /// 获取文件保存路径
        /// </summary>
        /// <param name="contentLenth">长度</param>
        /// <param name="fileName">要上传的文件名</param>
        /// <param name="upfileEntity">上传的限制类</param>
        /// <param name="attach">附件信息</param>
        /// <returns></returns>
        private string GetSavePath(int contentLenth, string fileName, UpFileEntity upfileEntity, AttachmentInfo attach)
        {
            if (Config.DistributeFileSystem == 1 && upfileEntity.DistributeFileSystem==1)
            {
                return GetDFSPath(upfileEntity,fileName,attach);
            }
            else {
                if (Config.FTPEnable == 1 && upfileEntity.FTPEnable == 1)
                {
                    if (FTPs.IsLocalhost(upfileEntity.FTPIdent))//区分本地或FTP
                    {
                        return GetLocalFtpPath(contentLenth, fileName, upfileEntity, attach);
                    }
                    else
                    {
                        return GetFtpPath(contentLenth, fileName, upfileEntity, attach);
                    }
                }
                else {
                    return GetNormalPath(contentLenth, fileName, upfileEntity, attach);
                }
            }
        }

        /// <summary>
        /// 获取普通存储路径
        /// </summary>
        /// <param name="contentLenth">文件长度</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="upfileEntity">上传限制</param>
        /// <param name="attach">附件信息</param>
        /// <returns></returns>
        private string GetNormalPath(int contentLenth, string fileName, UpFileEntity upfileEntity, AttachmentInfo attach)
        {
            string middlePath = GetAttachmentPathFormat("", "", upfileEntity);
            string sSavePath = FileHelper.DirRoot() + "/images/" + upfileEntity.Dir + middlePath;
            int nFileLen = contentLenth;
            string fileextname = Path.GetExtension(fileName).ToLower();
            string fileInfo = contentLenth.ToString() + "_" + fileextname.Replace(".", "");
            string sFilename = TypeConverter.StrToInt64(DateTime.Now.ToString("hhmmssfff")).ToString("x") + Randomer.Next(1000, 9999) + "-" + fileInfo + fileextname;
            while (File.Exists(sSavePath + sFilename))
            {
                sFilename = TypeConverter.StrToInt64(DateTime.Now.ToString("hhmmssfff")).ToString("x") + Randomer.Next(1000, 9999) + "-" + fileInfo + "_" + fileextname;
            }
            //创建目录
            if (!Directory.Exists(sSavePath))
            {
                Directory.CreateDirectory(sSavePath);
            }
            upfileEntity.GroupName = "none";
            upfileEntity.FTPIdent = -1;

            attach.FTPEnable = upfileEntity.FTPEnable;
            attach.DistributeFileSystem = upfileEntity.DistributeFileSystem;
            attach.FileName = upfileEntity.Dir + middlePath + sFilename;

            upfileEntity.Mast_File_Name = sSavePath + sFilename;//设置缩略图保存地址
            return sSavePath + sFilename;
        }
        /// <summary>
        /// 使用FTP配置-本地磁盘
        /// </summary>
        /// <param name="contentLenth">文件大小</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="upfileEntity">上传限制</param>
        /// <param name="attach">附件信息</param>
        /// <returns></returns>
        private string GetLocalFtpPath(int contentLenth, string fileName, UpFileEntity upfileEntity, AttachmentInfo attach)
        {
            FTPConfigInfo ftpConfig = FTPConfigs.GetFTP(upfileEntity.FTPIdent);
            string middlePath = GetFtpPathFormat("", "", ftpConfig.PathFormat,upfileEntity);//从配置中读取
            string sSavePath = ftpConfig.Uploadpath + upfileEntity.Dir + middlePath;//使用绝对路径-ftpConfig.Uploadpath根目录
            int nFileLen = contentLenth;
            string fileextname = Path.GetExtension(fileName).ToLower();
            string fileInfo = contentLenth.ToString() + "_" + fileextname.Replace(".", "");
            string sFilename = TypeConverter.StrToInt64(DateTime.Now.ToString("hhmmssfff")).ToString("x") + Randomer.Next(1000, 9999) + "-" + fileInfo + "=" + ftpConfig.Ident.ToString() + fileextname;
            while (File.Exists(sSavePath + sFilename))
            {
                sFilename = TypeConverter.StrToInt64(DateTime.Now.ToString("hhmmssfff")).ToString("x") + Randomer.Next(1000, 9999) + "-" + fileInfo + "=" + ftpConfig.Ident.ToString() + "_" + fileextname;
            }
            //创建目录
            if (!Directory.Exists(sSavePath))
            {
                Directory.CreateDirectory(sSavePath);
            }
            upfileEntity.GroupName = "none";
            upfileEntity.FTPIdent = ftpConfig.Ident;

            attach.FTPEnable = upfileEntity.FTPEnable;
            attach.DistributeFileSystem = upfileEntity.DistributeFileSystem;

            attach.FileName = upfileEntity.Dir + middlePath + sFilename;
            attach.FtpIdent = ftpConfig.Ident;
            attach.RemoteUrl = ftpConfig.Remoteurl;
            attach.RemoteUrl2 = ftpConfig.Remoteurl2;

            upfileEntity.Mast_File_Name = sSavePath + sFilename;//设置缩略图保存地址
            return sSavePath + sFilename;
        }
        /// <summary>
        /// 获取FTP的保存路径
        /// </summary>
        /// <param name="contentLenth">长度</param>
        /// <param name="fileName">要上传的文件名</param>
        /// <param name="upfileEntity">上传的限制</param>
        /// <param name="attach">附件信息</param>
        /// <returns></returns>
        private string GetFtpPath(int contentLenth, string fileName, UpFileEntity upfileEntity, AttachmentInfo attach)
        {
            FTPConfigInfo ftpConfig=FTPConfigs.GetFTP(upfileEntity.FTPIdent);
            string middlePath = GetFtpPathFormat("", "", ftpConfig.PathFormat,upfileEntity);//从配置中读取
            string sSavePath = ftpConfig.Uploadpath + upfileEntity.Dir + middlePath;//使用绝对路径-ftpConfig.Uploadpath根目录
            int nFileLen = contentLenth;
            string fileextname = Path.GetExtension(fileName).ToLower();
            string fileInfo = contentLenth.ToString() + "_" + fileextname.Replace(".", "");
            string sFilename = TypeConverter.StrToInt64(DateTime.Now.ToString("hhmmssfff")).ToString("x") + Randomer.Next(1000, 9999) + "-" + fileInfo + "=" + ftpConfig.Ident.ToString() + fileextname;


            upfileEntity.GroupName = "none";
            upfileEntity.FTPIdent = ftpConfig.Ident;

            attach.FTPEnable = upfileEntity.FTPEnable;
            attach.DistributeFileSystem = upfileEntity.DistributeFileSystem;

            attach.FtpIdent = ftpConfig.Ident;
            attach.FileName = upfileEntity.Dir + middlePath+sFilename;
            attach.RemoteUrl = ftpConfig.Remoteurl;
            attach.RemoteUrl2 = ftpConfig.Remoteurl2;

            upfileEntity.Mast_File_Name = sSavePath + sFilename;//设置缩略图保存地址
            return sSavePath + sFilename;
        }
        /// <summary>
        /// 获取分布式文件保存路径
        /// </summary>
        /// <param name="upfileEntity">上传的限制</param>
        /// <param name="attach">附件信息</param>
        /// <returns></returns>
        private string GetDFSPath(UpFileEntity upfileEntity, string fileName, AttachmentInfo attach)
        {
            FastDFSConfigInfo dfsConfig = FastDFSConfigs.GetFastDFS(upfileEntity.GroupName);

            upfileEntity.GroupName = dfsConfig.Groupname;
            upfileEntity.FTPIdent =-1;

            attach.GroupName = dfsConfig.Groupname;
            attach.FtpIdent = dfsConfig.BakFtpIdent;
            attach.RemoteUrl = dfsConfig.Remoteurl + dfsConfig.Groupname+"/";
            attach.RemoteUrl2 = dfsConfig.Remoteurl2 + dfsConfig.Groupname + "/";

            return fileName;
        }
        /// <summary>
        /// 重新设置远程地址
        /// </summary>
        /// <param name="attach"></param>
        private void SetRemoteFileName(AttachmentInfo attach)
        {
            attach.RemoteUrl += attach.FileName;
            attach.RemoteUrl2 += attach.FileName;
        }
        /// <summary>
        /// 重新设置
        /// </summary>
        /// <param name="upfileEntity"></param>
        /// <param name="saveFullPath"></param>
        private void SetMasterFileName(UpFileEntity upfileEntity, string saveFullPath)
        {
            upfileEntity.Mast_File_Name = saveFullPath;
        }
        #endregion

        #region 获取文件
        /// <summary>
        /// 获取文件
        /// </summary>
        /// <param name="path">远程地址</param>
        /// <param name="ftpIdent">FTP唯一标识</param>
        /// <param name="groupName">组名</param>
        /// <returns>文件字节</returns>
        public byte[] GetFile(string path, int ftpIdent, string groupName)
        {
            if (ftpIdent == -1 && groupName == "none")
            {

            }
            else if (ftpIdent != -1)
            {

            }
            else
            {

            }
            return null;
        }
        #endregion

        #region 拷贝文件

        #region 服务器与远程存储端下载
        /// <summary>
        /// 文件下载
        /// </summary>
        /// <returns></returns>
        public string DownLoadFile(OperateParam param)
        {
            if (param.Source_FtpIdent == -1 && param.Source_GroupName == "none")
            {
                return FileHelper.Root() + Config.WebImagesUrl + (param.Dir??"")+ param.Source;
            }
            else if (param.Source_FtpIdent != -1)
            {
                if (FTPs.IsLocalhost(param.Source_FtpIdent))
                {
                    FTPConfigInfo ftpconfig = FTPConfigs.GetFTP(param.Source_FtpIdent);
                    return ftpconfig.Uploadpath + param.Source;
                }
                else
                {
                    return DownLoadFile(param.FTP, param);
                }
            }
            else
            {
                return DownLoadDFSFile(param.DFS, param);
            }
        }
        /// <summary>
        /// 下载文件并获取文件所在本地目录
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public string DownLoadFiles(OperateParam param, bool isConnection = true)
        {
            if (param.Source_FtpIdent == -1 && param.Source_GroupName == "none")
            {
                return FileHelper.Root() + Config.WebImagesUrl + param.Source;
            }
            else if (param.Source_FtpIdent != -1)
            {
                if (FTPs.IsLocalhost(param.Source_FtpIdent))
                {
                    FTPConfigInfo ftpconfig = FTPConfigs.GetFTP(param.Source_FtpIdent);
                    return ftpconfig.Uploadpath + param.Source;
                }
                else
                {
                    return DownLoadFiles(param.FTP, param, "", isConnection);
                }
            }
            else
            {
                return DownLoadDFSFile(param.DFS, param);
            }
        }
        #region FTP
        private string DownLoadFile(IFTP ftp, OperateParam param,bool isConnection=true)
        {
            if (ftp != null)
            {

            }
            else
            {
                ftp = FTPs.Instrance(param.Source_FtpIdent);
            }
            if (string.IsNullOrEmpty(param.Target))
            {
                param.Target = GetOperateSavePath(param.ContentLength, param.Source, param);
            }
            
            FTPConfigInfo ftpconfig = (FTPConfigInfo)ftp.FtpConfig;
            string sourceDir = Path.GetDirectoryName(param.Source) + "/";
            if(sourceDir.Replace("\\","/").IndexOf(ftpconfig.Uploadpath)<0)
            {
                sourceDir = ftpconfig.Uploadpath + sourceDir;
            }
            string targetDir = Path.GetDirectoryName(param.Target)+"/";
            string tempPath = Config.FTPTempPath + targetDir;
            string dir = Path.GetDirectoryName(tempPath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            ftp.Connect();

            //切换到指定路径下,如果目录不存在,将创建
            if (!ftp.ChangeDir(sourceDir))
            {
                string[] pathArr = sourceDir.Replace("\\","/").Split('/');
                foreach (string pathstr in pathArr)
                {
                    if (pathstr.Trim() != "")
                    {
                        ftp.MakeDir(pathstr);
                        ftp.ChangeDir(pathstr);
                    }
                }
            }
            if (!ftp.IsConnected)
                return "";
            //绑定要下载的文件
            ftp.OpenDownload(Path.GetFileName(param.Source), tempPath + Path.GetFileName(param.Source));
            //开始下载
            while (ftp.DoDownload() > 0) { }

            if (isConnection)
            {
                ftp.Disconnect();
            }
            return tempPath + Path.GetFileName(param.Source);
        }

        private string DownLoadFiles(IFTP ftp, OperateParam param,string root="",bool isDisconnect=true)
        {
            string sourceDir = Path.GetDirectoryName(param.Source).Replace(param.ReplaceDir, "");
            string targetDir = Path.GetDirectoryName(param.Target).Replace(param.ReplaceDir, "");
            if (ftp != null)
            {
            }
            else
            {
                ftp = FTPs.Instrance(param.Source_FtpIdent);
            }
            FTPConfigInfo ftpconfig = (FTPConfigInfo)ftp.FtpConfig;
            string tempPath = Config.FTPTempPath + targetDir;
            if (string.IsNullOrEmpty(root))
            {
                root = tempPath;
            }
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }


            ftp.Connect();
            //切换到指定路径下,如果目录不存在,将创建
            if (!ftp.ChangeDir(tempPath))//递归的情况需修改
            {
                string[] pathArr = targetDir.Split('/');
                foreach (string pathstr in pathArr)
                {
                    if (pathstr.Trim() != "")
                    {
                        ftp.MakeDir(pathstr);
                        ftp.ChangeDir(pathstr);
                    }
                }
            }

            ArrayList files = ftp.ListFiles();
            ArrayList directories = ftp.ListDirectories();
            for (int i = 0; i < files.Count; i++)
            {
                if (!ftp.IsConnected)
                    return "";
                //绑定要下载的文件
                ftp.OpenDownload(files[i].ToString(), tempPath +"/"+ Path.GetFileName(files[i].ToString()));
                //开始下载
                while (ftp.DoDownload() > 0) { }
            }
            for (int i = 0; i < directories.Count; i++)
            {
                param.Source += "/" + directories[i].ToString();
                param.Target += "/" + directories[i].ToString();
                ftp.ChangeDir(directories[i].ToString());//切换路径
                DownLoadFiles(ftp, param, root,isDisconnect);
            }
            if (isDisconnect)
            {
                ftp.Disconnect();
            }
            return root;
        }
        #endregion

        #region 分布式文件系统
        private string DownLoadDFSFile(IDFS dfs, OperateParam param)
        {
            if (dfs != null)
            { }
            else
            {
                dfs = DFSProvider.Instrance(param.Source_GroupName);
            }
            FastDFSConfigInfo dfsconfig = (FastDFSConfigInfo)dfs.DfsConfig;
            string tempPath = Config.FTPTempPath + param.Target;
            string dir = Path.GetDirectoryName(tempPath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            byte[] bytes = dfs.DownloadFile(param.Source.Replace(dfsconfig.Remoteurl, ""));
            using (FileStream fileStream = new FileStream(tempPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                fileStream.Write(bytes, 0, bytes.Length);
            }
            return tempPath;
        }
        private byte[] DownLoadDFSFile(IDFS dfs, OperateParam param,bool isByte)
        {
            if (dfs != null)
            { }
            else
            {
                dfs = DFSProvider.Instrance(param.Source_GroupName);
            }
            FastDFSConfigInfo dfsconfig = (FastDFSConfigInfo)dfs.DfsConfig;
            return dfs.DownloadFile(param.Source.Replace(dfsconfig.Remoteurl, ""));
        }
        #endregion

        #endregion

        #region 服务器与远程存储端上传
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="targetPath"></param>
        /// <param name="ftpIdent"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public string UploadFile(OperateParam param)
        {
            if (param.FtpIdent == -1 && param.GroupName == "none")
            {
                return "";
            }
            else if (param.FtpIdent != -1)
            {
                return UploadFile(null, param);
            }
            else
            {
                return UploadDFSFile(null, param);
            }
        }
        #region FTP文件上传
        private string UploadFile(IFTP ftp, OperateParam param, bool isDisconnect = true)
        {
            if (ftp != null)
            {
            }
            else
            {
                ftp = FTPs.Instrance(param.FtpIdent);
            }
            FTPConfigInfo ftpconfig = (FTPConfigInfo)ftp.FtpConfig;
            #region 上传
            if (string.IsNullOrEmpty(param.Target))
            {
                param.Target = GetOperateSavePath(param.ContentLength, param.Source, param);
            }
            string targetPath = Path.GetDirectoryName(param.Target);
            //转换路径分割符为"/"
            targetPath = targetPath.Replace("\\", "/");
            targetPath = targetPath.StartsWith("/") ? targetPath : "/" + targetPath;

            //删除file参数文件
            bool delfile = true;
            //path = ftpConfig.Uploadpath + path;
            delfile = (ftpconfig.Reservelocalattach == 1) ? false : true;
            ftp.Connect();
            //切换到指定路径下,如果目录不存在,将创建
            if (param.IsChangeDir)
            {
                if (!ftp.ChangeDir(targetPath))
                {
                    string[] pathArr = targetPath.Split('/');
                    foreach (string pathstr in pathArr)
                    {
                        if (pathstr.Trim() != "")
                        {
                            ftp.MakeDir(pathstr);
                            ftp.ChangeDir(pathstr);
                        }
                    }
                }
            }

            if (!ftp.IsConnected)
                return "";

            //绑定要上传的文件
            string saveName = System.IO.Path.GetFileName(param.Source);
            if (param.IsSaveTarget)
            {
                saveName = System.IO.Path.GetFileName(param.Target);
            }
            if (!ftp.OpenUpload(param.Source, saveName))
            {
                //ftpupload.Disconnect();
                return "";
            }

            //开始进行上传
            while (ftp.DoUpload() > 0) { }

            if (delfile&&param.Source_FtpIdent!=-1)
            {
                File.Delete(param.Source);
            }
            #endregion
            if (isDisconnect)
            {
                ftp.Disconnect();
            }
            if (param.IsSaveTarget)
            {
                return ftpconfig.Remoteurl + targetPath.TrimStart('/').Replace(ftpconfig.Uploadpath, "") + "/" + System.IO.Path.GetFileName(param.Target);
            }
            else
            {
                return ftpconfig.Remoteurl + targetPath.TrimStart('/').Replace(ftpconfig.Uploadpath, "") + "/" + System.IO.Path.GetFileName(param.Source);
            }
        }
        //批量
        private string UploadFiles(IFTP ftp,OperateParam param, bool isDisconnect = true,string currentDir="")
        {
            if (ftp != null)
            {
            }
            else
            {
                ftp = FTPs.Instrance(param.FtpIdent);
            }
            FTPConfigInfo ftpconfig = (FTPConfigInfo)ftp.FtpConfig;
            #region 上传
            string targetPath = Path.GetDirectoryName(param.Target);
            //转换路径分割符为"/"
            targetPath = targetPath.Replace("\\", "/");
            targetPath = targetPath.StartsWith("/") ? targetPath : "/" + targetPath;
            targetPath += "/"+currentDir;//递归目录
            ftp.Connect();
            //切换到指定路径下,如果目录不存在,将创建

            if (!ftp.ChangeDir(targetPath))
            {
                string[] pathArr = targetPath.Split('/');
                foreach (string pathstr in pathArr)
                {
                    if (pathstr.Trim() != "")
                    {
                        ftp.MakeDir(pathstr);
                        ftp.ChangeDir(pathstr);
                    }
                }
            }
            //删除file参数文件
            bool delfile = true;
            //path = ftpConfig.Uploadpath + path;
            delfile = (ftpconfig.Reservelocalattach == 1) ? false : true;
            

            if (!ftp.IsConnected)
                return "";

            string[] files = FileHelper.GetFiles(param.Source + currentDir, false);
            string[] dirs = FileHelper.GetDirs(param.Source + currentDir);
            for (int i = 0; i < files.Length; i++)
            {
                string remoteFileName=Path.GetFileName(files[i]);
                //替换扩展名
                if (!string.IsNullOrEmpty(param.NeedChangeExtensions)&&Path.GetExtension(remoteFileName)!=Path.GetExtension(param.MasterName))
                {
                    if (param.NeedChangeExtensions.IndexOf(remoteFileName) >= 0)
                    {
                        remoteFileName = remoteFileName.Replace(Path.GetExtension(remoteFileName), Path.GetExtension(param.MasterName));
                    }
                }
                //绑定要上传的文件
                if (!ftp.OpenUpload(files[i], remoteFileName))
                {
                    //ftpupload.Disconnect();
                    return "";
                }
                //开始进行上传
                while (ftp.DoUpload() > 0) { }
                if (Path.GetFileName(files[i]) == param.MasterName)
                {
                    param.ResultPath = param.ResultPath + currentDir + "/" + Path.GetFileName(files[i]);
                }
            }
            if (isDisconnect)
            {
                ftp.Disconnect();
            }
            for (int i = 0; i < dirs.Length; i++)
            {
                string[] dirarr = dirs[i].Replace("\\", "").Split('/');
                UploadFiles(ftp, param, true, currentDir + dirarr[dirarr.Length - 1]);
            }

            if (delfile && param.Source_FtpIdent != -1)
            {
                FileHelper.DeleteDirectory(param.Source);
            }
            #endregion
            return ftpconfig.Remoteurl + param.ResultPath;
        }
        #endregion

        #region 分布式文件系统上传
        private string UploadDFSFile(IDFS dfs, OperateParam param)
        {
            if (dfs != null)
            { }
            else
            {
                dfs = DFSProvider.Instrance(param.GroupName);
            }
            FastDFSConfigInfo dfsconfig = (FastDFSConfigInfo)dfs.DfsConfig;
            byte[] bytes = null;
            using (FileStream filestream = new FileStream(param.Source, FileMode.Open, FileAccess.ReadWrite))
            {
                bytes = new byte[filestream.Length];
                filestream.Read(bytes, 0, bytes.Length);
            }
            string fileName = dfs.UploadFile(bytes, Path.GetExtension(param.Target).Replace(".", ""));

            return dfsconfig.Remoteurl + fileName;
        }

        private string UploadDFSFile(IDFS dfs, byte[] bytes, OperateParam param, bool isSlave=false)
        {
            if (dfs != null)
            { }
            else
            {
                dfs = DFSProvider.Instrance(param.GroupName);
            }
            FastDFSConfigInfo dfsconfig = (FastDFSConfigInfo)dfs.DfsConfig;
            string fileName = null;
            if (isSlave)
            {
                fileName = dfs.UploadSlaveFile(bytes,param.MastFilePath,param.SlaveNames[0], Path.GetExtension(param.Target).Replace(".", ""));
            }
            else
            {
                fileName=dfs.UploadFile(bytes, Path.GetExtension(param.Target).Replace(".", ""));
            }
            return dfsconfig.Remoteurl + fileName;
        }
        /// <summary>
        /// 批量
        /// </summary>
        /// <param name="dfs">文件系统提供者</param>
        /// <param name="sourcePath">源路径</param>
        /// <param name="targetPath">目标路径</param>
        /// <param name="groupName">组名</param>
        /// <param name="mastFileName">主文件</param>
        /// <param name="slavePrefix">从文件前缀</param>
        /// <param name="slaveNames">从文件名称或主文件名数据</param>
        /// <returns></returns>
        private string UploadDFSFiles(IDFS dfs, OperateParam param)
        {
            if (dfs != null)
            { }
            else
            {
                dfs = DFSProvider.Instrance(param.GroupName);
            }
            FastDFSConfigInfo dfsconfig = (FastDFSConfigInfo)dfs.DfsConfig;
            string path = param.Source.Replace(dfsconfig.Remoteurl, "");
            string filename=Path.GetFileName(path);
            string filext=Path.GetExtension(path);
            string fileName = ""; ;
            for (int i = 0; i < param.SlaveNames.Length; i++)
            {
                if (!string.IsNullOrEmpty(param.MastFilePath))
                {
                    byte[] bytes = dfs.DownloadFile(path + filename.Replace(filext, param.SlaveNames[i] + filext));
                    fileName = dfs.UploadSlaveFile(bytes, param.MastFilePath, Path.GetFileName(param.SlaveNames[i]), Path.GetExtension(param.SlaveNames[i]).Replace(".", ""));
                }
                else
                {
                    byte[] bytes = dfs.DownloadFile(path + param.SlaveNames[i]);
                    fileName = dfs.UploadFile(bytes, Path.GetExtension(param.Target).Replace(".", ""));
                }
            }
            return dfsconfig.Remoteurl + fileName;
        }

        #endregion

        #endregion

        /// <summary>
        /// 拷贝文件
        /// </summary>
        /// <param name="sourcePath">源文件地址</param>
        /// <param name="targetPath">目标文件</param>
        /// <param name="ftpIdent">FTP唯一标识</param>
        /// <param name="groupName">组名</param>
        public string CopyFile(OperateParam param)
        {
            if (param.FtpIdent == -1 && param.GroupName == "none")
            {
                string source=DownLoadFile(param);
                FileHelper.CopyFile(source, FileHelper.DirRoot() + "/images/" + param.Target, true);
                return Config.CdnImagesUrl + param.Target;
            }
            else if (param.FtpIdent != -1)
            {
                IFTP ftp = FTPs.Instrance(param.FtpIdent);
                FTPConfigInfo ftpConfig = (FTPConfigInfo)ftp.FtpConfig;
                param.FTP = ftp;
                if (FTPs.IsLocalhost(param.FtpIdent))
                {
                    string source=DownLoadFile(param);
                    FileHelper.CopyFile(source, ftpConfig.Uploadpath + param.Target, true);
                    return ftpConfig.Remoteurl + param.Target;
                }
                else {
                    param.Source =  param.Source.Replace(ftpConfig.Remoteurl, "");
                    if (!string.IsNullOrEmpty(param.Target))
                    {
                        param.Target = ftpConfig.Uploadpath + param.Target.Replace(ftpConfig.Remoteurl, "");
                    }
                    string tempPath = DownLoadFile(param);
                    param.Source = tempPath;//设置源
                    string filename = UploadFile(ftp, param);
                    return filename;
                }
            }
            else
            {
                IDFS dfs = DFSProvider.Instrance(param.GroupName);
                byte[] bytes = DownLoadDFSFile(dfs, param,true);
                string filename = UploadDFSFile(dfs, bytes, param);
                return filename;
            }
        }

        /// <summary>
        /// 拷贝文件
        /// </summary>
        /// <param name="sourcePaths">源文件列表</param>
        /// <param name="targetPaths">目标文件列表</param>
        /// <param name="ftpIdent">FTP唯一标识</param>
        /// <param name="groupName">组名</param>
        public string[] CopyFiles(string[] sourcePaths, string[] targetPaths, OperateParam param)
        {
            string[] filenames = new string[sourcePaths.Length];
            if (param.FtpIdent == -1 && param.GroupName == "none")
            {
                for (int i = 0; i < sourcePaths.Length; i++)
                {
                    param.Source = sourcePaths[i];
                    param.Target = targetPaths[i];
                    string tempPath = DownLoadFile(param);
                    FileHelper.CopyFile(tempPath, FileHelper.DirRoot() + "/images/" + targetPaths[i], true);
                }
                return null;
            }
            else if (param.FtpIdent != -1)
            {
                IFTP ftp = FTPs.Instrance(param.FtpIdent);
                FTPConfigInfo ftpConfig = (FTPConfigInfo)ftp.FtpConfig;
                param.FTP = ftp;
                param.Target = param.Target.Replace(ftpConfig.Remoteurl, "");
                param.Source = param.Source.Replace(ftpConfig.Remoteurl, "");
                if (FTPs.IsLocalhost(param.FtpIdent))
                {
                    for (int i = 0; i < sourcePaths.Length; i++)
                    {
                        param.Source = sourcePaths[i];
                        param.Target = targetPaths[i];
                        string tempPath = DownLoadFile(param);
                        FileHelper.CopyFile(tempPath, ftpConfig.Uploadpath + targetPaths[i], true);
                    }
                    return null;
                }
                else
                {
                    for (int i = 0; i < sourcePaths.Length; i++)
                    {
                        param.Source = ftpConfig.Uploadpath + sourcePaths[i].Replace(ftpConfig.Remoteurl, "");
                        param.Target = ftpConfig.Uploadpath + targetPaths[i].Replace(ftpConfig.Remoteurl, "");
                        string tempPath = DownLoadFile(param);
                        param.Source = tempPath;//设置源
                        filenames[i] = UploadFile(ftp, param);
                        ftp.Disconnect();
                    }
                    return filenames;
                }
            }
            else
            {
                IDFS dfs = DFSProvider.Instrance(param.GroupName);
                for (int i = 0; i < sourcePaths.Length; i++)
                {
                    param.Source = sourcePaths[i];
                    param.Target = targetPaths[i];
                    byte[] bytes = DownLoadDFSFile(dfs, param, true);
                    filenames[i] = UploadDFSFile(dfs, bytes, param);
                }
                return filenames;
            }
        }

        /// <summary>
        /// 拷贝文件
        /// </summary>
        /// <param name="sourceDir">源文件目录</param>
        /// <param name="targetDir">目标文件目录</param>
        /// <param name="ftpIdent">FTP唯一标识</param>
        /// <param name="groupName">组名</param>
        public string CopyFiles(OperateParam param)
        {
            if (param.FtpIdent == -1 && param.GroupName == "none")
            {
                string sourceDir = Path.GetDirectoryName(DownLoadFiles(param));
                string targetDir = Path.GetDirectoryName(param.Target);
                return FileHelper.CopyFiles(sourceDir, FileHelper.Root() + Config.WebImagesUrl + targetDir,true, true, param.MasterName);
            }
            else if (param.FtpIdent != -1)
            {
                IFTP ftp = FTPs.Instrance(param.FtpIdent);
                FTPConfigInfo ftpconfig=(FTPConfigInfo)ftp.FtpConfig;
                param.FTP = ftp;
                if (FTPs.IsLocalhost(param.FtpIdent))
                {
                    string sourceDir = Path.GetDirectoryName(DownLoadFiles(param));
                    string targetDir = Path.GetDirectoryName(param.Target);
                    return FileHelper.CopyFiles(sourceDir, FileHelper.Root() + Config.WebImagesUrl + targetDir, true, true, param.MasterName);
                }
                else
                {
                    param.ReplaceDir = "images/";
                    //param.Source = FileHelper.Root() + config.WebImagesUrl + param.Source;
                    string tempPath = DownLoadFiles(param, false);
                    param.Source = tempPath;//设置源
                    param.Target = ftpconfig.Uploadpath + param.Target;
                    return UploadFiles(ftp, param, true);
                    
                }
            }
            else
            {
                IDFS dfs = DFSProvider.Instrance(param.GroupName);
                if (string.IsNullOrEmpty(param.MastFilePath))
                {
                    //上传主文件
                    byte[] bytes = DownLoadDFSFile(dfs, param,true);
                    param.MastFilePath = UploadDFSFile(dfs, bytes, param);//设置主文件
                }
                return UploadDFSFiles(dfs, param);//上传所有从文件
            }
        }


        #endregion

        #region 切图
        /// <summary>
        /// 切图
        /// </summary>
        /// <param name="param">操作参数类</param>
        /// <returns></returns>
        public string Segmentation(SegmentationParam param)
        {
            if (param.FtpIdent == -1 && param.GroupName == "none")
            {
                string sourceDir = Path.GetDirectoryName(param.Source);
                string targetDir = Path.GetDirectoryName(param.Target);
                Bitmap oriImg = new Bitmap(FileHelper.Root() + param.config.WebImagesUrl + param.Source);

                string targetName = Path.GetFileName(param.Target);
                string extention=Path.GetExtension(param.Target);
                if (param.IsReset == 1)
                {
                    oriImg = Ocean.Core.Common.ImageMaker.Thumbnail.Thumbnail_Image(oriImg, new Size(param.ResetWidth, param.ResetHeight));
                    oriImg.Save(FileHelper.Root() + param.config.WebImagesUrl + targetDir +"\\"+ param.ResetName + ".jpg");
                }
                for (int i = 0; i < param.CutNames.Length; i++)
                {
                    Bitmap bmpTop = Ocean.Core.Common.ImageMaker.Segmentation.Segmentation_Image(oriImg, new Point(param.Point[i][0], param.Point[i][1]), new Size(param.Size[i][0], param.Size[i][1]));
                    bmpTop.Save(FileHelper.Root() + param.config.WebImagesUrl + targetDir + "\\" + param.CutNames[i] + ".jpg");
                    bmpTop.Dispose();
                }
                oriImg.Dispose();
                return null;
            }
            else if (param.FtpIdent != -1)
            {
                IFTP ftp = FTPs.Instrance(param.FtpIdent);
                FTPConfigInfo ftpConfig=(FTPConfigInfo)ftp.FtpConfig;
                if (FTPs.IsLocalhost(param.FtpIdent))
                {
                    string sourceDir = Path.GetDirectoryName(param.Source);
                    string targetDir = Path.GetDirectoryName(param.Target);
                    Bitmap oriImg = new Bitmap(FileHelper.Root() + param.config.WebImagesUrl + sourceDir);

                    string targetName = Path.GetFileName(param.Target);
                    string extention = Path.GetExtension(param.Target);
                    if (param.IsReset == 1)
                    {
                        oriImg = Ocean.Core.Common.ImageMaker.Thumbnail.Thumbnail_Image(oriImg, new Size(param.ResetWidth, param.ResetHeight));
                        oriImg.Save(targetDir + targetName.Replace(extention, "+" + param.ResetName + extention));
                    }
                    for (int i = 0; i < param.CutNames.Length; i++)
                    {
                        Bitmap bmpTop = Ocean.Core.Common.ImageMaker.Segmentation.Segmentation_Image(oriImg, new Point(param.Point[i][0], param.Point[i][1]), new Size(param.Size[i][0], param.Size[i][1]));
                        bmpTop.Save(targetDir + targetName.Replace(extention, "+" + param.CutNames[i] + extention));
                        bmpTop.Dispose();
                    }
                    oriImg.Dispose();
                    return ftpConfig+targetDir+"/"+targetName+extention;
                }
                else {
                    param.Source = ftpConfig.Uploadpath + param.Source.Replace(ftpConfig.Remoteurl, "");
                    param.Target = ftpConfig.Uploadpath + param.Target.Replace(ftpConfig.Remoteurl, "");
                    string sourceDir = Path.GetDirectoryName(param.Source);
                    string targetDir = Path.GetDirectoryName(param.Target);
                    //下载图片
                    string imgPath = DownLoadFile(ftp, param);

                    Bitmap oriImg = new Bitmap(imgPath);
                    string targetName = Path.GetFileName(param.Target);
                    string extention = Path.GetExtension(param.Target);

                    string tempPath = Config.FTPTempPath + targetDir + "/";

                    
                    if (param.IsReset == 1)
                    {
                        oriImg = Ocean.Core.Common.ImageMaker.Thumbnail.Thumbnail_Image(oriImg, new Size(param.ResetWidth, param.ResetHeight));
                        string tempfileName = tempPath +param.ResetName + extention;
                        oriImg.Save(tempfileName);
                        param.Source = tempfileName;
                        UploadFile(ftp, param, false);
                        param.IsChangeDir = false;
                    }
                    for (int i = 0; i < param.CutNames.Length; i++)
                    {
                        Bitmap bmpTop = Ocean.Core.Common.ImageMaker.Segmentation.Segmentation_Image(oriImg, new Point(param.Point[i][0], param.Point[i][1]), new Size(param.Size[i][0], param.Size[i][1]));
                        string tempfileName = tempPath + param.CutNames[i] + extention;
                        bmpTop.Save(tempfileName);
                        bmpTop.Dispose();
                        param.Source = tempfileName;
                        UploadFile(ftp, param, false);
                        param.IsChangeDir = false;
                    }
                    oriImg.Dispose();

                    ftp.Disconnect();
                    return ftpConfig + targetDir + "/" + targetName + extention;
                }
            }
            else
            {
                IDFS dfs = DFSProvider.Instrance(param.GroupName);

                string extention = Path.GetExtension(param.Source);
                //下载图片
                byte[] imgBytes = DownLoadDFSFile(dfs, param, true);
                Bitmap oriImg;
                using (Stream stream = new MemoryStream(imgBytes))
                {
                    oriImg = new Bitmap(stream);
                }

                if (param.IsReset == 1)
                {
                    oriImg = Ocean.Core.Common.ImageMaker.Thumbnail.Thumbnail_Image(oriImg, new Size(param.ResetWidth, param.ResetHeight));
                    byte[] newImgBytes = GetByteImage(oriImg, GetImgFormat(extention));
                    UploadDFSFile(dfs, newImgBytes, param);
                }
                for (int i = 0; i < param.CutNames.Length; i++)
                {
                    Bitmap bmpTop = Ocean.Core.Common.ImageMaker.Segmentation.Segmentation_Image(oriImg, new Point(param.Point[i][0], param.Point[i][1]), new Size(param.Size[i][0], param.Size[i][1]));
                    byte[] newImgBytes = GetByteImage(bmpTop, GetImgFormat(extention));
                    //从文件名
                    param.SlaveNames = new string[] { "+" + param.CutNames[i] };
                    UploadDFSFile(dfs,newImgBytes, param,true);
                }
                return param.MastFilePath;
            }

        }
        #endregion

        #region 文件路径设置
        /// <summary>
        /// 获取文件保存路径
        /// </summary>
        /// <param name="contentLenth">长度</param>
        /// <param name="fileName">要上传的文件名</param>
        /// <param name="param">上传的限制类</param>
        /// <returns></returns>
        private string GetOperateSavePath(int contentLenth, string fileName, OperateParam param)
        {
            if (Config.DistributeFileSystem == 1 && param.GroupName!="none")
            {
                return "";
            }
            else
            {
                if (Config.FTPEnable == 1 && param.FtpIdent != 1)
                {
                    if (FTPs.IsLocalhost(param.FtpIdent))//区分本地或FTP
                    {
                        return GetLocalFtpPath(contentLenth, fileName, param);
                    }
                    else
                    {
                        return GetFtpPath(contentLenth, fileName, param);
                    }
                }
                else
                {
                    return GetNormalPath(contentLenth, fileName, param);
                }
            }
        }
        /// <summary>
        /// 获取普通存储路径
        /// </summary>
        /// <param name="contentLenth">文件长度</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="upfileEntity">上传限制</param>
        /// <param name="attach">附件信息</param>
        /// <returns></returns>
        private string GetNormalPath(int contentLenth, string fileName, OperateParam param)
        {
            string middlePath = GetAttachmentPathFormat("", "", param);
            string sSavePath = FileHelper.Root() + Config.WebImagesUrl + param.Dir + middlePath;
            int nFileLen = contentLenth;
            string fileextname = Path.GetExtension(fileName).ToLower();
            string fileInfo = contentLenth.ToString() + "_" + fileextname.Replace(".", "");
            string sFilename = TypeConverter.StrToInt64(DateTime.Now.ToString("hhmmssfff")).ToString("x") + Randomer.Next(1000, 9999) + "-" + fileInfo + fileextname;
            while (File.Exists(sSavePath + sFilename))
            {
                sFilename = TypeConverter.StrToInt64(DateTime.Now.ToString("hhmmssfff")).ToString("x") + Randomer.Next(1000, 9999) + "-" + fileInfo + "_" + fileextname;
            }
            //创建目录
            if (!Directory.Exists(sSavePath))
            {
                Directory.CreateDirectory(sSavePath);
            }
            return sSavePath + sFilename;
        }
        /// <summary>
        /// 使用FTP配置-本地磁盘
        /// </summary>
        /// <param name="contentLenth">文件大小</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="upfileEntity">上传限制</param>
        /// <param name="attach">附件信息</param>
        /// <returns></returns>
        private string GetLocalFtpPath(int contentLenth, string fileName, OperateParam param)
        {
            FTPConfigInfo ftpConfig = FTPConfigs.GetFTP(param.FtpIdent);
            string middlePath = GetFtpPathFormat("", "", ftpConfig.PathFormat, param);//从配置中读取
            string sSavePath = ftpConfig.Uploadpath + param.Dir + middlePath;//使用绝对路径-ftpConfig.Uploadpath根目录
            int nFileLen = contentLenth;
            string fileextname = Path.GetExtension(fileName).ToLower();
            string fileInfo = contentLenth.ToString() + "_" + fileextname.Replace(".", "");
            string sFilename = TypeConverter.StrToInt64(DateTime.Now.ToString("hhmmssfff")).ToString("x") + Randomer.Next(1000, 9999) + "-" + fileInfo + "=" + ftpConfig.Ident.ToString() + fileextname;
            while (File.Exists(sSavePath + sFilename))
            {
                sFilename = TypeConverter.StrToInt64(DateTime.Now.ToString("hhmmssfff")).ToString("x") + Randomer.Next(1000, 9999) + "-" + fileInfo + "=" + ftpConfig.Ident.ToString() + "_" + fileextname;
            }
            //创建目录
            if (!Directory.Exists(sSavePath))
            {
                Directory.CreateDirectory(sSavePath);
            }
            return sSavePath + sFilename;
        }
        /// <summary>
        /// 获取FTP的保存路径
        /// </summary>
        /// <param name="contentLenth">长度</param>
        /// <param name="fileName">要上传的文件名</param>
        /// <param name="upfileEntity">上传的限制</param>
        /// <param name="attach">附件信息</param>
        /// <returns></returns>
        private string GetFtpPath(int contentLenth, string fileName, OperateParam param)
        {
            FTPConfigInfo ftpConfig = FTPConfigs.GetFTP(param.FtpIdent);
            string middlePath = GetFtpPathFormat("", "", ftpConfig.PathFormat, param);//从配置中读取
            string sSavePath = ftpConfig.Uploadpath + param.Dir + middlePath;//使用绝对路径-ftpConfig.Uploadpath根目录
            int nFileLen = contentLenth;
            string fileextname = Path.GetExtension(fileName).ToLower();
            if (contentLenth == 0)
            {
                if (param.Source.IndexOf("!") > 0)
                {
                    //http://fs1.juxiangke.com/articles/2013/01/12/62b80335886-2578_gif=2!39x40.gif
                    string[] contents = param.Source.Split('_');
                    if (contents[0].IndexOf('-') > 0)
                    {
                        contentLenth = TypeConverter.StrToInt(contents[0].Split('-')[1]);
                    }
                }
            }
            string fileInfo = contentLenth.ToString() + "_" + fileextname.Replace(".", "");
            string sFilename = TypeConverter.StrToInt64(DateTime.Now.ToString("hhmmssfff")).ToString("x") + Randomer.Next(1000, 9999) + "-" + fileInfo + "=" + ftpConfig.Ident.ToString() + fileextname;
            return sSavePath + sFilename;
        }

        /// <summary>
        /// 获得附件存放目录
        /// </summary>
        /// <param name="forumid"></param>
        /// <param name="Config"></param>
        /// <param name="fileExtName"></param>
        /// <returns></returns>
        private string GetAttachmentPathFormat(string path, string fileExtName, OperateParam param)
        {
            StringBuilder saveDir = new StringBuilder("");
            if (string.IsNullOrEmpty(param.PathFormat))
            {
                switch (Config.AttachSave)
                {
                    case 0:
                        {
                            if (path != "")
                            {
                                saveDir.Append(path);
                                saveDir.Append("/");
                                //saveDir.Append(Path.DirectorySeparatorChar);
                            }
                            break;
                        }
                    //附件保存方式 0=按年/月/日存入不同目录 1=按年/月/日/存入不同目录 2=按存入不同目录 3=按文件类型存入不同目录
                    case 1:
                        {
                            saveDir.Append(DateTime.Now.ToString("yyyy"));
                            saveDir.Append("/");
                            saveDir.Append(DateTime.Now.ToString("MM"));
                            saveDir.Append("/");
                            saveDir.Append(DateTime.Now.ToString("dd"));
                            saveDir.Append("/");
                            if (path != "")
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
                saveDir.Append(param.PathFormat);
            }
            return saveDir.ToString();
        }

        /// <summary>
        ///获取FTP存放方式
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileExtName"></param>
        /// <param name="pathFormat"></param>
        /// <returns></returns>
        private string GetFtpPathFormat(string path, string fileExtName, int pathFormat, OperateParam param)
        {
            StringBuilder saveDir = new StringBuilder("");
            //附件保存方式 0=按年/月/日存入不同目录 1=按年/月/日/存入不同目录 2=按存入不同目录 3=按文件类型存入不同目录
            if (string.IsNullOrEmpty(param.PathFormat))
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
                saveDir.Append(param.PathFormat);
            }
            return saveDir.ToString();
        }
        #endregion

        /// <summary>
        /// 待去除
        /// </summary>
        public string PathFormat
        {
            get;
            set;
        }
    }
}
