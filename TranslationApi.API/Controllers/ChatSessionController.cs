using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TranslationApi.Application.DTOs;
using TranslationApi.Application.Interfaces;

namespace TranslationApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatSessionController : ControllerBase
    {
        private readonly IChatSessionService _sessionService;

        public ChatSessionController(IChatSessionService sessionService)
        {
            _sessionService = sessionService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ChatSessionDto>>> GetAllSessions()
        {
            var sessions = await _sessionService.GetAllSessionsAsync();
            return Ok(sessions);
        }

        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<ChatSessionDto>>> GetCurrentUserSessions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sessions = await _sessionService.GetSessionsByUserIdAsync(userId);
            return Ok(sessions);
        }

        [HttpGet("active")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ChatSessionDto>>> GetActiveSessions()
        {
            var sessions = await _sessionService.GetActiveSessionsAsync();
            return Ok(sessions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ChatSessionDto>> GetSessionById(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var session = await _sessionService.GetSessionByIdAsync(id);
            
            if (session == null)
                return NotFound();
                
            if (!await _sessionService.IsUserSessionOwnerAsync(id, userId) && !User.IsInRole("Admin"))
                return Forbid();
                
            return Ok(session);
        }

        [HttpGet("{id}/messages")]
        public async Task<ActionResult<ChatSessionDetailDto>> GetSessionWithMessages(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var session = await _sessionService.GetSessionWithMessagesAsync(id);
            
            if (session == null)
                return NotFound();
                
            if (!await _sessionService.IsUserSessionOwnerAsync(id, userId) && !User.IsInRole("Admin"))
                return Forbid();
                
            return Ok(session);
        }

        [HttpPost]
        public async Task<ActionResult<ChatSessionDto>> CreateSession(Guid modelId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            try
            {
                var session = await _sessionService.CreateSessionAsync(userId, modelId);
                return CreatedAtAction(nameof(GetSessionById), new { id = session.Id }, session);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}/end")]
        public async Task<IActionResult> EndSession(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var session = await _sessionService.GetSessionByIdAsync(id);
            
            if (session == null)
                return NotFound();
                
            if (session.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();
                
            await _sessionService.EndSessionAsync(id);
            
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSession(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var session = await _sessionService.GetSessionByIdAsync(id);
            
            if (session == null)
                return NotFound();
                
            if (session.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();
                
            await _sessionService.DeleteSessionAsync(id);
            
            return NoContent();
        }
    }
} 