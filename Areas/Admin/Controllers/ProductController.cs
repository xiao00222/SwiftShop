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
    [Authorize(Roles =SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitofWork _unitofwork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitofWork unitofWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitofwork = unitofWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var result = _unitofwork.ProductRepository.GetAll(includeProperties:"Category").ToList();
           
            return View(result);
        }
        //Create
        public IActionResult Upsert(int? id)
        {
            ProductVM productVm = new()
            {
                CategoryList = _unitofwork.CategoryRepository.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.id.ToString()
                }),
                Product = new Product()

            };
            if(id==null)
            {
                //means its a create request
            return View(productVm);
            }
            else
            {
                //edit request
                productVm.Product = _unitofwork.ProductRepository.Getbyid(u => u.Id == id);
                return View(productVm);
            }
            
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM obj,IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string rootpath = _webHostEnvironment.WebRootPath;
                if(file!=null)
                {
                    //image name
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    //save location
                    string path= Path.Combine(rootpath, @"Images\Product");
                    if(!string.IsNullOrEmpty(obj.Product.ImageUrl))
                    {   //deletion for old image
                        var oldImagePath = Path.Combine(rootpath, obj.Product.ImageUrl.TrimStart('\\'));
                        if(System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using(var filestream=new FileStream(Path.Combine(path,filename),FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }
                    //save the file to the image url attribute
                    obj.Product.ImageUrl= @"\Images\Product\" + filename;
                }
                if(obj.Product.Id==0)
                {

                _unitofwork.ProductRepository.Add(obj.Product);
                }
                else
                {
                    _unitofwork.ProductRepository.Update(obj.Product);
                }
                _unitofwork.save();
                TempData["success"] = "Product Created Successfully";
                return RedirectToAction("Index");
            }
            else
            {
                obj.CategoryList = _unitofwork.CategoryRepository.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.id.ToString()
                });

            }
            return View(obj);
        }
        //[HttpGet]
        //public IActionResult Edit(int? id)
        //{
        //    if(id==null)
        //    {
        //        return NotFound();
        //    }
        //    var obj = _unitofwork.ProductRepository.Getbyid(u => u.Id == id);
        //    if(obj==null)
        //    {
        //        return NotFound();
        //    }
        //    return View(obj);
        //}
        //[HttpPost]
        //public IActionResult Edit(Product obj)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _unitofwork.ProductRepository.Update(obj);
        //        _unitofwork.save();
        //        TempData["success"] = "Product Updated Successfully";
        //        return RedirectToAction("Index");
        //    }
        //    return View(obj);
        //}
        //[HttpGet]
        //public IActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }
        //    var obj = _unitofwork.ProductRepository.Getbyid(u => u.Id == id);
        //    if (obj == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(obj);
        //}
        [HttpPost]
        public IActionResult Delete(Product obj)
        {
            if (ModelState.IsValid)
            {
                _unitofwork.ProductRepository.Remove(obj);
                _unitofwork.save();
                TempData["success"] = "Product Removed Successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        #region Api CALLS
        [Authorize(Roles =SD.Role_Admin)]
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> result = _unitofwork.ProductRepository.GetAll(includeProperties: "Category").ToList();
            return Json(new {data=result});
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var ProductTobeDeleted = _unitofwork.ProductRepository.Getbyid(u => u.Id == id);
            if (ProductTobeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
              var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath,ProductTobeDeleted.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            _unitofwork.ProductRepository.Remove(ProductTobeDeleted);
            _unitofwork.save();
            return Json(new { success = true, message = "Delete Successfull" });
        }
        #endregion
    }
}
