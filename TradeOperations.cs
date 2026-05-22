using System;
using System.Collections.Generic;
using System.Linq;

public enum TraderSortField
{
    Name,
    TradeAmount,
    SuccessRate,
    AmountOfTrades,
    WinRate
}

public enum SortDirection
{
    Ascending,
    Descending
}

public sealed class TraderFilterOptions
{
    public string? NameContains { get; init; }
    public decimal? MinTradeAmount { get; init; }
    public decimal? MaxTradeAmount { get; init; }
    public double? MinSuccessRate { get; init; }
    public double? MaxSuccessRate { get; init; }
    public int? MinAmountOfTrades { get; init; }
    public int? MaxAmountOfTrades { get; init; }
    public double? MinWinRate { get; init; }
    public double? MaxWinRate { get; init; }
}

public sealed class TradeOperations
{
    public List<Trader> Traders { get; }
    public List<Trader> FilteredTraders { get; private set; }
    public List<Trader> SortedTraders { get; private set; }

    public TradeOperations(IEnumerable<Trader> traders)
    {
        Traders = new List<Trader>(traders);
        FilteredTraders = new List<Trader>(Traders);
        SortedTraders = new List<Trader>(FilteredTraders);
    }

    public List<Trader> FilterTraders(TraderFilterOptions? filterOptions)
    {
        IEnumerable<Trader> filteredTraders = Traders;

        if (filterOptions is null)
        {
            FilteredTraders = filteredTraders.ToList();
            SortedTraders = new List<Trader>(FilteredTraders);
            return FilteredTraders;
        }

        if (!string.IsNullOrWhiteSpace(filterOptions.NameContains))
        {
            filteredTraders = filteredTraders.Where(trader =>
                ContainsSubstring(GetTraderName(trader), filterOptions.NameContains));
        }

        if (filterOptions.MinTradeAmount.HasValue)
        {
            filteredTraders = filteredTraders.Where(trader => trader.TotalTradeAmount >= filterOptions.MinTradeAmount.Value);
        }

        if (filterOptions.MaxTradeAmount.HasValue)
        {
            filteredTraders = filteredTraders.Where(trader => trader.TotalTradeAmount <= filterOptions.MaxTradeAmount.Value);
        }

        if (filterOptions.MinSuccessRate.HasValue)
        {
            filteredTraders = filteredTraders.Where(trader => trader.AverageProbability >= filterOptions.MinSuccessRate.Value);
        }

        if (filterOptions.MaxSuccessRate.HasValue)
        {
            filteredTraders = filteredTraders.Where(trader => trader.AverageProbability <= filterOptions.MaxSuccessRate.Value);
        }

        if (filterOptions.MinAmountOfTrades.HasValue)
        {
            filteredTraders = filteredTraders.Where(trader => trader.DealCount >= filterOptions.MinAmountOfTrades.Value);
        }

        if (filterOptions.MaxAmountOfTrades.HasValue)
        {
            filteredTraders = filteredTraders.Where(trader => trader.DealCount <= filterOptions.MaxAmountOfTrades.Value);
        }

        if (filterOptions.MinWinRate.HasValue)
        {
            filteredTraders = filteredTraders.Where(trader => trader.WinRate >= filterOptions.MinWinRate.Value);
        }

        if (filterOptions.MaxWinRate.HasValue)
        {
            filteredTraders = filteredTraders.Where(trader => trader.WinRate <= filterOptions.MaxWinRate.Value);
        }

        FilteredTraders = filteredTraders.ToList();
        SortedTraders = new List<Trader>(FilteredTraders);
        return FilteredTraders;
    }

    public List<Trader> SortTraders(TraderSortField sortField, SortDirection sortDirection = SortDirection.Ascending)
    {
        SortedTraders = SortTraderList(FilteredTraders, sortField, sortDirection);
        return SortedTraders;
    }

    public List<Trader> FilterAndSortTraders(
        TraderFilterOptions? filterOptions,
        TraderSortField sortField,
        SortDirection sortDirection = SortDirection.Ascending)
    {
        FilterTraders(filterOptions);
        return SortTraders(sortField, sortDirection);
    }

    public List<Trader> GetUnsortedTraders()
    {
        return Traders;
    }

    public List<Trader> GetFilteredTraders()
    {
        return FilteredTraders;
    }

    public List<Trader> GetSortedTraders()
    {
        return SortedTraders;
    }

    private static List<Trader> SortTraderList(
        IEnumerable<Trader> traders,
        TraderSortField sortField,
        SortDirection sortDirection)
    {
        Func<Trader, object?> sortSelector = sortField switch
        {
            TraderSortField.Name => trader => GetTraderName(trader),
            TraderSortField.TradeAmount => trader => trader.TotalTradeAmount,
            TraderSortField.SuccessRate => trader => trader.AverageProbability,
            TraderSortField.AmountOfTrades => trader => trader.DealCount,
            TraderSortField.WinRate => trader => trader.WinRate,
            _ => trader => GetTraderName(trader)
        };

        return sortDirection == SortDirection.Descending
            ? traders.OrderByDescending(sortSelector).ToList()
            : traders.OrderBy(sortSelector).ToList();
    }

    private static string GetTraderName(Trader trader)
    {
        string firstName = trader.FirstName ?? string.Empty;
        string lastName = trader.LastName ?? string.Empty;
        return $"{firstName} {lastName}".Trim();
    }

    private static bool ContainsSubstring(string source, string value)
    {
        return source.Contains(value, StringComparison.OrdinalIgnoreCase);
    }
}