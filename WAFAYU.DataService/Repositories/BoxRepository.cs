using Microsoft.EntityFrameworkCore;
using WAFAYU.DataService.Models;

namespace WAFAYU.DataService.Repositories
{
    public interface IBoxRepository : IBaseRepository<Box>
    {

    }
    public class BoxRepository : BaseRepository<Box>, IBoxRepository
    {
        public BoxRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
