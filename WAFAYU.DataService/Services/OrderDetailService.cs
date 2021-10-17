using AutoMapper;
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
using WAFAYU.DataService.Utilities;
using WAFAYU.DataService.ViewModels;

namespace WAFAYU.DataService.Services
{
    public interface IOrderDetailService : IBaseService<OrderDetail>
    {
        Task<OrderDetailListViewModel> Create(OrderDetailListViewModel model);
        Task<OrderDetailListUpdateViewModel> Update(int id, OrderDetailListUpdateViewModel model);
        Task<bool> Delete(int orderId, OrderDetailDeleteViewModel model);
    }
    public class OrderDetailService : BaseService<OrderDetail>, IOrderDetailService
    {
        private readonly IMapper _mapper;
        private readonly IBoxService _boxService;
        private readonly ISubOrderSerivce _orderService;
        private readonly IUserService _userService;
        public OrderDetailService(IUnitOfWork unitOfWork, IOrderDetailRepository repository, IMapper mapper, IBoxService boxService, ISubOrderSerivce orderService, IUserService userService) : base(unitOfWork, repository)
        {
            _mapper = mapper;
            _boxService = boxService;
            _orderService = orderService;
            _userService = userService;
        }

        public async Task<OrderDetailListViewModel> Create(OrderDetailListViewModel model)
        {
            var entities = _mapper.Map<ICollection<OrderDetailViewModel>, ICollection<OrderDetail>>(model.OrderDetails);
            int? orderId = null;
            foreach (var entity in entities)
            {
                if (orderId != null)
                    if (orderId != entity.OrderId) throw new ErrorResponse((int)HttpStatusCode.BadRequest, "orderId not matched");
                orderId = entity.OrderId;
            }
            foreach (var entity in entities)
            {
                await CreateAsync(entity);
                await _boxService.UpdateBoxStatus(entity.BoxId, entity.BoxId2, (int)BoxStatus.Used, entity.BoxCode);
            }
            var order = await _orderService.GetAsync((int)orderId);
            order.Status = (int)OrderStatus.CheckIn;
            await _orderService.UpdateAsync(order);
            if (model.MailMessage == null) return model;
            int customerId = (int)order.CustomerId;
            int ownerId = (int)order.OwnerId;
            var customer = await _userService.GetAsync(customerId);
            var owner = await _userService.GetAsync(ownerId);
            await MailUtils.SendMailGoogleSmtp(owner.Email, customer.Email, "Wafayu Add Box", model.MailMessage, "owner3213@gmail.com", "Aa12345@");
            return model;
        }

        public async Task<OrderDetailListUpdateViewModel> Update(int id, OrderDetailListUpdateViewModel model)
        {
            var entities = _mapper.Map<ICollection<OrderDetailUpdateViewModel>, ICollection<OrderDetail>>(model.OrderDetails);
            List<OrderDetail> orderDetails = (List<OrderDetail>)entities;
            foreach (var entity in entities) if (entity.OrderId != id) throw new ErrorResponse((int)HttpStatusCode.BadRequest, "Id not matched");
            foreach (var entity in entities)
            {
                await _boxService.UpdateBoxStatus(entity.BoxId, entity.BoxId2, (int)entity.Status, entity.BoxCode);
            }
            OrderDetail orderDetailToUpdate = null;
            for (int i = 0; i < orderDetails.Count; i++)
            {
                if(i%2 == 0)
                {
                    await _boxService.UpdateBoxStatus(orderDetails[i].BoxId, orderDetails[i].BoxId2, (int)orderDetails[i].Status, null);
                    orderDetailToUpdate = orderDetails[i];
                }
                else
                {
                    var orderToUpdate = Get(x => x.BoxId == orderDetailToUpdate.BoxId && x.BoxId2 == orderDetailToUpdate.BoxId2).FirstOrDefault();
                    Delete(orderToUpdate);
                    orderToUpdate.BoxId = orderDetails[i].BoxId;
                    orderToUpdate.BoxId2 = orderDetails[i].BoxId2;
                    OrderDetail orderToCreate = new OrderDetail();
                    orderToCreate.BoxId = orderDetails[i].BoxId;
                    orderToCreate.BoxId2 = orderDetails[i].BoxId2;
                    orderToCreate.OrderId = orderDetails[i].OrderId;
                    orderToCreate.Type = orderDetails[i].Type;
                    orderToCreate.Status = orderDetails[i].Status;
                    orderToCreate.Price = orderDetails[i].Price;
                    Create(orderToCreate);
                    await _boxService.UpdateBoxStatus(orderDetails[i].BoxId, orderDetails[i].BoxId2, (int)orderDetails[i].Status, orderDetails[i].BoxCode);
                }

                
            }
            if (model.MailMessage == null) return model;
            var order = await _orderService.GetAsync(id);
            int customerId = (int)order.CustomerId;
            int ownerId = (int)order.OwnerId;
            var customer = await _userService.GetAsync(customerId);
            var owner = await _userService.GetAsync(ownerId);
            await MailUtils.SendMailGoogleSmtp(owner.Email, customer.Email, "Wafayu Move Box", model.MailMessage, "owner3213@gmail.com", "Aa12345@");
            return model;
        }

        public async Task<bool> Delete(int orderId, OrderDetailDeleteViewModel model)
        {
            if(model.OrderId != orderId) throw new ErrorResponse((int)HttpStatusCode.BadRequest, "Id not matched");
            var entities = await Get(x => x.OrderId == orderId && x.Status != 0).ToListAsync();
            if (entities.Count == 0) throw new ErrorResponse((int)HttpStatusCode.BadRequest, "Can not found orderId");
            foreach(var entity in entities)
            {
                var box = _boxService.Get(x => x.Id == entity.BoxId).FirstOrDefault();
                box.Status = (int)BoxStatus.Active;
                box.BoxCode = null;
                _boxService.Update(box);
                if(entity.BoxId2 != null)
                {
                    var box2 = _boxService.Get(x => x.Id == entity.BoxId2).FirstOrDefault();
                    box2.Status = (int)BoxStatus.Active;
                    box2.BoxCode = null;
                    _boxService.Update(box2);
                }
                entity.Status = 0;
                await UpdateAsync(entity);
            }
            var orderToUpdate = await _orderService.GetAsync(orderId);
            orderToUpdate.Status = (int)OrderStatus.CheckOut;
            await _orderService.UpdateAsync(orderToUpdate);
            if (model.MailMessage == null) return true;
            var order = await _orderService.GetAsync(orderId);
            int customerId = (int)order.CustomerId;
            int ownerId = (int)order.OwnerId;
            var customer = await _userService.GetAsync(customerId);
            var owner = await _userService.GetAsync(ownerId);
            await MailUtils.SendMailGoogleSmtp(owner.Email, customer.Email, "Wafayu Move Box", model.MailMessage, "owner3213@gmail.com", "Aa12345@");
            return true;
        }
    }
    public interface ISubOrderDetailService : IBaseService<OrderDetail>
    {
    }
    public class SubOrderDetailService : BaseService<OrderDetail>, ISubOrderDetailService
    {
        public SubOrderDetailService(IUnitOfWork unitOfWork, IOrderDetailRepository repository) : base(unitOfWork, repository)
        {
        }
    }
}
