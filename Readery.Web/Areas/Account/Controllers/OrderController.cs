using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Readery.Core.DTOS;
using Readery.Core.enums;
using Readery.Core.Models;
using Readery.Core.Models.Identity;
using Readery.Core.Repositores;
using Readery.Core.Statics;
using Readery.DataAccess.Repositories;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace Readery.Web.Areas.Account.Controllers
{
    [Area("Admin")]
    public class OrderController(IUnitOfWork unitOfWork) : Controller
    {
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index(string? search = null,
                SortOrder sortOrder = SortOrder.Asc,
                string columnName = "Id",
                string? status = null,
                int pageNumber = 1,
                int pageSize = 5)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            string sortDirection = sortOrder == SortOrder.Asc ? SortOrder.Asc.ToString() : SortOrder.Desc.ToString();
            string? navigationProperty = columnName == "Email" ? "ApplicationUser" : null;

            var statusFilter = status switch
            {
                "Approved" => OrderStatus.Approved,
                "Processing" => OrderStatus.InProcess,
                "Shipped" => OrderStatus.Shipped,
                "Cancelled" => OrderStatus.Cancelled,
                "Refunded" => OrderStatus.Refunded,
                "Pending" => OrderStatus.Pending,
                _ => null,
            };

            ViewBag.pageSize = pageSize;
            ViewBag.search = search;
            ViewBag.sortOrder = sortOrder;
            ViewBag.sortColumn = columnName;
            ViewBag.statusFilter = statusFilter;

            if (User.IsInRole("Admin") || User.IsInRole("Employee"))
            {
                var allOrders = await unitOfWork.Orders.GetAll(
                    true,
                    p =>
                        (statusFilter == null || p.OrderStatus == statusFilter) &&
                         (string.IsNullOrEmpty(search) ||
                          p.Name.ToLower().Contains(search.ToLower()) ||
                          p.PhoneNumber.ToLower().Contains(search.ToLower()) ||
                          p.ApplicationUser != null && p.ApplicationUser.Email.ToLower().Contains(search.ToLower())),
                    columnName,
                    navigationProperty,
                    sortDirection,
                    pageNumber,
                    pageSize,
                    "OrderItems", "ApplicationUser"
                );

                return View(allOrders);
            }


            var customerOrCompanyorders = await unitOfWork.Orders.GetAll(
                true,
               p =>
                      (statusFilter == null || p.OrderStatus == statusFilter) &&
                      p.ApplicationUserId == Convert.ToInt32(userId) &&
                      (string.IsNullOrEmpty(search) ||
                      p.Name.ToLower().Contains(search.ToLower()) ||
                      p.PhoneNumber.ToLower().Contains(search.ToLower()) ||
                      p.ApplicationUser != null && p.ApplicationUser.Email.ToLower().Contains(search.ToLower())),
                columnName,
                navigationProperty,
                sortDirection,
                pageNumber,
                pageSize,
                "OrderItems", "ApplicationUser"
            );


            return View(customerOrCompanyorders);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var order = await unitOfWork.Orders.Get(
                o => o.Id == id,
                query => query
                    .Include(o => o.ApplicationUser)
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product) // Include Product for each OrderItem
            );

            if (order is null)
                return NotFound();

            return View(order);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> UpdateOrderDetail(OrderUpdate orderUpdate)
        {
            var orderFromDb = await unitOfWork.Orders.Get(u => u.Id == orderUpdate.Id);

            if (orderFromDb is null)
            {
                return NotFound();
            }

            orderFromDb.Name = orderUpdate.Name;
            orderFromDb.PhoneNumber = orderUpdate.PhoneNumber;
            orderFromDb.StreetAddress = orderUpdate.StreetAddress;
            orderFromDb.City = orderUpdate.City;
            orderFromDb.State = orderUpdate.State;
            orderFromDb.PostalCode = orderUpdate.PostalCode;

            if (!string.IsNullOrEmpty(orderUpdate.Carrier))
            {
                orderFromDb.Carrier = orderUpdate.Carrier;
            }

            if (!string.IsNullOrEmpty(orderUpdate.TrackingNumber))
            {
                orderFromDb.TrackingNumber = orderUpdate.TrackingNumber;
            }

            await unitOfWork.Complete();

            TempData["Success"] = "Order Details Updated Successfully.";

            return RedirectToAction(nameof(Details), new { id = orderFromDb!.Id });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> StartProcessing(int id)
        {
            var order = await unitOfWork.Orders.Get(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            order.OrderStatus = OrderStatus.InProcess;

            await unitOfWork.Complete();

            TempData["Success"] = "Order Status Updated to Processing Successfully.";

            return RedirectToAction(nameof(Details), new { id = order!.Id });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> ShipOrder(Order order)
        {
            var orderFromDb = await unitOfWork.Orders.Get(u => u.Id == order.Id);

            if (orderFromDb == null)
                return NotFound();

            orderFromDb.TrackingNumber = order.TrackingNumber;
            orderFromDb.Carrier = order.Carrier;

            orderFromDb.OrderStatus = OrderStatus.Shipped;
            orderFromDb.ShippingDate = DateTime.Now;

            if (orderFromDb.PaymentStatus == PaymentStatus.DelayedPayment)
            {
                orderFromDb.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            }

            await unitOfWork.Complete();

            TempData["Success"] = "Order Details Updated Successfully.";
            return RedirectToAction(nameof(Details), new { Id = order.Id });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var order = await unitOfWork.Orders.Get(o => o.Id == id);

            if (order == null)
                return NotFound();

            if (order.PaymentStatus == PaymentStatus.Approved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = order.PaymentIntentId
                };

                // Create a refund service and process the refund
                var service = new RefundService();
                Refund refund = service.Create(options);

                // Update the order status to 'Cancelled' and 'Refunded'
                unitOfWork.Orders.UpdateStatus(order.Id, OrderStatus.Cancelled, PaymentStatus.Refunded);
            }
            else
            {
                // If payment is not approved, just update the status to 'Cancelled'
                unitOfWork.Orders.UpdateStatus(order.Id, OrderStatus.Cancelled, PaymentStatus.Refunded);
            }

            await unitOfWork.Complete();

            TempData["Success"] = "Order is Cancelled Successfully.";
            return RedirectToAction(nameof(Details), new { Id = order.Id });
        }

        [HttpPost]
        public async Task<IActionResult> PayForCompanyUser(int id)
        {
            var order = await unitOfWork.Orders.Get(o => o.Id == id,
                query => query
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product));


            if (order is null)
                return NotFound();

            var domain = $"{Request.Scheme}://{Request.Host}/";
            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"Admin/Order/PaymentConfirmation/{order.Id}",
                CancelUrl = domain + $"Admin/Order/details/{order.Id}",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment"
            };

            foreach (var item in order.OrderItems)
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

            unitOfWork.Orders.UpdateStripePaymentId(order.Id, session.Id, session.PaymentIntentId);

            await unitOfWork.Complete();

            return Redirect(session.Url);
        }

        [HttpGet]
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> PaymentConfirmation(int id)
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var order = await unitOfWork.Orders.Get(u => u.Id == id &&
            u.ApplicationUserId == userId);

            if (order == null)
            {
                return NotFound();
            }
            if (order.PaymentStatus == PaymentStatus.DelayedPayment)
            {
                // order by company user

                var service = new SessionService();
                Session session = service.Get(order.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    unitOfWork.Orders.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);

                    unitOfWork.Orders.UpdateStatus(id, order.OrderStatus!, PaymentStatus.Approved);

                    await unitOfWork.Complete();
                }
            }

            return View(id);
        }

    }
}
