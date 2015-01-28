using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Data;
using System.Xml.Linq;

namespace Ocean.Core.Utility
{
    public static class FileHelper
    {
        // <summary>
        /// 路径分割符
        /// </summary>
        private const string PATH_SPLIT_CHAR = "\\";
        #region 获得当前绝对路径
        /// <summary>
        /// 获得当前绝对路径
        /// </summary>
        /// <param name="strPath">指定的路径</param>
        /// <returns>绝对路径</returns>
        public static string GetMapPath(string strPath)
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Server.MapPath(strPath);
            }
            else //非web程序引用
            {
                if (strPath.StartsWith("~/"))
                {
                    strPath = strPath.Replace("~/", "");
                }
                return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
            }
        }
        #endregion

        #region 删除文件
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="strPath"></param>
        public static void Delete(string strPath)
        {
            if (FileExists(strPath))
            {
                File.Delete(strPath);
            }
        }
        #endregion

        #region 返回文件是否存在
        /// <summary>
        /// 返回文件是否存在
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>是否存在</returns>
        public static bool FileExists(string filename)
        {
            return File.Exists(filename);
        }
        #endregion

        #region 以指定的ContentType输出指定文件文件
        /// <summary>
        /// 以指定的ContentType输出指定文件文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="fileName">输出的文件名</param>
        /// <param name="fileType">将文件输出时设置的ContentType</param>
        public static void ResponseFile(string filePath, string fileName, string fileType)
        {
            Stream iStream = null;

            // 缓冲区为10k
            byte[] buffer = new Byte[10000];

            // 文件长度
            int length;

            // 需要读的数据长度
            long dataToRead;

            try
            {
                // 打开文件
                iStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                // 需要读的数据长度
                dataToRead = iStream.Length;

                HttpContext.Current.Response.ContentType = fileType;
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName.Trim()).Replace("+", " "));

                while (dataToRead > 0)
                {
                    // 检查客户端是否还处于连接状态
                    if (HttpContext.Current.Response.IsClientConnected)
                    {
                        length = iStream.Read(buffer, 0, 10000);
                        HttpContext.Current.Response.OutputStream.Write(buffer, 0, length);
                        HttpContext.Current.Response.Flush();
                        buffer = new Byte[10000];
                        dataToRead = dataToRead - length;
                    }
                    else
                    {
                        // 如果不再连接则跳出死循环
                        dataToRead = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write("Error : " + ex.Message);
            }
            finally
            {
                if (iStream != null)
                {
                    // 关闭文件
                    iStream.Close();
                }
            }
            HttpContext.Current.Response.End();
        }
        #endregion

        #region 返回根目录
        /// <summary>
        /// 返回根目录
        /// </summary>
        /// <returns></returns>
        public static string Root()
        {
            if (HttpContext.Current == null)
            {
                return "/";
            }

            return HttpContext.Current.Request.ApplicationPath.EndsWith("/") ?
                HttpContext.Current.Request.ApplicationPath : HttpContext.Current.Request.ApplicationPath + "/";
        }
        /// <summary>
        /// 获取路径
        /// </summary>
        public static string DirRoot()
        {
            string appRoot = AppDomain.CurrentDomain.BaseDirectory;

            if (appRoot.EndsWith("\\"))
            {
                appRoot = appRoot.Remove(appRoot.Length - 1);
            }
            return appRoot;
        }
        #endregion

        #region 获取文件内容
        /// <summary>
        /// 获取文件内容
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetContent(string fileName)
        {
            StringBuilder Str = new StringBuilder();

            if (!File.Exists(GetMapPath(string.Format("{0}{1}", Root(), fileName.Trim()))))
                return null;
            StreamReader Reader = new StreamReader(GetMapPath(string.Format("{0}{1}", Root(), fileName.Trim())), Encoding.UTF8);
            Str.Append(Reader.ReadToEnd());
            Reader.Dispose();
            Reader.Close();
            return Str.ToString();
        }
        #endregion

        #region 操作
        /// <summary>
        /// 复制指定目录的所有文件,不包含子目录及子目录中的文件
        /// </summary>
        /// <param name="sourceDir">原始目录</param>
        /// <param name="targetDir">目标目录</param>
        /// <param name="overWrite">如果为true,表示覆盖同名文件,否则不覆盖</param>
        public static void CopyFiles(string sourceDir, string targetDir, bool overWrite)
        {
            CopyFiles(sourceDir, targetDir, overWrite, false);
        }

        /// <summary>
        /// 复制指定目录的所有文件
        /// </summary>
        /// <param name="sourceDir">原始目录</param>
        /// <param name="targetDir">目标目录</param>
        /// <param name="overWrite">如果为true,覆盖同名文件,否则不覆盖</param>
        /// <param name="copySubDir">如果为true,包含目录,否则不包含</param>
        public static void CopyFiles(string sourceDir, string targetDir, bool overWrite, bool copySubDir)
        {
            //创建目录
            CreateDirectory(targetDir);
            //复制当前目录文件
            foreach (string sourceFileName in Directory.GetFiles(sourceDir))
            {
                string targetFileName = Path.Combine(targetDir, sourceFileName.Substring(sourceFileName.LastIndexOf(PATH_SPLIT_CHAR) + 1));

                if (File.Exists(targetFileName))
                {
                    if (overWrite == true)
                    {
                        File.SetAttributes(targetFileName, FileAttributes.Normal);
                        File.Copy(sourceFileName, targetFileName, overWrite);
                    }
                }
                else
                {
                    File.Copy(sourceFileName, targetFileName, overWrite);
                }
            }
            //复制子目录
            if (copySubDir)
            {
                foreach (string sourceSubDir in Directory.GetDirectories(sourceDir))
                {
                    string targetSubDir = Path.Combine(targetDir, sourceSubDir.Substring(sourceSubDir.LastIndexOf(PATH_SPLIT_CHAR) + 1));
                    if (!Directory.Exists(targetSubDir))
                        Directory.CreateDirectory(targetSubDir);
                    CopyFiles(sourceSubDir, targetSubDir, overWrite, true);
                }
            }
        }
        /// <summary>
        /// 复制指定目录的所有文件
        /// </summary>
        /// <param name="sourceDir">原始目录</param>
        /// <param name="targetDir">目标目录</param>
        /// <param name="overWrite">如果为true,覆盖同名文件,否则不覆盖</param>
        /// <param name="copySubDir">如果为true,包含目录,否则不包含</param>
        public static string CopyFiles(string sourceDir, string targetDir, bool overWrite, bool copySubDir, string masterName, string reusltUrl = "")
        {
            //创建目录
            CreateDirectory(targetDir);
            //复制当前目录文件
            foreach (string sourceFileName in Directory.GetFiles(sourceDir))
            {
                string targetFileName = Path.Combine(targetDir, sourceFileName.Substring(sourceFileName.LastIndexOf(PATH_SPLIT_CHAR) + 1));

                if (File.Exists(targetFileName))
                {
                    if (overWrite == true)
                    {
                        File.SetAttributes(targetFileName, FileAttributes.Normal);
                        File.Copy(sourceFileName, targetFileName, overWrite);
                    }
                }
                else
                {
                    File.Copy(sourceFileName, targetFileName, overWrite);
                }
                if (Path.GetFileName(sourceFileName) == masterName)
                {
                    reusltUrl = targetDir + masterName;
                }
            }
            //复制子目录
            if (copySubDir)
            {
                foreach (string sourceSubDir in Directory.GetDirectories(sourceDir))
                {
                    string targetSubDir = Path.Combine(targetDir, sourceSubDir.Substring(sourceSubDir.LastIndexOf(PATH_SPLIT_CHAR) + 1));
                    if (!Directory.Exists(targetSubDir))
                        Directory.CreateDirectory(targetSubDir);
                    CopyFiles(sourceSubDir, targetSubDir, overWrite, true, masterName, reusltUrl);
                }
            }
            return reusltUrl;
        }
        /// <summary>
        /// Copy单个文件
        /// </summary>
        /// <param name="sourceFileName"></param>
        /// <param name="targetFileName"></param>
        /// <param name="overWrite"></param>
        public static void CopyFile(string sourceFileName, string targetFileName, bool overWrite)
        {
            string dir = Path.GetDirectoryName(targetFileName);
            if (!ExitsDirectory(dir))
            {
                CreateDirectory(dir);
            }
            File.Copy(sourceFileName, targetFileName, overWrite);
        }

        /// <summary>
        /// 剪切指定目录的所有文件,不包含子目录
        /// </summary>
        /// <param name="sourceDir">原始目录</param>
        /// <param name="targetDir">目标目录</param>
        /// <param name="overWrite">如果为true,覆盖同名文件,否则不覆盖</param>
        public static void MoveFiles(string sourceDir, string targetDir, bool overWrite)
        {
            MoveFiles(sourceDir, targetDir, overWrite, false);
        }

        /// <summary>
        /// 剪切指定目录的所有文件
        /// </summary>
        /// <param name="sourceDir">原始目录</param>
        /// <param name="targetDir">目标目录</param>
        /// <param name="overWrite">如果为true,覆盖同名文件,否则不覆盖</param>
        /// <param name="moveSubDir">如果为true,包含目录,否则不包含</param>
        public static void MoveFiles(string sourceDir, string targetDir, bool overWrite, bool moveSubDir)
        {
            //移动当前目录文件
            foreach (string sourceFileName in Directory.GetFiles(sourceDir))
            {
                string targetFileName = Path.Combine(targetDir, sourceFileName.Substring(sourceFileName.LastIndexOf(PATH_SPLIT_CHAR) + 1));
                if (File.Exists(targetFileName))
                {
                    if (overWrite == true)
                    {
                        File.SetAttributes(targetFileName, FileAttributes.Normal);
                        File.Delete(targetFileName);
                        File.Move(sourceFileName, targetFileName);
                    }
                }
                else
                {
                    File.Move(sourceFileName, targetFileName);
                }
            }
            if (moveSubDir)
            {
                foreach (string sourceSubDir in Directory.GetDirectories(sourceDir))
                {
                    string targetSubDir = Path.Combine(targetDir, sourceSubDir.Substring(sourceSubDir.LastIndexOf(PATH_SPLIT_CHAR) + 1));
                    if (!Directory.Exists(targetSubDir))
                        Directory.CreateDirectory(targetSubDir);
                    MoveFiles(sourceSubDir, targetSubDir, overWrite, true);
                    Directory.Delete(sourceSubDir);
                }
            }
        }
        public static void MoveFile(string sourceFile, string targetFile, bool overWrite, int target)
        {
            if (target == 1)
            {
                if (File.Exists(targetFile))
                {
                    if (overWrite == true)
                    {
                        File.SetAttributes(targetFile, FileAttributes.Normal);
                        File.Delete(targetFile);
                        File.Move(sourceFile, targetFile);
                    }
                }
                else
                {
                    File.Move(sourceFile, targetFile);
                }
            }
            else if (target == 2)
            {
                string targetFileDir = Path.GetDirectoryName(targetFile);
                string sourceFileName = Path.GetFileName(sourceFile);
                string content = null;
                if (File.Exists(targetFile))
                {
                    content = ReadFile(targetFile);
                    File.SetAttributes(targetFile, FileAttributes.Normal);
                    File.Delete(targetFile);

                }
                File.Move(sourceFile, targetFile);
                if (!string.IsNullOrEmpty(content))
                {
                    WriteAllText(Path.Combine(targetFileDir, content), content, Encoding.UTF8);
                }
            }
            else
            {
                MoveFile(sourceFile, targetFile, true, 1);
            }
        }

        /// <summary>
        /// 删除指定目录的所有文件，不包含子目录
        /// </summary>
        /// <param name="targetDir">操作目录</param>
        public static void DeleteFiles(string targetDir)
        {
            DeleteFiles(targetDir, false);
        }

        /// <summary>
        /// 删除指定目录的所有文件和子目录
        /// </summary>
        /// <param name="targetDir">操作目录</param>
        /// <param name="delSubDir">如果为true,包含对子目录的操作</param>
        public static void DeleteFiles(string targetDir, bool delSubDir)
        {
            foreach (string fileName in Directory.GetFiles(targetDir))
            {
                File.SetAttributes(fileName, FileAttributes.Normal);
                File.Delete(fileName);
            }
            if (delSubDir)
            {
                DirectoryInfo dir = new DirectoryInfo(targetDir);
                foreach (DirectoryInfo subDi in dir.GetDirectories())
                {
                    DeleteFiles(subDi.FullName, true);
                    subDi.Delete();
                }
            }
        }
        public static void DeleteFile(string targetFile)
        {
            if (Exists(targetFile))
            {
                File.Delete(targetFile);
            }
        }
        /// <summary>
        /// 创建指定目录
        /// </summary>
        /// <param name="targetDir"></param>
        public static void CreateDirectory(string targetDir)
        {
            DirectoryInfo dir = new DirectoryInfo(targetDir);
            if (!dir.Exists)
                dir.Create();
        }
        public static bool ExitsDirectory(string targetDir)
        {
            if (Directory.Exists(targetDir))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 建立子目录
        /// </summary>
        /// <param name="targetDir">目录路径</param>
        /// <param name="subDirName">子目录名称</param>
        public static void CreateDirectory(string parentDir, string subDirName)
        {
            CreateDirectory(parentDir + PATH_SPLIT_CHAR + subDirName);
        }
        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string CreateFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                return filePath;
            }
            else
            {
                string fn = Path.GetFileName(filePath);
                string dir = Path.GetDirectoryName(filePath);
                CreateDirectory(dir);
                return filePath;
            }
        }

        /// <summary>
        /// 删除指定目录
        /// </summary>
        /// <param name="targetDir">目录路径</param>
        public static void DeleteDirectory(string targetDir)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(targetDir);
            if (dirInfo.Exists)
            {
                DeleteFiles(targetDir, true);
                dirInfo.Delete(true);
            }
        }

        /// <summary>
        /// 删除指定目录的所有子目录,不包括对当前目录文件的删除
        /// </summary>
        /// <param name="targetDir">目录路径</param>
        public static void DeleteSubDirectory(string targetDir)
        {
            foreach (string subDir in Directory.GetDirectories(targetDir))
            {
                DeleteDirectory(subDir);
            }
        }
        /// <summary>
        /// 获取指定目录下的文件
        /// </summary>
        /// <param name="targetDir"></param>
        /// <returns></returns>
        public static string[] GetFiles(string targetDir, bool detail = false)
        {
            string[] dirFiles = Directory.GetFiles(targetDir);
            return dirFiles;
        }
        /// <summary>
        /// 获取指定目录下的文件
        /// </summary>
        /// <param name="targetDir"></param>
        /// <returns></returns>
        public static DataTable GetFiles(string targetDir)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Icon");
            dt.Columns.Add("Name");
            dt.Columns.Add("FullName");
            dt.Columns.Add("Length");
            dt.Columns.Add("CreateTime");
            string[] dirFiles = Directory.GetFiles(targetDir);
            for (int i = 0; i < dirFiles.Length; i++)
            {
                DataRow dr = dt.NewRow();
                FileInfo fileInfo = new FileInfo(dirFiles[i]);
                if (fileInfo.Name.StartsWith("_"))
                {
                    dr["Icon"] = "/images/icon/_html.gif";
                }
                else
                {
                    dr["Icon"] = "/images/icon/html.gif";
                }
                dr["Name"] = fileInfo.Name;
                dr["FullName"] = fileInfo.FullName;
                dr["Length"] = Convert.ToDecimal(fileInfo.Length / 1024.00);
                dr["CreateTime"] = fileInfo.CreationTime.ToString("yyyy-MM-dd");
                dt.Rows.Add(dr);
            }
            return dt;
        }
        /// <summary>
        /// 获取指定目录下的子目录与文件
        /// </summary>
        /// <param name="targetDir"></param>
        /// <returns></returns>
        public static DataTable GetDirectoryAndFiles(string targetDir)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Icon");
            dt.Columns.Add("Name");
            dt.Columns.Add("FullName");
            dt.Columns.Add("Length");
            dt.Columns.Add("CreateTime");
            DirectoryInfo dir = new DirectoryInfo(targetDir);
            FileSystemInfo[] dirFiles = dir.GetFileSystemInfos();
            for (int i = 0; i < dirFiles.Length; i++)
            {
                DataRow dr = dt.NewRow();
                if (dirFiles[i] is DirectoryInfo)
                {
                    dr["Icon"] = "/images/icon/folder.gif";
                    dr["Name"] = dirFiles[i].Name;
                    dr["FullName"] = dirFiles[i].FullName;
                    dr["Length"] = "";
                    dr["CreateTime"] = dirFiles[i].CreationTime.ToString("yyyy-MM-dd");
                }
                else
                {
                    FileInfo fileInfo = new FileInfo(dirFiles[i].FullName);
                    if (fileInfo.Name.StartsWith("_"))
                    {
                        dr["Icon"] = "/images/icon/_html.gif";
                    }
                    else
                    {
                        dr["Icon"] = "/images/icon/html.gif";
                    }
                    dr["Name"] = fileInfo.Name;
                    dr["FullName"] = fileInfo.FullName;
                    dr["Length"] = Convert.ToDecimal(fileInfo.Length / 1024.00);
                    dr["CreateTime"] = fileInfo.CreationTime.ToString("yyyy-MM-dd");
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
        /// <summary>
        /// 操作对象是否存在 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static bool IsExists(string file, FsoMethod method)
        {
            if (method == FsoMethod.File)
            {
                return File.Exists(file);
            }
            return ((method == FsoMethod.Folder) && Directory.Exists(file));
        }

        /// <summary>
        /// 获取文件夹信息
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static string[] GetDirs(string dir)
        {
            return Directory.GetDirectories(dir);
        }

        #region 读写操作
        /// <summary>
        /// 取后缀名
        /// </summary>
        /// <param name="filename">文件名</param> 
        /// <returns>.gif|.html格式</returns>
        public static string GetPostfixStr(string filename)
        {
            int start = filename.LastIndexOf(".");
            int length = filename.Length;
            string postfix = filename.Substring(start, length - start);
            return postfix;
        }

        /// <summary>
        /// 写文件
        /// </summary> 
        /// <param name="Path">文件路径</param>
        /// <param name="Strings">文件内容</param>
        public static void WriteFile(string Path, string Strings)
        {

            if (!System.IO.File.Exists(Path))
            {
                FileStream f = File.Create(Path);
                f.Close();
                f.Dispose();
            }
            StreamWriter f2 = new StreamWriter(Path, false, Encoding.UTF8);
            f2.WriteLine(Strings);
            f2.Close();
            f2.Dispose();

        }
        /// <summary>
        /// 写入或覆盖文件
        /// </summary> 
        /// <param name="Path">文件路径</param>
        /// <param name="Strings">文件内容</param>
        public static void WriteAllText(string Path, string Strings, Encoding enCoding)
        {
            if (!Exists(Path))
            {
                CreateFile(Path);
            }
            File.WriteAllText(Path, Strings, enCoding);
        }
        /// <summary>
        /// 读文件
        /// </summary>
        /// <param name="Path">文件路径</param>
        /// <returns></returns>
        public static string ReadFile(string Path)
        {
            string s = "";
            if (!File.Exists(Path))
                s = "不存在相应的目录";
            else
            {
                StreamReader f2 = new StreamReader(Path, System.Text.Encoding.UTF8);
                s = f2.ReadToEnd();
                f2.Close();
                f2.Dispose();
            }
            return s;
        }

        /// <summary>
        /// 读文件
        /// </summary>
        /// <param name="Path">文件路径</param>
        /// <param name="enCoding">文件存储编码</param>
        /// <returns></returns>
        public static string ReadFile(string Path, Encoding enCoding)
        {
            string s = "";
            if (!File.Exists(Path))
                s = "不存在相应的目录";
            else
            {
                StreamReader f2 = new StreamReader(Path, enCoding);
                s = f2.ReadToEnd();
                f2.Close();
                f2.Dispose();
            }
            return s;
        }
        /// <summary>
        /// 替换增加
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="replaceStr">替换字符串</param>
        /// <param name="newStr">新字符串</param>
        public static void FileAddReplace(string path, string replaceStr, string newStr)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ReadFile(path));
            sb.Replace(replaceStr, newStr);
            WriteFile(path, sb.ToString());
        }
        /// <summary>
        /// 追加文件
        /// </summary>
        /// <param name="Path">文件路径</param>
        /// <param name="strings">内容</param>
        public static void FileAdd(string Path, string strings)
        {
            StreamWriter sw = File.AppendText(Path);
            sw.Write(strings);
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
        /// <summary>
        /// 删除文件
        /// </summary> 
        /// <param name="Path">路径</param>
        public static void FileDel(string Path)
        {
            if (File.Exists(Path))
                File.Delete(Path);
        }
        /// <summary>
        /// 删除文件
        /// </summary> 
        /// <param name="Path">路径</param>
        public static bool Exists(string Path)
        {
            if (!File.Exists(Path))
                return false;
            return true;
        }
        #endregion

        #region xml文档
        /// <summary>
        /// 将指定目录下的子目录和文件生成xml文档
        /// </summary>
        /// <param name="targetDir">根目录</param>
        /// <returns>返回XmlDocument对象</returns>
        public static XDocument CreateXml(string targetDir)
        {
            XDocument xDocument = new XDocument();
            xDocument.Declaration = new XDeclaration("1.0", "utf-8", null);
            string rootName = targetDir.Substring(targetDir.LastIndexOf(PATH_SPLIT_CHAR) + 1);
            xDocument.Add(new XElement(rootName,
                from f in Directory.GetFiles(targetDir)
                select new XElement(
                    "File", f.Substring(f.LastIndexOf(PATH_SPLIT_CHAR) + 1))
                ));
            foreach (string directory in Directory.GetDirectories(targetDir))
            {
                XElement directoryElement = new XElement("Directory", new XAttribute("Name", directory.Substring(directory.LastIndexOf(PATH_SPLIT_CHAR) + 1)));
                xDocument.Element(rootName).Add(directoryElement);
                CreateBranch(directory, directoryElement, xDocument);
            }
            return xDocument;
        }

        /// <summary>
        /// 生成Xml分支
        /// </summary>
        /// <param name="targetDir">子目录</param>
        /// <param name="xmlNode">父目录XmlDocument</param>
        /// <param name="myDocument">XmlDocument对象</param>
        private static void CreateBranch(string targetDir, XElement directoryElement, XDocument xDocument)
        {
            directoryElement.Add(from f in Directory.GetFiles(targetDir)
                                 select new XElement(
                                     "File", f.Substring(f.LastIndexOf(PATH_SPLIT_CHAR) + 1))
                );

            foreach (string directory in Directory.GetDirectories(targetDir))
            {
                XElement directoryChildElement = new XElement("Directory", new XAttribute("Name", directory.Substring(directory.LastIndexOf(PATH_SPLIT_CHAR) + 1)));
                directoryElement.Add(directoryChildElement);
                CreateBranch(directory, directoryChildElement, xDocument);
            }
        }
        /// <summary>
        /// 读取XML文件(返回DataSet)
        /// </summary>
        /// <param name="file"></param>
        /// <param name="rootName"></param>
        /// <returns></returns>
        public static DataSet ReadXML(string file, string rootName)
        {
            string str = "";
            using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    try
                    {
                        str = reader.ReadToEnd();
                        reader.Dispose();
                        stream.Dispose();
                    }
                    catch
                    {
                    }
                }
            }
            StringReader readerxml = new StringReader(str);
            DataSet ds = new DataSet(rootName);
            ds.ReadXml(readerxml as System.IO.TextReader);
            readerxml.Close();
            return ds;

        }
        #endregion

        #region mingjie.yang

        /// <summary>
        /// 检查文件扩展名, 是否可以编辑该文件
        /// </summary>
        /// <param name="p_ext">扩展名</param>
        /// <returns>true</returns>
        public static bool CheckExtEdit(string ext)
        {
            string allowExt = ".ini|.txt|.log|.asp|.asa|.inc|.ascx|.config|.c|.cpp|.cs|.css|.java|.jsp|.js|.php|.sql|.vb|.xml|.htm|.html|.aspx|.cshtml|";
            if (allowExt.IndexOf(ext + "|") != -1)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 检查文件扩展名, 是否支持代码高亮
        /// </summary>
        /// <param name="p_ext">扩展名</param>
        /// <returns>true</returns>
        public static bool CheckExtHighlighter(string ext)
        {
            string allowExt = ".c|.cpp|.cs|.css|.java|.jsp|.js|.php|.sql|.vb|.xml|.htm|.html|.aspx|.cshtml|.txt|";
            if (allowExt.IndexOf(ext + "|") != -1)
                return true;
            else
                return false;
        }

        #endregion

        #region 64base文件流转为文件
        /// <summary>
        /// 将base64流转换为文件（图片）
        /// </summary>
        /// <param name="base64String"></param>
        /// <param name="savePath"></param>
        public static void Base64ToImage(string base64String, string savePath)
        {
            string Ipath = savePath.Replace("/", "\\");
            byte[] imageBytes = Convert.FromBase64String(base64String);
            using (MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                ms.Write(imageBytes, 0, imageBytes.Length);
                System.Drawing.Image imag = System.Drawing.Image.FromStream(ms, true);
                imag.Save(Ipath);
                imag.Dispose();
            }
        }
        /// <summary>
        /// 将base64流转换为文件（文件）
        /// </summary>
        /// <param name="base64String"></param>
        /// <param name="savePath"></param>
        public static void Base64ToFile(string base64String, string savePath)
        {
            byte[] Pbytes = Convert.FromBase64String(base64String);
            using (FileStream fs = new FileStream(savePath, FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(Pbytes);
                }
            }
        }
        /// <summary>
        /// Base64转换为图片
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
        public static System.Drawing.Image Base64ToImage(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            using (MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                ms.Write(imageBytes, 0, imageBytes.Length);
                System.Drawing.Image imag = System.Drawing.Image.FromStream(ms, true);
                return imag;
            }
        }

        /// <summary>
        /// 将 文件 转成 Stream
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Stream BytesToStream(string path)
        {
            FileStream file = new FileStream(path, FileMode.Create);
            return file;
        }

        /// <summary>
        /// 将 Stream 写入文件
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileName"></param>
        public static void StreamToFile(Stream stream, string fileName)
        {
            //把 Stream 转换成 byte[] 
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始 
            stream.Seek(0, SeekOrigin.Begin);
            // 把 byte[] 写入文件 
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(bytes);
                }
            }
        }
        /// <summary>
        /// binary转为文件
        /// </summary>
        /// <param name="fpath"></param>
        /// <param name="binary"></param>
        public static void BinaryToFile(string fpath, string binary)
        {
            using (FileStream fs = new FileStream(fpath, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(Convert.FromBase64String(binary));
                }
            }
        }
        /// <summary>
        /// 字节转为流再转为文件
        /// </summary>
        /// <param name="bt"></param>
        /// <param name="filename"></param>
        public static void BytesToStream(byte[] bt, string filename)
        {
            Stream stream = new MemoryStream(bt);
            // 把 Stream 转换成 byte[] 
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始 
            stream.Seek(0, SeekOrigin.Begin);
            // 把 byte[] 写入文件 
            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(bytes);
                    stream.Flush();
                    stream.Dispose();
                }
            }
        }
        #endregion
        #endregion
    }
}
