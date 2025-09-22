using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AgroScan.API.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("User ID not found in token");
            }
            return userId;
        }

        protected ActionResult HandleServiceException(Exception ex)
        {
            return ex switch
            {
                ArgumentNullException => BadRequest(new { message = ex.Message }),
                ArgumentException => BadRequest(new { message = ex.Message }),
                InvalidOperationException => BadRequest(new { message = ex.Message }),
                UnauthorizedAccessException => Unauthorized(new { message = ex.Message }),
                KeyNotFoundException => NotFound(new { message = ex.Message }),
                FileNotFoundException => NotFound(new { message = ex.Message }),
                _ => StatusCode(500, new { message = "An internal server error occurred" })
            };
        }

        protected ActionResult<T> HandleServiceResult<T>(T? result, string notFoundMessage = "Resource not found")
        {
            if (result == null)
            {
                return NotFound(new { message = notFoundMessage });
            }
            return Ok(result);
        }

        protected ActionResult HandleServiceResult(bool success, string notFoundMessage = "Resource not found")
        {
            if (!success)
            {
                return NotFound(new { message = notFoundMessage });
            }
            return NoContent();
        }

        protected ActionResult? ValidateModelState()
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                return BadRequest(new { message = "Validation failed", errors });
            }
            return null;
        }
    }
}
