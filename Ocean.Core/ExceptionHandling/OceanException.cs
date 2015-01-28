using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core.Logging;

namespace Ocean.Core.ExceptionHandling
{
    public class OceanException:Exception
    {
        public string Key
        {
            get;
            set;
        }
		public OceanException()
		{
			//
		}
        public OceanException(string msg): base(msg)
		{
			//
		}
        public OceanException(string key, string msg)
            : base(msg)
        {
            this.Key = key;
        }
        public OceanException(string msg, Exception ex)
            : base(msg)
        {
            if (ex != null)
            {
                Log4NetImpl.Write(msg + ":" + ex.ToString(), Log4NetImpl.ErrorLevel.Debug);
            }
        }
    }
}
