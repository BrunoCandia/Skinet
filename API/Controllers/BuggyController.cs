using API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    public class BuggyController : BaseApiController
    {
        [HttpGet("unauthorized")]
        public IActionResult GetUnauthorized(CancellationToken cancellationToken)
        {
            return Unauthorized();
        }

        [HttpGet("bad-request")]
        public IActionResult GetBadRequest(CancellationToken cancellationToken)
        {
            return BadRequest("Not a good request");
        }

        [HttpGet("not-found")]
        public IActionResult GetNotFound(CancellationToken cancellationToken)
        {
            return NotFound();
        }

        [HttpGet("internal-error")]
        public IActionResult GetInternalError(CancellationToken cancellationToken)
        {
            throw new Exception("This is a test exception");
        }

        [HttpPost("validation-error")]
        public IActionResult GetValidationError(CreateProductDto createProductDto, CancellationToken cancellationToken)
        {
            return Ok();
        }

        [Authorize]
        [HttpGet("secret")]
        public IActionResult GetSecret(CancellationToken cancellationToken)
        {
            var name = User.FindFirst(ClaimTypes.Name)?.Value;
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Ok("Hi " + name + "with id of " + id);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-secret")]
        public IActionResult GetAdminSecret(CancellationToken cancellationToken)
        {
            var name = User.FindFirst(ClaimTypes.Name)?.Value;
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("Admin");
            var roles = User.FindFirstValue(ClaimTypes.Role);

            return Ok(new
            {
                Name = name,
                Id = id,
                IsAdmin = isAdmin,
                Roles = roles
            });
        }
    }
}
