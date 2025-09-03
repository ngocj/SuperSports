using AutoMapper;
using SP.Application.Dto.BrandDto;
using SP.Application.Dto.CartDto;
using SP.Application.Dto.CategoryDto;
using SP.Application.Dto.DiscountDto;
using SP.Application.Dto.EmployeeDto;
using SP.Application.Dto.FeedbackDto;
using SP.Application.Dto.ImageDto;
using SP.Application.Dto.LoginDto;
using SP.Application.Dto.OrderDetailDto;
using SP.Application.Dto.OrderDto;
using SP.Application.Dto.ProductDto;
using SP.Application.Dto.ProductVariantDto;
using SP.Application.Dto.ProvinceDto;
using SP.Application.Dto.RoleDto;
using SP.Application.Dto.UserDto;
using SP.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Application.Mappings
{
    public class MapperProfiles : Profile
    {

        public MapperProfiles()
        {


            CreateMap<Discount, DiscountViewDto>().ReverseMap();
            CreateMap<Discount, DiscountCreateDto>().ReverseMap();

            //get username and prodcutvariant name
            CreateMap<FeedBack, FeedbackViewDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.ProductVariantName, opt => opt.MapFrom(src => src.OrderDetail.ProductVariant.Product.ProductName))
                .ReverseMap();
            CreateMap<FeedBack, FeedbackCreateDto>().ReverseMap();

            CreateMap<OrderDetail, OrderDetailViewDto>().ReverseMap();
            CreateMap<OrderDetailCreateDto, OrderDetail>();
            CreateMap<OrderDetail, OrderDetailCreateDto>();


            CreateMap<OrderCreateDto, Order>()
                .ForMember(dest => dest.Employee, opt => opt.Ignore());
            CreateMap<OrderCreateDto, Order>().ReverseMap();
            CreateMap<Order, OrderUpdateDto>();

            CreateMap<OrderUpdateDto, Order>()
                .ForMember(dest => dest.TotalPrice, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Employee, opt => opt.Ignore())
                .ForMember(dest => dest.OrderDetails, opt => opt.Ignore());


            CreateMap<Order, OrderViewDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.AddressDetail, opt => opt.MapFrom(src => src.User.AddressDetail))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.Name))
                .ReverseMap();
           

            CreateMap<Cart, CartViewDto>().ReverseMap();
            CreateMap<Cart, CartCreateDto>().ReverseMap();
            CreateMap<Cart, CartUpdateDto>().ReverseMap();
            
            CreateMap<Role, RoleViewDto>().ReverseMap();       
            CreateMap<Role, RoleCreateDto>().ReverseMap();

            CreateMap<Category,CategoryViewDto >().ReverseMap();
            CreateMap<Category, CategoryCreateDto>().ReverseMap();
            CreateMap<Category, CategoryUpdateDto>().ReverseMap();

            CreateMap<SubCategory, SubCategoryViewDto>().ReverseMap();
            CreateMap<SubCategory, SubCategoryCreateDto>().ReverseMap();

            CreateMap<Brand, BrandViewDto>().ReverseMap();
            CreateMap<Brand, BrandCreateDto>().ReverseMap();

            CreateMap<User, LoginViewDto>().ReverseMap();
            CreateMap<Employee, LoginViewDto>().ReverseMap();

            CreateMap<Province, ProvinceViewDto>().ReverseMap();
            CreateMap<District, DistrictViewDto>().ReverseMap();
            CreateMap<Ward, WardViewDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.WardName))
                .ReverseMap()
                .ForMember(dest => dest.WardName, opt => opt.MapFrom(src => src.Name));

            CreateMap<Image, ImageFileDto>().ReverseMap();

            CreateMap<User, UserViewDto>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName)).ReverseMap();           
            CreateMap<User, UserCreateDto>().ReverseMap();
            CreateMap<User, UserUpdateDto>().ReverseMap();
            CreateMap<UserUpdateDto, User>()
                .ForMember(dest => dest.PhoneNumber, opt => opt.Ignore())
                .ForMember(dest => dest.DateOfBirth, opt => opt.Ignore())
                .ForMember(dest => dest.AddressDetail, opt => opt.Ignore())
                .ForMember(dest => dest.WardId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.Ward, opt => opt.Ignore())
                .ForMember(dest => dest.Carts, opt => opt.Ignore())
                .ForMember(dest => dest.Orders, opt => opt.Ignore())
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.UserName, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.FeedBacks, opt => opt.Ignore());


            CreateMap<Employee, EmployeeCreateDto>().ReverseMap();
            CreateMap<Employee, EmployeeViewDto>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName))
                .ReverseMap();
            CreateMap<Employee, EmployeeUpdateDto>().ReverseMap();
            CreateMap<EmployeeUpdateDto, Employee>()
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.PhoneNumber, opt => opt.Ignore())
                .ForMember(dest => dest.DateOfBirth, opt => opt.Ignore())
                .ForMember(dest => dest.AddressDetail, opt => opt.Ignore())
                .ForMember(dest => dest.WardId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Ward, opt => opt.Ignore())
                .ForMember(dest => dest.Orders, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore());


            //get name subcategory

            CreateMap<Product, ProductViewDto>()
                .ForMember(dest => dest.SubCategoryName, opt => opt.MapFrom(src => src.SubCategory.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.SubCategory.Category.CategoryName))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand.BrandName))
                .ForMember(dest => dest.Percent, opt => opt.MapFrom(src => src.Discount.Percent))
                .ForMember(dest => dest.IsDiscountActive, opt => opt.MapFrom(src => src.Discount.IsActive)) 
                .ReverseMap();
            CreateMap<Product, ProductCreateDto>().ReverseMap();
            CreateMap<Product, ProductUpdateDto>().ReverseMap();

            
            CreateMap<ProductVariant, VariantViewDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName))              
                .ReverseMap();
            CreateMap<ProductVariant, VariantCreateDto>().ReverseMap();
            CreateMap<VariantUpdateDto, ProductVariant>()
            .ForMember(dest => dest.Images, opt => opt.Ignore());
         

            // registerDto
            CreateMap<User, RegisterDto>().ReverseMap();


          





        }
               
    }
}
