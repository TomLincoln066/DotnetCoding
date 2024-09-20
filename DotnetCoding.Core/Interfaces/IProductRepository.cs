using DotnetCoding.Core.Models;

namespace DotnetCoding.Core.Interfaces
{
    public interface IProductRepository : IGenericRepository<ProductDetails>
    {
        Task<IEnumerable<ProductDetails>> GetActiveProducts();
        void Add(ProductDetails product);

        // New method for searching products based on filters
        Task<IEnumerable<ProductDetails>> SearchProducts(string name,
            decimal? minPrice, decimal? maxPrice, DateTime? startDate, DateTime? endDate);

        // requirement -4 New method for adding to the approval queue
        void AddToApprovalQueue(ApprovalQueue approvalRequest);

        // requirement -6 New method for deleting a product (later after approval)
        void DeleteProduct(ProductDetails product);

        // requirement -7 New method to fetch products in the approval queue
        Task<IEnumerable<ApprovalQueue>> GetApprovalQueue();


        // requirement - 5 New method to fetch a product by its ID
        Task<ProductDetails> GetById(int productId);


        // requirement -8 New methods for handling approval/rejection
        Task<ApprovalQueue> GetApprovalRequestById(int approvalId);
        void RemoveFromApprovalQueue(ApprovalQueue approvalRequest);


        void Update(ProductDetails product);

    }
}
