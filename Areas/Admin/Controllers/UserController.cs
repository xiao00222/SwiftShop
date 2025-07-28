using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Swiftshop.Utility;
using SwiftShop.DataAccess.Data;
using SwiftShop.DataAccess.Repository;
using SwiftShop.Models;
using SwiftShop.Models.ViewModel;
using System.Collections.Generic;

namespace SwiftShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {

            return View();
        }

        //[HttpPost]
        //public IActionResult Delete(Company obj)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _unitofwork.CompanyRepository.Remove(obj);
        //        _unitofwork.save();
        //        TempData["success"] = "Company Removed Successfully";
        //        return RedirectToAction("Index");
        //    }
        //    return View(obj);
        //}
        #region Api CALLS
        [Authorize(Roles = SD.Role_Admin)]
        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> result = _context.ApplicationUsers.Include(u => u.Company).ToList();
            var userRoles = _context.UserRoles.ToList();
            var roles = _context.Roles.ToList();
            foreach (var user in result)
            {
                var roleid = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleid).Name;
                if (user.Company == null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }
            }
            return Json(new { data = result });
        }
        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var objfromdb = _context.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (objfromdb == null)
            {


                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }
            if(objfromdb.LockoutEnd!=null && objfromdb.LockoutEnd>DateTime.Now)
            {
                //user is locked
                objfromdb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objfromdb.LockoutEnd = DateTime.Now.AddYears(100);
            }
            _context.SaveChanges();
            return Json(new { success = true, message = " Successfull" });

        }
    }
            #endregion
}
