using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotnetCoding.Core.Interfaces;
using DotnetCoding.Core.Models;
using DotnetCoding.Services.Interfaces;

namespace DotnetCoding.Services
{
    public class ProductService : IProductService
    {
        //public IUnitOfWork _unitOfWork;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddProduct(ProductDetails product)
        {

            // requirement 3 - Check if the product price exceeds 10,000 dollars
            if (product.ProductPrice > 10000)
            {
                throw new Exception("Product price cannot exceed 10,000 dollars.");
            }

            product.PostedDate = DateTime.UtcNow;  // Initialize PostedDate with current time


            // requirement 4
            // If product price exceeds 5,000, add it to approval queue
            if (product.ProductPrice > 5000)
            {
                var approvalRequest = new ApprovalQueue
                {
                    ProductId = product.Id,
                    RequestReason = "Create",
                    RequestDate = DateTime.UtcNow,
                    State = "Pending"
                };
                _unitOfWork.Products.AddToApprovalQueue(approvalRequest);
            }
            else
            {
                _unitOfWork.Products.Add(product);  // Directly add product if price <= 5000
            }

            //_unitOfWork.Products.Add(product); // Add product

            await _unitOfWork.Save(); // Save changes
        }

        // requirement - 5 New method for updating product and checking for price increase
        public async Task UpdateProduct(ProductDetails updatedProduct)
        {
            var existingProduct = await _unitOfWork.Products.GetById(updatedProduct.Id);  // Fetch existing product

            if (existingProduct == null)
            {
                throw new Exception("Product not found.");
            }

            // Check if the price has increased by more than 50%
            var priceIncreasePercentage = ((updatedProduct.ProductPrice - existingProduct.ProductPrice) / (decimal)existingProduct.ProductPrice) * 100;

            if (priceIncreasePercentage > 50 || updatedProduct.ProductPrice > 5000)
            {
                var approvalRequest = new ApprovalQueue
                {
                    ProductId = updatedProduct.Id,
                    RequestReason = "Update",
                    RequestDate = DateTime.UtcNow,
                    State = "Pending"
                };
                _unitOfWork.Products.AddToApprovalQueue(approvalRequest);  // Push to approval queue
            }
            else
            {
                existingProduct.ProductPrice = updatedProduct.ProductPrice;  // Update price
                existingProduct.ProductDescription = updatedProduct.ProductDescription;  // Update other fields
                await _unitOfWork.Save();  // Save the updated product directly
            }

            await _unitOfWork.Save();
        }


        // requirement 6 - New method for deleting product by pushing it to the approval queue
        public async Task DeleteProduct(int productId)
        {
            var product = await _unitOfWork.Products.GetById(productId);

            if (product == null)
            {
                throw new Exception("Product not found.");
            }

            // Add the deletion request to the approval queue
            var approvalRequest = new ApprovalQueue
            {
                ProductId = product.Id,
                RequestReason = "Delete",
                RequestDate = DateTime.UtcNow,
                State = "Pending"
            };
            _unitOfWork.Products.AddToApprovalQueue(approvalRequest);

            // Save the request to the approval queue (do not delete the product yet)
            await _unitOfWork.Save();
        }


        public async Task<IEnumerable<ProductDetails>> GetActiveProducts()
        {
            return await _unitOfWork.Products.GetActiveProducts();
        }

        public async Task<IEnumerable<ProductDetails>> GetAllProducts()
        {
            var productDetailsList = await _unitOfWork.Products.GetAll();
            return productDetailsList;
        }

        // requirement 2 - Implementing product search based on name, price range, and date range
        public async Task<IEnumerable<ProductDetails>> SearchProducts(string name, decimal? minPrice, decimal? maxPrice, DateTime? startDate, DateTime? endDate)
        {
            return await _unitOfWork.Products.SearchProducts(name, minPrice, maxPrice, startDate, endDate);
        }

        // requirement 7 - New method to fetch products in the approval queue
        public async Task<IEnumerable<ApprovalQueue>> GetApprovalQueue()
        {
            return await _unitOfWork.Products.GetApprovalQueue();  // Fetch all items from the approval queue
        }


        // requirement 8- New method to handle approval of product requests
        public async Task ApproveProduct(int approvalId)
        {
            var approvalRequest = await _unitOfWork.Products.GetApprovalRequestById(approvalId);

            if (approvalRequest == null)
            {
                throw new Exception("Approval request not found.");
            }

            var product = await _unitOfWork.Products.GetById(approvalRequest.ProductId);

            if (approvalRequest.RequestReason == "Create")
            {
                // Reset the Id so that the database generates a new one
                product.Id = 0;  // Set Id to 0 to let the database generate the identity value
                // Make sure the product status is set to Active when created
                product.ProductStatus = "Active";  // Set status to "Active"
                // Create product
                _unitOfWork.Products.Add(product);
            }
            else if (approvalRequest.RequestReason == "Update")
            {
                // Update product
                product.ProductPrice = product.ProductPrice;
                product.ProductDescription = product.ProductDescription;

                product.ProductStatus = "Active";  // Mark the product as active
                _unitOfWork.Products.Update(product);  // Save updated product details

            }
            else if (approvalRequest.RequestReason == "Delete")
            {
                // Delete product
                _unitOfWork.Products.DeleteProduct(product);
            }

            // Remove from the approval queue
            _unitOfWork.Products.RemoveFromApprovalQueue(approvalRequest);
            await _unitOfWork.Save();
        }

        // requirement 8, 9 - New method to handle rejection of product requests
        public async Task RejectProduct(int approvalId)
        {
            var approvalRequest = await _unitOfWork.Products.GetApprovalRequestById(approvalId);

            if (approvalRequest == null)
            {
                throw new Exception("Approval request not found.");
            }

            // Simply remove the request from the approval queue, product stays in original state
            _unitOfWork.Products.RemoveFromApprovalQueue(approvalRequest);
            await _unitOfWork.Save();
        }

    }
}
