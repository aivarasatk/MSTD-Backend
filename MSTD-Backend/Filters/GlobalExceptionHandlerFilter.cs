using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MSTD_Backend.Models.Response;
using Serilog;

namespace MSTD_Backend.Filters
{
    public class GlobalExceptionHandlerFilter : IExceptionFilter
    {
        private readonly ILogger _logger;

        public GlobalExceptionHandlerFilter(ILogger logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.Warning(context.Exception, "Unexpected server error occured");
            var internalServerError = new ObjectResult(new ResponseMessage("Unexpected server error occured", context.Exception.Message))
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
            context.Result = internalServerError;
        }
    }
}
