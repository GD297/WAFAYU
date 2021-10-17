using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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
    public interface IShelfService : IBaseService<Shelf>
    {
        bool CheckBoxIsUsed(int storageId);
        decimal[] GetBoxPrice(int storageId);
        Task<DynamicModelResponse<ShelfViewModel>> GetAll(ShelfViewModel model, string[] fields, int page, int size);
        Task<Shelf> Create(int StorageId);
        Task<ShelfViewModel> Create(ShelfCreateViewModel model);
        Task<Shelf> Delete(int Id);
        Task<ShelfDetailViewModel> GetById(int id);
        Task<int> CountBoxRemaining(List<ShelfViewModel> shelves);
    }
    public class ShelfService : BaseService<Shelf>, IShelfService
    {
        private readonly IBoxService _boxService;
        private readonly ISubOrderDetailService _orderDetailService;
        private readonly IConfigurationProvider _mapper;
        public ShelfService(IUnitOfWork unitOfWork, IShelfRepository repository, IBoxService boxService, IMapper mapper, ISubOrderDetailService orderDetailService) : base(unitOfWork, repository)
        {
            _boxService = boxService;
            _mapper = mapper.ConfigurationProvider;
            _orderDetailService = orderDetailService;
        }

        public bool CheckBoxIsUsed(int storageId)
        {
            var shelves = Get(x => x.StorageId == storageId).ToList();
            if (shelves == null) return true;
            _boxService.CheckBoxIsUsed(shelves);
            return true;
        }
        public bool CheckBoxIsUsedByShelfId(int shelfId)
        {
            _boxService.CheckBoxIsUsedInShelf(shelfId);
            return true;
        }

        public async Task<int> CountBoxRemaining(List<ShelfViewModel> shelves)
        {
            return await _boxService.CountBoxRemaining(shelves);
        }

        public async Task<Shelf> Create(int storageId)
        {
            var entity = new Shelf();
            entity.StorageId = storageId;
            entity.Status = 1;
            await CreateAsync(entity);
            List<string> boxPositions = BoxPositionConstant.BoxPosition;
            for (int i = 0; i < 12; i++)
            {
                var boxPosition = boxPositions[i];
                await _boxService.Create(entity.Id, boxPosition);
            }
            return entity;
        }

        public async Task<ShelfViewModel> Create(ShelfCreateViewModel model)
        {
            var result = await Create((int)model.StorageId);
            var shelf = Get(x => x.Id == result.Id).ProjectTo<ShelfViewModel>(_mapper).FirstOrDefault();
            return shelf;
        }

        public async Task<Shelf> Delete(int Id)
        {
            var entity = await Get(x => x.Id == Id && x.Status == 1).Include(x => x.Boxes).FirstOrDefaultAsync();
            if (entity == null) throw new ErrorResponse((int)HttpStatusCode.BadRequest, "Can not found");
            CheckBoxIsUsedByShelfId(entity.Id);
            entity.Status = 0;
            await UpdateAsync(entity);
            return entity;
        }

        public async Task<DynamicModelResponse<ShelfViewModel>> GetAll(ShelfViewModel model, string[] fields, int page, int size)
        {
            if (model.StorageId == null) throw new ErrorResponse((int)HttpStatusCode.BadRequest, "Id can not null");
            var shelves = Get(x => x.StorageId == model.StorageId && x.Status != 0).ProjectTo<ShelfViewModel>(_mapper)
                .DynamicFilter(model)
                .Select<ShelfViewModel>(ShelfViewModel.Fields.Union(fields).ToArray().ToDynamicSelector<ShelfViewModel>())
                .PagingIQueryable(page, size, CommonConstant.LimitPaging, CommonConstant.DefaultPaging);
            if (shelves.Item2.ToList().Count < 1) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not found");
            var rs = new DynamicModelResponse<ShelfViewModel>
            {
                Metadata = new PagingMetaData
                {
                    Page = page,
                    Size = size,
                    Total = shelves.Item1
                },
                Data = await shelves.Item2.ToListAsync()
            };
            return rs;
        }

        public decimal[] GetBoxPrice(int storageId)
        {
            if (Get(x => x.StorageId == storageId && x.Status == 1).FirstOrDefault() == null) return new decimal[2] { 0, 0 };
            int shelfId = Get(x => x.StorageId == storageId && x.Status == 1).FirstOrDefault().Id;
            return _boxService.GetPrice(shelfId);
        }

        public async Task<ShelfDetailViewModel> GetById(int id)
        {
            var shelf = await Get(x => x.Id == id && x.Status == 1).Include(x => x.Boxes).ProjectTo<ShelfDetailViewModel>(_mapper).FirstOrDefaultAsync();
            var listBoxesInShelf = shelf.Boxes.ToList();
            List<int> indexes = new List<int>();
            var listBox = shelf.Boxes.Select(s => s.Id).ToList();
            for (int i = 0; i < listBoxesInShelf.Count; i++)
            {
                if (listBoxesInShelf[i].Status == (int)BoxStatus.Used)
                {
                    var orderDetail = _orderDetailService.Get(x => x.BoxId == listBoxesInShelf[i].Id && x.Status != 0).FirstOrDefault();
                    if (orderDetail != null)
                    {
                        listBoxesInShelf[i].Type = orderDetail.Type;
                        listBoxesInShelf[i].OrderId = (int)orderDetail.OrderId;
                        if (orderDetail.BoxId2 != null)
                        {
                            listBoxesInShelf[i].BoxId2 = (int)orderDetail.BoxId2;
                            var inde = listBox.IndexOf((int)orderDetail.BoxId2);
                            indexes.Add(listBox.IndexOf((int)orderDetail.BoxId2));
                        }
                    }
                }
            }
            if (indexes.Count > 0)
            {
                for (int i = indexes.Count - 1; i >= 0; i--)
                {
                    listBoxesInShelf.RemoveAt(indexes[i]);
                }
            }
            shelf.Boxes = listBoxesInShelf;
            return shelf;
        }
    }
}
