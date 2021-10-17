using System.Threading.Tasks;
using WAFAYU.DataService.Models;
using WAFAYU.DataService.Repositories;
using WAFAYU.DataService.UnitOfWorks;

namespace WAFAYU.DataService.Services
{
    public interface IImageService : IBaseService<Image>
    {
        Task<Image> Delete(int id);
    }
    public class ImageService : BaseService<Image>, IImageService
    {
        public ImageService(IUnitOfWork unitOfWork, IImageRepository repository) : base(unitOfWork, repository)
        {
        }

        public async Task<Image> Delete(int id)
        {
            var entity = await GetAsync(id);
            await DeleteAsync(entity);
            return entity;
        }
    }
}
