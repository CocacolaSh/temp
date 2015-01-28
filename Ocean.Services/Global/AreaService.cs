using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core.Data;
using Ocean.Data;
using Ocean.Entity;

namespace Ocean.Services
{
    public class AreaService : ServiceBase<Area>, IAreaService
    {
        public AreaService(IRepository<Area> areaRepository, IDbContext context)
            : base(areaRepository, context)
        {

        }
    }
}