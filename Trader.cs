using System;
using System.Collections.Generic;
using System.Text.Json;

public sealed class Trader
{
    public int Id { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Email { get; init; }
    public string? Gender { get; init; }
    public List<int> SourceBusinessCases { get; } = new();
    public List<Trade> Deals { get; } = new();
    public List<Trade> FinishedTrades { get; } = new();
    public List<Trade> OngoingTrades { get; } = new();
    public decimal TotalTradeAmount
    {
        get
        {
            decimal total = 0;
            foreach (Trade deal in Deals)
            {
                total += deal.AmountToBeEarned;
            }

            return total;
        }
    }
    public double? AverageProbability
    {
        get
        {
            int probabilityCount = 0;
            int probabilitySum = 0;

            foreach (Trade deal in Deals)
            {
                if (!deal.Probability.HasValue)
                {
                    continue;
                }

                probabilityCount++;
                probabilitySum += deal.Probability.Value;
            }

            if (probabilityCount == 0)
            {
                return null;
            }

            return (double)probabilitySum / probabilityCount;
        }
    }
    public int DealCount => Deals.Count;
    public int FinishedTradeCount => FinishedTrades.Count;
    public int OngoingTradeCount => OngoingTrades.Count;
    public int Wins => CountTradesByStatus(IsWinStatus);
    public int Losses => CountTradesByStatus(IsLostStatus);
    public double? WinRate
    {
        get
        {
            int decidedTrades = Wins + Losses;
            if (decidedTrades == 0)
            {
                return null;
            }

            return (double)Wins / decidedTrades * 100;
        }
    }
    public decimal? AverageAmountPerDeal
    {
        get
        {
            if (DealCount == 0)
            {
                return null;
            }

            return TotalTradeAmount / DealCount;
        }
    }
    public decimal? AverageAmountPerFinishedTrade => CalculateAverageAmount(FinishedTrades);
    public decimal? AverageAmountPerOngoingTrade => CalculateAverageAmount(OngoingTrades);
    public double? AverageFinishedTradeProbability => CalculateAverageProbability(FinishedTrades);
    public double? AverageOngoingTradeProbability => CalculateAverageProbability(OngoingTrades);

    public static Trader? FromBusinessCase(JsonElement businessCase)
    {
        if (!businessCase.TryGetProperty("owner", out JsonElement owner) || owner.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        if (!owner.TryGetProperty("id", out JsonElement ownerIdElement) || !ownerIdElement.TryGetInt32(out int ownerId))
        {
            return null;
        }

        var trader = new Trader
        {
            Id = ownerId,
            FirstName = ReadString(owner, "firstName"),
            LastName = ReadString(owner, "lastName"),
            Email = ReadString(owner, "contactInfo.email"),
            Gender = ReadString(owner, "gender")
        };

        trader.AddDealFromBusinessCase(businessCase);

        return trader;
    }

    public void AddSourceBusinessCase(int businessCaseId)
    {
        if (!SourceBusinessCases.Contains(businessCaseId))
        {
            SourceBusinessCases.Add(businessCaseId);
        }
    }

    public void AddDealFromBusinessCase(JsonElement businessCase)
    {
        if (!businessCase.TryGetProperty("id", out JsonElement businessCaseIdElement)
            || !businessCaseIdElement.TryGetInt32(out int businessCaseId))
        {
            return;
        }

        AddSourceBusinessCase(businessCaseId);

        bool hasDeal = false;
        foreach (Trade deal in Deals)
        {
            if (deal.TradeId == businessCaseId)
            {
                hasDeal = true;
                break;
            }
        }

        if (hasDeal)
        {
            return;
        }

        string? status = ReadString(businessCase, "status");

        Trade newTrade = new Trade
        {
            TradeId = businessCaseId,
            AmountToBeEarned = ReadDecimal(businessCase, "totalAmount") ?? 0,
            Probability = ReadInt32(businessCase, "probability"),
            ValidFrom = ReadString(businessCase, "validFrom"),
            ValidTill = ReadString(businessCase, "validTill"),
            ScheduledEnd = ReadString(businessCase, "scheduledEnd"),
            CreatedAt = ReadString(businessCase, "rowInfo.createdAt"),
            Status = status,
            Finished = IsFinishedStatus(status)
        };

        Deals.Add(newTrade);

        if (newTrade.Finished)
        {
            FinishedTrades.Add(newTrade);
        }

        if (IsOngoingStatus(status))
        {
            OngoingTrades.Add(newTrade);
        }
    }

    private static decimal? CalculateAverageAmount(List<Trade> trades)
    {
        if (trades.Count == 0)
        {
            return null;
        }

        decimal total = 0;
        foreach (Trade trade in trades)
        {
            total += trade.AmountToBeEarned;
        }

        return total / trades.Count;
    }

    private static double? CalculateAverageProbability(List<Trade> trades)
    {
        int count = 0;
        int sum = 0;

        foreach (Trade trade in trades)
        {
            if (!trade.Probability.HasValue)
            {
                continue;
            }

            sum += trade.Probability.Value;
            count++;
        }

        if (count == 0)
        {
            return null;
        }

        return (double)sum / count;
    }

    private static bool IsFinishedStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return false;
        }

        return status.Contains("WIN", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsWinStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return false;
        }

        return status.Contains("WIN", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsLostStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return false;
        }

        return status.Contains("LOST", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsOngoingStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return false;
        }

        return status.Contains("ACTIVE", StringComparison.OrdinalIgnoreCase);
    }

    private int CountTradesByStatus(Func<string?, bool> statusMatcher)
    {
        int count = 0;
        foreach (Trade trade in Deals)
        {
            if (statusMatcher(trade.Status))
            {
                count++;
            }
        }

        return count;
    }

    private static string? ReadString(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out JsonElement value) || value.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        return value.GetString();
    }

    private static decimal? ReadDecimal(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out JsonElement value) || value.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        if (value.ValueKind == JsonValueKind.Number && value.TryGetDecimal(out decimal decimalValue))
        {
            return decimalValue;
        }

        if (value.ValueKind == JsonValueKind.String && decimal.TryParse(value.GetString(), out decimal parsedDecimal))
        {
            return parsedDecimal;
        }

        return null;
    }

    private static int? ReadInt32(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out JsonElement value) || value.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out int intValue))
        {
            return intValue;
        }

        if (value.ValueKind == JsonValueKind.String && int.TryParse(value.GetString(), out int parsedInt))
        {
            return parsedInt;
        }

        return null;
    }
}