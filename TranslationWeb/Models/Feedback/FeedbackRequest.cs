using System.ComponentModel.DataAnnotations;

namespace TranslationWeb.Models.Feedback
{
    public class CreateFeedbackRequest
    {
        public Guid MessageId { get; set; }

        [Required(ErrorMessage = "Đánh giá là bắt buộc")]
        public string Rating { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Nhận xét không được vượt quá 500 ký tự")]
        public string Comment { get; set; } = string.Empty;
    }

    public class UpdateFeedbackRequest
    {
        public Guid FeedbackId { get; set; }

        [Required(ErrorMessage = "Đánh giá là bắt buộc")]
        public string Rating { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Nhận xét không được vượt quá 500 ký tự")]
        public string Comment { get; set; } = string.Empty;
    }
}