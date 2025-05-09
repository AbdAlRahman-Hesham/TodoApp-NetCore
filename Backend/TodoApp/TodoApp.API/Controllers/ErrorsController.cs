using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.Errors;

namespace TodoApp.Api.Controllers;

[Route("errors/{code}")]
[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorsController : ControllerBase
{
    public ActionResult Error(int code)
    {
        return code switch
        {
            400 => BadRequest(new ApiError(code, "A bad request, you have made")),
            401 => Unauthorized(new ApiError(code, "Unauthorized")),
            404 => NotFound(new ApiError(code, "Resource not found")),
            500 => StatusCode(500, new ApiError(code, "An internal server error occurred")),
            _ => StatusCode(code, new ApiError(code))
        };

    }
}
