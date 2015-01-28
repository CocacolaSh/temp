using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Communication.Common;

namespace Ocean.Communication.Comet
{
    public static class CometThreadPool
    {
        private static object state = new object();
        public static List<CometWaitThread> waitThreads = new List<CometWaitThread>();
        //private static int nextWaitThread;
        private static int maxWaitThreads;

        #region 把需要监听的客户端平均分配到各个线程中
        /// <summary>
        /// 把需要监听的客户端平均分配到各个线程中
        /// </summary>
        /// <param name="request"></param>
        public static void QueueCometWaitRequest(CometWaitRequest request)
        {
            CometWaitThread waitThread;

            //#region 把需要监听的客户端平均分配到各个线程中
            //lock (state)
            //{
            //    // else, get the next wait thread
            //    waitThread = waitThreads[nextWaitThread];
            //    // cycle the thread that we want
            //    nextWaitThread++;
            //    if (nextWaitThread == maxWaitThreads)
            //        nextWaitThread = 0;
            //    CometWaitRequest.RequestCount++;
            //}
            //#endregion

            //根据SendUserId求余分配客户端到指定线程
            lock (state)
            {
                int nThread = GuidCoveter.CoveterFirstChat(request.SendUserId) % maxWaitThreads;
                waitThread = waitThreads[nThread];
                CometWaitRequest.RequestCount++;
            }

            // 加入线程
            waitThread.QueueCometWaitRequest(request);
        }
        #endregion

        #region 初始化处理线程
        /// <summary>
        /// 初始化处理线程
        /// </summary>
        /// <param name="count">线程数</param>
        public static void CreateThreads(int count)
        {
            for (int i = 0; i < count; i++)
            {
                CometWaitThread waitThread = new CometWaitThread();
                waitThreads.Add(waitThread);
            }

            maxWaitThreads = count;
        }
        #endregion

        #region 返回用户所处的线程
        /// <summary>
        /// 返回用户所处的线程
        /// </summary>
        public static CometWaitThread GetUserThread(Guid sendUserId)
        {
            //根据sendUserId求余获取当前用户所在线程
            int nThread = GuidCoveter.CoveterFirstChat(sendUserId) % maxWaitThreads;

            if (waitThreads != null)
            {
                CometWaitThread waitThread = waitThreads[nThread];
                return waitThread;
            }

            return null;
        }
        #endregion

        #region 根据用户Id从当前线程队列移除监听客户端
        /// <summary>
        /// 根据用户Id从当前线程队列移除监听客户端
        /// </summary>
        public static void DequeueCometWaitRequest(Guid sendUserId)
        {
            lock (state)
            {
                //根据sendUserId求余获取当前用户所在线程
                int nThread = GuidCoveter.CoveterFirstChat(sendUserId) % maxWaitThreads;
                CometWaitThread waitThread = waitThreads[nThread];
                waitThread.DequeueCometWaitRequest(sendUserId);
            }
        }
        #endregion
    }
}