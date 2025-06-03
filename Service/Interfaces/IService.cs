using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IService<T>
    {
        Task<T> GetById(int id);
        Task<List<T>> GetAll();
        Task<T> Add(T item);
        Task Update(int id, T item);
        Task Delete(int id);
    }
}
