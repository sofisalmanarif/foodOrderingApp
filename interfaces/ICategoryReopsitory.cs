using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using foodOrderingApp.models;

namespace foodOrderingApp.interfaces
{
    public interface ICategoryReopsitory
    {
        string Add(Category newCategory);
        IEnumerable<Category> AllCategories();

        string Delete(Guid CategoryId);
        string Update(Category UpdateCategory);
    }
}