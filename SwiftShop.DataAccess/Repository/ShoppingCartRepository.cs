using SwiftShop.DataAccess.Data;
using SwiftShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SwiftShop.DataAccess.Repository
{
   public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
       private readonly ApplicationDbContext _context;
        public ShoppingCartRepository(ApplicationDbContext context):base(context) 
        {
   
            _context = context;
        }

        public void Update(ShoppingCart obj)
        {
            _context.ShoppingCarts.Update(obj);
        }
    }
}
