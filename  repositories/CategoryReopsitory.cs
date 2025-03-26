using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using foodOrderingApp.data;
using foodOrderingApp.interfaces;
using foodOrderingApp.models;

namespace foodOrderingApp.repositories
{
    public class CategoryReopsitory : ICategoryReopsitory
    {
        private readonly AppDbContext _context;
        public CategoryReopsitory(AppDbContext context)
        {
            _context = context;

        }
        public string Add(Category newCategory)
        {
            if (newCategory == null)
            {
                throw new ArgumentNullException(nameof(newCategory), "Category can't be empty");
            }


            _context.Categories.Add(newCategory);
            _context.SaveChanges();
            return $"{newCategory.Name}  Added Sucessfully";
        }

        public IEnumerable<Category> AllCategories()
        {
            return _context.Categories.ToList();
        }

        public string Delete(Guid categoryId)
        {
            var existingCategory = _context.Categories.Find(categoryId);
            if (existingCategory == null)
            {
                throw new KeyNotFoundException("Category Not Found");
            }
            _context.Categories.Remove(existingCategory);
            _context.SaveChanges();
            return $"{existingCategory.Name} Deleted Sucessfully";
        }
        public string Update(Category updateCategory)
        {
            var existingCategory = _context.Categories.Find(updateCategory.Id);
            if (existingCategory == null)
            {
                throw new KeyNotFoundException("Category Not Found");
            }
            existingCategory.Name = updateCategory.Name;
            _context.SaveChanges();
            return $"{existingCategory.Name} Updated Sucessfully";
        }

        



    }
}