using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TranslationApi.Application.Interfaces;
using TranslationApi.Domain.Entities;
using TranslationApi.Domain.Enums;

namespace TranslationApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;
        private readonly IChatMessageService _messageService;
        private readonly IChatSessionService _sessionService;

        public FeedbackController(
            IFeedbackService feedbackService,
            IChatMessageService messageService,
            IChatSessionService sessionService)
        {
            _feedbackService = feedbackService;
            _messageService = messageService;
            _sessionService = sessionService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Feedback>>> GetAllFeedbacks()
        {
            // Lấy các phản hồi cả Like và Dislike
            var likeFeedbacks = await _feedbackService.GetFeedbacksByRatingAsync(FeedbackRating.Like);
            var dislikeFeedbacks = await _feedbackService.GetFeedbacksByRatingAsync(FeedbackRating.Dislike);
            
            var allFeedbacks = new List<Feedback>();
            allFeedbacks.AddRange(likeFeedbacks);
            allFeedbacks.AddRange(dislikeFeedbacks);
            
            return Ok(allFeedbacks);
        }

        [HttpGet("message/{messageId}")]
        public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedbacksByMessage(Guid messageId)
        {
            var message = await _messageService.GetMessageByIdAsync(messageId);
            if (message == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var session = await _sessionService.GetSessionByIdAsync(message.SessionId);

            if (session.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            var feedbacks = await _feedbackService.GetFeedbacksByMessageIdAsync(messageId);
            return Ok(feedbacks);
        }

        [HttpGet("session/{sessionId}")]
        public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedbacksBySession(Guid sessionId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isSessionOwner = await _sessionService.IsUserSessionOwnerAsync(sessionId, userId);

            if (!isSessionOwner && !User.IsInRole("Admin"))
                return Forbid();

            var feedbacks = await _feedbackService.GetFeedbacksBySessionIdAsync(sessionId);
            return Ok(feedbacks);
        }

        [HttpGet("session/{sessionId}/average")]
        public async Task<ActionResult<double>> GetAverageRating(Guid sessionId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isSessionOwner = await _sessionService.IsUserSessionOwnerAsync(sessionId, userId);

            if (!isSessionOwner && !User.IsInRole("Admin"))
                return Forbid();

            var average = await _feedbackService.GetAverageRatingAsync(sessionId);
            return Ok(average);
        }

        [HttpGet("session/{sessionId}/distribution")]
        public async Task<ActionResult<Dictionary<FeedbackRating, int>>> GetRatingDistribution(Guid sessionId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isSessionOwner = await _sessionService.IsUserSessionOwnerAsync(sessionId, userId);

            if (!isSessionOwner && !User.IsInRole("Admin"))
                return Forbid();

            var distribution = await _feedbackService.GetRatingDistributionAsync(sessionId);
            return Ok(distribution);
        }

        [HttpPost]
        public async Task<ActionResult<Feedback>> CreateFeedback([FromBody] CreateFeedbackRequest request)
        {
            var message = await _messageService.GetMessageByIdAsync(request.MessageId);
            if (message == null)
                return NotFound("Tin nhắn không tồn tại");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var session = await _sessionService.GetSessionByIdAsync(message.SessionId);

            if (session.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            try
            {
                var feedback = await _feedbackService.AddFeedbackAsync(
                    request.MessageId,
                    request.Rating,
                    request.Comment);

                return CreatedAtAction(nameof(GetFeedback), new { id = feedback.Id }, feedback);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Feedback>> GetFeedback(Guid id)
        {
            var feedback = await _feedbackService.GetFeedbackByIdAsync(id);
            
            if (feedback == null)
                return NotFound();
                
            var message = await _messageService.GetMessageByIdAsync(feedback.MessageId);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var session = await _sessionService.GetSessionByIdAsync(message.SessionId);
            
            if (session.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();
                
            return Ok(feedback);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFeedback(Guid id, [FromBody] UpdateFeedbackRequest request)
        {
            var feedback = await _feedbackService.GetFeedbackByIdAsync(id);
            
            if (feedback == null)
                return NotFound();
                
            var message = await _messageService.GetMessageByIdAsync(feedback.MessageId);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var session = await _sessionService.GetSessionByIdAsync(message.SessionId);
            
            if (session.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();
                
            await _feedbackService.UpdateFeedbackAsync(id, request.Rating, request.Comment);
            
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeedback(Guid id)
        {
            var feedback = await _feedbackService.GetFeedbackByIdAsync(id);
            
            if (feedback == null)
                return NotFound();
                
            var message = await _messageService.GetMessageByIdAsync(feedback.MessageId);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var session = await _sessionService.GetSessionByIdAsync(message.SessionId);
            
            if (session.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();
                
            await _feedbackService.DeleteFeedbackAsync(id);
            
            return NoContent();
        }
    }

    public class CreateFeedbackRequest
    {
        public Guid MessageId { get; set; }
        public FeedbackRating Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }

    public class UpdateFeedbackRequest
    {
        public FeedbackRating Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
} 