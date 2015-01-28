using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Senparc.Weixin.MP.Entities.Menu
{
    public interface IBaseButton
    {
        string name { get; set; }
    }

    /// <summary>
    /// 所有按钮基类
    /// </summary>
    public class BaseButton : IBaseButton
    {
        //public ButtonType type { get; set; }
        /// <summary>
        /// 按钮描述，既按钮名字，不超过16个字节，子菜单不超过40个字节
        /// </summary>
        public string name { get; set; }
    }
}
//{\"button\":[{\"name\":\"福农宝\",\"sub_button\":[{\"type\":\"view\",\"name\":\"产品介绍\",\"url\":\"http://funongbao.b2c123.com/funongbao\"},{\"type\":\"view\",\"name\":\"我的福农宝\",\"url\":\"http://funongbao.b2c123.com/funongbao\"},{\"type\":\"view\",\"name\":\"调额申请\",\"url\":\"http://funongbao.b2c123.com/apply\"}]},{\"type\":\"view\",\"name\":\"贷款申请\",\"\"url\":\"http://funongbao.b2c123.com/bankloan/loanapply\"},{\"name\":\"客户服务\",\"sub_button\":[{	\"type\":\"view\",\"name\":\"人工客服\",\"url\":\"http://funongbao.b2c123.com\"},{\"type\":\"view\",\"name\":\"便民服务\",\"url\":\"http://funongbao.b2c123.com/\"},{\"type\":\"view\",\"name\":\"周边网点\",\"url\":\"http://funongbao.b2c123.com\"}]}]}
 //{\"button\":[{\"name\":\"福农宝\",\"sub_button\":[{\"type\":\"view\",\"name\":\"产品介绍\",\"url\":\"http://funongbao.b2c123.com\"},{\"type\":\"view\",\"name\":\"我的福农宝\",\"url\":\"http://funongbao.b2c123.com/funongbao/myfnb\"},{\"type\":\"view\",\"name\":\"调额申请\",\"url\":\"http://funongbao.b2c123.com/apply\"}]},{\"type\":\"view\",\"name\":\"贷款申请\",\"url\":\"http://funongbao.b2c123.com/bankloan/loanapply\"},{\"name\":\"客户服务\",\"sub_button\":[{	\"type\":\"view\",\"name\":\"手机银行下载\",\"url\":\"http://funongbao.b2c123.com\"}]}]}