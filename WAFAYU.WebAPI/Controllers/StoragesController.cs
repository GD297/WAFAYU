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
    [Route("api/v{version:apiVersion}/storages")]
    [ApiController]
    [ApiVersion("1")]
    public class StoragesController : ControllerBase
    {
        private readonly IStorageService _storageService;
        public StoragesController(IStorageService storageService)
        {
            _storageService = storageService;
        }
        /// <summary>
        /// Get all storages
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fields"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        [MapToApiVersion("1")]
        [Authorize]
        [ProducesResponseType(typeof(DynamicModelResponse<StorageViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Get([FromQuery] StorageViewModel model, [FromQuery] string[] fields, int page = CommonConstant.DefaultPage, int size = CommonConstant.DefaultPaging)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            return Ok(await _storageService.GetAll(model, fields, page, size, accessToken));
        }
        /// <summary>
        /// Get storage by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [MapToApiVersion("1")]
        [Authorize]
        [ProducesResponseType(typeof(DynamicModelResponse<StorageDetailViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetDetail(int id)
        {
            return Ok(await _storageService.GetById(id));
        }
        /// <summary>
        /// Create new storage
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [MapToApiVersion("1")]
        [Authorize(Roles = "Owner")]
        [ProducesResponseType(typeof(StorageCreateSuccessViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Add(StorageCreateViewModel model)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var result = await _storageService.Create(model, accessToken);
            return Ok(result);
        }
        /// <summary>
        /// Update storage
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
        public async Task<IActionResult> Update(int id, StorageUpdateViewModel entity)
        {
            await _storageService.Update(id, entity);
            return Ok("Update success");
        }
        /// <summary>
        /// Delete storage
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
            await _storageService.Delete(id);
            return Ok("Deleted");
        }
    }
}
