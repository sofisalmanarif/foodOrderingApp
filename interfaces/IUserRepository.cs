using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using foodOrderingApp.models;

namespace foodOrderingApp.interfaces
{
    public interface IUserRepository
    {
        User? GetById(Guid id);
        IEnumerable<User> GetAll();
        Guid Add(User user);
        string Update(User user);
        string Delete(Guid id);
        string Login(LoginDto user);
        User? Profile(Guid id);
    }
}