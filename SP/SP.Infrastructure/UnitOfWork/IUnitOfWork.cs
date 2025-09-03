using SP.Infrastructure.Context;
using SP.Infrastructure.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Infrastructure.UnitOfWork
{
   
    public interface IUnitOfWork
    {
        public IRoleRepository RoleRepository { get; }
        public IUserRepository UserRepository { get; }
        public IEmployeeRepository EmployeeRepository { get; }
        public ICategoryRepository CategoryRepository { get; }
        public IProvinceRepository ProvinceRepository { get; }
        public IDistrictRepository DistrictRepository { get; }
        public IWardRepository WardRepository { get; }
        public IBrandRepository BrandRepository { get; }
        public IProductRepository ProductRepository { get; }
        public IProductVariantRepository ProductVariantRepository { get; }
        public ICartRepository CartRepository { get; }
        public IFeedbackRepository FeedbackRepository { get; }
        public IDiscountRepository DiscountRepository { get; }
        public IOrderRepository OrderRepository { get; }
        public IOrderDetailRepository OrderDetailRepository { get; }
        public IImageRepository ImageRepository { get; }
        public ISubCategoryRepository SubCategoryRepository { get; }

        public Task<int> SaveChangeAsync();

        // dispose
        public void Dispose()
        {
            
        }
    }
}
