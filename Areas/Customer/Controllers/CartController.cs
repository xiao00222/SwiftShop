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

namespace SwiftShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitofWork _unitofwork;

        public CartController(IUnitofWork unitofwork)
        {
            _unitofwork = unitofwork;
        }

        public IActionResult Index()
        {
            var claimsidentity = (ClaimsIdentity)User.Identity;
            var userid = claimsidentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var shoppingCartVM = new ShoppingCartVM
            {
                ShoppingCartList = _unitofwork.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == userid, includeProperties: "product"),
                OrderHeader = new()
            };

            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                cart.Price = CalculatePrice(cart);
                shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(shoppingCartVM);
        }

        public IActionResult Plus(int cartId)
        {
            var cartfromdb = _unitofwork.ShoppingCartRepository.Getbyid(u => u.Id == cartId);
            cartfromdb.Count += 1;
            _unitofwork.ShoppingCartRepository.Update(cartfromdb);
            _unitofwork.save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cartfromdb = _unitofwork.ShoppingCartRepository.Getbyid(u => u.Id == cartId);
            if (cartfromdb.Count <= 1)
            {
                _unitofwork.ShoppingCartRepository.Remove(cartfromdb);
            }
            else
            {
                cartfromdb.Count -= 1;
                _unitofwork.ShoppingCartRepository.Update(cartfromdb);
            }
            _unitofwork.save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var cartfromdb = _unitofwork.ShoppingCartRepository.Getbyid(u => u.Id == cartId);
            _unitofwork.ShoppingCartRepository.Remove(cartfromdb);
            _unitofwork.save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Summary()
        {
            var claimsidentity = (ClaimsIdentity)User.Identity;
            var userid = claimsidentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var shoppingCartVM = new ShoppingCartVM
            {
                ShoppingCartList = _unitofwork.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == userid, includeProperties: "product"),
                OrderHeader = new()
            };

            var user = _unitofwork.ApplicationUserRepository.Getbyid(u=>u.Id==userid);
            if (user != null)
            {
                shoppingCartVM.OrderHeader.ApplicationUser = user;
                shoppingCartVM.OrderHeader.Name = user.Name;
                shoppingCartVM.OrderHeader.PhoneNumber = user.PhoneNumber;
                shoppingCartVM.OrderHeader.City = user.City;
                shoppingCartVM.OrderHeader.State = user.State;
                shoppingCartVM.OrderHeader.StreetAddress = user.StreetAddress;
            }

            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                cart.Price = CalculatePrice(cart);
                shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(shoppingCartVM);
        }
        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost(ShoppingCartVM shoppingCartVM)
        {
            var claimsidentity = (ClaimsIdentity)User.Identity;
            var userid = claimsidentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCartVM.ShoppingCartList = _unitofwork.ShoppingCartRepository
          .GetAll(u => u.ApplicationUserId == userid, includeProperties: "product");
            shoppingCartVM.OrderHeader.OderDate = System.DateTime.Now;
            shoppingCartVM.OrderHeader.ApplicationUserId = userid;
            //to not populate the navigation property
            ApplicationUser applicationuser = _unitofwork.ApplicationUserRepository.Getbyid(u => u.Id == userid);


            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                cart.Price = CalculatePrice(cart);
                shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            if (applicationuser.CompanyID.GetValueOrDefault() == 0)
            {
                //regular account capture payment
                shoppingCartVM.OrderHeader.PaymentStatus = SD.Status_Pendiing;
                shoppingCartVM.OrderHeader.OrderStatus = SD.Status_Pendiing;
            }
            else
            {
                //Company Account
                shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatus_DelayedPayment;
                shoppingCartVM.OrderHeader.OrderStatus = SD.Status_Approved;

            }
            _unitofwork.OrderHeaderRepository.Add(shoppingCartVM.OrderHeader);
            _unitofwork.save();
            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                OrderDetails orderDetails = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = shoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };
                _unitofwork.OrderDetailsRepository.Add(orderDetails);
                _unitofwork.save();
            }
            if (applicationuser.CompanyID.GetValueOrDefault() == 0)
            {

                var domain = "https://localhost:7193/";

                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={shoppingCartVM.OrderHeader.Id}",
                    CancelUrl = domain + "customer/cart/index",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };
                //iterate over each product in the cart and calculate their price etc and then add all items in the list
                foreach (var item in shoppingCartVM.ShoppingCartList)
                {
                    var sessionlineitem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions()
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.product.Title
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionlineitem);
                }
                    var service = new Stripe.Checkout.SessionService();
                    Session session = service.Create(options);
                    _unitofwork.OrderHeaderRepository.UpdateStripePaymentID(shoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                    _unitofwork.save();
                    Response.Headers.Add("Location", session.Url);
                    return new StatusCodeResult(303);   

                }

                return RedirectToAction(nameof(OrderConfirmation), new { id = shoppingCartVM.OrderHeader.Id });
            }

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitofwork.OrderHeaderRepository.Getbyid(u => u.Id == id,includeProperties:"ApplicationUser");
            if (orderHeader.PaymentStatus != SD.PaymentStatus_DelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                if(session.PaymentStatus.ToLower()=="paid")
                {
                    _unitofwork.OrderHeaderRepository.UpdateStripePaymentID(id, session.Id, session.PaymentIntentId);
                    _unitofwork.OrderHeaderRepository.UpdateStatus(id, SD.Status_Approved, SD.PaymentStatus_Approved);
                    _unitofwork.save();

                }
                HttpContext.Session.Clear();
            }
            List<ShoppingCart> shoppingCarts = _unitofwork.ShoppingCartRepository.GetAll
                (u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            _unitofwork.ShoppingCartRepository.RemoveRange(shoppingCarts);
            _unitofwork.save();

            return View(id);
        }

        private double CalculatePrice(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.product.Price;
            }
            else if (shoppingCart.Count <= 100)
            {
                return shoppingCart.product.Price50;
            }
            else
            {
                return shoppingCart.product.Price100;
            }
        }
    }
}
