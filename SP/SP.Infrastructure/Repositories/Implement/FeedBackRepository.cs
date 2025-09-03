using Microsoft.EntityFrameworkCore;
using SP.Domain.Entity;
using SP.Infrastructure.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Infrastructure.Repositories.Implement
{
    public class FeedBackRepository : GenericRepository<FeedBack>,IFeedbackRepository
    {
        public FeedBackRepository(Context.SPContext context) : base(context)
        {
        }
        public override async Task<IEnumerable<FeedBack>> GetAllAsync()
        {
            return await _SPContext.Set<FeedBack>()
                         .Include(fb => fb.User)
                         .Include(fb => fb.OrderDetail)
                         .ThenInclude(od => od.ProductVariant)
                         .ThenInclude(pv => pv.Product)
                         .ToListAsync();
        }
        public override async Task<FeedBack> GetByIdAsync(int id)
        {
            return await _SPContext.Set<FeedBack>()
                         .Include(fb => fb.User)
                         .Include(fb => fb.OrderDetail)
                         .ThenInclude(od => od.ProductVariant)
                         .ThenInclude(pv => pv.Product)
                         .FirstOrDefaultAsync(fb => fb.Id == id);
        }
        public override async Task AddAsync(FeedBack entity)
        {
            // Thêm feedback mới
            await base.AddAsync(entity);

            // Lấy tất cả rating của variant này
            var feedbacks = await _SPContext.Feedbacks
                .Where(fb => fb.ProductVariantId == entity.ProductVariantId)
                .ToListAsync();

            double avgRating = 0;
            if (feedbacks.Any())
            {
                avgRating = feedbacks.Average(fb => fb.Rating);
            }

            // Tìm ProductVariant cần cập nhật
            var variant = await _SPContext.ProductVariants.FindAsync(entity.ProductVariantId);
            if (variant != null)
            {
                variant.Rating = avgRating;
                _SPContext.Update(variant);
                await _SPContext.SaveChangesAsync();
            }
        }


    }

}
