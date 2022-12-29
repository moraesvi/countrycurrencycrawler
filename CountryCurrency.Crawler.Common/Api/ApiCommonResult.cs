using Microsoft.Extensions.Logging;
using System;

namespace CountryCurrency.Crawler.Common
{
    public class CommonResult
    {
        private bool _hasSuccessResult;
        public CommonResult() 
        {
            _hasSuccessResult = false;
        }
        public CommonResult ApiResult<TResult>(TResult result)
        {
            _hasSuccessResult = true;

            Result = result;
            return this;
        }
        public CommonResult ApiResult(string msg)
        {
            Message = msg;

            return this;
        }
        public CommonResult ApiResult(ILogger log, Exception ex, bool showErrorDetails = false)
        {            
            log.LogError(string.Concat(ex.Message, "\n", ex.InnerException?.Message));
            log.LogError(ex.StackTrace);

            if (showErrorDetails)
            {
                Message = string.Concat(ex.Message, "\n", ex.InnerException?.Message);
                Stacktrace = ex.StackTrace;
            }

            return this;
        }
        public bool Ok => _hasSuccessResult;
        public object Result { get; private set; }
        public string Message { get; private set; }
        public string Stacktrace { get; private set; }
    }
}
