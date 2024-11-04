using AutoMapper;
using Readery.Core.DTOS.Products;
using Readery.Core.Models;
using Readery.Core.Models.viewModels;
using Readery.Web.DTOS.Category.Create;
using Readery.Web.Models;

namespace Readery.Web.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateCategoryDto, Category>();
            CreateMap<CreateProductDto, Product>();
            CreateMap<ProductViewModel, Product>();
            CreateMap<Product, ProductViewModel>();
            CreateMap<Company, CompanyViewModel>();
        }
    }
}
