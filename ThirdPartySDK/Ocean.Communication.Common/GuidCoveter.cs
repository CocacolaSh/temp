using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Communication.Common
{
    public static class GuidCoveter
    {
        public static int CoveterFirstChat(Guid value)
        {
            string firstChat = value.ToString().ToLower().Substring(0, 1);
            byte[] array = new byte[1]; //定义一组数组array
            array = System.Text.Encoding.ASCII.GetBytes(firstChat); //string转换的字母
            int asciicode = (short)(array[0]);
            return asciicode;
        }
    }
}