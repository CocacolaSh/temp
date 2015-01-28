using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Core.ExceptionHandling
{
    public class ExceptionManager
    {
        public static OceanException CannotAvailablyExecute(Exception e, string msg)
        {
            return new OceanException(e.Message + "Trace info:" + msg);
        }

        public static OceanException NotSupportedException(string error)
        {
            return new OceanException("NotSupportedException:"+error);
        }

        public static OceanException InvalidOperationException(string error)
        {
            return new OceanException("InvalidOperationException:"+error);
        }

        public static OceanException ArgumentException(string error)
        {
            return new OceanException("ArgumentException:"+error);
        }

        public static OceanException ArgumentNullException(string error)
        {
            return new OceanException("ArgumentNullException:"+error);
        }

        public static OceanException ArgumentOutOfRangeException(string error)
        {
            return new OceanException("ArgumentOutOfRangeException:"+error);
        }

        public static OceanException SystemException(string error)
        {
            return new OceanException("SystemException:"+error);
        }
        
        public static OceanException MessageException(string msg)
		{
            return new OceanException(msg);
		}
        public static OceanException MessageException(string key, string msg)
        {
            return new OceanException(key,msg);
        }
        public static OceanException MessageException(string msg,Exception ex)
        {
            return new OceanException(msg,ex);
        }
    }
}
