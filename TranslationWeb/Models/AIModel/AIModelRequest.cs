using System.ComponentModel.DataAnnotations;

namespace TranslationWeb.Models.AIModel
{
    public class AIModelRequest
    {
        [Required(ErrorMessage = "Tên là bắt buộc")]
        [StringLength(50, ErrorMessage = "Tên không được vượt quá 50 ký tự")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phiên bản là bắt buộc")]
        [StringLength(20, ErrorMessage = "Phiên bản không được vượt quá 20 ký tự")]
        public string Version { get; set; } = string.Empty;

        [Required(ErrorMessage = "API Endpoint là bắt buộc")]
        public string ApiEndPoint { get; set; } = string.Empty;

        [Required(ErrorMessage = "API Key là bắt buộc")]
        public string ApiKey { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    public class UpdateAIModelRequest : AIModelRequest
    {
        [Required(ErrorMessage = "ID bắt buộc")]
        public Guid Id { get; set; }

        public class ActivateModelRequest
        {
            public Guid Id { get; set; }
        }
    }
}