using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Readery.Core.Repositores;
using Readery.Web.DTOS.Category.Create;
using Readery.Web.DTOS.Category.Update;
using Readery.Web.Models;

namespace Readery.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class CategoryController(IUnitOfWork unitOfWork, IMapper mapper) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var categories = await unitOfWork.Categories.GetAll();

        return View(categories);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]

    public async Task<IActionResult> Create(CreateCategoryDto createCategoryDto)
    {
        if (!ModelState.IsValid)
        {

            return View();
        }

        var addedCategory = mapper.Map<Category>(createCategoryDto);

        await unitOfWork.Categories.AddAsync(addedCategory);

        await unitOfWork.Complete();

        TempData["success"] = "Category is created sucessfully";

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null || id == 0) { return NotFound(); }

        var category = await unitOfWork.Categories.Get(c => c.Id == id);

        if (category == null)
            return NotFound();

        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateCategoryDto updateCategoryDto)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }
        var category = await unitOfWork.Categories.Get(g => g.Id == updateCategoryDto.Id);

        if (category == null)
            return NotFound();

        category.DisplayOrder = updateCategoryDto.DisplayOrder;

        category.Name = updateCategoryDto.Name;

        await unitOfWork.Complete();

        TempData["success"] = "Category is Edit sucessfully";

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == 0 || id is null) { return NotFound(); }

        var category = await unitOfWork.Categories.Get(c => c.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeletePost(int id)
    {
        var category = await unitOfWork.Categories.Get(c => c.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        unitOfWork.Categories.DeleteAsync(category);

        await unitOfWork.Complete();

        TempData["success"] = "Category is Deleted sucessfully";

        return RedirectToAction("Index");
    }
}
