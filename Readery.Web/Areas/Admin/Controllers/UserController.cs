using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Readery.Core.Models.Identity;
using Readery.Core.DTOS;
using Microsoft.EntityFrameworkCore;
using Readery.Core.Pagination;
using Readery.Core.enums;
using Readery.Core.Repositores;

namespace Readery.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController(UserManager<ApplicationUser>
        userManager, RoleManager<ApplicationRole> roleManager,
        IUnitOfWork unitOfWork) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index(
    string? search = null,
    SortOrder sortOrder = SortOrder.Asc,
    string columnName = "City",
    int pageNumber = 1,
    int pageSize = 5)
        {
            var usersPaginated = await userManager.Users.PaginatedListAsync(pageNumber, pageSize);

            var userRolesViewModel = new List<UserViewModel>();

            foreach (var user in usersPaginated.Items)
            {
                var roles = await GetUserRoles(user);

                if (!string.IsNullOrWhiteSpace(search) &&
                    (roles.Any(role => role.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    user.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    user.Email.Contains(search, StringComparison.OrdinalIgnoreCase)))
                {
                    userRolesViewModel.Add(new UserViewModel
                    {
                        Email = user.Email,
                        Name = user.Name,
                        Roles = roles,
                        IsActive = user.IsActive,
                    });
                }
            }

            if (!userRolesViewModel.Any())
            {
                foreach (var user in usersPaginated.Items)
                {
                    var roles = await GetUserRoles(user);
                    userRolesViewModel.Add(new UserViewModel
                    {
                        Email = user.Email,
                        Name = user.Name,
                        Roles = roles,
                        IsActive = user.IsActive,
                    });
                }
            }

            var paginatedUserWithRoles = new PaginatedList<UserViewModel>(
                userRolesViewModel, userRolesViewModel.Count, pageNumber, pageSize);

            ViewBag.pageSize = pageSize;
            ViewBag.search = search;
            ViewBag.sortOrder = sortOrder;
            ViewBag.sortColumn = columnName;

            return View(paginatedUserWithRoles);
        }



        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var allRoles = await roleManager.Roles.Select(r => r.Name).ToListAsync();

            var companies = await unitOfWork.Companies.GetAll(); // Replace _context with your actual DbContext

            var viewModel = new UserCreateViewModel
            {
                AllRoles = allRoles,
                Companies = companies.Items.Select(c => new Company { Id = c.Id, Name = c.Name }).ToList() // Adjust as necessary
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.Email,
                    Name = model.Name
                };

                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    if (model.SelectedRoles != null)
                    {
                        foreach (var role in model.SelectedRoles)
                        {
                            await userManager.AddToRoleAsync(user, role);
                        }

                        // If the user is in the Company role, associate them with the selected company
                        if (model.SelectedRoles.Contains("Company") && model.SelectedCompanyId.HasValue)
                        {
                            user.CompanyId = model.SelectedCompanyId.Value; // Set the CompanyId
                        }
                    }

                    await userManager.UpdateAsync(user); // Save the user with the company association if applicable

                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            model.AllRoles = await roleManager.Roles.Select(r => r.Name).ToListAsync();
            var companies = await unitOfWork.Companies.GetAll();

            model.Companies =
                companies
                .Items
                .Select(c => new Company { Id = c.Id, Name = c.Name }
                ).ToList();

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(string email)
        {
            var user = await userManager.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await userManager.GetRolesAsync(user);

            var viewModel = new UserEditViewModel
            {
                Email = user.Email,
                Name = user.Name,
                SelectedRoles = roles.ToList(),
                IsActive = user.IsActive
            };

            var allRoles = await roleManager.Roles.Select(r => r.Name).ToListAsync();
            viewModel.AllRoles = allRoles;

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return NotFound();
                }

                user.Name = model.Name;
                user.IsActive = model.IsActive;

                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    // Remove existing roles
                    var existingRoles = await userManager.GetRolesAsync(user);
                    await userManager.RemoveFromRolesAsync(user, existingRoles);

                    // Add new roles
                    if (model.SelectedRoles != null)
                    {
                        await userManager.AddToRolesAsync(user, model.SelectedRoles);
                    }

                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Re-fetch roles if model state is invalid
            model.AllRoles = await roleManager.Roles.Select(r => r.Name).ToListAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            if (user != null)
            {
                var logins = await userManager.GetLoginsAsync(user);
                foreach (var login in logins)
                {
                    await userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
                }

                var result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return RedirectToAction("Index");
        }



        private async Task<List<string>> GetUserRoles(ApplicationUser user)
        {
            return new List<string>(await userManager.GetRolesAsync(user));
        }

    }
}
