using System.Linq;
using System.Net;
using WAFAYU.DataService.Models;
using WAFAYU.DataService.Repositories;
using WAFAYU.DataService.Responses;
using WAFAYU.DataService.UnitOfWorks;

namespace WAFAYU.DataService.Services
{
    public interface IRoleService : IBaseService<Role>
    {
        string GetRoleName(int id);
        int GetRoleId(string roleName);
    }
    public class RoleService : BaseService<Role>, IRoleService
    {
        public RoleService(IUnitOfWork unitOfWork, IRoleRepository repository) : base(unitOfWork, repository)
        {
        }

        public int GetRoleId(string roleName)
        {
            var result = Get(x => x.Name == roleName).FirstOrDefault();
            if (result == null) throw new ErrorResponse((int)HttpStatusCode.BadRequest, "Rolename can not found");
            return result.Id;
        }

        public string GetRoleName(int id)
        {
            var result = Get(x => x.Id == id).FirstOrDefault();
            if (result == null) throw new ErrorResponse((int)HttpStatusCode.BadRequest, "Email can not found");
            return result.Name;
        }
    }
}
