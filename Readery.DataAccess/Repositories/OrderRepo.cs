using Microsoft.EntityFrameworkCore;
using Readery.Core.Models;
using Readery.Core.Repositores;
using Readery.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readery.DataAccess.Repositories
{
    public class OrderRepo(ApplicationDbContext applicationDbContext) : GenericRepository<Order>(applicationDbContext), IOrderRepo
    {
        private readonly ApplicationDbContext _db = applicationDbContext;

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderFromDb = _db.Orders.FirstOrDefault(u => u.Id == id);
            if (orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;

                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }

            }
        }

        public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
        {
            var orderFromDb = _db.Orders.FirstOrDefault(u => u.Id == id);
            if (orderFromDb != null)
            {
                if (!string.IsNullOrEmpty(sessionId))
                {
                    orderFromDb.SessionId = sessionId;
                }

                if (!string.IsNullOrEmpty(paymentIntentId))
                {
                    orderFromDb.PaymentIntentId = paymentIntentId;
                    orderFromDb.PaymentDate = DateTime.UtcNow;  // Record the payment date when the payment intent ID is updated
                }

            }
        }


    }
}
