//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Ocean.Communication.Communication
//{
//    public class CometWaitRequest
//    {
//        /// <summary>
//        /// 与MVC的委托句柄
//        /// </summary>
//        public delegate bool Execute(ArgumentModel modelArgument);
//        public Execute execute { get; set; }
//        private int _uId;

//        #region 监听客户端的添加时间
//        /// <summary>
//        /// 监听客户端的添加时间
//        /// </summary>
//        private DateTime dateTimeAdded;
//        #endregion

//        #region 监听客户端数量
//        /// <summary>
//        /// 监听客户端数量
//        /// </summary>
//        public static int RequestCount;
//        #endregion

//        #region 轮询请求
//        /// <summary>
//        /// 轮询请求
//        /// </summary>
//        /// <param name="uId"></param>
//        public CometWaitRequest(int uId)
//        {
//            _uId = uId;
//            dateTimeAdded = DateTime.Now;
//        }
//        #endregion

//        #region 客户端请求用户Id
//        /// <summary>
//        /// 客户端请求用户Id
//        /// </summary>
//        public int UId
//        {
//            get
//            {
//                return _uId;
//            }
//        }
//        #endregion

//        #region 监听客户端的添加时间
//        /// <summary>
//        /// 监听客户端的添加时间
//        /// </summary>
//        public DateTime DateTimeAdded
//        {
//            get { return this.dateTimeAdded; }
//        }
//        #endregion
//    }
//}