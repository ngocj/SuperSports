using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SP.Application.Dto.FeedbackDto;
using SP.Application.Service.Interface;
using SP.Domain.Entity;

namespace SP.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IFeedBackService _feedbackService;
        public FeedbackController(IMapper mapper, IFeedBackService feedbackService)
        {
            _mapper = mapper;
            _feedbackService = feedbackService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllFeedbacks()
        {
            var feedbacks = await _feedbackService.GetAllFeedbacks();
            var feedbackDto = _mapper.Map<IEnumerable<FeedbackViewDto>>(feedbacks);
            return Ok(feedbackDto);
        }
        [HttpGet("{id}")]
            public async Task<IActionResult> GetFeedbackById(int id)
        {
            var feedback = await _feedbackService.GetFeedbackById(id);
            if (feedback == null)
            {
                return NotFound();
            }
            var feedbackDto = _mapper.Map<FeedbackViewDto>(feedback);
            return Ok(feedbackDto);
        }
        [HttpPost]
        public async Task<IActionResult> CreateFeedback([FromBody] FeedbackCreateDto feedbackCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var feedback = _mapper.Map<FeedBack>(feedbackCreateDto);
            await _feedbackService.CreateFeedback(feedback);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> UpdateFeedback([FromBody] FeedbackViewDto feedbackViewDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var feedback = await _feedbackService.GetFeedbackById(feedbackViewDto.Id);
            if (feedback == null)
            {
                return NotFound();
            }
            var feedbackToUpdate = _mapper.Map<FeedBack>(feedbackViewDto);
            await _feedbackService.UpdateFeedback(feedbackToUpdate);
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeedback(int id)
        {
            var feedback = await _feedbackService.GetFeedbackById(id);
            if (feedback == null)
            {
                return NotFound();
            }
            await _feedbackService.DeleteFeedback(id);
            return Ok();
        }

    }
}
