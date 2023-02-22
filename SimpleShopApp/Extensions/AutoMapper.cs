using AutoMapper;
using SimpleShopApp.Models;
using SimpleShopApp.Entities;

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
            CreateMap<Role, RoleModel>();
        }
    }
}
