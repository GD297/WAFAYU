using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WAFAYU.DataService.Models;
using WAFAYU.DataService.Repositories;
using WAFAYU.DataService.UnitOfWorks;
using WAFAYU.DataService.ViewModels;

namespace WAFAYU.DataService.Services
{
    public interface IPendingOrderService : IBaseService<PendingOrder>
    {
        Task<PendingOrder> Create(PendingOrderViewModel model);
        Task<List<PendingOrder>> CountBoxBooked(List<SpacePackage> spacePackages, DateTime timeFrom);
    }
    public class PendingOrderService : BaseService<PendingOrder>, IPendingOrderService
    {
        private readonly IMapper _mapper;
        private readonly ISubOrderSerivce _subOrderService;
        public PendingOrderService(IUnitOfWork unitOfWork, IPendingOrderRepository repository, IMapper mapper, ISubOrderSerivce subOrderSerivce) : base(unitOfWork, repository)
        {
            _mapper = mapper;
            _subOrderService = subOrderSerivce;
        }

        public async Task<List<PendingOrder>> CountBoxBooked(List<SpacePackage> spacePackages, DateTime timeFrom)
        {
            var spacePackageIds = spacePackages.Select(a => a.Id).ToList();
            var pendingOrders = Get(x => spacePackageIds.Any(a => a == x.SpacePackageId)).ToList();

            var orderIds = pendingOrders.Select(a => a.OrderId).ToList();
            var orders = _subOrderService.Get(x => orderIds.Any(a => a == x.Id) && x.PickupTime.Value.AddMonths((int)x.Months) > timeFrom).ToList();

            var ids = orders.Select(a => a.Id).ToList();
            var result = await Get(x => ids.Any(a => a == x.OrderId)).ToListAsync();
            return result;
        }

        public async Task<PendingOrder> Create(PendingOrderViewModel model)
        {
            var entity = _mapper.Map<PendingOrder>(model);
            await CreateAsync(entity);
            return entity;
        }
    }
}
