using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImApiDotNet;

namespace Ocean.Framework.Sms
{
    public class APIClientOcean
    {
        /// <summary>
        /// IP地址
        /// </summary>
        public string DBIP
        { 
            private set;
            get; 
        }

        /// <summary>
        /// 用户名
        /// </summary>
        public string DBUser 
        { 
            private set; 
            get; 
        }

        /// <summary>
        /// 密码
        /// </summary>
        public string DBPwd
        { 
            private set; 
            get; 
        }

        /// <summary>
        /// API编码
        /// </summary>
        public string ApiCode
        { 
            private set; 
            get; 
        }

        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DBName 
       { 
            private set; 
            get; 
        }

        /// <summary>
        /// 结果编码
        /// </summary>
        public SmsReturnCode SmsReturnCode
        {
            private set;
            get;
        }

        /// <summary>
        /// APIClient
        /// </summary>
        public APIClient APIClient 
        {
            private set;
            get; 
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(string dbIP, string dbUser, string dbPwd, string apiCode, string dbName)
        {
            DBIP = dbIP;
            DBUser = dbUser;
            DBPwd = dbPwd;
            ApiCode = apiCode;
            DBName = dbName;
            APIClient = new APIClient();
            SmsReturnCode = (SmsReturnCode)APIClient.init(dbIP, dbUser, dbPwd, apiCode, dbName);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Release()
        {
            if (APIClient != null)
            {
                APIClient.release();
                APIClient = null;
            }
        }
    }
}