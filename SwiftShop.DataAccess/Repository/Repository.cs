using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using SwiftShop.Repository;
using SwiftShop.DataAccess.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;



namespace SwiftShop.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        internal DbSet<T> Tset { get; set; }
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            this.Tset=_context.Set<T>();//this will be _context.categories=Tset
            _context.Products.Include(u => u.Category).Include(u => u.CategoryID);//can add multiple includes
        }
        public void Add(T entity)
        {
            Tset.Add(entity);
            
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter, string? includeProperties = null)
        {
            IQueryable<T> query = Tset;
            if(filter != null)
            {
                query = Tset.Where(filter);
            }

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeprop in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {

                    query= query.Include(includeprop);
                }

            }
               return query.ToList();
        }

        public T Getbyid(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            IQueryable<T> query = Tset;

            if (tracked = true)
            {

                query = Tset;
                
            }
            else
            {

                query = Tset.AsNoTracking();

            }
            query = Tset.Where(filter);
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeprop in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {

                    query = query.Include(includeprop);
                }

            }
            return query.FirstOrDefault();
        }

        public void Remove(T entity)
        {
            Tset.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            Tset.RemoveRange(entity);
        }
    }
}
