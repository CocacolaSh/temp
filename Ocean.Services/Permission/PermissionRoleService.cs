using Ocean.Core.Data;
using Ocean.Data;
using Ocean.Entity;

namespace Ocean.Services
{
    public class PermissionRoleService : ServiceBase<PermissionRole>, IPermissionRoleService
    {
        public PermissionRoleService(IRepository<PermissionRole> permissionRoleRepository, IDbContext context)
            : base(permissionRoleRepository, context)
        {

        }
    }
}