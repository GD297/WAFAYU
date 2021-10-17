using Microsoft.EntityFrameworkCore;
using WAFAYU.DataService.Models;

namespace WAFAYU.DataService.Repositories
{
    public interface IStorageRepository : IBaseRepository<Storage>
    {

    }
    public class StorageRepository : BaseRepository<Storage>, IStorageRepository
    {
        public StorageRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
