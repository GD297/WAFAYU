using AutoMapper;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WAFAYU.DataService.Models;
using WAFAYU.DataService.Repositories;
using WAFAYU.DataService.Responses;
using WAFAYU.DataService.UnitOfWorks;
using WAFAYU.DataService.ViewModels;

namespace WAFAYU.DataService.Services
{
    public interface ISpacePackageService : IBaseService<SpacePackage>
    {
        SpacePackage Get(int storageId, string boxType, int quantity);
        Task<SpacePackage> Create(SpacePackageViewModel model);
        decimal[] GetBoxPrice(int storageId);
        Task<SpacePackage> UpdateSpacePackagePrice(SpacePackage smallBox, SpacePackage bigBox);
        Task<int> CountBoxBooked(int storageId, DateTime timeFrom);
    }
    public class SpacePackageService : BaseService<SpacePackage>, ISpacePackageService
    {
        private readonly IMapper _mapper;
        private readonly IPendingOrderService _pendingOrderService;
        public SpacePackageService(IUnitOfWork unitOfWork, ISpacePackageRepository repository, IMapper mapper, IPendingOrderService pendingOrderService) : base(unitOfWork, repository)
        {
            _mapper = mapper;
            _pendingOrderService = pendingOrderService;
        }

        public async Task<int> CountBoxBooked(int storageId, DateTime timeFrom)
        {
            var spacePackages = Get(x => x.StorageId == storageId).ToList();
            var pendingOrders = await _pendingOrderService.CountBoxBooked(spacePackages, timeFrom);
            var spacePackageIds = pendingOrders.Select(a => a.SpacePackageId).ToList();
            var result = Get(x => spacePackageIds.Any(a => a == x.Id)).ToList();
            int boxUsed = 0;
            foreach (var rs in result)
            {
                if (rs.BoxType == "Small") boxUsed = boxUsed + (int)rs.Quantity;
                else boxUsed = boxUsed + (int)rs.Quantity * 2;
            }
            return boxUsed;
        }

        public async Task<SpacePackage> Create(SpacePackageViewModel model)
        {
            var entity = _mapper.Map<SpacePackage>(model);
            await CreateAsync(entity);
            return entity;
        }

        public SpacePackage Get(int storageId, string boxType, int quantity)
        {
            var result = Get(x => x.StorageId == storageId && x.BoxType == boxType && x.Quantity == quantity).FirstOrDefault();
            if (result == null) throw new ErrorResponse((int)HttpStatusCode.BadRequest, "Storage Id not found");
            return result;
        }

        public decimal[] GetBoxPrice(int storageId)
        {
            var smallBox = Get(x => x.StorageId == storageId && x.Quantity == 12 && x.BoxType == "Small").FirstOrDefault();
            var bigBox = Get(x => x.StorageId == storageId && x.Quantity == 12 && x.BoxType == "Big").FirstOrDefault();
            decimal[] prices = new decimal[2];
            if (smallBox != null) prices[0] = (decimal)smallBox.Price;
            else prices[0] = 0;
            if (bigBox != null) prices[1] = (decimal)bigBox.Price;
            else prices[1] = 0;
            return prices;
        }

        public async Task<SpacePackage> UpdateSpacePackagePrice(SpacePackage smallBox, SpacePackage bigBox)
        {
            await UpdateAsync(smallBox);
            await UpdateAsync(bigBox);
            return bigBox;
        }
    }
}
