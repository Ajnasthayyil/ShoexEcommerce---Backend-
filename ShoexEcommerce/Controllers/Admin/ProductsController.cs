using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoexEcommerce.API.Requests.Product;
using ShoexEcommerce.Application.DTOs.Product;
using ShoexEcommerce.Application.Interfaces.Product;
using System.Text.Json;

namespace ShoexEcommerce.API.Controllers.Admin
{
    [Route("api/admin/products")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ProductsAdminController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductsAdminController(IProductService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var result = await _service.GetAllAsync(ct);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var result = await _service.GetByIdAsync(id, ct);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateProductRequest form, CancellationToken ct)
        {
            var uploads = new List<FileUpload>();

            if (form.Images != null)
            {
                foreach (var file in form.Images)
                {
                    if (file == null || file.Length == 0) continue;

                    var ms = new MemoryStream();
                    await file.CopyToAsync(ms, ct);
                    ms.Position = 0;
                    uploads.Add(new FileUpload(ms, file.FileName));
                }
            }

            var dto = new CreateProductDto
            {
                Name = form.Name,
                Price = form.Price,
                Description = form.Description,
                BrandId = form.BrandId,
                GenderId = form.GenderId,
                SizeIds = form.SizeIds,
                Images = uploads,
                PrimaryImageIndex = form.PrimaryImageIndex
            };

            var result = await _service.CreateAsync(dto, ct);
            return StatusCode(result.StatusCode, result);
        }
        

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateProductRequest req, CancellationToken ct)
        {

            var dto = new UpdateProductDto
            {
                Name = req.Name,
                Price = req.Price,
                Description = req.Description,
                BrandId = req.BrandId,
                GenderId = req.GenderId,
                SizeIds = req.SizeIds
            };

            var result = await _service.UpdateAsync(id, dto, ct);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPatch("{id:int}/toggle")]
        public async Task<IActionResult> Toggle(int id, CancellationToken ct)
        {
            var result = await _service.ToggleAsync(id, ct);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            return StatusCode(result.StatusCode, result);
        }


        [HttpPut("{productId:int}/sizes/{sizeId:int}/stock")]
        public async Task<IActionResult> UpdateSizeStock(
            int productId,
            int sizeId,
            [FromForm] UpdateStockDto dto,
            CancellationToken ct)
        {
            
            var result = await _service.UpdateSizeStockAsync(productId, sizeId, dto, ct);
            return StatusCode(result.StatusCode, result);
        }


        [HttpPatch("{productId:int}/sizes/{sizeId:int}/stock")]
        public async Task<IActionResult> AdjustSizeStock(
            int productId,
            int sizeId,
            [FromForm] AdjustStockDto dto,
            CancellationToken ct)
        {
            var result = await _service.AdjustSizeStockAsync(productId, sizeId, dto, ct);
            return StatusCode(result.StatusCode, result);
        }

        

        // IMAGES
        [HttpPost("{id:int}/images")]
        public async Task<IActionResult> AddImages([FromRoute] int id, [FromForm] List<IFormFile> images, CancellationToken ct)
        {
            var result = await _service.AddImagesAsync(id, images, ct);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{productId:int}/images/{imageId:int}")]
        public async Task<IActionResult> DeleteImage([FromRoute] int productId, [FromRoute] int imageId, CancellationToken ct)
        {
            var result = await _service.DeleteImageAsync(productId, imageId, ct);
            return StatusCode(result.StatusCode, result);
        }
    }
}