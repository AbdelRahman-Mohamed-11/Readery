using Readery.Core.Models;
using Readery.Web.Models;


namespace Readery.Core.Repositores;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Category> Categories { get; }

    IGenericRepository<Product> Products { get; }

    IGenericRepository<Company> Companies { get; }

    IGenericRepository<ShoppingCart> ShoppingCarts { get; }

    IGenericRepository<OrderItem> OrderItems { get; }

    IOrderRepo Orders { get; }
    Task<int> Complete();
}

