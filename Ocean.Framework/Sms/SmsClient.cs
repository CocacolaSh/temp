using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Ocean.Core.Utility;
using Ocean.Core.Common;
using Ocean.Framework.Configuration.global.config;
using ImApiDotNet;
using Ocean.Core.Logging;

namespace Ocean.Framework.Sms
{
    public class SmsClient
    {
        private static ILog _log = LogManager.GetLogger(typeof(SmsClient));

        #region public static APIClient Apiclient MAS实例
        private static object _objLock = new object();
        private static APIClientOcean _apiClientOcean = null;

        /// <summary>
        /// MAS实例
        /// </summary>
        public static APIClientOcean APIClientOcean
        {
            get
            {
                if (_apiClientOcean == null)
                {
                    lock (_objLock)
                    {
                        if (_apiClientOcean == null)
                        {
                            _apiClientOcean = new APIClientOcean();
                            SerializableStringDictionary config = GlobalConfig.GetConfig();
                            _apiClientOcean.Init(config["DBIP"], config["DBUser"], config["DBPwd"], config["ApiCode"], config["DBName"]);
                        }

                        return _apiClientOcean;
                    }
                }

                return _apiClientOcean;
            }
        }

        /// <summary>
        /// 释放MAS资源
        /// </summary>
        public static void ReleaseAPIClient()
        {
            if (_apiClientOcean != null)
            {
                _apiClientOcean.Release();
                _apiClientOcean = null;
            }
            _log.Debug("释放短信连接！");
        }
        #endregion

        public SmsClient()
        {
            //SerializableStringDictionary config = GlobalConfig.GetConfig();
            ////配置有改变则重新连接
            //if (config["DBIP"] != _apiClientOcean.DBIP
            //    || config["DBUser"] != _apiClientOcean.DBUser
            //    || config["DBPwd"] != _apiClientOcean.DBPwd
            //    || config["ApiCode"] != _apiClientOcean.ApiCode
            //    || config["DBName"] != _apiClientOcean.DBName)
            //{
            //    ReleaseAPIClient();
            //}
        }

        /// <summary>
        /// 执行发送请求
        /// </summary>
        public bool Execute(SendSmsRequest request,ref string message)
        {
            if (APIClientOcean.SmsReturnCode != SmsReturnCode.操作成功)
            {
                message = APIClientOcean.SmsReturnCode.ToString();
                return false;
            }

            SmsReturnCode smsReturnCode = SmsReturnCode.未知错误;

            //发送短信
            if (request.SendTime.HasValue && request.SrcID.HasValue)
            {
                smsReturnCode = (SmsReturnCode)APIClientOcean.APIClient.sendSM(
                    request.Mobiles.ToArray<string>(), request.Message, ((DateTime)request.SendTime).ToString("yyyy-MM-dd HH:mm:ss"), request.SMID, (long)request.SrcID);
            }
            else if ((!request.SendTime.HasValue) && request.SrcID.HasValue)
            {
                smsReturnCode = (SmsReturnCode)APIClientOcean.APIClient.sendSM(
                    request.Mobiles.ToArray<string>(), request.Message, request.SMID, (long)request.SrcID);
            }
            else if (request.SendTime.HasValue && (!request.SrcID.HasValue))
            {
                smsReturnCode = (SmsReturnCode)APIClientOcean.APIClient.sendSM(
                    request.Mobiles.ToArray<string>(), request.Message, ((DateTime)request.SendTime).ToString("yyyy-MM-dd HH:mm:ss"), request.SMID, 0);
            }
            else if ((!request.SendTime.HasValue) && (!request.SrcID.HasValue))
            {
                smsReturnCode = (SmsReturnCode)APIClientOcean.APIClient.sendSM(
                    request.Mobiles.ToArray<string>(), request.Message, request.SMID);
            }
            else
            {
                message = "参数错误";
                return false;
            }

            Log4NetImpl.Write("MAS服务器返回：" + smsReturnCode.ToString());

            if (smsReturnCode != SmsReturnCode.操作成功)
            {
                message = smsReturnCode.ToString();
                return false;
            }

            return true;
        }
    }
}