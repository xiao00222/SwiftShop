using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swiftshop.Utility;
using SwiftShop.DataAccess.Repository;
using SwiftShop.Models;

namespace SwiftShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitofWork _unitofwork;

        public HomeController(ILogger<HomeController> logger,IUnitofWork unitofWork)
        {
            _logger = logger;
            _unitofwork=unitofWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> ProductList=_unitofwork.ProductRepository.GetAll(includeProperties:"Category");
            return View(ProductList);
        }
        public IActionResult Details(int Id)
        {
            ShoppingCart shoppingCart = new()
            {
                product = _unitofwork.ProductRepository.Getbyid(u => u.Id == Id, includeProperties: "Category"),
                Count = 1,
                ProductId = Id
            };
            return View(shoppingCart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart cart)
        {
            var claimsidentity = (ClaimsIdentity)User.Identity;
            var useridentity = claimsidentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            cart.ApplicationUserId = useridentity;
            cart.Id = 0;
            ShoppingCart cartfromdb= _unitofwork.ShoppingCartRepository.Getbyid(u=>u.ApplicationUserId==useridentity&&
            u.ProductId==cart.ProductId);
            if(cartfromdb!=null)
            {
                cartfromdb.Count += cart.Count;
                _unitofwork.ShoppingCartRepository.Update(cartfromdb);
                _unitofwork.save();

            }
            else
            {
                _unitofwork.ShoppingCartRepository.Add(cart);
                _unitofwork.save();

                //HttpContext.Session.SetInt32(SD.SessionCart, _unitofwork.ShoppingCartRepository.
                //    GetAll(u => u.ApplicationUserId == useridentity).Count());
            }
            TempData["success"] = "cart updated successfully"; 
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
