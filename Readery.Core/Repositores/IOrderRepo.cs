using Readery.Core.Models;


namespace Readery.Core.Repositores
{
    public interface IOrderRepo : IGenericRepository<Order>
    {
        void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);

        void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId);
    }
}
