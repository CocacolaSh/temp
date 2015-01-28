using System;
using System.Linq;
using Ocean.Core.Data;
using Ocean.Data;
using Ocean.Entity.DTO;
using Ocean.Core;
using Ocean.Core.Data.OrderBy;
using Ocean.Entity;

namespace Ocean.Services
{
    public class AdminLoggerService : ServiceBase<AdminLogger>, IAdminLoggerService
    {
        public AdminLoggerService(IRepository<AdminLogger> adminLoggerRepository, IDbContext context)
            : base(adminLoggerRepository, context)
        {

        }

        /// <summary>
        /// 根据搜索条件获取分页数据
        /// </summary>
        public PagedList<AdminLogger> GetPageList(int pageIndex, int pageSize, AdminLoggerDTO adminLoggerDTO)
        {
            IQueryable<AdminLogger> query = base.Table;

            if (adminLoggerDTO.StartDate.HasValue)
            {
                query = query.Where(l => l.CreateDate >= adminLoggerDTO.StartDate);
            }

            if (adminLoggerDTO.EndDate.HasValue)
            {
                query = query.Where(l => l.CreateDate <= ((DateTime)adminLoggerDTO.EndDate).AddDays(1));
            }

            if (!string.IsNullOrWhiteSpace(adminLoggerDTO.AdminName))
            {
                query = query.Where(l => l.AdminName == adminLoggerDTO.AdminName);
            }

            if (adminLoggerDTO.Module.HasValue)
            {
                query = query.Where(l => l.Module == adminLoggerDTO.Module);
            }

            query = query.OrderByDescending("CreateDate");
            int count = query.Count();

            if (pageIndex == 1)
            {
                query = query.Take<AdminLogger>(pageSize);
            }
            else
            {
                query = query.Skip<AdminLogger>((pageIndex - 1) * pageSize).Take<AdminLogger>(pageSize);
            }

            PagedList<AdminLogger> pageItems = new PagedList<AdminLogger>(query.ToList(), pageIndex, pageSize);
            pageItems.TotalItemCount = count;
            return pageItems;
        }
    }
}