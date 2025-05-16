using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using foodOrderingApp.models;

namespace foodOrderingApp.interfaces
{
    public interface IResturantRepository
    {
        Object? GetById(Guid id);
        IEnumerable<Restaurant> GetAll(int pageSize,int pageNumber);
        Restaurant Add(Restaurant restaurant);
        string Update(Restaurant restaurant);
        string Delete(Guid id);
        string Verify(Guid id);
        string ToggleOrders(Guid id);
        IEnumerable<Restaurant> GelAllNotVerified(int pageSize,int pageNumber);
        IEnumerable<Restaurant> GetAllRestaurantsByCategory(Guid categoryId);

    }
}