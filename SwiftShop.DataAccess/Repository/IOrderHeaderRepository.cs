using SwiftShop.Models;
using SwiftShop.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftShop.DataAccess.Repository
{
    public interface IOrderHeaderRepository:IRepository<OrderHeader>
    {
        void Update(OrderHeader obj);
        void UpdateStatus(int id, string orderstatus, string? paymentstatus = null);
        void UpdateStripePaymentID(int id, string SessionId, string paymentintentID);
    }
}
