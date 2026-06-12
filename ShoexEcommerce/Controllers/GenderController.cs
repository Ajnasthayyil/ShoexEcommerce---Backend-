using Microsoft.AspNetCore.Mvc;
using ShoexEcommerce.Application.Interfaces.Gender;

namespace ShoexEcommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenderController : ControllerBase
    {
        private readonly IGenderService _service;

        public GenderController(IGenderService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpPatch("{id}/toggle")]
        public async Task<IActionResult> ToggleActive(int id)
            => Ok(await _service.ToggleActiveAsync(id));
    }
}
