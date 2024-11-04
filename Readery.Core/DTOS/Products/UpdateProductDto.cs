using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;


namespace Readery.Core.DTOS.Products;

public class UpdateProductDto
{
    public int Id { get; set; }
    public string Description { get; set; }

    public string Title { get; set; }

    public string ISBN { get; set; }

    public string Author { get; set; }

    [Display(Name = "List Price")]
    public decimal ListPrice { get; set; }

    [Display(Name = "Price for 1-50")]
    public decimal Price { get; set; }

    [Display(Name = "Price for 50+")]
    public decimal Price50 { get; set; }

    [Display(Name = "Price for 100+")]
    public decimal Price100 { get; set; }

    public int CategoryId { get; set; }

    public IFormFile? Image { get; set; }
}
