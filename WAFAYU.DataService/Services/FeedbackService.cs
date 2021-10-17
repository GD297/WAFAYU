using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
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
    public interface IFeedbackService : IBaseService<Feedback>
    {
        Task<FeedbackCreateViewModel> Create(FeedbackCreateViewModel model, string idToken);
        Task<FeedbackCreateViewModel> Update(int orderId, int storageId, FeedbackCreateViewModel model);
        Task<DynamicModelResponse<FeedbackViewModel>> GetAll(int storageId, string[] fields, int page, int size);
        Task<Dictionary<int, double?>> GetStorageRating(int storageId);
        Task<Dictionary<double?, string>> GetOrderRating(int OrderId);
    }
    public class FeedbackService : BaseService<Feedback>, IFeedbackService
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        public FeedbackService(IUnitOfWork unitOfWork, IFeedbackRepository repository, IMapper mapper, IUserService userService) : base(unitOfWork, repository)
        {
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<FeedbackCreateViewModel> Create(FeedbackCreateViewModel model, string idToken)
        {
            var secureToken = new JwtSecurityTokenHandler().ReadJwtToken(idToken);
            var uid = secureToken.Claims.First(claim => claim.Type == "user_id").Value;
            var user = _userService.GetUser(uid);
            var entity = _mapper.Map<Feedback>(model);
            entity.UserId = user.Id;
            await CreateAsync(entity);
            return model;
        }

        public async Task<DynamicModelResponse<FeedbackViewModel>> GetAll(int storageId, string[] fields, int page, int size)
        {
            var feedbacks = await Get(x => x.StorageId == storageId).ProjectTo<FeedbackViewModel>(_mapper.ConfigurationProvider).ToListAsync();
            var rs = new DynamicModelResponse<FeedbackViewModel>
            {
                Metadata = new PagingMetaData
                {
                    Page = page,
                    Size = size,
                    Total = feedbacks.Count
                },
                Data = feedbacks
            };
            return rs;
        }

        public async Task<Dictionary<double?, string>> GetOrderRating(int OrderId)
        {
            var feedback = await Get(x => x.OrderId == OrderId).FirstOrDefaultAsync();
            Dictionary<double?, string> result = new Dictionary<double?, string>();
            if (feedback != null) result.Add(feedback.Rating, feedback.Comment);
            else result.Add(-1, null);
            return result;
        }

        public async Task<Dictionary<int, double?>> GetStorageRating(int storageId)
        {
            var feedbacks = await Get(x => x.StorageId == storageId).ToListAsync();
            Dictionary<int, double?> result = new Dictionary<int, double?>();
            result.Add(0, null);
            if (feedbacks.Count == 0) return result;
            result.Clear();
            double rating = 0;
            int total = 0;
            foreach (var feedback in feedbacks)
            {
                total++;
                rating += (float)feedback.Rating;
            }
            result.Add(total, rating / total);
            return result;
        }

        public async Task<FeedbackCreateViewModel> Update(int orderId, int storageId, FeedbackCreateViewModel model)
        {
            if (orderId != model.OrderId || storageId != model.StorageId) throw new ErrorResponse((int)HttpStatusCode.NotFound, "Can not found");
            var entity = Get(x => x.OrderId == orderId && x.StorageId == storageId).FirstOrDefault();
            var updateEntity = _mapper.ConfigurationProvider.CreateMapper().Map(model, entity);
            await UpdateAsync(updateEntity);
            return model;
        }
    }
}
