using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetworkApp.Data.Repository
{
    public interface IRepository<T> where T : class
    {
        //Task<IEnumerable<T>> GetAllAsync();
        //Task<T> GetAsync(int id);
        //Task CreateAsync(T item);
        //Task UpdateAsync(T item);
        //Task DeleteAsync(T item);

        //Old sync methods
        IEnumerable<T> GetAll();
        T Get(int id);
        void Create(T item);
        void Update(T item);
        void Delete(T item);

    }
}
