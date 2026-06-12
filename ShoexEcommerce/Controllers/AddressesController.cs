using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Utilities.Encoders;
using ShoexEcommerce.Application.Common;
using ShoexEcommerce.Application.DTOs.Address;
using ShoexEcommerce.Application.Interfaces.Address;
using ShoexEcommerce.Domain.Entities;
using ShoexEcommerce.Infrastructure.Services;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ShoexEcommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ShippingAddressesController : ControllerBase
    {
        private readonly IAddressService _service;
        public ShippingAddressesController(IAddressService service) => _service = service;

        private int GetUserId()
            => int.Parse(User.FindFirstValue("userid")!);

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] AddAddressDto dto, CancellationToken ct)
        {
            var res = await _service.AddAsync(GetUserId(), dto, ct);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("my")]
        public async Task<IActionResult> My(CancellationToken ct)
        {
            var res = await _service.GetMyAsync(GetUserId(), ct);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromForm] AddAddressDto dto, CancellationToken ct)
        {
            var res = await _service.UpdateAsync(GetUserId(), id, dto, ct);
            return StatusCode(res.StatusCode, res);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var userIdStr = User.FindFirstValue("userid");
            if (string.IsNullOrWhiteSpace(userIdStr) || !int.TryParse(userIdStr, out var userId))
                return Unauthorized(ApiResponse<string>.Fail("Login required", 401));

            var res = await _service.DeleteAsync(userId, id, ct);
            return StatusCode(res.StatusCode, res);
        }


        [HttpPut("{id:int}/set-default")]
        public async Task<IActionResult> SetDefault(int id, CancellationToken ct)
        {
            var res = await _service.SetDefaultAsync(GetUserId(), id, ct);
            return StatusCode(res.StatusCode, res);
        }
    }
}



