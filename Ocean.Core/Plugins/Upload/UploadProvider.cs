using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Ocean.Core.Plugins.Upload.Common;

namespace Ocean.Core.Plugins.Upload
{
    public class UploadProvider
    {
        private static Hashtable _instance = new Hashtable();
        private static object lockHelper = new object();

        /// <summary>
        /// 获取验证码的类实例
        /// </summary>
        /// <param name="assemlyName">用于区分库文件的名称(默认：JpegImage)</param>
        /// <returns></returns>
        public static IUpload GetInstance()
        {
            return GetInstance("Common");
        }
        public static IUpload GetInstance(string assemlyName)
        {
            return GetInstance(assemlyName, false);
        }
        public static IUpload GetInstance(string assemlyName, bool isFormat)
        {
            //IUpload upload = null;
            //try
            //{
            //    upload = (IUpload)Activator.CreateInstance(Type.GetType(string.Format("OceanCMS.Plugin.Upload.{0}.UtilUpload, OceanCMS.Plugin.Upload.{0}", assemlyName), false, true));
            //}
            //catch
            //{
            //    upload = new UtilUpload();
            //}
            if (!_instance.ContainsKey(assemlyName))
            {
                lock (lockHelper)
                {
                    if (!_instance.ContainsKey(assemlyName))
                    {
                        IUpload p = null;
                        try
                        {
                            p = (IUpload)Activator.CreateInstance(Type.GetType(string.Format("Ocean.Plugins.Upload.{0}.UtilUpload, Ocean.Plugins", assemlyName), false, true));
                        }
                        catch
                        {
                            p = new UtilUpload();
                        }
                        _instance.Add(assemlyName, p);
                    }
                }
            }
            IUpload upload = (IUpload)_instance[assemlyName];
            return upload;
        }
    }
}
