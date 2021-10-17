using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WAFAYU.DataService.Enums;
using WAFAYU.DataService.Models;
using WAFAYU.DataService.Repositories;
using WAFAYU.DataService.Responses;
using WAFAYU.DataService.UnitOfWorks;
using WAFAYU.DataService.ViewModels;

namespace WAFAYU.DataService.Services
{
    public interface IBoxService : IBaseService<Box>
    {
        decimal[] GetPrice(int shelfId);
        bool CheckBoxIsUsed(List<Shelf> shelves);
        bool CheckBoxIsUsedInShelf(int shelfId);
        Task<Box> Create(int shelfId, string boxPosition);
        Task<Box> UpdateBoxStatus(int boxId1, int? boxId2, int status, string boxCode);
        Task<int> CountBoxRemaining(List<ShelfViewModel> shelves);
    }
    public class BoxService : BaseService<Box>, IBoxService
    {
        public BoxService(IUnitOfWork unitOfWork, IBoxRepository repository) : base(unitOfWork, repository)
        {
        }

        public bool CheckBoxIsUsed(List<Shelf> shelves)
        {
            foreach (var shelf in shelves)
            {
                if (Get(x => x.ShelfId == shelf.Id && x.Status == (int)BoxStatus.Used).FirstOrDefault() != null)
                    throw new ErrorResponse((int)HttpStatusCode.BadRequest, "Box is being used");
            }
            return true;
        }

        public bool CheckBoxIsUsedInShelf(int shelfId)
        {
            if (Get(x => x.ShelfId == shelfId && x.Status == (int)BoxStatus.Used).FirstOrDefault() != null)
                throw new ErrorResponse((int)HttpStatusCode.BadRequest, "Box is being used");
            return true;
        }

        public async Task<int> CountBoxRemaining(List<ShelfViewModel> shelves)
        {
            var shelfIds = shelves.Where(x => x.Status != 0).Select(x => x.Id).ToList();
            var boxNotUsed = await Get(x => ((shelfIds.Any(a => a == x.ShelfId)) && (x.Status == (int)BoxStatus.Active))).ToListAsync();
            return boxNotUsed.Count;
        }

        public async Task<Box> Create(int shelfId, string boxPosition)
        {
            var entity = new Box();
            entity.ShelfId = shelfId;
            entity.Status = 1;
            entity.Position = boxPosition;
            await CreateAsync(entity);
            return entity;
        }

        public decimal[] GetPrice(int shelfId)
        {
            decimal[] result = new decimal[2];
            decimal? priceType1 = 0;
            decimal? priceType2 = 0;
            if (Get(x => x.ShelfId == shelfId && x.Type == 1).FirstOrDefault() != null)
                priceType1 = Get(x => x.ShelfId == shelfId && x.Type == 1).FirstOrDefault().Price;
            if (Get(x => x.ShelfId == shelfId && x.Type == 2).FirstOrDefault() != null)
                priceType2 = Get(x => x.ShelfId == shelfId && x.Type == 2).FirstOrDefault().Price;
            if (priceType1 == null) priceType1 = 0;
            if (priceType2 == null) priceType2 = 0;
            result[0] = (decimal)priceType1;
            result[1] = (decimal)priceType2;
            return result;
        }

        public async Task<Box> UpdateBoxStatus(int boxId1, int? boxId2, int status, string boxCode)
        {
            var entity = await GetAsync(boxId1);
            entity.Status = status;
            entity.BoxCode = boxCode;
            await UpdateAsync(entity);
            if (boxId2 == null) return entity;
            entity = await GetAsync(boxId2);
            entity.Status = status;
            entity.BoxCode = boxCode;
            await UpdateAsync(entity);
            return entity;
        }
    }
}
