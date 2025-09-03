using SP.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Application.Service.Interface
{
    public interface IFeedBackService
    {
        Task<IEnumerable<FeedBack>> GetAllFeedbacks();
        Task<FeedBack> GetFeedbackById(int id);
        Task CreateFeedback(FeedBack feedback);
        Task UpdateFeedback(FeedBack feedback);
        Task DeleteFeedback(int id);

    }
}
