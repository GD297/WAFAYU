using Microsoft.EntityFrameworkCore;
using WAFAYU.DataService.Models;

namespace WAFAYU.DataService.Repositories
{
    public interface ISpacePackageRepository : IBaseRepository<SpacePackage>
    {

    }
    public class SpacePackageRepository : BaseRepository<SpacePackage>, ISpacePackageRepository
    {
        public SpacePackageRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
