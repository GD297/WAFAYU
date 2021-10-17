using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using WAFAYU.DataService.Constants;
using WAFAYU.DataService.Responses;
using WAFAYU.DataService.Services;
using WAFAYU.DataService.ViewModels;

namespace WAFAYU.WebAPI.Controllers
{
    [Route("api/v{version:apiVersion}/orders")]
    [ApiController]
    [ApiVersion("1")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        /// <summary>
        /// Get all orders
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        [MapToApiVersion("1")]
        [Authorize]
        [ProducesResponseType(typeof(DynamicModelResponse<CustomerOrderViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(DynamicModelResponse<OwnerOrderViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Get([FromQuery] string[] fields, int page = CommonConstant.DefaultPage, int size = CommonConstant.DefaultPaging)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var role = claimsIdentity.FindFirst(ClaimTypes.Role)?.Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            return Ok(await _orderService.GetAll(role, fields, page, size, accessToken));
        }
        /// <summary>
        /// Get Order by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [MapToApiVersion("1")]
        [Authorize]
        [ProducesResponseType(typeof(DynamicModelResponse<StorageViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var role = claimsIdentity.FindFirst(ClaimTypes.Role)?.Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            return Ok(await _orderService.GetById(id, role, accessToken));
        }
        /// <summary>
        /// User payment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [MapToApiVersion("1")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Payment(OrderPaymentViewModel model)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            return Ok(await _orderService.Payment(model, accessToken));
        }
    }
}
