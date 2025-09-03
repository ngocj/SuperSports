using SP.Application.Service.Interface;
using SP.Domain.Entity;
using SP.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Application.Service.Implement
{
    public class FeedBackService : IFeedBackService
    {
        private readonly IUnitOfWork _unitOfWork;
        public FeedBackService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task CreateFeedback(FeedBack feedback)
        {
            await _unitOfWork.FeedbackRepository.AddAsync(feedback);
            await _unitOfWork.SaveChangeAsync();
            
        }

        public async Task DeleteFeedback(int id)
        {
            var result = await _unitOfWork.FeedbackRepository.GetByIdAsync(id);
            if (result != null)
            {
                await _unitOfWork.FeedbackRepository.DeleteAsync(result);
                await _unitOfWork.SaveChangeAsync();
            }
            
        }

        public async Task<IEnumerable<FeedBack>> GetAllFeedbacks()
        {
            return await _unitOfWork.FeedbackRepository.GetAllAsync();
            
        }

        public async Task<FeedBack> GetFeedbackById(int id)
        {
            return  await _unitOfWork.FeedbackRepository.GetByIdAsync(id);
            
        }

        public async Task UpdateFeedback(FeedBack feedback)
        {
            var result = await _unitOfWork.FeedbackRepository.GetByIdAsync(feedback.Id);
            if (result != null)
            {
                await _unitOfWork.FeedbackRepository.UpdateAsync(feedback);
                await _unitOfWork.SaveChangeAsync();
            }
            
        }
    }
}
