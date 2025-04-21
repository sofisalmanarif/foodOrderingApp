using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using foodOrderingApp.models;

namespace foodOrderingApp.interfaces
{
    public interface IMenuItemRepository
    {
        MenuItem? GetById(Guid id);
        IEnumerable<Object> GetAll(Guid menuId);
        MenuItem Add(MenuItemDto menuItem);
        MenuItem Update(MenuItem menuItem);
        string Delete(Guid id,Guid ownerId);

        string IsAvailable(Guid id);
        string UpdateVarient(MenuItemVarient varient);
        
    }
}