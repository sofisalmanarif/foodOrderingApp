using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace foodOrderingApp.interfaces
{
    public interface IPayment
    {
        string CreateOrder(double amount,Guid userId);
    }
}