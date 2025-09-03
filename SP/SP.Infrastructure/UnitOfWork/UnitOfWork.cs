using SP.Infrastructure.Context;
using SP.Infrastructure.Repositories.Implement;
using SP.Infrastructure.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace SP.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SPContext _SPContext;
        public IRoleRepository RoleRepository { get; }

        public ICategoryRepository CategoryRepository { get; }
        public IProvinceRepository ProvinceRepository { get; }

        public IDistrictRepository DistrictRepository { get; }

        public IWardRepository WardRepository { get; }

        public IBrandRepository BrandRepository { get; }

        public IUserRepository UserRepository { get; }

        public IEmployeeRepository EmployeeRepository { get; }

        public IProductRepository ProductRepository { get; }

        public IProductVariantRepository ProductVariantRepository { get; }

        public ICartRepository CartRepository { get; }

        public IFeedbackRepository FeedbackRepository { get; }

        public IDiscountRepository DiscountRepository { get; }

        public IOrderRepository OrderRepository { get; }

        public IOrderDetailRepository OrderDetailRepository { get; }

        public IImageRepository ImageRepository { get; }
        public ISubCategoryRepository SubCategoryRepository { get; }

        public UnitOfWork(SPContext sPContext)
        {
            _SPContext = sPContext;
            RoleRepository = new RoleRepository(_SPContext);
            CategoryRepository = new CategoryRepository(_SPContext);
            ProvinceRepository = new ProvinceRepository(_SPContext);
            DistrictRepository = new DistrictRepository(_SPContext);
            WardRepository = new WardRepository(_SPContext);
            BrandRepository = new BrandRepository(_SPContext);
            UserRepository = new UserRepository(_SPContext);
            EmployeeRepository = new EmployeeRepository(_SPContext);
            ProductRepository = new ProductRepository(_SPContext);
            ProductVariantRepository = new ProductVariantRepository(_SPContext);
            CartRepository = new CartRepository(_SPContext);
            FeedbackRepository = new FeedBackRepository(_SPContext);
            DiscountRepository = new DiscountRepository(_SPContext);
            OrderRepository = new OrderRepository(_SPContext);
            OrderDetailRepository = new OrderDetailRepository(_SPContext);
            ImageRepository = new ImageRepository(_SPContext);         
            SubCategoryRepository = new SubCategoryRepository(_SPContext);
        }

        public Task<int> SaveChangeAsync()
        {
           return _SPContext.SaveChangesAsync();
        }
        public void Dispose()
        {
            _SPContext.Dispose();
        }
    }
}
