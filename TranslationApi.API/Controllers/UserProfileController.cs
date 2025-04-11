
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TranslationApi.Application.DTOs;
using TranslationApi.Application.Interfaces;

namespace TranslationApi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;

        public UserProfileController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<UserListDto>> GetUserProfile()
        {
            var userId = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var profile = await _userProfileService.GetUserProfileAsync(userId);
                return Ok(profile);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Không tìm thấy thông tin người dùng.");
            }
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserDto updateUserDto)
        {
            var userId = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _userProfileService.UpdateUserProfileAsync(userId, updateUserDto);
            if (result)
            {
                return Ok(new { message = "Cập nhật thông tin thành công." });
            }

            return BadRequest("Không thể cập nhật thông tin người dùng.");
        }

        [HttpPut("preferences")]
        public async Task<IActionResult> UpdatePreferences([FromBody] UpdateUserPreferencesDto preferencesDto)
        {
            var userId = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _userProfileService.UpdateUserPreferencesAsync(userId, preferencesDto);
            if (result)
            {
                return Ok(new { message = "Cập nhật tùy chọn thành công." });
            }

            return BadRequest("Không thể cập nhật tùy chọn người dùng.");
        }

        [HttpGet("languages/frequent")]
        public async Task<ActionResult<List<string>>> GetFrequentLanguages()
        {
            var userId = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var languages = await _userProfileService.GetFrequentlyUsedLanguagesAsync(userId);
            return Ok(languages);
        }

        [HttpPost("translation/increment")]
        public async Task<IActionResult> IncrementTranslationCount()
        {
            var userId = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _userProfileService.IncrementTranslationCountAsync(userId);
            if (result)
            {
                return Ok(new { message = "Cập nhật số lần dịch thành công." });
            }

            return BadRequest("Không thể cập nhật số lần dịch.");
        }

        [HttpPost("lastactive/update")]
        public async Task<IActionResult> UpdateLastActive()
        {
            var userId = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _userProfileService.UpdateLastActiveAsync(userId);
            if (result)
            {
                return Ok(new { message = "Cập nhật thời gian hoạt động thành công." });
            }

            return BadRequest("Không thể cập nhật thời gian hoạt động.");
        }
    }
}