using System.ComponentModel.DataAnnotations;

namespace TranslationWeb.Models.AIModel
{
    public class CreateAIModelRequest
    {
        [Required(ErrorMessage = "Tên là bắt buộc")]
        [StringLength(50, ErrorMessage = "Tên không được vượt quá 50 ký tự")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phiên bản là bắt buộc")]
        [StringLength(20, ErrorMessage = "Phiên bản không được vượt quá 20 ký tự")]
        public string Version { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cấu hình là bắt buộc")]
        public string Config { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }

    public class UpdateAIModelRequest
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Tên là bắt buộc")]
        [StringLength(50, ErrorMessage = "Tên không được vượt quá 50 ký tự")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phiên bản là bắt buộc")]
        [StringLength(20, ErrorMessage = "Phiên bản không được vượt quá 20 ký tự")]
        public string Version { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cấu hình là bắt buộc")]
        public string Config { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }

    public class ActivateModelRequest
    {
        public Guid Id { get; set; }
    }
}