using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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
            var user = await _userManager.FindByEmailAsync(User.FindFirst(ClaimTypes.Email)?.Value);

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

            return Ok(new { success = true, message = "Cập nhật thông tin người dùng thành công" });
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

            return Ok(new { success = true, message = "Đặt lại mật khẩu thành công" });
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

            var action = lockoutDto.IsLocked ? "khóa" : "mở khóa";
            return Ok(new { success = true, message = $"Đã {action} tài khoản thành công" });
        }

        [HttpGet("google-login")]
        public IActionResult GoogleLogin([FromQuery] string returnUrl = null)
        {
            // Lưu lại URL chuyển hướng để sử dụng sau khi đăng nhập thành công
            var clientReturnUrl = returnUrl ?? Url.Content("~/");

            // Tạo URL callback cho Google
            var redirectUrl = Url.ActionLink("GoogleResponse", "Auth", new { redirect = clientReturnUrl });
            Console.WriteLine($"Redirect URL: {redirectUrl}");

            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return Challenge(properties, "Google");
        }

        [HttpGet("google-response")]
        public async Task<ActionResult<AuthResponseDto>> GoogleResponse([FromQuery] string redirect = null)
        {
            try
            {
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    Console.WriteLine("Error: Không thể lấy thông tin đăng nhập từ Google");

                    if (!string.IsNullOrEmpty(redirect))
                    {
                        return Redirect($"{redirect}?error=Không+thể+đăng+nhập+bằng+Google.+Thông+tin+đăng+nhập+không+hợp+lệ.");
                    }

                    return BadRequest(new { error = "Không thể đăng nhập bằng Google. Thông tin đăng nhập không hợp lệ." });
                }

                // Kiểm tra email
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                if (string.IsNullOrEmpty(email))
                {
                    Console.WriteLine("Error: Không tìm thấy email trong thông tin từ Google");

                    if (!string.IsNullOrEmpty(redirect))
                    {
                        return Redirect($"{redirect}?error=Không+thể+đăng+nhập+bằng+Google.+Email+không+được+cung+cấp.");
                    }

                    return BadRequest(new { error = "Không thể đăng nhập bằng Google. Email không được cung cấp." });
                }

                var user = await _userManager.FindByEmailAsync(email);

                // Nếu user chưa tồn tại, tạo mới
                if (user == null)
                {
                    var userName = info.Principal.FindFirstValue(ClaimTypes.Name) ?? email;

                    user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true,
                        ChatSessions = new List<ChatSession>()
                    };

                    var result = await _userManager.CreateAsync(user);
                    if (!result.Succeeded)
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        Console.WriteLine($"Error creating user: {errors}");

                        if (!string.IsNullOrEmpty(redirect))
                        {
                            return Redirect($"{redirect}?error=Không+thể+tạo+tài+khoản:+{Uri.EscapeDataString(errors)}");
                        }

                        return BadRequest(new { error = $"Không thể tạo tài khoản: {errors}" });
                    }

                    // Gán role User
                    if (!await _roleManager.RoleExistsAsync("User"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("User"));
                    }
                    await _userManager.AddToRoleAsync(user, "User");
                }

                // Tạo token JWT
                var userRoles = await _userManager.GetRolesAsync(user);
                var token = _tokenService.CreateToken(user, userRoles.ToList());

                // Nếu có URL chuyển hướng, thêm token vào URL và chuyển hướng
                if (!string.IsNullOrEmpty(redirect))
                {
                    return Redirect($"{redirect}?token={token}");
                }

                // Nếu không có URL chuyển hướng, trả về response bình thường
                return new AuthResponseDto
                {
                    UserId = user.Id ?? "",
                    UserName = user.UserName ?? "",
                    Email = user.Email ?? "",
                    Token = token,
                    Expiration = _tokenService.GetExpirationDate(),
                    Roles = userRoles.ToList()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong quá trình xử lý đăng nhập Google: {ex.Message}");

                if (!string.IsNullOrEmpty(redirect))
                {
                    return Redirect($"{redirect}?error={Uri.EscapeDataString(ex.Message)}");
                }

                return StatusCode(500, new { error = "Lỗi server khi xử lý đăng nhập Google" });
            }
        }

        // Phương thức để xử lý callback từ Google thông qua signin-google
        [Route("/signin-google")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public Task<IActionResult> GoogleCallback()
        {
            try
            {
                Console.WriteLine("Xử lý callback Google tại /signin-google");

                // Chuyển hướng đến api/Auth/google-response sau khi xác thực
                return Task.FromResult<IActionResult>(Redirect("/api/Auth/google-response"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xử lý Google callback: {ex.Message}");
                return Task.FromResult<IActionResult>(BadRequest(new { error = "Lỗi khi xử lý Google callback" }));
            }
        }
    }
}