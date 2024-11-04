using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Readery.Core.DTOS;
using Readery.Core.Models.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using Readery.Utilities.Interfaces;
using Readery.Utilities.EmailModels;

namespace SeaSprayStay.Web.Areas.Account.Controllers
{
    [Area("Account")]
    [AllowAnonymous]
    public class AccountController(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager,
        IEmailService emailService) : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(temp => temp.Errors)
                                           .Select(temp => temp.ErrorMessage);
                return View(registerDto);
            }

            ApplicationUser applicationUser = new ApplicationUser
            {
                Email = registerDto.Email,
                UserName = registerDto.Email,
                Name = registerDto.Name,
                City = registerDto.City,
                State = registerDto.State,
                Street = registerDto.Street,
                PhoneNumber = registerDto.PhoneNumber,
                PostalCode = registerDto.PostalCode,
            };

            var result = await userManager.CreateAsync(applicationUser, registerDto.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Register", error.Description);
                }

                return View(registerDto);
            }

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto, string? returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(temp => temp.Errors)
                                           .Select(temp => temp.ErrorMessage);
                return View(loginDto);
            }

            var user = await userManager.FindByEmailAsync(loginDto.Email);

            if (user == null || !user.IsActive)
            {
                ModelState.AddModelError(string.Empty, "Your account is not active. Please contact support.");
                ViewBag.Errors = ModelState.Values.SelectMany(temp => temp.Errors)
                                                   .Select(temp => temp.ErrorMessage);
                return View(loginDto);
            }

            var result = await signInManager.PasswordSignInAsync(loginDto.Email,
                loginDto.Password, loginDto.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }

                return RedirectToAction("Index", "Home", new { area = "Customer" });
            }
            else
            {
                // If the login fails, add an error message to the model state
                ModelState.AddModelError(string.Empty, "Email or password are incorrect.");
                ViewBag.Errors = ModelState.Values.SelectMany(temp => temp.Errors)
                                           .Select(temp => temp.ErrorMessage);

                return View(loginDto);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }

        [HttpGet]
        public async Task<IActionResult> IsEmailAlreadyInUse(string email)
        {
            return await userManager.FindByEmailAsync(email) == null ? Json(true) : Json(false);
        }


        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Find the user by email
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Something Incorrect");
                return View();
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink = Url.Action("ResetPassword", "Account", new { token, email = user.Email }, Request.Scheme);

            var emailMessage = new EmailMessage
            {
                Name = user.Name,
                ToAddress = user.Email,
                Subject = "Reset Your Password",
                Body = resetLink  // Make sure this is the actual link
            };

            await emailService.SendEmail(emailMessage);


            return RedirectToAction("ForgotPasswordConfirmation");
        }


        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }


        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                return RedirectToAction("Login");
            }

            var model = new ResetPasswordDto { Token = token, Email = email };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Do not reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation");
            }

            var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View();
        }


        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLogin(string provider, string? returnUrl = null)
        {
            // Set the redirect URL after external login callback
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            // Configure external authentication properties
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            // Challenge the user to log in with the external provider (Google/Facebook)
            return Challenge(properties, provider);
        }

        // Callback action after external login provider returns
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            // Check if there was a remote error during the login process
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, "Login was canceled. Please try again.");
                return RedirectToAction(nameof(Login));
            }

            // Retrieve external login information
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Extract email and name from external login information
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var name = info.Principal.FindFirstValue("name") ?? email;

            if (email != null)
            {
                // Check if the user already exists in the database
                var user = await userManager.FindByEmailAsync(email);

                if (user != null)
                {
                    // User exists, sign them in
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    // User does not exist, create a new user account
                    var newUser = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        Name = name,
                    };

                    // Attempt to create the new user
                    var result = await userManager.CreateAsync(newUser);
                    if (result.Succeeded)
                    {
                        // add user to Customer Role
                        await userManager.AddToRoleAsync(newUser, "Customer");
                        // Optionally add the external login
                        result = await userManager.AddLoginAsync(newUser, info);
                        if (result.Succeeded)
                        {
                            // Sign in the new user
                            await signInManager.SignInAsync(newUser, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                    }

                    // Handle errors in user creation
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return RedirectToAction(nameof(Register)); // Redirect to registration if there's an error
                }
            }

            return RedirectToAction(nameof(Login)); // Fallback to login if email is null
        }
    }
}
