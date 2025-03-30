using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TranslationApi.Application.Contracts;
using TranslationApi.Application.DTOs;
using TranslationApi.Application.Interfaces;
using TranslationApi.Domain.Enums;

namespace TranslationApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatMessageController : ControllerBase
    {
        private readonly IChatMessageService _messageService;
        private readonly IChatSessionService _sessionService;
        private readonly ITranslationService _translationService;

        public ChatMessageController(
            IChatMessageService messageService,
            IChatSessionService sessionService,
            ITranslationService translationService)
        {
            _messageService = messageService;
            _sessionService = sessionService;
            _translationService = translationService;
        }

        [HttpGet("session/{sessionId}")]
        public async Task<ActionResult<IEnumerable<ChatMessageDto>>> GetMessagesBySession(Guid sessionId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isSessionOwner = await _sessionService.IsUserSessionOwnerAsync(sessionId, userId);

            if (!isSessionOwner && !User.IsInRole("Admin"))
                return Forbid();

            var messages = await _messageService.GetMessagesBySessionIdAsync(sessionId);
            return Ok(messages);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ChatMessageDto>> GetMessageById(Guid id)
        {
            var message = await _messageService.GetMessageByIdAsync(id);

            if (message == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var session = await _sessionService.GetSessionByIdAsync(message.SessionId);

            if (session.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            return Ok(message);
        }

        [HttpPost]
        public async Task<ActionResult<SendMessageResponse>> SendMessage([FromBody] SendMessageRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isSessionOwner = await _sessionService.IsUserSessionOwnerAsync(request.SessionId, userId);

            if (!isSessionOwner)
                return Forbid();

            // Lưu tin nhắn của người dùng
            var userMessage = await _messageService.AddMessageAsync(
                request.SessionId,
                request.Content,
                SenderType.User,
                request.MessageType);

            // Gửi tin nhắn đến dịch vụ dịch
            var startTime = DateTime.UtcNow;
            var translationRequest = new TranslationRequest
            {
                SourceText = request.Content,
                SourceLanguage = request.FromLanguage,
                TargetLanguage = request.ToLanguage
            };

            var translationResponse = await _translationService.TranslateTextAsync(translationRequest);
            var responseTime = (long)(DateTime.UtcNow - startTime).TotalMilliseconds;

            // Lưu phản hồi
            var aiResponse = await _messageService.AddMessageAsync(
                request.SessionId,
                translationResponse.TranslatedText,
                SenderType.AI,
                request.MessageType,
                responseTime);

            return Ok(new SendMessageResponse
            {
                UserMessage = userMessage,
                AIResponse = aiResponse,
                TranslationTime = responseTime
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMessage(Guid id, [FromBody] UpdateMessageRequest request)
        {
            var message = await _messageService.GetMessageByIdAsync(id);

            if (message == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var session = await _sessionService.GetSessionByIdAsync(message.SessionId);

            if (session.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            await _messageService.UpdateMessageContentAsync(id, request.Content);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(Guid id)
        {
            var message = await _messageService.GetMessageByIdAsync(id);

            if (message == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var session = await _sessionService.GetSessionByIdAsync(message.SessionId);

            if (session.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            await _messageService.DeleteMessageAsync(id);

            return NoContent();
        }
    }

    public class SendMessageRequest
    {
        public Guid SessionId { get; set; }
        public string Content { get; set; }
        public MessageType MessageType { get; set; } = MessageType.Text;
        public string FromLanguage { get; set; } = "auto";
        public string ToLanguage { get; set; } = "vi";
    }

    public class SendMessageResponse
    {
        public ChatMessageDto UserMessage { get; set; }
        public ChatMessageDto AIResponse { get; set; }
        public long TranslationTime { get; set; }
    }

    public class UpdateMessageRequest
    {
        public string Content { get; set; }
    }
}