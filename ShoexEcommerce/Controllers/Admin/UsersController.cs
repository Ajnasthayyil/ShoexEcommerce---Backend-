using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoexEcommerce.Application.DTOs.User;
using ShoexEcommerce.Application.Interfaces.User;

namespace ShoexEcommerce.API.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IAdminUserService _service;

        public UsersController(IAdminUserService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var res = await _service.GetAllAsync(ct);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var res = await _service.GetByIdAsync(id, ct);
            return StatusCode(res.StatusCode, res);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{userId}/block")]
        public async Task<IActionResult> ToggleBlock(int userId, CancellationToken ct)
        {
            var adminId = int.Parse(User.FindFirst("userid")!.Value);

            var res = await _service.ToggleBlockAsync(adminId, userId, ct);

            return StatusCode(res.StatusCode, res);
        }



        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var res = await _service.DeleteAsync(id, ct);
            return StatusCode(res.StatusCode, res);
        }
    }
}
