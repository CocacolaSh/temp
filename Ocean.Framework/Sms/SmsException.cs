using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Framework.Sms
{
    public class SmsException : Exception
    {
        public SmsReturnCode ErrorCode
        {
            get;
            private set;
        }

        public SmsException(SmsReturnCode errCode, Exception execption)
            : this(errCode, execption.Message)
        {

        }

        public SmsException(SmsReturnCode errCode, string message)
            : base(message)
        {
            ErrorCode = errCode;
        }

        public SmsException(string message)
            : this(SmsReturnCode.未知错误, message)
        {

        }
    }
}
