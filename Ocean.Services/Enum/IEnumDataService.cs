using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Entity.Enums;
using Ocean.Core;
using Ocean.Entity;

namespace Ocean.Services
{
    public interface IEnumDataService : IService<EnumData>
    {
        /// <summary>
        /// 根据枚举类型Id获取分页枚举数据列表
        /// </summary>
        PagedList<EnumData> GetPageListByEnumTypeId(Guid enumTypeId, int pageIndex, int pageSize);
    }
}