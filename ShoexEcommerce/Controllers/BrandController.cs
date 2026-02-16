using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoexEcommerce.Application.DTOs.Brand;
using ShoexEcommerce.Application.Interfaces.Brand;

namespace ShoexEcommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _brandService.GetAllAsync();
            return StatusCode(result.StatusCode, result);
        }

        
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _brandService.GetByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

       
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateBrandDto dto)
        {
            var result = await _brandService.CreateAsync(dto);
            return StatusCode(result.StatusCode, result);
        }

       
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromForm] BrandUpdateDto dto)
        {
            var result = await _brandService.UpdateAsync(id, dto);
            return StatusCode(result.StatusCode, result);
        }

        
        [Authorize(Roles = "Admin")]
        [HttpPatch("{id:int}/toggle")]
        public async Task<IActionResult> Toggle(int id)
        {
            var result = await _brandService.ToggleActiveAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _brandService.DeleteAsync(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}
