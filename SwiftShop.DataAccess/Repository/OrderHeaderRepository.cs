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
   public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
       private readonly ApplicationDbContext _context;
        public OrderHeaderRepository(ApplicationDbContext context):base(context) 
        {
   
            _context = context;
        }
        //public void save()
        //{
        //   _context.SaveChanges();
        //}

        public void Update(OrderHeader obj)
        {
            _context.OrderHeader.Update(obj);
        }

        public void UpdateStatus(int id, string orderstatus, string? paymentstatus = null)
        {
            var orderfromdb= _context.OrderHeader.FirstOrDefault(u=>u.Id==id);
            if (orderfromdb != null)
            {
                orderfromdb.OrderStatus = orderstatus;
                if (!string.IsNullOrEmpty(paymentstatus))
                {
                    orderfromdb.PaymentStatus = paymentstatus;
                }
            }
        }

        public void UpdateStripePaymentID(int id, string SessionId, string paymentintentID)
        {
            var orderfromdb = _context.OrderHeader.FirstOrDefault(u => u.Id == id);
            if (!string.IsNullOrEmpty(SessionId))
            {
                orderfromdb.SessionId = SessionId;
            }
            if (!string.IsNullOrEmpty(paymentintentID))
            {
                orderfromdb.PaymentIntentId= paymentintentID;
                orderfromdb.PaymentTime= DateTime.Now;
            }

        }
    }
}
