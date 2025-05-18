using BettingSystem.Data;
using System.Text;

namespace BettingSystem.Services;

public class SummaryService : ISummaryService
{
    public string GetSummary()
    {
        var sb = new StringBuilder();

        var totalAmount = InMemoryStorage.Results.Sum(r => r.Stake);
        var totalProfitOrLoss = InMemoryStorage.Results.Sum(r => r.ProfitOrLoss);

        var topWinners = InMemoryStorage.ClientStats.Values
            .OrderByDescending(c => c.TotalProfit)
            .Take(5);

        var topLosers = InMemoryStorage.ClientStats.Values
            .OrderByDescending(c => c.TotalLoss)
            .Take(5);

        sb.AppendLine($"Total bets processed: {InMemoryStorage.TotalBetsProcessed}");
        sb.AppendLine($"Total amount bet: {totalAmount}");
        sb.AppendLine($"Total profit/loss: {totalProfitOrLoss}");

        sb.AppendLine("Top 5 clients by profit:");
        foreach (var client in topWinners)
        {
            sb.AppendLine($" - {client.Client}: {client.TotalProfit}");
        }

        sb.AppendLine("Top 5 clients by loss:");
        foreach (var client in topLosers)
        {
            sb.AppendLine($" - {client.Client}: {client.TotalLoss}");
        }

        return sb.ToString();
    }
}
