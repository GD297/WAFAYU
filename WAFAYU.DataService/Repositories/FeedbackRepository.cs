using Microsoft.EntityFrameworkCore;
using WAFAYU.DataService.Models;

namespace WAFAYU.DataService.Repositories
{
    public interface IFeedbackRepository : IBaseRepository<Feedback>
    {

    }
    public class FeedbackRepository : BaseRepository<Feedback>, IFeedbackRepository
    {
        public FeedbackRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
