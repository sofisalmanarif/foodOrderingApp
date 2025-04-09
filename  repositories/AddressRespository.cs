using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using foodOrderingApp.data;
using foodOrderingApp.interfaces;
using foodOrderingApp.models;

namespace foodOrderingApp.repositories
{
    public class AddressRespository : IAddressRepository
    {
        private readonly AppDbContext _context;
     

        public AddressRespository(AppDbContext context)
        {
            _context = context;
        }
        public string Add(Address address)
        {
            if (address == null)
            {

                throw new ArgumentNullException(nameof(Address), "Address cannot be null.");

            }
            _context.Address.Add(address);
            _context.SaveChanges();
             return "Address Created Sucessfully";
        }

        public string Delete(Guid userId)
        {
           var address =  _context.Address.FirstOrDefault(a=>a.RefId==userId);

           if(address==null){
            throw new  KeyNotFoundException("Address Not Found");
           }
           _context.Address.Remove(address);
           _context.SaveChanges();
           return "Address Deleted Sucessfully";
        }

        public Address Edit(Address address)
        {
            throw new NotImplementedException();
        }

        public Address Get(Guid id)
        {
            var address = _context.Address.FirstOrDefault(a=>a.RefId ==id);
            if(address==null){
                throw new KeyNotFoundException("Address Not Found");
            }
            return address;
        }
    }
}