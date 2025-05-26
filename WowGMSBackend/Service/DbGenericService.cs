using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowGMSBackend.DBContext;
using WowGMSBackend.Interfaces;

namespace WowGMSBackend.Service
{
    public class DbGenericService<T> : IDBService<T> where T : class
    {
        private readonly WowDbContext _context;
        public DbGenericService(WowDbContext context)
        {
            _context = context;
        }

        public async Task AddObjectAsync(T obj)
        {
            _context.Set<T>().Add(obj);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteObjectAsync(T obj)
        {
                _context.Set<T>().Remove(obj);
                await _context.SaveChangesAsync();
        }

        public async Task UpdateObjectAsync(T obj)
            {
                _context.Set<T>().Update(obj);
                await _context.SaveChangesAsync();
            }
        public async Task<IEnumerable<T>> GetAllObjectsAsync()
            {
                return await _context.Set<T>().AsNoTracking().ToListAsync();
            }

        public async Task<T> GetObjectByIdAsync(params object[] keyValues)
            {
                return await _context.Set<T>().FindAsync(keyValues);
            }
        public async Task SaveObjects(List<T> objects)
            {
                foreach (T obj in objects)
                {
                    _context.Set<T>().Add(obj);
                    _context.SaveChanges();
                }
            }
        }
    }
