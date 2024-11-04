using Readery.Core.Models.Identity;
using Readery.Core.Repositores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readery.Core.Models
{
    public class Order : BaseEntity
    {
        public int ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ShippingDate { get; set; }
        public decimal OrderTotal { get; set; }
        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateOnly PaymentDueDate { get; set; }
        public string PhoneNumber { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Name { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = [];

        // paybal
        public string? PayPalOrderId { get; set; }
        public string? PayPalCaptureId { get; set; }

        // stripe
        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set; }

    }

}
