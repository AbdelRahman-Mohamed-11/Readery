using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Readery.Core.Models;
using Readery.Core.Models.Identity;
using Readery.Core.Models.viewModels;
using Readery.Core.Repositores;
using Readery.Core.Statics;
using Readery.DataAccess.Data;
using Readery.DataAccess.Repositories;
using Readery.Utilities.Interfaces;
using Stripe.Checkout;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace Readery.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController(IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager,
        IConfiguration config) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var shoppingCarts = await unitOfWork
                .ShoppingCarts.GetAll(false, null, null, null, null, null, null, "Product");

            var cartViewModel = new CartViewModel
            {
                ShoppingCarts = shoppingCarts,
                Order = new(),
            };

            foreach (var item in cartViewModel.ShoppingCarts.Items)
            {
                item.Price = GetPriceBasedOnQuantity(item);
                Console.WriteLine($"Product ID: {item.ProductId}, Quantity: {item.Quantity}, Price: {item.Price}");
                cartViewModel.Order.OrderTotal += (item.Price * item.Quantity);
            }

            return View(cartViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> IncreaseQuantity(int cartId)
        {
            var cartFromDb = await unitOfWork.ShoppingCarts.Get(u => u.Id == cartId);

            if (cartFromDb == null)
                return NotFound();

            cartFromDb.Quantity += 1;

            await unitOfWork.Complete();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DecreaseQuantity(int cartId)
        {
            var cartFromDb = await unitOfWork.ShoppingCarts.Get(u => u.Id == cartId);

            if (cartFromDb == null)
                return NotFound();

            if (cartFromDb.Quantity > 1)
            {
                cartFromDb.Quantity -= 1;
            }
            else
            {
                unitOfWork.ShoppingCarts.DeleteAsync(cartFromDb);

                var numberOfItems = HttpContext.Session.GetInt32(SessionsApp.SessionCart);

                if (numberOfItems == 1)
                    HttpContext.Session.SetInt32(SessionsApp.SessionCart, 0);
                else
                {
                    HttpContext.Session.SetInt32(SessionsApp.SessionCart,
                        numberOfItems!.Value - 1);
                }
            }

            await unitOfWork.Complete();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int cartId)
        {
            var cartFromDb = await unitOfWork.ShoppingCarts.Get(u => u.Id == cartId);

            if (cartFromDb == null)
                return NotFound();

            unitOfWork.ShoppingCarts.DeleteAsync(cartFromDb);


            await unitOfWork.Complete();

            var numberOfItems = HttpContext.Session.GetInt32(SessionsApp.SessionCart);

            if (numberOfItems == 1)
                HttpContext.Session.SetInt32(SessionsApp.SessionCart, 0);
            else
            {
                HttpContext.Session.SetInt32(SessionsApp.SessionCart,
                    numberOfItems!.Value - 1);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Summary()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return NotFound();

            var user = await userManager.FindByIdAsync(userId!);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var shoppingCart = await unitOfWork
                .ShoppingCarts.GetAll(false, u => u.ApplicationUserId == Convert.ToInt32(userId),
                null, null, null, null, null, "Product");

            CartViewModel cartViewModel = new()
            {
                ShoppingCarts = shoppingCart,

                Order = new()
            };

            cartViewModel.Order.Name = user.Name;
            cartViewModel.Order.StreetAddress = user.Street;
            cartViewModel.Order.State = user.State;
            cartViewModel.Order.City = user.City;
            cartViewModel.Order.PostalCode = user.PostalCode;
            cartViewModel.Order.PhoneNumber = user.PhoneNumber;

            foreach (var cart in cartViewModel.ShoppingCarts.Items)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                cartViewModel.Order.OrderTotal += (cart.Price * cart.Quantity);
            }

            return View(cartViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Summary(CartViewModel cartViewModel,
            string paymentMethod)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return NotFound();

            cartViewModel.ShoppingCarts = await unitOfWork
                .ShoppingCarts.GetAll(false, u => u.ApplicationUserId == Convert.ToInt32(userId), null, null, null, null, null, "Product");

            cartViewModel.Order.OrderDate = DateTime.Now;
            cartViewModel.Order.ApplicationUserId = Convert.ToInt32(userId);

            ApplicationUser applicationUser = await userManager.FindByIdAsync(userId!);
            if (applicationUser == null) return NotFound("User not found");

            foreach (var cart in cartViewModel.ShoppingCarts.Items)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                cartViewModel.Order.OrderTotal += (cart.Price * cart.Quantity);
            }

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                await unitOfWork.Orders.AddAsync(cartViewModel.Order);

                await unitOfWork.Complete();

                cartViewModel.Order.OrderStatus = OrderStatus.Pending;
                cartViewModel.Order.PaymentStatus = PaymentStatus.Pending;

                var orderItems = cartViewModel.ShoppingCarts.Items.Select(cart => new OrderItem
                {
                    ProductId = cart.ProductId,
                    OrderId = cartViewModel.Order.Id,
                    Price = cart.Price,
                    Quantity = cart.Quantity,
                }).ToList();

                await unitOfWork.OrderItems.BulkInsertAsync(orderItems);

                if (paymentMethod == "Stripe")
                {
                    var domain = $"{Request.Scheme}://{Request.Host}/";
                    var options = new SessionCreateOptions
                    {
                        SuccessUrl = domain + $"customer/cart/OrderConfirmation/{cartViewModel.Order.Id}",
                        CancelUrl = domain + "customer/cart/index",
                        LineItems = new List<SessionLineItemOptions>(),
                        Mode = "payment"
                    };

                    foreach (var item in cartViewModel.ShoppingCarts.Items)
                    {
                        var sessionLineItem = new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (long)(item.Price * 100),
                                Currency = "usd",
                                ProductData = new SessionLineItemPriceDataProductDataOptions { Name = item.Product.Title }
                            },
                            Quantity = item.Quantity
                        };
                        options.LineItems.Add(sessionLineItem);
                    }

                    var service = new SessionService();
                    var session = service.Create(options);

                    unitOfWork.Orders.UpdateStripePaymentId(cartViewModel.Order.Id, session.Id, session.PaymentIntentId);

                    await unitOfWork.Complete();

                    return Redirect(session.Url);
                }
                else if (paymentMethod == "PayPal")
                {
                    // PayPal payment handling
                    var paypalUrl = await CreatePayPalPayment(cartViewModel.Order);
                    if (!string.IsNullOrEmpty(paypalUrl))
                    {
                        return Redirect(paypalUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Could not create PayPal payment.");
                        return View(cartViewModel);
                    }
                }
            }

            else
            {
                cartViewModel.Order.OrderStatus = OrderStatus.Approved;
                cartViewModel.Order.PaymentStatus = PaymentStatus.DelayedPayment;

                await unitOfWork.Orders.AddAsync(cartViewModel.Order);

                await unitOfWork.Complete();


                var orderItems = cartViewModel.ShoppingCarts.Items.Select(cart => new OrderItem
                {
                    ProductId = cart.ProductId,
                    OrderId = cartViewModel.Order.Id,
                    Price = cart.Price,
                    Quantity = cart.Quantity,
                }).ToList();

                await unitOfWork.OrderItems.BulkInsertAsync(orderItems);

                await unitOfWork.Complete();

                return RedirectToAction(nameof(OrderConfirmation), new { Id = cartViewModel.Order.Id });
            }

            return View(cartViewModel);
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var order = await unitOfWork.Orders.Get(u => u.Id == id
            && u.ApplicationUserId == userId);

            if (order == null)
            {
                return NotFound();
            }
            // Check if the payment status is not delayed payment
            if (order.PaymentStatus != PaymentStatus.DelayedPayment)
            {
                // Create a new SessionService to retrieve the payment session
                var service = new SessionService();
                Session session = service.Get(order.SessionId);

                // Check if the payment status is "paid"
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    // Update Stripe payment information (SessionId and PaymentIntentId)
                    unitOfWork.Orders.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);

                    // Update the order status to "Approved" and payment status to "Approved"
                    unitOfWork.Orders.UpdateStatus(id, PaymentStatus
                        .Approved, PaymentStatus.Approved);

                    await unitOfWork.Complete();


                }
            }
            // Retrieve all shopping carts associated with the user
            // (ApplicationUserId)
            var shoppingCartsPagintated = await unitOfWork
                .ShoppingCarts.GetAll(true, u => u.ApplicationUserId ==
                order.ApplicationUserId);

            var shoppingCartsIds = shoppingCartsPagintated.Items.Select(i => i.Id);

            await unitOfWork.ShoppingCarts.BulkDeleteAsync<ShoppingCart>(shoppingCartsIds);

            await unitOfWork.Complete();

            return View(id);
        }







        // for paybal , but test not working in egypt        
        //[HttpGet]
        //public async Task<IActionResult> PaymentSuccess(int orderId)
        //{
        //    //logger.LogInformation($"PaymentSuccess called for Order ID: {orderId}");

        //    var clientId = config["PayPalOptions:ClientId"];
        //    var secret = config["PayPalOptions:ClientSecret"];
        //    var baseUrl = "https://api-m.sandbox.paypal.com"; // Switch to live URL in production

        //    try
        //    {
        //        // Obtain access token
        //        var client = new HttpClient();
        //        var byteArray = Encoding.ASCII.GetBytes($"{clientId}:{secret}");
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

        //        var body = new Dictionary<string, string>
        //        {
        //            { "grant_type", "client_credentials" }
        //        };

        //        var tokenResponse = await client.PostAsync($"{baseUrl}/v1/oauth2/token", new FormUrlEncodedContent(body));
        //        if (!tokenResponse.IsSuccessStatusCode)
        //        {
        //            //logger.LogError("Failed to obtain PayPal access token.");
        //            return RedirectToAction("PaymentError");
        //        }

        //        var tokenContent = await tokenResponse.Content.ReadAsStringAsync();
        //        dynamic tokenJson = JsonConvert.DeserializeObject(tokenContent);
        //        string accessToken = tokenJson.access_token;

        //        // Capture the payment
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        //        var captureResponse = await client.PostAsync($"{baseUrl}/v2/checkout/orders/capture", null);

        //        if (!captureResponse.IsSuccessStatusCode)
        //        {
        //            //logger.LogError("Failed to capture PayPal payment.");
        //            return RedirectToAction("PaymentError");
        //        }

        //        var captureContent = await captureResponse.Content.ReadAsStringAsync();
        //        dynamic captureJson = JsonConvert.DeserializeObject(captureContent);
        //        string captureId = captureJson.purchase_units[0].payments.captures[0].id;
        //        DateTime captureDate = captureJson.purchase_units[0].payments.captures[0].create_time;

        //        // Update order status and store PayPal details
        //        var order = await unitOfWork.Orders.Get(u => u.Id == orderId);
        //        if (order != null)
        //        {
        //            order.PaymentStatus = PaymentStatus.Approved;
        //            order.OrderStatus = OrderStatus.Approved;
        //            order.PayPalOrderId = captureJson.id;
        //            order.PayPalCaptureId = captureId;
        //            order.PayPalPaymentDate = captureDate;

        //            unitOfWork.Orders.UpdateAsync(order);
        //            await unitOfWork.Complete();

        //            //logger.LogInformation($"Order ID: {orderId} marked as paid with PayPal Capture ID: {captureId}");
        //        }

        //        return RedirectToAction(nameof(OrderConfirmation), new { orderId = orderId });
        //    }
        //    catch (Exception ex)
        //    {
        //        //logger.LogError($"Exception in PaymentSuccess: {ex.Message}");
        //        return RedirectToAction("PaymentError");
        //    }
        //}


        // paybal for create the token
        private async Task<string?> CreatePayPalPayment(Order order)
        {
            var clientId = config["PayPalOptions:ClientId"];
            var secret = config["PayPalOptions:ClientSecret"];
            var baseUrl = "https://api-m.sandbox.paypal.com"; // Use sandbox for testing

            try
            {
                // Obtain access token
                var client = new HttpClient();
                var byteArray = Encoding.ASCII.GetBytes($"{clientId}:{secret}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var body = new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" }
                };

                var tokenResponse = await client.PostAsync($"{baseUrl}/v1/oauth2/token", new FormUrlEncodedContent(body));
                if (!tokenResponse.IsSuccessStatusCode)
                {
                    //logger.LogError("Failed to obtain PayPal access token.");
                    return null;
                }

                var tokenContent = await tokenResponse.Content.ReadAsStringAsync();
                dynamic tokenJson = JsonConvert.DeserializeObject(tokenContent);
                string accessToken = tokenJson.access_token;

                // Create payment
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var payment = new
                {
                    intent = "CAPTURE",
                    purchase_units = new[]
                    {
                        new {
                            amount = new {
                                currency_code = "USD",
                                value = order.OrderTotal.ToString("F2")
                            }
                        }
                    },
                    redirect_urls = new
                    {
                        return_url = $"http://localhost:5195/Customer/Cart/PaymentSuccess/{order.Id}",
                        //cancel_url = Url.Action("PaymentCancel", "Cart", new { orderId = order.Id }, Request.Scheme)
                    }
                };

                var paymentContent = new StringContent(JsonConvert.SerializeObject(payment), Encoding.UTF8, "application/json");
                var paymentResponse = await client.PostAsync($"{baseUrl}/v2/checkout/orders", paymentContent);

                if (!paymentResponse.IsSuccessStatusCode)
                {
                    //logger.LogError("Failed to create PayPal order.");
                    return null;
                }

                var paymentResult = await paymentResponse.Content.ReadAsStringAsync();
                dynamic paymentJson = JsonConvert.DeserializeObject(paymentResult);

                foreach (var link in paymentJson.links)
                {
                    if (link.rel == "approve")
                    {
                        // Store PayPal Order ID
                        order.PayPalOrderId = paymentJson.id;
                        return link.href;
                    }
                }

                //logger.LogError("Approval link not found in PayPal response.");
                return null;
            }
            catch (Exception ex)
            {
                //logger.LogError($"Exception during PayPal payment creation: {ex.Message}");
                return null;
            }
        }



        private decimal GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Quantity <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else if (shoppingCart.Quantity <= 100)
            {
                return shoppingCart.Product.Price50;
            }
            else
            {
                return shoppingCart.Product.Price100;
            }
        }


    }
}
