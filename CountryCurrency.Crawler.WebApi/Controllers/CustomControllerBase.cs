using CountryCurrency.Crawler.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace CountryCurrency.Crawler.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class CustomControllerBase : ControllerBase
    {
        private readonly ILogger _logger;
        public CustomControllerBase(ILogger logger) 
        {
            _logger = logger;
        }
        public ActionResult OkResult<TResult>(TResult result)
        {
            return Ok(new CommonResult().ApiResult(result));
        }
        public ActionResult ForbidResult(string msg)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new CommonResult().ApiResult(msg));
        }
        public ActionResult BadRequestResult<TResult>(Exception ex)
        {
            return BadRequest(new CommonResult().ApiResult(_logger, ex));
        }
    }
}
