using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TranslationApi.API.DTOs;
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
                    UserName = user.UserName,
                    Email = user.Email,
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
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
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
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Token = _tokenService.CreateToken(user, userRoles.ToList()),
                Expiration = _tokenService.GetExpirationDate(),
                Roles = userRoles.ToList()
            };
        }
    }
} 