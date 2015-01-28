using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Core.Plugins.Upload
{
    public class SegmentationParam:OperateParam
    {
        public SegmentationParam()
        {
            Point = new List<int[]>();
            Size = new List<int[]>();
        }
        #region 重置图片大小
        /// <summary>
        /// 宽度
        /// </summary>
        public int ResetWidth { get; set; }
        /// <summary>
        /// 高度
        /// </summary>
        public int ResetHeight { get; set; }
        /// <summary>
        /// 是否重置大小
        /// </summary>
        public int IsReset { get; set; }
        /// <summary>
        /// 重置后的名称
        /// </summary>
        public string ResetName { get; set; }
        #endregion

        #region 切图
        /// <summary>
        /// 切图文件名
        /// </summary>
        public string[] CutNames { get; set; }
        /// <summary>
        /// 图片切割位置
        /// </summary>
        public IList<int[]> Point { get; set; }
        /// <summary>
        /// 图片切割宽高
        /// </summary>
        public IList<int[]> Size { get; set; }
        #endregion
    }
}
