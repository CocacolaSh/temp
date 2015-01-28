using System;
using Ocean.Core.Data;
using Ocean.Data;
using Ocean.Core;
using Ocean.Entity;

namespace Ocean.Services
{
    public class EnumDataService : ServiceBase<EnumData>, IEnumDataService
    {
        public EnumDataService(IRepository<EnumData> enumDataRepository, IDbContext context)
            : base(enumDataRepository, context)
        {

        }

        /// <summary>
        /// 根据枚举类型Id获取分页枚举数据列表
        /// </summary>
        public PagedList<EnumData> GetPageListByEnumTypeId(Guid enumTypeId, int pageIndex, int pageSize)
        {
            return GetPageList(c => c.EnumTypeId == enumTypeId, "Sort", pageIndex, pageSize);
        }
    }
}