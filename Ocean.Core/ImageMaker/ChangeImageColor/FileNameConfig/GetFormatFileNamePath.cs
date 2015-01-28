using System.IO;

namespace Ocean.Core.ImageMaker.FileNameConfig
{
    public class GetFormatFileNamePath
    {
        /// <summary>
        /// 获取第一个指定格式的图片的路径
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetFilePath(string folderPath, string fileName)
        {
            string[] fileList = Directory.GetFiles(folderPath, string.Format("*.{0}.png", fileName));
            if (fileList.Length > 0)
            {
                return fileList[0];
            }
            return "";
        }
    }
}
