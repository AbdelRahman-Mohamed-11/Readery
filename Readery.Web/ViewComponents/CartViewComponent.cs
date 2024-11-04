using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Readery.Core.Repositores;
using Readery.Core.Statics;
using System.Security.Claims;

namespace Readery.Web.ViewComponents
{
    public class CartViewComponent(IUnitOfWork unitOfWork) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                var shoppingPaginations = await unitOfWork.ShoppingCarts.GetAll();
                HttpContext.Session.SetInt32(SessionsApp.SessionCart, shoppingPaginations.Items.Count);
            }
            else
            {
                HttpContext.Session.SetInt32(SessionsApp.SessionCart, 0);
            }

            return View(HttpContext.Session.GetInt32(SessionsApp.SessionCart));
        }
    }
}
