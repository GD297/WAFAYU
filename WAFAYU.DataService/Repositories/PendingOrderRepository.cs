using Microsoft.EntityFrameworkCore;
using WAFAYU.DataService.Models;

namespace WAFAYU.DataService.Repositories
{
    public interface IPendingOrderRepository : IBaseRepository<PendingOrder>
    {

    }
    public class PendingOrderRepository : BaseRepository<PendingOrder>, IPendingOrderRepository
    {
        public PendingOrderRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
