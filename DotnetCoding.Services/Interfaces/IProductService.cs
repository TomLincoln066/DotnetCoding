using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotnetCoding.Core.Models;

namespace DotnetCoding.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDetails>> GetAllProducts();
        Task<IEnumerable<ProductDetails>> GetActiveProducts();  // requirement 1 - New method for fetching active products

        // requirement - 2 method for searching products
        Task<IEnumerable<ProductDetails>> SearchProducts(string name, decimal? minPrice, decimal? maxPrice, DateTime? startDate, DateTime? endDate);

        // requirement - 7 Add the method definition for fetching approval queue
        Task<IEnumerable<ApprovalQueue>> GetApprovalQueue();

        // requirement - 8
        Task ApproveProduct(int approvalId);  // Approve product method
        // requirement - 8
        Task RejectProduct(int approvalId);  // Reject product method

        // requirement for the User can create, update, and delete any product.
        Task AddProduct(ProductDetails product);
        Task UpdateProduct(ProductDetails product);
        Task DeleteProduct(int productId);

    }
}
