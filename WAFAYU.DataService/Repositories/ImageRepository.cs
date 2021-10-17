using Microsoft.EntityFrameworkCore;
using WAFAYU.DataService.Models;

namespace WAFAYU.DataService.Repositories
{
    public interface IImageRepository : IBaseRepository<Image>
    {

    }
    public class ImageRepository : BaseRepository<Image>, IImageRepository
    {
        public ImageRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
