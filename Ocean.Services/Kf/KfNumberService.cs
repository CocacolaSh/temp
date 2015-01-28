using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Entity;
using Ocean.Core.Data;
using Ocean.Data;

namespace Ocean.Services
{
    public class KfNumberService : ServiceBase<KfNumber>, IKfNumberService
    {
        public KfNumberService(IRepository<KfNumber> kfNumberRepository, IDbContext context)
            : base(kfNumberRepository, context)
        {
            
        }
    }
}