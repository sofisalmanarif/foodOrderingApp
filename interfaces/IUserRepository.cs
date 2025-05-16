using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using foodOrderingApp.models;

namespace foodOrderingApp.interfaces
{
    public interface IUserRepository
    {
        User GetById(Guid id);
        IEnumerable<User> GetAll(int pageSize,int pageNumber);
        Guid Add(User user);
        string Update(UpdateUserDto user, Guid userId);
        string Delete(Guid id);
        object Login(LoginDto user);
        User? Profile(Guid id);

        string SaveFirebasePushNotificationToken(FirebaseTokenDto firebaseTokenDto);
        string SaveFirebasePushNotificationToken(Guid UserId);
    }
}