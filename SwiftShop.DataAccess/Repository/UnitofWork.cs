using SwiftShop.DataAccess.Data;
using SwiftShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftShop.DataAccess.Repository
{
    public class UnitofWork : IUnitofWork
    {
        private ApplicationDbContext _context;
        public ICategoryRepository CategoryRepository { get; private set; }
        public IProductRepository ProductRepository { get; private set; }
        public ICompanyRepository CompanyRepository { get; private set; }
        public IShoppingCartRepository ShoppingCartRepository { get; private set; }
        public IApplicationUserRepository ApplicationUserRepository { get; private set; }
        public IOrderHeaderRepository OrderHeaderRepository { get; private set; }
        public IOrderDetailsRepository OrderDetailsRepository { get; private set; }
        public UnitofWork(ApplicationDbContext context)
        {
            _context=context;
            CategoryRepository =new CategoryRepository(_context);
            ProductRepository=new ProductRepository(_context);
           CompanyRepository=new CompanyRepository(_context);
            ShoppingCartRepository=new ShoppingCartRepository(_context);
            ApplicationUserRepository= new ApplicationUserRepository(_context);
            OrderDetailsRepository=new OrderDetailsRepository(_context);
            OrderHeaderRepository=new OrderHeaderRepository(_context);
        }

        public void save()
        {
            _context.SaveChanges();
        }
    }
}
