using Readery.Core.Repositores;
using Readery.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readery.Core.Models;

public class Product : BaseEntity
{
    public string Description { get; set; }

    public string Title { get; set; }

    public string ISBN { get; set; }

    public string Author { get; set; }

    public decimal ListPrice { get; set; }

    public decimal Price { get; set; }

    public decimal Price50 { get; set; }

    public decimal Price100 { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public Category Category { get; set; }
}
