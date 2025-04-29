using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace foodOrderingApp.services.Redis
{
    public interface ICacheService
    {
        T Get<T>(string key);
        void Set<T>(string key, T value, TimeSpan ttl);

        void Delete(string key);
    }
}