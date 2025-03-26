using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using foodOrderingApp.models;

namespace foodOrderingApp.interfaces
{
    public interface IAddressRepository
    {
        string Add(Address address);
        Address Get(Guid id);
        string Delete(Guid id);
        Address Edit(Address address);
    }
}