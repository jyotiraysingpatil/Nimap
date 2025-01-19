using Microsoft.EntityFrameworkCore;
using nimap.Data;
using nimap.Models;

namespace nimap.Repositories
{
    public class CategoryRepository :ICategoryRepository
    {
        private readonly AppDbContext _context;
        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddCategoryAsync(Category category)
        {
            if (category == null) throw new ArgumentNullException(nameof(category));

           
            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryName.ToLower() == category.CategoryName.ToLower());
            if (existingCategory != null)
            {
                throw new InvalidOperationException("A category with the same name already exists.");
            }

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }



        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category != null)
            {
                // Remove associated products
                _context.Products.RemoveRange(category.Products);

                // Remove the category
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }



        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

            public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            if (category == null) throw new ArgumentNullException(nameof(category));
            var existingCategory = await _context.Categories.FindAsync(category.CategoryId);
            if (existingCategory == null) throw new KeyNotFoundException("Category not found");

            existingCategory.CategoryName = category.CategoryName;
            _context.Categories.Update(existingCategory);
            await _context.SaveChangesAsync();
        }

    }
}
