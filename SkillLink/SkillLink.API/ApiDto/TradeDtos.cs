namespace SkillLink.API.ApiDto
{
    public class CreateTradeDto
    {
        public int ReceiverId { get; set; }
        public int OfferedSkillId { get; set; }
        public int RequestedSkillId { get; set; }
        public string? Message { get; set; }
    }

    // TradeActionDto is no longer needed since userId comes from token,
    // but kept empty for backward compatibility in case clients still send it.
    public class TradeActionDto
    {
    }
}
