using AutoMapper;
using SimpleShopApp.Models;
using SimpleShopApp.DAL;

namespace SimpleShopApp.Extensions
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<Category, CategoryModel>();
            CreateMap<Product, ProductModel>()
                .ForMember(d => d.CategoryName, s => s.MapFrom(e => e.Category.Name));
            CreateMap<ProductModel, Product>();
        }
    }
}
