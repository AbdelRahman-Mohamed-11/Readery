using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Readery.Core.enums;
using Readery.Core.Models;
using Readery.Core.Repositores;
using Readery.Core.Statics;
using Readery.Web.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace Readery.Web.Areas.Customer.Controllers
{
    [Area("Customer")]

    public class HomeController(IUnitOfWork unitOfWork) : Controller
    {


        [HttpGet]
        [AllowAnonymous]

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);



            var products = await unitOfWork.Products.GetAll(false,
                null, null, null, null, 1, 8, null);


            return View(products);

        }


        [HttpGet]
        [AllowAnonymous]

        public async Task<IActionResult> Details(int productId)
        {
            var product = await unitOfWork
               .Products.Get(c => c.Id == productId, c => c.Include(c => c.Category));

            if (product == null)
            {
                return NotFound();
            }

            ShoppingCart shoppingCart = new()
            {
                ProductId = productId,
                Product = product,
                Quantity = 1,
            };


            return View(shoppingCart);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Details(ShoppingCart shoppingCart)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty,
                    "Quanity Must be greater than 1.");

                return View(shoppingCart);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var existingShoppingCart = await unitOfWork
                .ShoppingCarts
                .Get(
                s => s.ApplicationUserId == Convert.ToInt32(userId)
                && s.ProductId == shoppingCart.ProductId);


            if (existingShoppingCart == null)
            {
                shoppingCart!.ApplicationUserId = Convert.ToInt32(userId);

                var numberOfItems = HttpContext.Session.GetInt32(SessionsApp.SessionCart) ?? 0;

                HttpContext.Session.SetInt32(SessionsApp.SessionCart, numberOfItems + 1);

                await unitOfWork.ShoppingCarts.AddAsync(shoppingCart);
            }

            else
            {
                existingShoppingCart.Quantity += shoppingCart.Quantity;
            }

            await unitOfWork.Complete();

            return RedirectToAction(nameof(Index));
        }

    }
}
