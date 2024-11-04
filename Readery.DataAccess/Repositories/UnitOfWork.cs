using Readery.Core.Models;
using Readery.Core.Repositores;
using Readery.DataAccess.Data;
using Readery.Web.Models;

namespace Readery.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        public IGenericRepository<Category> Categories { get; private set; }

        public IGenericRepository<Product> Products { get; private set; }

        public IGenericRepository<Company> Companies { get; private set; }

        public IGenericRepository<ShoppingCart> ShoppingCarts { get; private set; }

        public IGenericRepository<OrderItem> OrderItems { get; private set; }

        public IOrderRepo Orders { get; private set; }
        public UnitOfWork(ApplicationDbContext dbContext)
        {

            _dbContext = dbContext;

            Categories = new GenericRepository<Category>(_dbContext);

            Products = new GenericRepository<Product>(_dbContext);

            Companies = new GenericRepository<Company>(_dbContext);

            ShoppingCarts = new GenericRepository<ShoppingCart>(_dbContext);

            OrderItems = new GenericRepository<OrderItem>(_dbContext);

            Orders = new OrderRepo(_dbContext);
        }

        public async Task<int> Complete() => await _dbContext.SaveChangesAsync();


        public async void Dispose() => await _dbContext.DisposeAsync();
    }
}
