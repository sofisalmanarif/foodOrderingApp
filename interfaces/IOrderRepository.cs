
using foodOrderingApp.models;

namespace foodOrderingApp.interfaces
{
    public interface IOrderRepository
    {
        Order Add(OrderDto newOrder,Guid userId);
    }
}