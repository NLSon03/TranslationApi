namespace TranslationWeb.Models.Feedback
{
    public class FeedbackResponse
    {
        public Guid Id { get; set; }
        public Guid MessageId { get; set; }
        public string Rating { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class FeedbackStatsResponse
    {
        public double AverageRating { get; set; }
        public int LikeCount { get; set; }
        public int DislikeCount { get; set; }
        public int TotalCount => LikeCount + DislikeCount;
    }
}