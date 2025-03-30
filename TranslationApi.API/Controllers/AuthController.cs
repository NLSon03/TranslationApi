using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TranslationApi.Application.DTOs;
using TranslationApi.Application.Interfaces;
using TranslationApi.Domain.Entities;

namespace TranslationApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
        {
            // Kiểm tra xem email đã được đăng ký chưa
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return BadRequest(new { error = "Email đã tồn tại trong hệ thống" });
            }

            // Tạo user mới
            var user = new ApplicationUser
            {
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                ChatSessions = new List<ChatSession>()
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
                // Kiểm tra nếu đây là user đầu tiên, gán quyền Admin
                if (_userManager.Users.Count() == 1)
                {
                    // Đảm bảo role Admin tồn tại
                    if (!await _roleManager.RoleExistsAsync("Admin"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    }

                    await _userManager.AddToRoleAsync(user, "Admin");
                }
                else
                {
                    // Đảm bảo role User tồn tại
                    if (!await _roleManager.RoleExistsAsync("User"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("User"));
                    }

                    await _userManager.AddToRoleAsync(user, "User");
                }

                // Lấy danh sách role
                var userRoles = await _userManager.GetRolesAsync(user);

                return new AuthResponseDto
                {
                    UserId = user.Id,
                    UserName = user.UserName ?? "",
                    Email = user.Email ?? "",
                    Token = _tokenService.CreateToken(user, userRoles.ToList()),
                    Expiration = _tokenService.GetExpirationDate(),
                    Roles = userRoles.ToList()
                };
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
            {
                return Unauthorized(new { error = "Email hoặc mật khẩu không đúng" });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (result.Succeeded)
            {
                // Lấy danh sách role của user
                var userRoles = await _userManager.GetRolesAsync(user);

                return new AuthResponseDto
                {
                    UserId = user.Id ?? "",
                    UserName = user.UserName ?? "",
                    Email = user.Email ?? "",
                    Token = _tokenService.CreateToken(user, userRoles.ToList()),
                    Expiration = _tokenService.GetExpirationDate(),
                    Roles = userRoles.ToList()
                };
            }

            return Unauthorized(new { error = "Email hoặc mật khẩu không đúng" });
        }

        [Authorize]
        [HttpGet("current-user")]
        public async Task<ActionResult<AuthResponseDto>> GetCurrentUser()
        {
            var user = await _userManager.FindByEmailAsync(User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value);

            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            return new AuthResponseDto
            {
                UserId = user.Id ?? "",
                UserName = user.UserName ?? "",
                Email = user.Email ?? "",
                Token = _tokenService.CreateToken(user, userRoles.ToList()),
                Expiration = _tokenService.GetExpirationDate(),
                Roles = userRoles.ToList()
            };
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UserListDto>>> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = new List<UserListDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(new UserListDto
                {
                    Id = user.Id,
                    UserName = user.UserName ?? "",
                    Email = user.Email ?? "",
                    EmailConfirmed = user.EmailConfirmed,
                    LockoutEnabled = user.LockoutEnabled,
                    LockoutEnd = user.LockoutEnd,
                    Roles = roles.ToList()
                });
            }

            return Ok(userDtos);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("users/{id}")]
        public async Task<ActionResult<UserListDto>> GetUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            return new UserListDto
            {
                Id = user.Id,
                UserName = user.UserName ?? "",
                Email = user.Email ?? "",
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEnd = user.LockoutEnd,
                Roles = roles.ToList()
            };
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(string id, UpdateUserDto updateDto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.UserName = updateDto.UserName;
            user.Email = updateDto.Email;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // Cập nhật roles
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRolesAsync(user, updateDto.Roles);

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("users/{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(string id, ChangePasswordDto passwordDto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, passwordDto.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("users/{id}/toggle-lockout")]
        public async Task<IActionResult> ToggleLockout(string id, ToggleLockoutDto lockoutDto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (lockoutDto.IsLocked)
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            }
            else
            {
                await _userManager.SetLockoutEndDateAsync(user, null);
            }

            return NoContent();
        }
    }
}