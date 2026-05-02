using SwiftShop.DataAccess.Data;
using SwiftShop.Models;
using SwiftShop.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftShop.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context):base(context) 
        {
            _context = context;
        }
        public void Update(Product obj)
        {   //custom logic
           var objfromdb= _context.Products.FirstOrDefault(u=>u.Id==obj.Id);
            if (objfromdb != null)
            {
                objfromdb.Title = obj.Title;
                objfromdb.Price= obj.Price;
                objfromdb.Price50 = obj.Price50;
                objfromdb.Price100=obj.Price100;
                objfromdb.ListPrice=obj.ListPrice;
                objfromdb.CategoryID= obj.CategoryID;
                objfromdb.Author = obj.Author;
                if(obj.ImageUrl!=null)
                {
                    objfromdb.ImageUrl= obj.ImageUrl;
                }

            }
        }
    }
}
