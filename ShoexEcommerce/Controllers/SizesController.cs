using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SheoxEcommerce.Application.DTOs.Size;
using ShoexEcommerce.Application.Interfaces.Product;

namespace ShoexEcommerce.API.Controllers.Admin
{
    [Route("api/admin/sizes")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class SizesController : ControllerBase
    {
        private readonly ISizeService _service;

        public SizesController(ISizeService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSizeDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpPatch("{id:int}/toggle")]
        public async Task<IActionResult> Toggle(int id)
        {
            var result = await _service.ToggleAsync(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}
