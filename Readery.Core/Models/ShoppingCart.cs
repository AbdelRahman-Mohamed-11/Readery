using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Readery.Core.Models.Identity;
using Readery.Core.Repositores;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readery.Core.Models
{
    public class ShoppingCart : BaseEntity
    {
        public int ProductId { get; set; }

        [ValidateNever]
        public Product Product { get; set; }

        [ValidateNever]
        public int ApplicationUserId { get; set; }

        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        [Range(1, 1000, ErrorMessage = "the quantity must between 1 and 1000")]
        public int Quantity { get; set; }

        [NotMapped]
        public decimal Price { get; set; }
    }

}
