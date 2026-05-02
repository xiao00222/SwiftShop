using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swiftshop.Utility
{
    public static class SD
    {
        public const String Role_Customer = "Customer";
        public const String Role_Company = "Company";
        public const String Role_Admin= "Admin";
        public const String Role_Employee = "Employee";

        public const String Status_Pendiing = "Pending";
        public const String Status_Approved = "Approved";
        public const String Status_InProcess = "Processing";
        public const String Status_Shipped = "Shipped";
        public const String Status_Cancelled = "Cancelled";
        public const String Status_Refund = "Refunded";
        //Payment Statuses
        public const String PaymentStatus_Pending = "Pending";
        public const String PaymentStatus_Approved = "Approved";
        public const String PaymentStatus_DelayedPayment = "ApprovedForDelayedPayment";
        public const String PaymentStatus_Rejected = "Rejected";
        public const String SessionCart = "SessionShoppingCart";
    }
}
