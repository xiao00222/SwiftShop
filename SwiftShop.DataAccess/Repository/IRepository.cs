using System.Linq.Expressions;

namespace SwiftShop.Repository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter=null, string? includeProperties =null);
        //func to get individual record
        T Getbyid(Expression<Func<T,bool>> filter   , string? includeProperties = null,bool tracked= false);//euqivalent to FirstOrDefault 
       void Add(T entity);
        //void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
    }
}
