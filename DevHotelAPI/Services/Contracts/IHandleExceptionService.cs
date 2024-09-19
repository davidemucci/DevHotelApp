using Microsoft.AspNetCore.Mvc;

namespace DevHotelAPI.Services.Contracts
{
    public interface IHandleExceptionService
    {
        public ActionResult HandleException(Exception ex, Guid id, string action, string entity);
    }
}
