using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Communication.Common;
using Ocean.Communication.Model;
using System.Timers;
using Ocean.Communication.Commands;
using log4net;
using Ocean.Core.Caching;

namespace Ocean.Communication.Comet
{
    /// <summary>
    /// 消息管理器
    /// </summary>
    public class MessageManager
    {
        private readonly ILog log = LogManager.GetLogger(typeof(MessageManager));
        private static ICacheManager _cacheManager = new MemoryCacheManager();

        #region The message count.
        /// <summary>
        /// The message count.
        /// </summary>
        private static int messageCount;
        #endregion

        #region The duplex channel stop.
        /// <summary>
        /// The duplex channel stop.
        /// </summary>
        public static bool duplexChannelStop = true;
        #endregion

        #region The obj lock.
        /// <summary>
        /// The obj lock.
        /// </summary>
        private static object objLock = new object();
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        private MessageManager()
        {
            timer.Interval = 1000 * 90;//1分钟
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Enabled = true;
        }
        #endregion

        #region Timer 定时检查异常离线客服
        /// <summary>
        /// 定时器
        /// </summary>
        private Timer timer = new Timer();

        /// <summary>
        /// 定时器事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // 清除不在线客服
            lock (objLock)
            {
                try
                {
                    timer.Enabled = false;
                    // 离线客服
                    List<OnlineKFModel> listOffline = new List<OnlineKFModel>();

                    Dictionary<Guid, OnlineKFModel> kfItems = _cacheManager.Get<Dictionary<Guid, OnlineKFModel>>("OnlineKFItems");

                    if (kfItems == null)
                    {
                        return;
                    }

                    // 检测在线队列，150秒无活动，设为离线
                    foreach (KeyValuePair<Guid, OnlineKFModel> kf in kfItems)
                    {
                        OnlineKFModel modelOnlineKF = kf.Value;
                        TimeSpan ts = DateTime.Now - modelOnlineKF.LastActiveDate;

                        if (ts.TotalSeconds >= 150)
                        {
                            listOffline.Add(modelOnlineKF);
                        }
                    }

                    // 清理离线客服
                    string errorMsg = string.Empty;

                    foreach (OnlineKFModel model in listOffline)
                    {
                        //移除缓存中的客服
                        RemoveCacheKF(model.KfNumberId);
                        //发送客服离线通知
                        new CommandManager(null).TryHandleCommand(
                            "OffLine", new string[] { model.KfNumberId.ToString(), "1" }, ref errorMsg);
                    }
                }
                catch (Exception exception)
                {
                    log.Warn("清理离线客服出现异常：" + exception.Message);
                }
                finally 
                {
                    timer.Enabled = true;
                }
            }
        }
        #endregion

        #region 用户消息管理实例 private static ChatMessageManage instance
        private static MessageManager instance;

        /// <summary>
        /// 用户消息管理
        /// </summary>
        public static MessageManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (objLock)
                    {
                        return instance ?? (instance = new MessageManager());
                    }
                }

                return instance;
            }
        }
        #endregion

        #region 接收来自WCF服务端的信息 public void AddMessageFromServer(SafeCollection<MessageModel> listMessage)
        /// <summary>
        /// 接收来自WCF服务端的信息
        /// </summary>
        public void AddMessageFromServer(SafeCollection<MessageModel> listMessage)
        {
            foreach (MessageModel model in listMessage)
            {
                CometWaitThread cometWaitThread = CometThreadPool.GetUserThread(model.ReceiveUserId);

                if (cometWaitThread != null)
                {
                    cometWaitThread.AddMessage(model);
                }
            }
        }
        #endregion

        #region 接收来自服务端的通知 public void AddNoticeFromServer(SafeCollection<NoticeModel> listNotice)
        /// <summary>
        /// 接收来自服务端的信息
        /// </summary>
        public void AddNoticeFromServer(SafeCollection<NoticeModel> listNotice)
        {
            foreach (NoticeModel model in listNotice)
            {
                CometWaitThread cometWaitThread = CometThreadPool.GetUserThread(model.ReceiveUserId);

                if (cometWaitThread != null)
                {
                    cometWaitThread.AddNotice(model);
                }
            }
        }
        #endregion

        //缓存相关操作

        #region [更新客服在本地队列的缓存] public void CacheLocalKF(OnlineKFModel onlineKFModel)
        /// <summary>
        /// 更新客服在本地队列的缓存
        /// </summary>
        /// <param name="onlineKFModel"></param>
        public void CacheLocalKF(OnlineKFModel onlineKFModel)
        {
            ///保存一份登录到本服务器的在线用户队列
            if (_cacheManager.Get<Dictionary<Guid, OnlineKFModel>>("OnlineKFItems") == null) //检测缓存是否存在,新增缓存并增加用户信息
            {
                Dictionary<Guid, OnlineKFModel> dictionaryKF = new Dictionary<Guid, OnlineKFModel>();
                dictionaryKF.Add(onlineKFModel.KfNumberId, onlineKFModel);
                _cacheManager.Set("OnlineKFItems", dictionaryKF, 10080);//10080=1周
            }
            else if (CheckOnlineKFCacheIsEmpty(onlineKFModel.KfNumberId)) //检测缓存中是否存在当前用户的数据,更新用户信息
            {
                _cacheManager.Get<Dictionary<Guid, OnlineKFModel>>("OnlineKFItems")[onlineKFModel.KfNumberId] = onlineKFModel;
            }
            else //向缓存中新增用户信息 
            {
                _cacheManager.Get<Dictionary<Guid, OnlineKFModel>>("OnlineKFItems").Add(onlineKFModel.KfNumberId, onlineKFModel);
            }
        }
        #endregion

        #region [检测指定客服信息是否缓存在本服务器Cache队列中] public bool CheckOnlineKFCacheIsEmpty(Guid id)
        /// <summary>
        /// 检测指定客服信息是否缓存在本服务器Cache队列中
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <returns>bool</returns>
        public bool CheckOnlineKFCacheIsEmpty(Guid id)
        {
            Dictionary<Guid, OnlineKFModel> dictionaryKFs = _cacheManager.Get<Dictionary<Guid, OnlineKFModel>>("OnlineKFItems");

            if (dictionaryKFs == null || !dictionaryKFs.ContainsKey(id))
            {
                return false;
            }

            return true;
        }
        #endregion

        #region [获取在线客服信息] public MpUser GetOnlineKFInfo(Guid id)
        /// <summary>
        /// 获取在线客服信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public OnlineKFModel GetOnlineKFInfo(Guid id)
        {
            Dictionary<Guid, OnlineKFModel> dictionaryKFs = _cacheManager.Get<Dictionary<Guid, OnlineKFModel>>("OnlineKFItems");

            if (dictionaryKFs == null || !dictionaryKFs.ContainsKey(id))
            {
                return null;
            }
            else
            {
                return dictionaryKFs[id];
            }
        }
        #endregion

        #region [移除缓存中的客服] public void RemoveCacheKF(Guid id)
        /// <summary>
        /// 移除缓存中的客服
        /// </summary>
        /// <param name="uId"></param>
        public void RemoveCacheKF(Guid id)
        {
            //移除缓存中的客服
            if (CheckOnlineKFCacheIsEmpty(id))
            {
                try
                {
                    _cacheManager.Get<Dictionary<Guid, OnlineKFModel>>("OnlineKFItems").Remove(id);
                }
                catch { }
            }
        }
        #endregion
    }
}