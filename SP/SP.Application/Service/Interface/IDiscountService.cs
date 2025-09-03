using SP.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Application.Service.Interface
{
    public interface IDiscountService
    {
        Task<IEnumerable<Discount>> GetAllDiscounts();
        Task<Discount> GetDiscountById(int id);
        Task CreateDiscount(Discount discount);
        Task UpdateDiscount(Discount discount);
        Task DeleteDiscount(int id);

    }
}
