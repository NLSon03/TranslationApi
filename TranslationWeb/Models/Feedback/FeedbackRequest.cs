using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TranslationWeb.Models.Feedback
{
    public class FeedbackRequest
    {
        [Required(ErrorMessage = "Nội dung phản hồi là bắt buộc")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Nội dung phản hồi phải từ 10-500 ký tự")]
        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        [JsonPropertyName("rating")]
        public int Rating { get; set; } = 0; // 0 = dislike, 1 = like

        [JsonPropertyName("userId")]
        public string? UserId { get; set; }

        [JsonPropertyName("translationId")]
        public int? TranslationId { get; set; }
    }
} 