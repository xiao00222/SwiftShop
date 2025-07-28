using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Swiftshop.Utility;
using SwiftShop.DataAccess.Data;
using SwiftShop.DataAccess.Repository;
using SwiftShop.Models;

namespace SwiftShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)] //Can also be applied to individual methods
    public class CategoryController : Controller
    {
        private readonly IUnitofWork _unitofWork;
        public CategoryController(IUnitofWork unitofWork)
        {
            _unitofWork = unitofWork;
        }
        public IActionResult Index()
        {
            var CategoryList = _unitofWork.CategoryRepository.GetAll().ToList();
            return View(CategoryList);
        }
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = _unitofWork.CategoryRepository.Getbyid(u => u.id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _unitofWork.CategoryRepository.Update(obj);
                _unitofWork.save();
                TempData["Success"] = "Category Updated Successfully";

                //Tempdata is used to display a notification
                //temporarily after an event occurs,notification is displayed on the next render
                return RedirectToAction("Index");

            }
            return View();

        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            //Custom Validation
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "Category Name and Display Order cannot be same");
            }
            if (ModelState.IsValid)
            {
                _unitofWork.CategoryRepository.Add(obj);
                _unitofWork.save();
                TempData["Success"] = "Category Created Successfully";

                return RedirectToAction("Index");

            }
            return View(obj);

        }
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = _unitofWork.CategoryRepository.Getbyid(u => u.id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            var obj = _unitofWork.CategoryRepository.Getbyid(u => u.id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitofWork.CategoryRepository.Remove(obj);
            _unitofWork.save();
            TempData["Success"] = "Category Deleted Successfully";

            return RedirectToAction("Index");

        }

    }
}
