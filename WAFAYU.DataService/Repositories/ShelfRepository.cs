using Microsoft.EntityFrameworkCore;
using WAFAYU.DataService.Models;

namespace WAFAYU.DataService.Repositories
{
    public interface IShelfRepository : IBaseRepository<Shelf>
    {

    }
    public class ShelfRepository : BaseRepository<Shelf>, IShelfRepository
    {
        public ShelfRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
