using Microsoft.AspNetCore.Mvc;
using ShoexEcommerce.Application.Interfaces.Product;

namespace ShoexEcommerce.API.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductsController(IProductService service)
        {
            _service = service;
        }
      [HttpGet]
        public async Task<IActionResult> GetActive(CancellationToken ct)
        {
            var result = await _service.GetActiveProductsAsync(ct);
            return StatusCode(result.StatusCode, result);
        }
      [HttpGet("{id:int}")]
        public async Task<IActionResult> GetActiveById(int id, CancellationToken ct)
        {
            var result = await _service.GetActiveProductByIdAsync(id, ct);
            return StatusCode(result.StatusCode, result);
        }
    }
}
