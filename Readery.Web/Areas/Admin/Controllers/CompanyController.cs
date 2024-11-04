using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Readery.Core.enums;
using Readery.Core.Repositores;
using Readery.Core.Models.viewModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.AspNetCore.Mvc.Rendering;
using Readery.Core.Models;

namespace Readery.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CompanyController(IUnitOfWork unitOfWork, IMapper mapper,
        IWebHostEnvironment webHostEnvironment) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index(string? search = null,
            SortOrder sortOrder =
            SortOrder.Asc,
            string columnName = "Name",
            int pageNumber = 1, int pageSize = 5)
        {
            bool conditionToSearch = !String.IsNullOrEmpty(search);

            string sortDirection = sortOrder == SortOrder.Asc ? SortOrder.Asc.ToString() : SortOrder.Desc.ToString();


            var Companys = await unitOfWork.Companies.GetAll(

                    conditionToSearch, p => p.Name.ToLower().Contains(search!.ToLower()) ||
                               p.City.ToLower().Contains(search.ToLower()) ||
                             p.State.ToString().Contains(search),
                   columnName,
                   null,
                   sortDirection,
                pageNumber, pageSize, null);


            ViewBag.pageSize = pageSize;
            ViewBag.search = search;
            ViewBag.sortOrder = sortOrder;
            ViewBag.sortColumn = columnName;
            return View(Companys);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(CompanyViewModel createCompanyDto)
        {
            if (!ModelState.IsValid)
            {

                return View(createCompanyDto);
            }


            var addedCompany = mapper.Map<Company>(createCompanyDto);


            await unitOfWork.Companies.AddAsync(addedCompany);

            await unitOfWork.Complete();

            TempData["success"] = "Company is created sucessfully";

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null || id == 0) { return NotFound(); }

            var Company = await unitOfWork.Companies.Get(c => c.Id == id);

            if (Company == null)
                return NotFound();

            var categories = await unitOfWork.Categories.GetAll();

            var categoriesList = categories.Items.Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() });

            var CompanyViewModel = mapper.Map<CompanyViewModel>(Company);


            return View(CompanyViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CompanyViewModel
            updateCompanyDto
           )
        {
            if (!ModelState.IsValid)
            {
                return View(updateCompanyDto);
            }

            var Company = await unitOfWork.Companies.Get(c => c.Id == updateCompanyDto.Id);

            if (Company == null) return NotFound();



            Company.Name = updateCompanyDto.Name;
            Company.PhoneNumber = updateCompanyDto.PhoneNumber;
            Company.State = updateCompanyDto.State;
            Company.StreetAddress = updateCompanyDto.StreetAddress;
            Company.PostalCode = updateCompanyDto.PostalCode;
            Company.City = updateCompanyDto.City;

            await unitOfWork.Complete();

            TempData["success"] = "Company is Edit sucessfully";

            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var Company = await unitOfWork.Companies.Get(c => c.Id == id);

            if (Company == null)
            {
                return NotFound();
            }

            unitOfWork.Companies.DeleteAsync(Company);

            await unitOfWork.Complete();

            TempData["success"] = "Company is Deleted sucessfully";

            return RedirectToAction("Index");
        }


    }
}
