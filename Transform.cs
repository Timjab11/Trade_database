using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public static class Transform
{
    public static List<Trader> ImportTraders(string dataFilePath)
    {
        string absolutePath = Path.GetFullPath(dataFilePath);
        string raw = File.ReadAllText(absolutePath);

        using JsonDocument jsonDocument = JsonDocument.Parse(raw);
        JsonElement root = jsonDocument.RootElement;

        var tradersById = new Dictionary<int, Trader>();

        if (!root.TryGetProperty("data", out JsonElement dataArray) || dataArray.ValueKind != JsonValueKind.Array)
        {
            return new List<Trader>();
        }

        foreach (JsonElement businessCase in dataArray.EnumerateArray())
        {
            Trader? trader = Trader.FromBusinessCase(businessCase);
            if (trader is null)
            {
                continue;
            }

            if (tradersById.TryGetValue(trader.Id, out Trader? existingTrader))
            {
                existingTrader.AddDealFromBusinessCase(businessCase);

                continue;
            }

            tradersById[trader.Id] = trader;
        }

        return new List<Trader>(tradersById.Values);
    }

    public static void Main()
    {
        string baseDirectory = Directory.GetCurrentDirectory();
        string inputPath = Path.Combine(baseDirectory, "data.json");
        string outputPath = Path.Combine(baseDirectory, "traders.json");

        List<Trader> traders = ImportTraders(inputPath);

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        string outputJson = JsonSerializer.Serialize(traders, options);
        File.WriteAllText(outputPath, outputJson);

        Console.WriteLine($"Imported {traders.Count} traders to {outputPath}");
    }
}