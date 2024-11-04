using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Readery.Core.DTOS.Products;
using Readery.Core.enums;
using Readery.Core.Models;
using Readery.Core.Models.viewModels;
using Readery.Core.Pagination;
using Readery.Core.Repositores;
using Readery.DataAccess.Repositories;
using Readery.Web.DTOS.Category.Create;
using Readery.Web.DTOS.Category.Update;
using Readery.Web.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Readery.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController(IUnitOfWork unitOfWork, IMapper mapper,
        IWebHostEnvironment webHostEnvironment) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index(
            string? search = null, SortOrder sortOrder =
            SortOrder.Asc,
            string columnName = "Title",
            int pageNumber = 1, int pageSize = 5)
        {
            bool conditionToSearch = !String.IsNullOrEmpty(search);

            string sortDirection = sortOrder == SortOrder.Asc ? SortOrder.Asc.ToString() : SortOrder.Desc.ToString();


            var products = await unitOfWork.Products.GetAll(

                    conditionToSearch, p => p.Title.ToLower().Contains(search!.ToLower()) ||
                               p.Author.ToLower().Contains(search.ToLower()) ||
                               p.ISBN.ToLower().Contains(search.ToLower()) ||
                             p.ListPrice.ToString().Contains(search),
                   columnName,
                   null,
                   sortDirection,
                pageNumber, pageSize, "Category");


            ViewBag.pageSize = pageSize;
            ViewBag.search = search;
            ViewBag.sortOrder = sortOrder;
            ViewBag.sortColumn = columnName;
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await unitOfWork.Categories.GetAll();

            var categoriesList = categories.Items.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = c.Name, Value = c.Id.ToString() });

            var productViewModel = new ProductViewModel
            {
                CategoriesListItems = categoriesList,
            };

            return View(productViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(ProductViewModel createProductDto, IFormFile image)
        {
            if (!ModelState.IsValid)
            {
                createProductDto.CategoriesListItems = (await unitOfWork.Categories.GetAll())
                    .Items.Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() });

                return View(createProductDto);
            }

            string uniqueFileName = await SaveUploadedFile(image);


            var addedProduct = mapper.Map<Product>(createProductDto);

            addedProduct.ImageUrl = uniqueFileName;

            await unitOfWork.Products.AddAsync(addedProduct);

            await unitOfWork.Complete();

            TempData["success"] = "Product is created sucessfully";

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null || id == 0) { return NotFound(); }

            var product = await unitOfWork.Products.Get(c => c.Id == id);

            if (product == null)
                return NotFound();

            var categories = await unitOfWork.Categories.GetAll();

            var categoriesList = categories.Items.Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() });

            var productViewModel = mapper.Map<ProductViewModel>(product);

            productViewModel.CategoriesListItems = categoriesList;

            return View(productViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductViewModel updateProductDto,
            IFormFile? image)
        {
            if (!ModelState.IsValid)
            {
                // Repopulate categories for dropdown
                updateProductDto.CategoriesListItems = (await unitOfWork.Categories.GetAll())
                    .Items.Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
                return View(updateProductDto);
            }

            var product = await unitOfWork.Products.Get(c => c.Id == updateProductDto.Id);

            if (product == null) return NotFound();

            string? imageUrl = null;

            if (image != null)
            {
                DeleteImage(product.ImageUrl);
                imageUrl = await SaveUploadedFile(image);
            }

            product.Author = updateProductDto.Author;
            product.Description = updateProductDto.Description;
            product.CategoryId = updateProductDto.CategoryId;
            product.ISBN = updateProductDto.ISBN;
            product.Title = updateProductDto.Title;
            product.ListPrice = updateProductDto.ListPrice;
            product.Price = updateProductDto.Price;
            product.Price100 = updateProductDto.Price100;
            product.Price50 = updateProductDto.Price50;

            if (imageUrl != null)
            {
                product.ImageUrl = imageUrl!;
            }

            await unitOfWork.Complete();

            TempData["success"] = "Product is Edit sucessfully";

            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await unitOfWork.Products.Get(c => c.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            DeleteImage(product.ImageUrl);

            unitOfWork.Products.DeleteAsync(product);

            await unitOfWork.Complete();

            TempData["success"] = "Product is Deleted sucessfully";

            return RedirectToAction("Index");
        }

        private async Task<string> SaveUploadedFile(IFormFile imageFile)
        {
            // Check if image is not null
            if (imageFile == null)
            {
                throw new ArgumentNullException(nameof(imageFile), "Image file is required.");
            }

            // Define the upload folder path
            string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images/products");

            // Generate a unique file name
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;

            // Ensure the directory exists
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Combine the folder path with the unique file name
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Copy the file to the server
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            // Return the unique file name
            return uniqueFileName;
        }

        private void DeleteImage(string imageUrl)
        {
            if (!string.IsNullOrEmpty(imageUrl))
            {
                string oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, "images/products", imageUrl);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }
        }
    }
}
