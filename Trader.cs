using System;
using System.Collections.Generic;
using System.Text.Json;

public sealed class Deal
{
    public int TradeId { get; init; }
    public decimal AmountToBeEarned { get; init; }
    public int? Probability { get; init; }
}

public sealed class Trader
{
    public int Id { get; init; }
    public string? FullName { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Email { get; init; }
    public string? Gender { get; init; }
    public List<int> SourceBusinessCases { get; } = new();
    public List<Deal> Deals { get; } = new();
    public decimal TotalTradeAmount
    {
        get
        {
            decimal total = 0;
            foreach (Deal deal in Deals)
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

            foreach (Deal deal in Deals)
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
            FullName = ReadString(owner, "fullName") ?? ReadString(owner, "fullNameWithoutTitles"),
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
        foreach (Deal deal in Deals)
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

        Deals.Add(new Deal
        {
            TradeId = businessCaseId,
            AmountToBeEarned = ReadDecimal(businessCase, "totalAmount") ?? 0,
            Probability = ReadInt32(businessCase, "probability")
        });
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