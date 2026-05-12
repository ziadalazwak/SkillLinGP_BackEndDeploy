namespace SkillLink.API.ApiDto
{
    public class CreateReviewRequestDto
    {
        public int RevieweeId { get; set; }
        public int? SessionId { get; set; }
        public int? SkillTradeId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
