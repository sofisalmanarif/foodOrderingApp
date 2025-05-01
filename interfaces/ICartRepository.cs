using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using foodOrderingApp.models.Dtos;

namespace foodOrderingApp.interfaces
{
    public interface ICartRepository
    {
        string Add(CartDto cartDto);
    }
}