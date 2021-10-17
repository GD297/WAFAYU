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
    [Route("api/v{version:apiVersion}/shelves")]
    [ApiController]
    [ApiVersion("1")]
    public class ShelfController : ControllerBase
    {
        private readonly IShelfService _shelfService;
        public ShelfController(IShelfService shelfSerivce)
        {
            _shelfService = shelfSerivce;
        }
        /// <summary>
        /// Get all shelves
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fields"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        [MapToApiVersion("1")]
        [Authorize]
        [ProducesResponseType(typeof(DynamicModelResponse<ShelfViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Get([FromQuery] ShelfViewModel model, [FromQuery] string[] fields, int page = CommonConstant.DefaultPage, int size = CommonConstant.DefaultPaging)
        {
            return Ok(await _shelfService.GetAll(model, fields, page, size));
        }
        /// <summary>
        /// Get Shelf by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [MapToApiVersion("1")]
        [Authorize]
        [ProducesResponseType(typeof(ShelfDetailViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            return Ok(await _shelfService.GetById(id));
        }
        /// <summary>
        /// Create shelf
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [MapToApiVersion("1")]
        [Authorize]
        [ProducesResponseType(typeof(StorageCreateSuccessViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Add(ShelfCreateViewModel model)
        {
            var result = await _shelfService.Create(model);
            return Ok(result);
        }
        /// <summary>
        /// Delete shelf
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [MapToApiVersion("1")]
        [MapToApiVersion("2")]
        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            await _shelfService.Delete(id);
            return Ok("Deleted");
        }
    }
}
