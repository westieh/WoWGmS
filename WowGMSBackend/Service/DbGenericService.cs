using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WowGMSBackend.DBContext;
using WowGMSBackend.Interfaces;

namespace WowGMSBackend.Service
{
    /// <summary>
    /// Generic database service providing basic CRUD operations 
    /// for any entity type T using Entity Framework Core.
    /// </summary>
    public class DbGenericService<T> : IDBService<T> where T : class
    {
        private readonly WowDbContext _context;

        /// <summary>
        /// Initializes the generic service with the provided database context.
        /// </summary>
        public DbGenericService(WowDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a single object of type T to the database asynchronously.
        /// </summary>
        public async Task AddObjectAsync(T obj)
        {
            _context.Set<T>().Add(obj);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Removes a single object of type T from the database asynchronously.
        /// </summary>
        public async Task DeleteObjectAsync(T obj)
        {
            _context.Set<T>().Remove(obj);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing object of type T in the database asynchronously.
        /// </summary>
        public async Task UpdateObjectAsync(T obj)
        {
            _context.Set<T>().Update(obj);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves all objects of type T from the database asynchronously.
        /// Uses AsNoTracking for better performance when read-only access is sufficient.
        /// </summary>
        public async Task<IEnumerable<T>> GetAllObjectsAsync()
        {
            return await _context.Set<T>().AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Finds a single object of type T by its primary key(s) asynchronously.
        /// Supports composite keys via object array.
        /// </summary>
        public async Task<T> GetObjectByIdAsync(params object[] keyValues)
        {
            return await _context.Set<T>().FindAsync(keyValues);
        }

        /// <summary>
        /// Saves a list of objects of type T to the database.
        /// Each object is added individually followed by SaveChanges.
        /// Note: This could be optimized by batching if the list is large.
        /// </summary>
        public async Task SaveObjects(List<T> objects)
        {
            _context.Set<T>().AddRange(objects);
            await _context.SaveChangesAsync();
        }
    }
}
