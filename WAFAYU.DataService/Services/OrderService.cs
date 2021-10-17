using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Threading.Tasks;
using WAFAYU.DataService.Constants;
using WAFAYU.DataService.Enums;
using WAFAYU.DataService.Models;
using WAFAYU.DataService.Repositories;
using WAFAYU.DataService.Responses;
using WAFAYU.DataService.UnitOfWorks;
using WAFAYU.DataService.Utilities;
using WAFAYU.DataService.ViewModels;

namespace WAFAYU.DataService.Services
{
    public interface IOrderService : IBaseService<Order>
    {
        Task<DynamicModelResponse<OwnerOrderViewModel>> GetAllByOwner(OwnerOrderViewModel model, string[] fields, int page, int size, int ownerId);
        Task<object> GetAll(string role, string[] fields, int page, int size, string idToken);
        Task<DynamicModelResponse<CustomerOrderViewModel>> GetAllByCustomer(CustomerOrderViewModel model, string[] fields, int page, int size, int customerId);
        Task<CustomerOrderViewModel> Payment(OrderPaymentViewModel model, string idToken);
        Task<CustomerOrderViewModel> GetCustomerOrderById(int Id);
        Task<object> GetById(int id, string role, string idToken);
        Task<OwnerOrderDetailViewModel> GetDetailByOwner(int id, int ownerId);
        Task<CustomerOrderDetailViewModel> GetDetailByCustomer(int id, int customerId);
    }
    public class OrderService : BaseService<Order>, IOrderService
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userSerivce;
        private readonly IStorageService _storageService;
        private readonly ISpacePackageService _spacePackageService;
        private readonly IPendingOrderService _pendingOrderService;
        private readonly IFeedbackService _feedbackService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly IBoxService _boxService;

        public OrderService(IUnitOfWork unitOfWork, IOrderRepository repository, IMapper mapper, IUserService userService, IStorageService storageService, ISpacePackageService spacePackageService, IPendingOrderService pendingOrderService, IFeedbackService feedbackService, IOrderDetailService orderDetailService, IBoxService boxService) : base(unitOfWork, repository)
        {
            _mapper = mapper;
            _userSerivce = userService;
            _storageService = storageService;
            _spacePackageService = spacePackageService;
            _pendingOrderService = pendingOrderService;
            _feedbackService = feedbackService;
            _orderDetailService = orderDetailService;
            _boxService = boxService;
        }

        public async Task<object> GetAll(string role, string[] fields, int page, int size, string idToken)
        {
            var secureToken = new JwtSecurityTokenHandler().ReadJwtToken(idToken);
            var uid = secureToken.Claims.First(claim => claim.Type == "user_id").Value;
            var user = _userSerivce.GetUser(uid);
            var userId = user.Id;
            if (role == "Customer") return await GetAllByCustomer(new CustomerOrderViewModel(), fields, page, size, userId);
            return await GetAllByOwner(new OwnerOrderViewModel(), fields, page, size, userId);
        }

        public async Task<List<BoxUsedViewModel>> GetBoxUsedLocation(int orderId)
        {
            var orderDetails = _orderDetailService.Get(x => x.OrderId == orderId && x.Status == (int)BoxStatus.Used).Select(x => x.BoxId).ToList();
            var boxUsed = await _boxService.Get(x => orderDetails.Any(a => a == x.Id)).ProjectTo<BoxUsedViewModel>(_mapper.ConfigurationProvider).ToListAsync();
            if(boxUsed.Count <= 0) return boxUsed;
            for(int i = 0; i < boxUsed.Count; i++)
            {
                var boxType = _orderDetailService.Get(x => x.OrderId == orderId && x.BoxId == boxUsed[i].BoxId && x.Status == (int)BoxStatus.Used).FirstOrDefault();
                boxUsed[i].BoxType = (int)boxType.Type;
            }
            return boxUsed;
        }

        public async Task<DynamicModelResponse<CustomerOrderViewModel>> GetAllByCustomer(CustomerOrderViewModel model, string[] fields, int page, int size, int customerId)
        {
            var orders = Get(x => x.CustomerId == customerId).OrderByDescending(x => x.PaidTime).ProjectTo<CustomerOrderViewModel>(_mapper.ConfigurationProvider)
                .DynamicFilter(model)
                .Select<CustomerOrderViewModel>(CustomerOrderViewModel.Fields.Union(fields).ToArray().ToDynamicSelector<CustomerOrderViewModel>())
                .PagingIQueryable(page, size, CommonConstant.LimitPaging, CommonConstant.DefaultPaging);
            if (orders.Item2.ToList().Count < 1) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not found");
            var result = await orders.Item2.ToListAsync();
            foreach (var order in result)
            {
                var pendingOrder = _pendingOrderService.Get(x => x.OrderId == order.Id).ToList();
                List<SpacePackage> spacePackageOrder = new List<SpacePackage>();
                foreach (var pending in pendingOrder)
                {
                    spacePackageOrder.AddRange(_spacePackageService.Get(x => x.Id == pending.SpacePackageId).ToList());
                }
                int storageId = 0;
                foreach (var package in spacePackageOrder)
                {
                    if (package.BoxType == "Big")
                    {
                        storageId = package.StorageId;
                        result[result.IndexOf(order)].BigBoxQuantity = (int)package.Quantity;
                        result[result.IndexOf(order)].BigBoxPrice = (int)package.Price;
                    }
                    else
                    {
                        storageId = package.StorageId;
                        result[result.IndexOf(order)].SmallBoxPrice = (int)package.Price;
                        result[result.IndexOf(order)].SmallBoxQuantity = (int)package.Quantity;
                    }
                }
                var storage = _storageService.Get(x => x.Id == storageId).FirstOrDefault();
                result[result.IndexOf(order)].Name = storage.Name;
                result[result.IndexOf(order)].Address = storage.Address;
                result[result.IndexOf(order)].StorageId = storage.Id;
                var user = _userSerivce.Get(x => x.Id == storage.OwnerId).FirstOrDefault();
                result[result.IndexOf(order)].OwnerPhone = user.Phone;
                result[result.IndexOf(order)].OwnerName = user.Name;
                result[result.IndexOf(order)].OwnerAvatar = user.Avatar;
                var feedback = await _feedbackService.GetOrderRating((int)order.Id);
                if (feedback.Select(x => x.Key).First() == -1) result[result.IndexOf(order)].Rating = null;
                else result[result.IndexOf(order)].Rating = feedback.Select(x => x.Key).FirstOrDefault();
                result[result.IndexOf(order)].Comment = feedback.Select(x => x.Value).FirstOrDefault();
                var boxUsed = await GetBoxUsedLocation((int)order.Id);
                result[result.IndexOf(order)].BoxUsed = boxUsed;
            }
            var rs = new DynamicModelResponse<CustomerOrderViewModel>
            {
                Metadata = new PagingMetaData
                {
                    Page = page,
                    Size = size,
                    Total = orders.Item1
                },
                Data = result
            };
            return rs;
        }

        public async Task<DynamicModelResponse<OwnerOrderViewModel>> GetAllByOwner(OwnerOrderViewModel model, string[] fields, int page, int size, int ownerId)
        {
            var orders = Get(x => x.OwnerId == ownerId).OrderByDescending(x => x.PaidTime).ProjectTo<OwnerOrderViewModel>(_mapper.ConfigurationProvider)
                .DynamicFilter(model)
                .Select<OwnerOrderViewModel>(OwnerOrderViewModel.Fields.Union(fields).ToArray().ToDynamicSelector<OwnerOrderViewModel>())
                .PagingIQueryable(page, size, CommonConstant.LimitPaging, CommonConstant.DefaultPaging);
            if (orders.Item2.ToList().Count < 1) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not found");
            var result = await orders.Item2.ToListAsync();
            foreach (var order in result)
            {
                var pendingOrder = _pendingOrderService.Get(x => x.OrderId == order.Id).ToList();
                List<SpacePackage> spacePackageOrder = new List<SpacePackage>();
                foreach (var pending in pendingOrder)
                {
                    spacePackageOrder.AddRange(_spacePackageService.Get(x => x.Id == pending.SpacePackageId).ToList());
                }
                int storageId = 0;
                foreach (var package in spacePackageOrder)
                {
                    if (package.BoxType == "Big")
                    {
                        storageId = package.StorageId;
                        result[result.IndexOf(order)].BigBoxQuantity = (int)package.Quantity;
                        result[result.IndexOf(order)].BigBoxPrice = (int)package.Price;
                    }
                    else
                    {
                        storageId = package.StorageId;
                        result[result.IndexOf(order)].SmallBoxPrice = (int)package.Price;
                        result[result.IndexOf(order)].SmallBoxQuantity = (int)package.Quantity;
                    }
                }
                var storage = _storageService.Get(x => x.Id == storageId).FirstOrDefault();
                result[result.IndexOf(order)].Name = storage.Name;
                result[result.IndexOf(order)].Address = storage.Address;
                result[result.IndexOf(order)].StorageId = storage.Id;
                var feedback = await _feedbackService.GetOrderRating((int)order.Id);
                var CustomerId = Get(x => x.Id == order.Id).Select(a => a.CustomerId).FirstOrDefault();
                var user = _userSerivce.Get(x => x.Id == CustomerId).FirstOrDefault();
                result[result.IndexOf(order)].CustomerPhone = user.Phone;
                result[result.IndexOf(order)].CustomerName = user.Name;
                result[result.IndexOf(order)].CustomerAvatar = user.Avatar;
                if (feedback.Select(x => x.Key).First() == -1) result[result.IndexOf(order)].Rating = null;
                else result[result.IndexOf(order)].Rating = feedback.Select(x => x.Key).FirstOrDefault();
                result[result.IndexOf(order)].Comment = feedback.Select(x => x.Value).FirstOrDefault();
                var boxUsed = await GetBoxUsedLocation((int)order.Id);
                result[result.IndexOf(order)].BoxUsed = boxUsed;
            }
            var rs = new DynamicModelResponse<OwnerOrderViewModel>
            {
                Metadata = new PagingMetaData
                {
                    Page = page,
                    Size = size,
                    Total = orders.Item1
                },
                Data = result
            };
            return rs;
        }

        public async Task<object> GetById(int id, string role, string idToken)
        {
            var secureToken = new JwtSecurityTokenHandler().ReadJwtToken(idToken);
            var uid = secureToken.Claims.First(claim => claim.Type == "user_id").Value;
            var user = _userSerivce.GetUser(uid);
            var userId = user.Id;
            if (role == "Customer") return await GetDetailByCustomer(id, userId);
            return await GetDetailByOwner(id, userId);
        }

        public async Task<CustomerOrderViewModel> GetCustomerOrderById(int Id)
        {
            var result = await Get(x => x.Id == Id).ProjectTo<CustomerOrderViewModel>(_mapper.ConfigurationProvider)
                .DynamicFilter(new CustomerOrderViewModel()).FirstOrDefaultAsync();
           
            return result;
        }

        public async Task<CustomerOrderDetailViewModel> GetDetailByCustomer(int id, int customerId)
        {
            var orderDetail = Get(x => x.CustomerId == customerId && x.Id == id).ProjectTo<CustomerOrderDetailViewModel>(_mapper.ConfigurationProvider).FirstOrDefault();
            if (orderDetail == null) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not found");
            var pendingOrder = _pendingOrderService.Get(x => x.OrderId == orderDetail.Id).ToList();
            List<SpacePackage> spacePackageOrder = new List<SpacePackage>();
            foreach (var pending in pendingOrder)
            {
                spacePackageOrder.AddRange(_spacePackageService.Get(x => x.Id == pending.SpacePackageId).ToList());
            }
            int storageId = 0;
            foreach (var package in spacePackageOrder)
            {
                if (package.BoxType == "Big")
                {
                    storageId = package.StorageId;
                    orderDetail.BigBoxQuantity = (int)package.Quantity;
                    orderDetail.BigBoxPrice = (int)package.Price;
                }
                else
                {
                    storageId = package.StorageId;
                    orderDetail.SmallBoxPrice = (int)package.Price;
                    orderDetail.SmallBoxQuantity = (int)package.Quantity;
                }
            }
            var storage = _storageService.Get(x => x.Id == storageId).FirstOrDefault();
            orderDetail.Name = storage.Name;
            orderDetail.Address = storage.Address;
            orderDetail.StorageId = storage.Id;
            var user = _userSerivce.Get(x => x.Id == storage.OwnerId).FirstOrDefault();
            orderDetail.OwnerPhone = user.Phone;
            orderDetail.OwnerName = user.Name;
            orderDetail.OwnerAvatar = user.Avatar;
            var feedback = await _feedbackService.GetOrderRating((int)orderDetail.Id);
            if (feedback.Select(x => x.Key).First() == -1) orderDetail.Rating = null;
            else orderDetail.Rating = feedback.Select(x => x.Key).FirstOrDefault();
            orderDetail.Comment = feedback.Select(x => x.Value).FirstOrDefault();
            var boxUsed = await GetBoxUsedLocation((int)orderDetail.Id);
            orderDetail.BoxUsed = boxUsed;
            return orderDetail;
        }

        public async Task<OwnerOrderDetailViewModel> GetDetailByOwner(int id, int ownerId)
        {
            var orderDetail = Get(x => x.OwnerId == ownerId && x.Id == id).ProjectTo<OwnerOrderDetailViewModel>(_mapper.ConfigurationProvider).FirstOrDefault();
            if (orderDetail == null) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not found");
            var pendingOrder = _pendingOrderService.Get(x => x.OrderId == orderDetail.Id).ToList();
            List<SpacePackage> spacePackageOrder = new List<SpacePackage>();
            foreach (var pending in pendingOrder)
            {
                spacePackageOrder.AddRange(_spacePackageService.Get(x => x.Id == pending.SpacePackageId).ToList());
            }
            int storageId = 0;
            foreach (var package in spacePackageOrder)
            {
                if (package.BoxType == "Big")
                {
                    storageId = package.StorageId;
                    orderDetail.BigBoxQuantity = (int)package.Quantity;
                    orderDetail.BigBoxPrice = (int)package.Price;
                }
                else
                {
                    storageId = package.StorageId;
                    orderDetail.SmallBoxPrice = (int)package.Price;
                    orderDetail.SmallBoxQuantity = (int)package.Quantity;
                }
            }
            var storage = _storageService.Get(x => x.Id == storageId).FirstOrDefault();
            orderDetail.Name = storage.Name;
            orderDetail.Address = storage.Address;
            orderDetail.StorageId = storage.Id;
            var order = Get(x => x.Id == orderDetail.Id).FirstOrDefault();
            var user = _userSerivce.Get(x => x.Id == order.CustomerId).FirstOrDefault();
            orderDetail.CustomerPhone = user.Phone;
            orderDetail.CustomerName = user.Name;
            orderDetail.CustomerAvatar = user.Avatar;
            var feedback = await _feedbackService.GetOrderRating((int)orderDetail.Id);
            if (feedback.Select(x => x.Key).First() == -1) orderDetail.Rating = null;
            else orderDetail.Rating = feedback.Select(x => x.Key).FirstOrDefault();
            orderDetail.Comment = feedback.Select(x => x.Value).FirstOrDefault();
            var boxUsed = await GetBoxUsedLocation((int)orderDetail.Id);
            orderDetail.BoxUsed = boxUsed;
            return orderDetail;
        }

        public async Task<CustomerOrderViewModel> Payment(OrderPaymentViewModel model, string idToken)
        {
            var secureToken = new JwtSecurityTokenHandler().ReadJwtToken(idToken);
            var uid = secureToken.Claims.First(claim => claim.Type == "user_id").Value;
            var user = _userSerivce.GetUser(uid);
            int storageId = model.StorageId;
            var remaingBox = await _storageService.CountRemainingBoxes(storageId, (DateTime)model.PickupTime);
            var boxBooked = remaingBox - model.SmallBoxQuantity - model.BigBoxQuantity * 2;
            if (boxBooked < 0) throw new ErrorResponse((int)HttpStatusCode.BadRequest, "Box is out of stocked");
            var entity = _mapper.Map<Order>(model);
            entity.CustomerName = user.Name;
            entity.CustomerAvatar = user.Avatar;
            entity.CustomerPhone = user.Phone;
            var ownerId = await _storageService.GetOwnerId(model.StorageId);
            entity.OwnerId = ownerId;
            entity.CustomerId = user.Id;
            await CreateAsync(entity);
            SpacePackageViewModel spaceModel = new SpacePackageViewModel();
            spaceModel.BoxType = "Small";
            spaceModel.Quantity = model.SmallBoxQuantity;
            spaceModel.StorageId = model.StorageId;
            spaceModel.Price = model.SmallBoxPrice;
            var spacePackage = await _spacePackageService.Create(spaceModel);
            PendingOrderViewModel pendingModel = new PendingOrderViewModel();
            pendingModel.OrderId = entity.Id;
            pendingModel.SpacePackageId = spacePackage.Id;
            await _pendingOrderService.Create(pendingModel);
            spaceModel.BoxType = "Big";
            spaceModel.Price = model.BigBoxPrice;
            spaceModel.Quantity = model.BigBoxQuantity;
            var spacePackage2 = await _spacePackageService.Create(spaceModel);
            pendingModel.SpacePackageId = spacePackage2.Id;
            await _pendingOrderService.Create(pendingModel);
            return await GetCustomerOrderById(entity.Id);
        }
    }

    public interface ISubOrderSerivce : IBaseService<Order>
    {
    }
    public class SubOrderService : BaseService<Order>, ISubOrderSerivce
    {
        public SubOrderService(IUnitOfWork unitOfWork, IOrderRepository repository) : base(unitOfWork, repository)
        {
        }
    }
}
