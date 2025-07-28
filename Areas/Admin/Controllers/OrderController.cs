using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using Swiftshop.Utility;
using SwiftShop.DataAccess.Migrations;
using SwiftShop.DataAccess.Repository;
using SwiftShop.Models;
using SwiftShop.Models.ViewModel;
using System.Security.Claims;

namespace SwiftShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitofWork _unitofWork;

        public OrderController(IUnitofWork unitofWork)
        {
            _unitofWork = unitofWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int orderId)
        {
            OrderVM orderVM = new()
            {
                OrderHeader = _unitofWork.OrderHeaderRepository.Getbyid(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetails = _unitofWork.OrderDetailsRepository.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
            };
            return View(orderVM);
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult UpdateOrderDetails(OrderVM orderVM)
        {
            if (orderVM == null || orderVM.OrderHeader == null)
            {
                return BadRequest("Order data is incomplete.");
            }

            var orderFromDb = _unitofWork.OrderHeaderRepository.Getbyid(u => u.Id == orderVM.OrderHeader.Id);

            if (orderFromDb != null)
            {
                orderFromDb.Name = orderVM.OrderHeader.Name;
                orderFromDb.PhoneNumber = orderVM.OrderHeader.PhoneNumber;
                orderFromDb.StreetAddress = orderVM.OrderHeader.StreetAddress;
                orderFromDb.City = orderVM.OrderHeader.City;
                orderFromDb.State = orderVM.OrderHeader.State;

                if (!string.IsNullOrEmpty(orderVM.OrderHeader.Courier))
                {
                    orderFromDb.Courier = orderVM.OrderHeader.Courier;
                }

                if (!string.IsNullOrEmpty(orderVM.OrderHeader.TrackingOrder))
                {
                    orderFromDb.TrackingOrder = orderVM.OrderHeader.TrackingOrder;
                }

                _unitofWork.OrderHeaderRepository.Update(orderFromDb);
                _unitofWork.save();

                TempData["Success"] = "Order Details Updated Successfully";
            }

            return RedirectToAction(nameof(Details), new { orderId = orderFromDb.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult StartProcessing(OrderVM orderVM)
        {
            if (orderVM == null || orderVM.OrderHeader == null)
            {
                return BadRequest("Order data is incomplete.");
            }

            _unitofWork.OrderHeaderRepository.UpdateStatus(orderVM.OrderHeader.Id, SD.Status_InProcess);
            _unitofWork.save();

            TempData["Success"] = "Order status updated to In Process.";
            return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder(OrderVM orderVM)
        {
            if (orderVM == null || orderVM.OrderHeader == null)
            {
                return BadRequest("Order data is incomplete.");
            }
            var orderHeader = _unitofWork.OrderHeaderRepository.Getbyid(u => u.Id == orderVM.OrderHeader.Id);
            orderHeader.TrackingOrder = orderVM.OrderHeader.TrackingOrder;
            orderHeader.Courier = orderVM.OrderHeader.Courier;
            orderHeader.OrderStatus = SD.Status_Shipped;
            orderHeader.ShippingDate = DateTime.Now;
            if (orderHeader.PaymentStatus == SD.PaymentStatus_DelayedPayment)
            {
                orderHeader.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            }
            _unitofWork.OrderHeaderRepository.Update(orderHeader);
            _unitofWork.save();

            TempData["Success"] = "Order status updated to In Process.";
            return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancelOrder(OrderVM orderVM)
        {
            var orderHeader = _unitofWork.OrderHeaderRepository.Getbyid(u => u.Id == orderVM.OrderHeader.Id);
            if (orderHeader.PaymentStatus == SD.PaymentStatus_Approved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };
                var service = new RefundService();
                Refund refund = service.Create(options);
                _unitofWork.OrderHeaderRepository.UpdateStatus(orderHeader.Id, SD.Status_Cancelled, SD.Status_Refund);
            }
            else
            {
                _unitofWork.OrderHeaderRepository.UpdateStatus(orderHeader.Id, SD.Status_Cancelled, SD.Status_Cancelled);

            }
            _unitofWork.save();
            TempData["Success"] = "Order Cancelled Successfully";
            return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });

        }
        [ActionName("Details")]
        [HttpPost]
        public IActionResult Details_Pay_now(OrderVM orderVM)
        {
            orderVM.OrderHeader = _unitofWork.OrderHeaderRepository.Getbyid
                (u => u.Id == orderVM.OrderHeader.Id, includeProperties: "ApplicationUser");
            orderVM.OrderDetails = _unitofWork.OrderDetailsRepository.GetAll
               (u => u.OrderHeaderId == orderVM.OrderHeader.Id, includeProperties: "Product");
            var domain = "https://localhost:7193/";
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderId={orderVM.OrderHeader.Id}",
                CancelUrl = domain + $"admin/order/details?orderId={orderVM.OrderHeader.Id}",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };
            //iterate over each product in the cart and calculate their price etc and then add all items in the list
            foreach (var item in orderVM.OrderDetails)
            {
                var sessionlineitem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        UnitAmount = (long)(item.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title
                        }
                    },
                    Quantity = item.Count
                };
                options.LineItems.Add(sessionlineitem);
            }
            var service = new Stripe.Checkout.SessionService();
            Session session = service.Create(options);
            _unitofWork.OrderHeaderRepository.UpdateStripePaymentID(orderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitofWork.save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);

        }
        public IActionResult PaymentConfirmation(int orderHeaderId)
        {
            OrderHeader orderHeader = _unitofWork.OrderHeaderRepository.Getbyid(u => u.Id == orderHeaderId);
            if (orderHeader.PaymentStatus == SD.PaymentStatus_DelayedPayment)
            {   //company order
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitofWork.OrderHeaderRepository.UpdateStripePaymentID(orderHeaderId, session.Id, session.PaymentIntentId);
                    _unitofWork.OrderHeaderRepository.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, SD.PaymentStatus_Approved);
                    _unitofWork.save();

                }
            }
           
            return View(orderHeaderId);
        }



        #region API CALLS

        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> objorderheader;

            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                objorderheader = _unitofWork.OrderHeaderRepository.GetAll(includeProperties: "ApplicationUser").ToList();
            }
            else
            {
                var claimsidentity = (ClaimsIdentity)User.Identity;
                var userid = claimsidentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                objorderheader = _unitofWork.OrderHeaderRepository.GetAll(u => u.ApplicationUserId == userid, includeProperties: "ApplicationUser");
            }

            switch (status)
            {
                case "Pending":
                    objorderheader = objorderheader.Where(u => u.PaymentStatus == SD.PaymentStatus_Pending);
                    break;
                case "Approved":
                    objorderheader = objorderheader.Where(u => u.PaymentStatus == SD.Status_Approved);
                    break;
                case "inprocess":
                    objorderheader = objorderheader.Where(u => u.PaymentStatus == SD.Status_InProcess);
                    break;
                case "completed":
                    objorderheader = objorderheader.Where(u => u.OrderStatus == SD.Status_Shipped);
                    break;
            }

            return Json(new { data = objorderheader });
        }
    }
}

        #endregion
    
