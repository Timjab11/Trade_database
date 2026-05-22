public sealed class Trade
{
    public int TradeId { get; init; }
    public decimal AmountToBeEarned { get; init; }
    public int? Probability { get; init; }
    public string? ValidFrom { get; init; }
    public string? ValidTill { get; init; }
    public string? ScheduledEnd { get; init; }
    public string? CreatedAt { get; init; }
    public string? Status { get; init; }
    public bool Finished { get; init; }
}