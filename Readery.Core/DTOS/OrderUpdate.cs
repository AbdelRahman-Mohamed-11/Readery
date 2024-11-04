using Readery.Core.Models.Identity;
using Readery.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readery.Core.DTOS
{
    public class OrderUpdate
    {
        public int Id { get; set; }
        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; }

        public string PhoneNumber { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Name { get; set; }

    }
}
