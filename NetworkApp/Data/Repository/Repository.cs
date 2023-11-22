using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetworkApp.Data.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected DbContext _db;

        public DbSet<T> Set { get; private set; }

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            var set = _db.Set<T>();
            set.Load();

            Set = set;
        }

        //public Task CreateAsync(T item)
        //{
        //    Set.AddAsync(item);
        //    _db.SaveChangesAsync();
        //    return Task.CompletedTask;
        //}

        //public async Task DeleteAsync(T item)
        //{
        //    Set.Remove(item);
        //    await _db.SaveChangesAsync();
        //}

        //public async Task<T> GetAsync(int id)
        //{
        //    return await Set.FindAsync(id);
        //}

        //public async Task<IEnumerable<T>> GetAllAsync()
        //{
        //    return Set;
        //}

        //public async Task UpdateAsync(T item)
        //{
        //    Set.Update(item);
        //    await _db.SaveChangesAsync();
        //}

        //Old sync methods
        //public Repository(ApplicationDbContext db)
        //{
        //    _db = db;
        //    var set = _db.Set<T>();
        //    set.Load();

        //    Set = set;
        //}

        public void Create(T item)
        {
            Set.Add(item);
            _db.SaveChanges();
        }

        public void Delete(T item)
        {
            Set.Remove(item);
            _db.SaveChanges();
        }

        public T Get(int id)
        {
            return Set.Find(id);
        }

        public IEnumerable<T> GetAll()
        {
            return Set;
        }

        public void Update(T item)
        {
            Set.Update(item);
            _db.SaveChanges();
        }

    }
}
