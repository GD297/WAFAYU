using Microsoft.EntityFrameworkCore;
using WAFAYU.DataService.Models;

namespace WAFAYU.DataService.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {

    }
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
