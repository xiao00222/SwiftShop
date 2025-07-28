using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Swiftshop.Utility;
using SwiftShop.DataAccess.Repository;
using SwiftShop.Models;
using SwiftShop.Models.ViewModel;
using System.Collections.Generic;

namespace SwiftShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitofWork _unitofwork;
        public CompanyController(IUnitofWork unitofWork)
        {
            _unitofwork = unitofWork;
           
        }
        public IActionResult Index()
        {
            var result = _unitofwork.CompanyRepository.GetAll().ToList();
           
            return View(result);
        }
        //Create
        public IActionResult Upsert(int? id)
        {
            if(id==null)
            {
                //means its a create request
            return View(new Company());
            }
            else
            {
                //edit request
                Company companyobj = _unitofwork.CompanyRepository.Getbyid(u => u.Id == id);
                return View(companyobj);
            }
            
        }

        [HttpPost]
        public IActionResult Upsert(Company obj)
        {
            if(ModelState.IsValid)
            {
                if(obj.Id==0)
                {
                    _unitofwork.CompanyRepository.Add(obj);
                }
                else
                {
                    _unitofwork.CompanyRepository.Update(obj);
                }
                _unitofwork.save();
                TempData["success"] = "Company Created Successfully";
                return RedirectToAction("Index");
            }
            else
            {
                return View(obj);
            }

        }
        [HttpPost]
        public IActionResult Delete(Company obj)
        {
            if (ModelState.IsValid)
            {
                _unitofwork.CompanyRepository.Remove(obj);
                _unitofwork.save();
                TempData["success"] = "Company Removed Successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        #region Api CALLS
        [Authorize(Roles =SD.Role_Admin)]
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> result = _unitofwork.CompanyRepository.GetAll().ToList();
            return Json(new {data=result});
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var CompanyTobeDeleted = _unitofwork.CompanyRepository.Getbyid(u => u.Id == id);
            _unitofwork.CompanyRepository.Remove(CompanyTobeDeleted);
            _unitofwork.save();
            return Json(new { success = true, message = "Delete Successfull" });
        }
        #endregion
    }
}
