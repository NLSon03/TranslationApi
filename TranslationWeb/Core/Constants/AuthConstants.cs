namespace TranslationWeb.Core.Constants
{
    public static class AuthConstants
    {
        public static class Storage
        {
            public const string UserSession = "user_session";
            public const string ReturnUrl = "return_url";
            public const string LastActivity = "last_activity";
        }

        public static class TokenSettings
        {
            public const int AccessTokenExpirationMinutes = 60; // 1 giờ
            public const int RefreshTokenExpirationDays = 7; // 7 ngày
            public const int TokenRefreshThresholdMinutes = 5; // Refresh trước 5 phút khi hết hạn
        }

        public static class SessionSettings
        {
            public const int TimeoutMinutes = 30; // Session timeout sau 30 phút không hoạt động
            public const int ActivityCheckIntervalSeconds = 60; // Kiểm tra hoạt động mỗi 60 giây
        }

        public static class ValidationRules
        {
            public const int MinPasswordLength = 8;
            public const int MaxPasswordLength = 100;
            public const int MinUsernameLength = 3;
            public const int MaxUsernameLength = 50;
            public const string PasswordRequirements = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
            public const string UsernameAllowedCharacters = @"^[a-zA-Z0-9_-]+$";
        }

        public static class ErrorMessages
        {
            public const string InvalidCredentials = "Tên đăng nhập hoặc mật khẩu không đúng";
            public const string AccountLocked = "Tài khoản đã bị khóa";
            public const string SessionExpired = "Phiên làm việc đã hết hạn";
            public const string TokenExpired = "Token đã hết hạn";
            public const string InvalidToken = "Token không hợp lệ";
            public const string UserNotFound = "Không tìm thấy người dùng";
            public const string EmailAlreadyExists = "Email đã được sử dụng";
            public const string UsernameAlreadyExists = "Tên đăng nhập đã được sử dụng";
            public const string InvalidPassword = "Mật khẩu phải có ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt";
            public const string InvalidUsername = "Tên đăng nhập chỉ được chứa chữ cái, số và các ký tự - _";
            public const string ServerError = "Có lỗi xảy ra, vui lòng thử lại sau";
        }

        public static class ValidationMessages
        {
            public const string RequiredUsername = "Vui lòng nhập tên đăng nhập";
            public const string RequiredPassword = "Vui lòng nhập mật khẩu";
            public const string RequiredEmail = "Vui lòng nhập email";
            public const string InvalidEmail = "Email không hợp lệ";
            public const string PasswordMismatch = "Mật khẩu xác nhận không khớp";
            public const string InvalidPasswordLength = "Mật khẩu phải có độ dài từ 8 đến 100 ký tự";
            public const string InvalidUsernameLength = "Tên đăng nhập phải có độ dài từ 3 đến 50 ký tự";
        }

        public static class ClaimTypes
        {
            public const string UserId = "uid";
            public const string Username = "username";
            public const string Email = "email";
            public const string Role = "role";
            public const string AccessToken = "access_token";
            public const string RefreshToken = "refresh_token";
            public const string TokenExpiration = "token_expiration";
            public const string RefreshTokenExpiration = "refresh_token_expiration";
        }

        public static class Roles
        {
            public const string Admin = "Admin";
            public const string User = "User";
        }
    }
}