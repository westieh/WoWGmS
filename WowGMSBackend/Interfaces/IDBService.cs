using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WowGMSBackend.Interfaces
{
    public interface IDBService<T> where T : class
    {
        Task AddObjectAsync(T obj);
        Task DeleteObjectAsync(T obj);
        Task UpdateObjectAsync(T obj);
        Task<IEnumerable<T>> GetAllObjectsAsync();
        Task<T> GetObjectByIdAsync(params object[] keyValues);
    }
}
