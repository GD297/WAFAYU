using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using WAFAYU.DataService.Constants;
using WAFAYU.DataService.Responses;
using WAFAYU.DataService.Services;
using WAFAYU.DataService.ViewModels;

namespace WAFAYU.WebAPI.Controllers
{
    [Route("api/v{version:apiVersion}/feedbacks")]
    [ApiController]
    [ApiVersion("1")]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;
        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }
        /// <summary>
        /// Get feedbacks by StorageId
        /// </summary>
        /// <param name="StorageId"></param>
        /// <param name="fields"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        [MapToApiVersion("1")]
        [Authorize]
        [ProducesResponseType(typeof(DynamicModelResponse<FeedbackViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Get([FromQuery] int StorageId, [FromQuery] string[] fields, int page = CommonConstant.DefaultPage, int size = CommonConstant.DefaultPaging)
        {
            return Ok(await _feedbackService.GetAll(StorageId, fields, page, size));
        }
        /// <summary>
        /// Create new feedback
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [MapToApiVersion("1")]
        [Authorize]
        [ProducesResponseType(typeof(FeedbackCreateViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Add(FeedbackCreateViewModel model)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var result = await _feedbackService.Create(model, accessToken);
            return Ok(result);
        }
        /// <summary>
        /// Update feedback
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="storageId"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        [MapToApiVersion("1")]
        [MapToApiVersion("2")]
        [Authorize]
        [HttpPut]
        [ProducesResponseType(typeof(FeedbackCreateViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Update(int orderId, int storageId, FeedbackCreateViewModel entity)
        {
            var result = await _feedbackService.Update(ord3erId, storageId, entity);
            return Ok(result);
        }
    }
}
