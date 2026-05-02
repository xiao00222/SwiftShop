using Microsoft.AspNetCore.Mvc;
using SwiftShop.DataAccess.Repository;
using System.Security.Claims;

namespace SwiftShop.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        private readonly IUnitofWork _unitOfWork;

        public CartViewComponent(IUnitofWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IViewComponentResult Invoke()
        {
            int count = 0;

            if (User.Identity.IsAuthenticated)
            {
                var userId = ((ClaimsIdentity)User.Identity)
                             .FindFirst(ClaimTypes.NameIdentifier)
                             .Value;

                count = _unitOfWork.ShoppingCartRepository
                         .GetAll(u => u.ApplicationUserId == userId)
                         .Count();
            }

            return View(count); // sends cart count to view
        }
    }
}
