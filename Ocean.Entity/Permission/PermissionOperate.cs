using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core;

namespace Ocean.Entity
{
    public enum PermissionOperate
    {
        /// <summary>
        /// 管理
        /// </summary>
        manager,

        /// <summary>
        /// 查看
        /// </summary> 
        view,

        /// <summary>
        /// 添加
        /// </summary>
        add,

        /// <summary>
        /// 编辑
        /// </summary>
        edit,

        /// <summary>
        /// 删除
        /// </summary>
        delete,

        /// <summary>
        /// 搜索
        /// </summary>
        search,

        /// <summary>
        /// 审核
        /// </summary>
        audit,

        /// <summary>
        /// 冻结
        /// </summary>
        freeze,

        /// <summary>
        /// 解冻
        /// </summary>
        unfreeze,

        /// <summary>
        /// 打印
        /// </summary>
        print,

        /// <summary>
        /// 导入
        /// </summary>
        import,

        /// <summary>
        /// 导出
        /// </summary>
        export,

        /// <summary>
        /// 跟进
        /// </summary>
        track,

        /// <summary>
        /// 授权
        /// </summary>
        auth,

        /// <summary>
        /// 受理
        /// </summary>
        process,

        /// <summary>
        /// 回访
        /// </summary>
        visit,

        /// <summary>
        /// 初始化
        /// </summary>
        init,

        /// <summary>
        /// 设计
        /// </summary>
        design,

        /// <summary>
        /// 配置
        /// </summary>
        configuration,

        /// <summary>
        /// 缓存操作
        /// </summary>
        cache,
    }
}