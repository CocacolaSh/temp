using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Core.Utility
{
    using System.Net.Mail;

    public class SendMail
    {
        //属性

        #region 发送结果
        /// <summary>
        /// 发布状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 发送状态码
        /// </summary>
        public int StatusCode { get; set; }
        #endregion

        #region SMTP服务器配置
        /// <summary>
        /// SMTP服务器
        /// </summary>
        private string SmtpHost = "";

        /// <summary>
        /// SMTP服务器端口
        /// </summary>
        private int SmtpPort = 25;

        /// <summary>
        /// 用户名
        /// </summary>
        private string UserName = "";

        /// <summary>
        /// 密码
        /// </summary>
        private string Password = "";

        /// <summary>
        /// 启用SSL加密发送
        /// </summary>
        private bool EnableSsl = false;

        /// <summary>
        /// 发送人地址
        /// </summary>
        private string FromAddress = "";

        /// <summary>
        /// 发送人名字
        /// </summary>
        private string FromName = "";

        #endregion

        //构造函数

        public SendMail(string sFromName, string sFromAddress, string sSmtpHost, string sSmtpPort, string sUserName, string sPassword, bool bEnableSsl)
        {
            this.FromName = sFromName;
            this.FromAddress = sFromAddress;
            this.SmtpHost = sSmtpHost;
            if (!int.TryParse(sSmtpPort, out SmtpPort))
            {
                this.SmtpPort = 25;
            }
            else
            {
                this.SmtpPort = int.Parse(sSmtpPort);
            }
            this.UserName = sUserName;
            this.Password = sPassword;
            this.EnableSsl = bEnableSsl;
        }

        public bool Send(string To, string Subject, string Body, bool IsHtmlBody)
        {
            Status = "";
            StatusCode = 0;
            SmtpClient client = CreateSmtpClient();
            MailMessage message = CreateMailMessage(To, Subject, Body, IsHtmlBody);
            try
            {
                client.Send(message);
                Status = "发送成功";
                StatusCode = 250;
                return true;
            }
            catch (SmtpException ex)
            {
                Status = ex.Message;
                StatusCode = StatusCode = (int)ex.StatusCode;//在命令成功是，服务器返回代码250，如果失败返回代码550，如果出错，返500，501，502，504或421
                //LogFile.Write(ex);
                if (ex.StatusCode == SmtpStatusCode.GeneralFailure && ex.Message.Contains("身份验证失败"))
                {
                    try
                    {
                        SendByCdo(To, Subject, Body, IsHtmlBody);
                        Status = "发送成功";
                        StatusCode = 250;
                        return true;
                    }
                    catch (Exception ex2)
                    {
                        Status = ex2.Message;
                        //LogFile.Write(ex2);
                    }
                }
                return false;
            }
            finally
            {
                client = null;
                message = null;
            }
        }

        public SmtpClient CreateSmtpClient()
        {
            SmtpClient client = new SmtpClient();
            client.Host = SmtpHost;
            client.Port = SmtpPort;
            client.EnableSsl = EnableSsl;
            client.UseDefaultCredentials = true;
            client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            client.Credentials = new System.Net.NetworkCredential(UserName, Password);
            return client;
        }

        public MailMessage CreateMailMessage(string To, string Subject, string Body, bool IsBodyHtml)
        {
            MailMessage Message = new MailMessage();
            Message.From = new System.Net.Mail.MailAddress(FromAddress, FromName);
            Message.To.Add(To);

            Message.Subject = Subject;
            Message.Body = Body;
            Message.IsBodyHtml = IsBodyHtml;

            Message.SubjectEncoding = System.Text.Encoding.UTF8;
            Message.BodyEncoding = System.Text.Encoding.UTF8;
            Message.Priority = System.Net.Mail.MailPriority.Normal;

            return Message;
        }

        public bool SendByCdo(string To, string Subject, string Body, bool IsHtmlBody)
        {
            System.Web.Mail.MailMessage mail1 = new System.Web.Mail.MailMessage();
            mail1.Subject = Subject;
            mail1.Body = Body;
            mail1.From = FromAddress;
            mail1.To = To;
            if (IsHtmlBody)
                mail1.BodyFormat = System.Web.Mail.MailFormat.Html;
            else
                mail1.BodyFormat = System.Web.Mail.MailFormat.Text;
            mail1.BodyEncoding = Encoding.UTF8;
            mail1.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", 1);
            mail1.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusername", UserName);
            mail1.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendpassword", Password);
            System.Web.Mail.SmtpMail.SmtpServer = SmtpHost;

            System.Web.Mail.SmtpMail.Send(mail1);
            return true;
        }
    }
}
