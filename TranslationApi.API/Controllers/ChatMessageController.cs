using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TranslationApi.Application.Contracts;
using TranslationApi.Application.Interfaces;
using TranslationApi.Domain.Entities;
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
        public async Task<ActionResult<IEnumerable<ChatMessage>>> GetMessagesBySession(Guid sessionId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isSessionOwner = await _sessionService.IsUserSessionOwnerAsync(sessionId, userId);
            
            if (!isSessionOwner && !User.IsInRole("Admin"))
                return Forbid();
                
            var messages = await _messageService.GetMessagesBySessionIdAsync(sessionId);
            return Ok(messages);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ChatMessage>> GetMessageById(Guid id)
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
        public async Task<ActionResult<ChatMessage>> SendMessage([FromBody] SendMessageRequest request)
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
                
            return Ok(new { userMessage, aiResponse });
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

    public class UpdateMessageRequest
    {
        public string Content { get; set; }
    }
} 