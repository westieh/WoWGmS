using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowGMSBackend.DBContext;

namespace WowGMSBackend.Service
{
    public class DbGenericService<T> : IDBService<T> where T : class
    {
        public async Task AddObjectAsync(T obj)
        {
            using (var context = new WowDbContext())
            {
                context.Set<T>().Add(obj);
                await context.SaveChangesAsync();
            }
        }
        public async Task DeleteObjectAsync(T obj)
        {
            using (var context = new WowDbContext())
            {
                context.Set<T>().Remove(obj);
                await context.SaveChangesAsync();
            }
        }
        public async Task UpdateObjectAsync(T obj)
        {
            using (var context = new WowDbContext())
            {
                context.Set<T>().Update(obj);
                await context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<T>> GetAllObjectsAsync()
        {
            using (var context = new WowDbContext())
            {
                return await context.Set<T>().AsNoTracking().ToListAsync();
            }
        }
        public async Task<T> GetObjectByIdAsync(params object[] keyValues)
        {
            using (var context = new WowDbContext())
            {
                return await context.Set<T>().FindAsync(keyValues);
            }
        }
        public async Task SaveObjects(List<T> objects)
        {
            using (var context = new WowDbContext())
            {
                foreach (T obj in objects)
                {
                    context.Set<T>().Add(obj);
                    context.SaveChanges();
                }
            }
        }
    }
}
