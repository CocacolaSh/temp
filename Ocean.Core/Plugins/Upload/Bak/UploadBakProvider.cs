using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core.ExceptionHandling;

namespace Ocean.Core.Plugins.Bak
{
    public class UploadBakProvider
    {
        private static Dictionary<string, IUploadBak> _instance = new Dictionary<string, IUploadBak>();
        private static object lockHelper = new object();

        /// <summary>
        /// 获取备份的类实例
        /// </summary>
        /// <param name="assemlyName">用于区分库文件的名称(默认：Common)</param>
        /// <returns></returns>
        public static IUploadBak GetInstance()
        {
            return GetInstance("UploadRecord");
        }
        public static IUploadBak GetInstance(string assemlyName)
        {
            if (string.IsNullOrEmpty(assemlyName))
            {
                assemlyName = "UploadBak";
            }
            if (!_instance.ContainsKey(assemlyName))
            {
                lock (lockHelper)
                {
                    if (!_instance.ContainsKey(assemlyName))
                    {
                        IUploadBak p = null;
                        try
                        {
                            p = (IUploadBak)Activator.CreateInstance(Type.GetType(string.Format("Ocean.Plugins.UploadRecord.{0}, Ocean.Plugins.UploadRecord", assemlyName), false, true));
                        }
                        catch
                        {
                            throw ExceptionManager.MessageException("请检查配置！");
                        }
                        _instance.Add(assemlyName, p);
                    }
                }
            }
            IUploadBak upload = (IUploadBak)_instance[assemlyName];
            return upload;
        }
    }
}
