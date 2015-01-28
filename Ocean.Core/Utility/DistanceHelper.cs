using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Ocean.Core.Utility
{
    public class Coordinate
    {
        public string error { set; get; }

        public string x { set; get; }

        public string y { set; get; }
    }

    public class DistanceHelper
    {
        private const double EARTH_RADIUS = 6378.137;//地球半径

        private static double rad(double d)
        {
            return d * Math.PI / 180.0;
        }

        /// <summary>
        /// 根据两点的经纬度计算距离
        /// </summary>
        public static double GetDistance(double lat1, double lng1, double lat2, double lng2)
        {
            double radLat1 = rad(lat1);
            double radLat2 = rad(lat2);
            double a = radLat1 - radLat2;
            double b = rad(lng1) - rad(lng2);
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            s = Math.Round(s * 10000) / 10000;
            return s;
        }

        /// <summary>
        /// 把谷歌(微信)坐标转移成百度坐标
        /// </summary>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">纬度</param>
        public static bool ConvertCoordinate(ref double longitude, ref double latitude)
        {
            HttpRequests httpRequests = new HttpRequests();
            string jsonText = httpRequests.DownloadHtml(string.Format("http://api.map.baidu.com/ag/coord/convert?from=2&to=4&x={0}&y={1}", longitude, latitude), null);
            //反序列化JSON字符串  
            Coordinate ja = JsonConvert.DeserializeObject<Coordinate>(jsonText);

            if (ja.error == "0")
            {
                byte[] x = Convert.FromBase64String(ja.x);
                longitude = double.Parse(Encoding.Default.GetString(x));
                byte[] y = Convert.FromBase64String(ja.y);
                latitude = double.Parse(Encoding.Default.GetString(y));
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}