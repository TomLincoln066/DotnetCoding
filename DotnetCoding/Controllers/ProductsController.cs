using Microsoft.AspNetCore.Mvc;
using DotnetCoding.Core.Models;
using DotnetCoding.Services.Interfaces;

namespace DotnetCoding.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        public readonly IProductService _productService;
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        ///// <summary>
        ///// Get the list of product
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //public async Task<IActionResult> GetProductList()
        //{
        //    var productDetailsList = await _productService.GetAllProducts();
        //    if (productDetailsList == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(productDetailsList);
        //}

        /// <summary>
        /// requirement 1 & 2
        /// Get the list of active products, with search filters (name, price range, date range)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetProductList([FromQuery] string name, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var productDetailsList = await _productService.SearchProducts(name, minPrice, maxPrice, startDate, endDate);
            if (productDetailsList == null)
            {
                return NotFound();
            }
            return Ok(productDetailsList);
        }


        // requirement -7  New endpoint to get products in the approval queue
        [HttpGet("approval-queue")]
        public async Task<IActionResult> GetApprovalQueue()
        {
            var approvalQueue = await _productService.GetApprovalQueue();
            if (approvalQueue == null)
            {
                return NotFound();
            }
            return Ok(approvalQueue);
        }


        // requirement 8 - Endpoint for approving a product
        [HttpPost("approve/{approvalId}")]
        public async Task<IActionResult> ApproveProduct(int approvalId)
        {
            await _productService.ApproveProduct(approvalId);
            return Ok();
        }

        // requirement 8 - Endpoint for rejecting a product
        [HttpPost("reject/{approvalId}")]
        public async Task<IActionResult> RejectProduct(int approvalId)
        {
            await _productService.RejectProduct(approvalId);
            return Ok();
        }


        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDetails product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _productService.AddProduct(product);
            return Ok("Product created successfully.");
        }


        [HttpPut]
        [Route("update/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDetails product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            product.Id = id;  // Ensure the product being updated matches the ID in the request
            await _productService.UpdateProduct(product);
            return Ok("Product updated successfully.");
        }


        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteProduct(id);
            return Ok("Product deletion request added to approval queue.");
        }

    }
}
