using Microsoft.EntityFrameworkCore;
using nimap.Data;
using nimap.Models;

namespace nimap.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync(int page, int pageSize)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task AddProductAsync(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));

            var existingProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductName.ToLower() == product.ProductName.ToLower() && p.CategoryId == product.CategoryId);
            if (existingProduct != null)
            {
                throw new InvalidOperationException("A product with the same name already exists in this category.");
            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }



        public async Task UpdateProductAsync(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));
            var existingProduct = await _context.Products.FindAsync(product.ProductId);
            if (existingProduct == null) throw new KeyNotFoundException("Product not found");

            existingProduct.ProductName = product.ProductName;
            existingProduct.CategoryId = product.CategoryId;
            _context.Products.Update(existingProduct);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetTotalProductsAsync()
        {
            return await _context.Products.CountAsync();
        }
    }
}
