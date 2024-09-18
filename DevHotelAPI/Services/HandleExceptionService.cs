using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ILogger = Serilog.ILogger;
namespace DevHotelAPI.Services
{
    public class HandleExceptionService : ControllerBase
    {
        private readonly ILogger _logger;
        public HandleExceptionService(ILogger logger) 
        {
            _logger = logger;
        }

        public ActionResult HandleException(Exception ex, Guid id, string action, string entity)
        {
            if (!(ex is UnauthorizedAccessException || ex is ArgumentNullException))
                _logger.Error(ex.Message, ex);

            return ex switch
            {
                UnauthorizedAccessException => BadRequest(ex.Message),
                ArgumentNullException => NotFound(ex.Message),
                DbUpdateException => BadRequest($"Database error {action} {entity} with id {id}."),
                _ => BadRequest($"Error {action} {entity} with id {id}.")
            };
        }

    }
}
