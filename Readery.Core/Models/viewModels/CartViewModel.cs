using Readery.Core.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readery.Core.Models.viewModels
{
    public class CartViewModel
    {
        public PaginatedList<ShoppingCart> ShoppingCarts { get; set; }

        public Order Order { get; set; }
    }
}
