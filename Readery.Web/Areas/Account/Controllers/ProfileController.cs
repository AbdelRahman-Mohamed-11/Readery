using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Readery.Core.DTOS; // Update with your actual namespace
using Readery.Core.Models.Identity;

namespace Readery.Web.Areas.Account.Controllers
{
    [Authorize]
    [Area("Account")]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager; // UserManager for ApplicationUser
        private readonly IWebHostEnvironment _hostingEnvironment; // To access the wwwroot folder

        public ProfileController(UserManager<ApplicationUser> userManager, IWebHostEnvironment hostingEnvironment)
        {
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Profile
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User); // Get the currently logged-in user
            if (user == null)
            {
                return NotFound();
            }

            var model = new UserProfileViewModel
            {
                Name = user.UserName, // Assuming UserName is used as the display name
                PhotoUrl = user.ProfilePhotoUrl, // Ensure PhotoUrl is a property in your ApplicationUser
                NewPassword = null // Optional, not needed here
            };

            return View(model);
        }

        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new UserProfileViewModel
            {
                Name = user.UserName, // Assuming UserName is used as the display name
                PhotoUrl = user.ProfilePhotoUrl, // Ensure ProfilePhotoUrl is a property in your ApplicationUser
                NewPassword = null // Optional, not needed here
            };

            return View(model);
        }

        // POST: Account/Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserProfileViewModel model,
            IFormFile photo)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User); // Get the currently logged-in user
            if (user == null)
            {
                return NotFound();
            }

            // Update user information
            user.UserName = model.Name; // Update UserName

            if (photo != null && photo.Length > 0)
            {
                // Save the photo
                var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = Path.GetFileName(photo.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                // Update the user's photo URL
                user.ProfilePhotoUrl = $"/uploads/{fileName}"; // Ensure you have a property ProfilePhotoUrl in your ApplicationUser
            }

            // Update password if provided
            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }

            // Update the user in the database
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            return RedirectToAction("Index"); // Redirect to the profile page after updating
        }
    }
}