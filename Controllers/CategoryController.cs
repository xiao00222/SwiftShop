using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using SwiftShop.DataAccess.Data;
using SwiftShop.Models;

namespace SwiftShop.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CategoryController(ApplicationDbContext context)
        {
            _context= context;
        }
        public IActionResult Index()
        {
            var CategoryList = _context.Categories.ToList();
            return View(CategoryList);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }
            var category = await _context.Categories.FirstOrDefaultAsync(u => u.id == id);
            if(category==null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                 _context.Update(obj);
                await _context.SaveChangesAsync();
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
        public async Task<IActionResult> Create(Category obj)
        {
            //Custom Validation
            if(obj.Name==obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "Category Name and Display Order cannot be same");
            }
            if (ModelState.IsValid)
            {
                await _context.Categories.AddAsync(obj);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Category Created Successfully";
        
                return RedirectToAction("Index");

            }
            return View(obj);
           
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = await _context.Categories.FirstOrDefaultAsync(u => u.id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost,ActionName("Delete")]
        public async Task<IActionResult> DeletePost (int? id)
        {
            var obj = await _context.Categories.FirstOrDefaultAsync(u => u.id == id);
            if (obj == null)
            {
                return NotFound();
            }
              _context.Categories.Remove(obj);
              await _context.SaveChangesAsync();
            TempData["Success"] = "Category Deleted Successfully";
           
            return RedirectToAction("Index");

        }

    }
}
