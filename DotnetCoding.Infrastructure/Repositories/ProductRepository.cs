using DotnetCoding.Core.Interfaces;
using DotnetCoding.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetCoding.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<ProductDetails>, IProductRepository
    {
        public ProductRepository(DbContextClass dbContext) : base(dbContext)
        {

        }

        // Add method to add new product to the DbContext
        public void Add(ProductDetails product)
        {
            _dbContext.Products.Add(product);  // Add product to DbSet
        }

        // Get active products sorted by latest posted date
        public async Task<IEnumerable<ProductDetails>> GetActiveProducts()
        {
            return await _dbContext.Products
                .Where(p => p.ProductStatus == "Active")
                .OrderByDescending(p => p.PostedDate)
                .ToListAsync(); // Sort by latest posted date
        }


        // requirement 2 -  Implementing search based on name, price range, and posted date range
        public async Task<IEnumerable<ProductDetails>> SearchProducts(string name, decimal? minPrice, decimal? maxPrice, DateTime? startDate, DateTime? endDate)
        {
            var query = _dbContext.Products.Where(p => p.ProductStatus == "Active").AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(p => p.ProductName.Contains(name));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.ProductPrice >= minPrice);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.ProductPrice <= maxPrice);
            }

            if (startDate.HasValue)
            {
                query = query.Where(p => p.PostedDate >= startDate);
            }

            if (endDate.HasValue)
            {
                query = query.Where(p => p.PostedDate <= endDate);
            }

            return await query.OrderByDescending(p => p.PostedDate).ToListAsync();  // Sort by latest posted date
        }

        // requirement -4  Method to add approval request to ApprovalQueue
        public void AddToApprovalQueue(ApprovalQueue approvalRequest)
        {
            _dbContext.ApprovalQueue.Add(approvalRequest);  // Add approval request to DbSet
        }



        // requirement -5 Implementing method to get product by ID
        public async Task<ProductDetails> GetById(int productId)
        {
            return await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == productId);
        }

        // requirement -6 Implementing the method to delete the product (to be called after approval)
        public void DeleteProduct(ProductDetails product)
        {
            _dbContext.Products.Remove(product);
        }


        // requirement -7 Implementing method to fetch all approval requests ordered by RequestDate
        public async Task<IEnumerable<ApprovalQueue>> GetApprovalQueue()
        {
            return await _dbContext.ApprovalQueue
                .OrderBy(a => a.RequestDate)  // Order by request date, oldest first
                .ToListAsync();
        }


        public async Task<ApprovalQueue> GetApprovalRequestById(int approvalId)
        {
            return await _dbContext.ApprovalQueue.FirstOrDefaultAsync(a => a.Id == approvalId);
        }

        public void RemoveFromApprovalQueue(ApprovalQueue approvalRequest)
        {
            _dbContext.ApprovalQueue.Remove(approvalRequest);
        }


        public void Update(ProductDetails product)
        {
            _dbContext.Products.Update(product);  // Mark entity as modified
        }
    }
}
