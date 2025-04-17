using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TranslationApi.Application.Common;
using TranslationApi.Application.Interfaces;

namespace TranslationApi.Application.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<LocalFileStorageService> _logger;
        private readonly string[] _allowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
        private const int MaxFileSizeInMB = 5;
        private const long MaxDirectorySizeInMB = 100; // 100MB per user
        private const string UploadDirectory = "uploads";
        private const string AvatarDirectory = "avatars";

        public LocalFileStorageService(
            IWebHostEnvironment environment,
            ILogger<LocalFileStorageService> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string userId, string fileType)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    throw new ArgumentException("File không hợp lệ");
                }

                if (!await IsValidImageAsync(file))
                {
                    throw new ArgumentException("File không đúng định dạng hoặc kích thước quá lớn");
                }

                var uploadPath = Path.Combine(_environment.WebRootPath, UploadDirectory, fileType, userId);
                Directory.CreateDirectory(uploadPath);

                // Kiểm tra kích thước thư mục
                if (await GetDirectorySizeInMB(uploadPath) > MaxDirectorySizeInMB)
                {
                    throw new InvalidOperationException($"Thư mục đã vượt quá giới hạn {MaxDirectorySizeInMB}MB");
                }

                // Tạo tên file unique với userId
                var fileName = FileNameHelper.GenerateUniqueFileName(file.FileName, userId);
                var filePath = Path.Combine(uploadPath, fileName);

                // Xóa file cũ nếu là avatar
                if (fileType == AvatarDirectory)
                {
                    await CleanupOldAvatarsAsync(uploadPath);
                }

                // Lưu file mới
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _logger.LogInformation(
                    "File uploaded successfully. User: {UserId}, Type: {FileType}, Name: {FileName}", 
                    userId, fileType, fileName);

                return "/" + Path.Combine(UploadDirectory, fileType, userId, fileName).Replace("\\", "/");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, 
                    "Error uploading file. User: {UserId}, Type: {FileType}, OriginalName: {OriginalName}", 
                    userId, fileType, file?.FileName);
                throw;
            }
        }

        public async Task DeleteFileAsync(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
            {
                return;
            }

            try
            {
                var filePath = Path.Combine(_environment.WebRootPath, fileUrl.TrimStart('/'));
                if (File.Exists(filePath))
                {
                    await Task.Run(() => File.Delete(filePath));
                    _logger.LogInformation("File deleted successfully: {FileUrl}", fileUrl);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file: {FileUrl}", fileUrl);
                throw;
            }
        }

        public async Task<bool> IsValidImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return false;
            }

            if (file.Length > MaxFileSizeInMB * 1024 * 1024)
            {
                return false;
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !_allowedImageExtensions.Contains(extension))
            {
                return false;
            }

            try
            {
                using var stream = file.OpenReadStream();
                var buffer = new byte[4];
                await stream.ReadAsync(buffer.AsMemory(0, 4));
                stream.Position = 0;

                // JPEG: FF D8 FF
                if (buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF)
                    return true;
                
                // PNG: 89 50 4E 47
                if (buffer[0] == 0x89 && buffer[1] == 0x50 && 
                    buffer[2] == 0x4E && buffer[3] == 0x47)
                    return true;
                
                // GIF: "GIF8"
                if (buffer[0] == 0x47 && buffer[1] == 0x49 && 
                    buffer[2] == 0x46 && buffer[3] == 0x38)
                    return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating image file: {FileName}", file.FileName);
                return false;
            }

            return false;
        }

        private async Task CleanupOldAvatarsAsync(string directory)
        {
            if (Directory.Exists(directory))
            {
                try
                {
                    var files = Directory.GetFiles(directory);
                    foreach (var file in files)
                    {
                        await DeleteFileAsync(file);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error cleaning up old avatars in directory: {Directory}", directory);
                    throw;
                }
            }
        }

        private async Task<double> GetDirectorySizeInMB(string path)
        {
            try
            {
                var bytes = await Task.Run(() => 
                    Directory.GetFiles(path, "*", SearchOption.AllDirectories)
                        .Sum(file => new FileInfo(file).Length));
                
                return bytes / (1024.0 * 1024.0); // Convert to MB
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating directory size: {Path}", path);
                return 0;
            }
        }
    }
}