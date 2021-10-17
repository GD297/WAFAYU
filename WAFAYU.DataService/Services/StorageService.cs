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
    public interface IStorageService : IBaseService<Storage>
    {
        Task<DynamicModelResponse<StorageViewModel>> GetAll(StorageViewModel model, string[] fields, int page, int size, string idToken);
        Task<StorageCreateSuccessViewModel> Create(StorageCreateViewModel model, string idToken);
        Task<Storage> Update(int id, StorageUpdateViewModel model);
        Task<Storage> Delete(int id);
        Task<int> GetOwnerId(int id);
        Task<StorageDetailViewModel> GetById(int id);
        Task<int> CountRemainingBoxes(int storageId, DateTime timeFrom);
    }
    public class StorageService : BaseService<Storage>, IStorageService
    {
        private readonly IMapper _mapper;
        private readonly IShelfService _shelfService;
        private readonly ISpacePackageService _spacePackageService;
        private readonly IUserService _userService;
        private readonly IImageService _imageService;
        private readonly IFeedbackService _feedbackService;
        public StorageService(IUnitOfWork unitOfWork, IStorageRepository repository, IMapper mapper, IShelfService shelfService, ISpacePackageService spacePackageService, IUserService userService, IImageService imageService, IFeedbackService feedbackService) : base(unitOfWork, repository)
        {
            _mapper = mapper;
            _shelfService = shelfService;
            _userService = userService;
            _spacePackageService = spacePackageService;
            _imageService = imageService;
            _feedbackService = feedbackService;
        }

        public async Task<int> CountRemainingBoxes(int storageId, DateTime timeFrom)
        {
            var storages = await Get(x => x.Id == storageId).Include(x => x.Shelves).FirstOrDefaultAsync();
            var result = _mapper.Map<StorageDetailViewModel>(storages);
            // count in shelf
            var shelves = result.Shelves.ToList();
            int box = await _shelfService.CountBoxRemaining(shelves);

            // count in order
            var boxInUsed = await _spacePackageService.CountBoxBooked(result.Id, timeFrom);

            return box - boxInUsed;
        }

        public async Task<StorageCreateSuccessViewModel> Create(StorageCreateViewModel model, string idToken)
        {
            var secureToken = new JwtSecurityTokenHandler().ReadJwtToken(idToken);
            var uid = secureToken.Claims.First(claim => claim.Type == "user_id").Value;
            var user = _userService.GetUser(uid);
            var entity = _mapper.Map<Storage>(model);
            entity.OwnerId = user.Id;
            entity.Status = (int)StorageStatus.Pending;
            await CreateAsync(entity);
            for (int i = 0; i < model.ShelvesQuantity; i++)
            {
                await _shelfService.Create(entity.Id);
            }
            SpacePackageViewModel spaceModel = new SpacePackageViewModel();
            spaceModel.BoxType = "Small";
            spaceModel.Quantity = 12;
            spaceModel.StorageId = entity.Id;
            spaceModel.Price = model.SmallBoxPrice;
            await _spacePackageService.Create(spaceModel);
            spaceModel.BoxType = "Big";
            spaceModel.Price = model.BigBoxPrice;
            await _spacePackageService.Create(spaceModel);
            var result = _mapper.Map<StorageCreateSuccessViewModel>(entity);
            return result;
        }

        public async Task<Storage> Delete(int id)
        {
            var entity = await GetAsync(id);
            if (entity == null)
                throw new ErrorResponse((int)HttpStatusCode.NotFound, "Not found");
            var result = _shelfService.CheckBoxIsUsed(id);
            if (!result) throw new ErrorResponse((int)HttpStatusCode.BadRequest, "Box is being used");
            entity.Status = 0;
            await UpdateAsync(entity);
            return entity;
        }

        public async Task<DynamicModelResponse<StorageViewModel>> GetAll(StorageViewModel model, string[] fields, int page, int size, string idToken)
        {
            var secureToken = new JwtSecurityTokenHandler().ReadJwtToken(idToken);
            var uid = secureToken.Claims.First(claim => claim.Type == "user_id").Value;
            var user = _userService.GetUser(uid);
            var storages = Get(x => x.Status == (int)StorageStatus.Accepted).ProjectTo<StorageViewModel>(_mapper.ConfigurationProvider);
            if (user.RoleId == 1)
            {
                storages = Get(x => x.Status != (int)StorageStatus.Deleted && x.OwnerId == user.Id).ProjectTo<StorageViewModel>(_mapper.ConfigurationProvider);
            }
            List<StorageViewModel> listStorages = storages.ToList();
            if (listStorages.Count == 0) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not found");
            for (int i = 0; i < listStorages.Count; i++)
            {
                decimal[] prices = _spacePackageService.GetBoxPrice((int)listStorages[i].Id);
                listStorages[i].PriceFrom = prices[0];
                listStorages[i].PriceTo = prices[1];
                var owner = await _userService.GetUser((int)listStorages[i].OwnerId);
                if (owner != null)
                {
                    listStorages[i].OwnerName = owner.Name;
                    listStorages[i].OwnerPhone = owner.Phone;
                    listStorages[i].OwnerAvatar = owner.Avatar;
                }
                var ratings = await _feedbackService.GetStorageRating((int)listStorages[i].Id);
                listStorages[i].NumberOfRatings = ratings.FirstOrDefault().Key;
                listStorages[i].Rating = ratings.FirstOrDefault().Value;
                listStorages[i].RemainingBoxes = await CountRemainingBoxes((int)listStorages[i].Id, DateTime.Now);
            }
            if(model.IsSortedPrice != null)
            {
                if ((bool)model.IsSortedPrice)
                {
                    storages = listStorages.AsQueryable().OrderBy(s => s.PriceFrom);
                }
                else
                {
                    storages = listStorages.AsQueryable().OrderByDescending(s => s.PriceFrom);
                }
                model.IsSortedPrice = null;
            }
            else if (model.IsSortedRating != null)
            {
                if ((bool)model.IsSortedRating)
                {
                    storages = listStorages.AsQueryable().OrderByDescending(s => s.Rating);
                }
                else
                {
                    storages = listStorages.AsQueryable().OrderBy(s => s.Rating);
                }
                model.IsSortedRating = null;
            }
            else
            {
                storages = listStorages.AsQueryable().OrderByDescending(s => s.Rating).ThenBy(s => s.PriceFrom);
            }
            

            var result = storages.DynamicFilter(model)
                .PagingIQueryable(page, size, CommonConstant.LimitPaging, CommonConstant.DefaultPaging);
            if (result.Item2.ToList().Count < 1) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not found");
            var rs = new DynamicModelResponse<StorageViewModel>
            {
                Metadata = new PagingMetaData
                {
                    Page = page,
                    Size = size,
                    Total = result.Item1
                },
                Data = result.Item2.ToList()
            };
            return rs;
        }

        public async Task<StorageDetailViewModel> GetById(int id)
        {
            var storages = await Get(x => x.Id == id && x.Status != 0).Include(x => x.Shelves)
                .Include(x => x.Images).FirstOrDefaultAsync();
            if (storages == null) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Storage Not Found");
            var result = _mapper.Map<StorageDetailViewModel>(storages);
            result.RemainingBoxes = await CountRemainingBoxes(id, DateTime.Now);
            return result;
        }

        public async Task<int> GetOwnerId(int id)
        {
            var storage = await GetAsync(id);
            if (storage == null) throw new ErrorResponse((int)HttpStatusCode.BadRequest, "Storage not found");
            return (int)storage.OwnerId;
        }

        public async Task<Storage> Update(int id, StorageUpdateViewModel model)
        {
            var imagesNoTracking = Get(x => x.Id == id).Include(x => x.Images).AsNoTracking().FirstOrDefault();
            var entity = await GetAsync(id);
            if (entity == null) throw new ErrorResponse((int)HttpStatusCode.BadRequest, "Storage not found");
            var updateEntity = _mapper.ConfigurationProvider.CreateMapper().Map(model, entity);
            await UpdateAsync(updateEntity);

            entity = await Get(x => x.Id == id).Include(x => x.Images).FirstOrDefaultAsync();

            ICollection<Image> images = null;
            if (entity.Images != null && entity.Images.Count > 0) images = imagesNoTracking.Images;
            entity = await Get(x => x.Id == id).FirstOrDefaultAsync();

            var listImageIdToDelete = new List<int>();
            if (images != null)
            {
                var listImagesIdUpdate = model.Images.Select(x => x.Id).ToList();
                foreach (var image in images)
                {
                    if (!listImagesIdUpdate.Contains(image.Id)) listImageIdToDelete.Add(image.Id);
                }
            }

            if (listImageIdToDelete.Count > 0)
            {
                foreach (var imageId in listImageIdToDelete)
                {
                    await _imageService.Delete(imageId);
                }
            }
            var bigBoxEntity = _spacePackageService.Get(entity.Id, "Big", 12);
            bigBoxEntity.Price = model.PriceTo;
            var smallBoxEntity = _spacePackageService.Get(entity.Id, "Small", 12);
            smallBoxEntity.Price = model.PriceFrom;
            await _spacePackageService.UpdateSpacePackagePrice(bigBoxEntity, smallBoxEntity);
            return updateEntity;
        }
    }
}
