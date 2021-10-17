using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using WAFAYU.DataService.Responses;
using WAFAYU.DataService.Services;
using WAFAYU.DataService.ViewModels;

namespace WAFAYU.WebAPI.Controllers
{
    [Route("api/v{version:apiVersion}/order-details")]
    [ApiController]
    [ApiVersion("1")]
    public class OrderDetailController : ControllerBase
    {
        private readonly IOrderDetailService _orderDetailService;
        public OrderDetailController(IOrderDetailService orderDetailService)
        {
            _orderDetailService = orderDetailService;
        }
        /// <summary>
        /// Create order detail
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [MapToApiVersion("1")]
        [Authorize]
        [ProducesResponseType(typeof(OrderDetailListViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Add(OrderDetailListViewModel model)
        {
            var result = await _orderDetailService.Create(model);
            return Ok("Added successful");
        }
        /// <summary>
        /// Update order detail
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <returns>ReportAttributeCategoryUpdateModel</returns>
        [MapToApiVersion("1")]
        [MapToApiVersion("2")]
        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Update(int id, OrderDetailListUpdateViewModel entity)
        {
            await _orderDetailService.Update(id, entity);
            return Ok("Update success");
        }
        /// <summary>
        /// Delete order detail
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [MapToApiVersion("1")]
        [MapToApiVersion("2")]
        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Delete(int id, OrderDetailDeleteViewModel model)
        {
            await _orderDetailService.Delete(id, model);
            return Ok("Delete success");
        }
    }
}
