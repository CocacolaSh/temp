using Ocean.Core.Data;
using Ocean.Data;
using Ocean.Entity;

namespace Ocean.Services
{
    public class EnumTypeService : ServiceBase<EnumType>, IEnumTypeService
    {
        public EnumTypeService(IRepository<EnumType> enumTypeRepository, IDbContext context)
            : base(enumTypeRepository, context)
        {

        }
    }
}