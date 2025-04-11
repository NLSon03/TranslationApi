
using System.Text.RegularExpressions;

namespace TranslationApi.Application.Common
{
    public static class FileNameHelper
    {
        public static string GenerateUniqueFileName(string originalFileName, string userId)
        {
            // Lấy extension từ tên file gốc
            var extension = Path.GetExtension(originalFileName);
            
            // Tạo tên file mới với format: userId_timestamp_sanitizedOriginalName
            var timestamp = DateTime.UtcNow.Ticks;
            var sanitizedName = SanitizeFileName(Path.GetFileNameWithoutExtension(originalFileName));
            
            return $"{userId}_{timestamp}_{sanitizedName}{extension}";
        }

        public static string SanitizeFileName(string fileName)
        {
            // Loại bỏ các ký tự không hợp lệ
            var invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            var invalidRegEx = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);
            
            // Thay thế các ký tự không hợp lệ bằng dấu gạch ngang
            fileName = Regex.Replace(fileName, invalidRegEx, "-");
            
            // Chuyển thành lowercase và giới hạn độ dài
            fileName = fileName.ToLowerInvariant();
            if (fileName.Length > 50)
            {
                fileName = fileName.Substring(0, 50);
            }
            
            return fileName;
        }
    }
}