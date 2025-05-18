using BettingSystem.Data;
using BettingSystem.Models;

namespace BettingSystem.Services;

public class BetWorkerManager : IBetWorkerManager
{
    private readonly List<Task> _workers = new();
    private readonly CancellationTokenSource _cts = new();

    public void StartWorkers(int numberOfWorkers = 4)
    {
        for (int i = 0; i < numberOfWorkers; i++)
        {
            var task = Task.Run(() => ProcessBets(_cts.Token));
            _workers.Add(task);
        }
    }

    public async Task StopWorkersAsync()
    {
        _cts.Cancel();
        await Task.WhenAll(_workers);
    }

    private async Task ProcessBets(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested || !InMemoryStorage.BetQueue.IsEmpty)
        {
            if (InMemoryStorage.BetQueue.TryDequeue(out var bet))
            {
                await Task.Delay(50); // Simula el procesamiento de 50ms

                if (IsStatusSequenceValid(bet))
                {
                    double profitOrLoss = 0;

                    switch (bet.Status)
                    {
                        case BetStatus.WINNER:
                            profitOrLoss = bet.Amount * bet.Odds;
                            break;
                        case BetStatus.LOSER:
                            profitOrLoss = -bet.Amount;
                            break;
                        case BetStatus.VOID:
                            profitOrLoss = 0;
                            break;
                    }

                    var result = new BetResult
                    {
                        Id = bet.Id,
                        Client = bet.Client,
                        Stake = bet.Amount,
                        ProfitOrLoss = profitOrLoss,
                        Status = bet.Status
                    };

                    InMemoryStorage.Results.Add(result);
                    Interlocked.Increment(ref InMemoryStorage.TotalBetsProcessed);

                    UpdateClientStats(result);
                }
                else
                {
                    InMemoryStorage.BetsForReview.Add(bet);
                }

                InMemoryStorage.LastBetById[bet.Id] = bet;
            }
            else
            {
                await Task.Delay(10);
            }
        }
    }

    private bool IsStatusSequenceValid(Bet newBet)
    {
        if (InMemoryStorage.LastBetById.TryGetValue(newBet.Id, out var lastBet))
        {
            return lastBet.Status == BetStatus.OPEN && newBet.Status != BetStatus.OPEN;
        }

        return newBet.Status == BetStatus.OPEN;
    }

    private void UpdateClientStats(BetResult result)
    {
        InMemoryStorage.ClientStats.AddOrUpdate(result.Client,
            new ClientStats
            {
                Client = result.Client,
                TotalProfit = result.ProfitOrLoss > 0 ? result.ProfitOrLoss : 0,
                TotalLoss = result.ProfitOrLoss < 0 ? -result.ProfitOrLoss : 0
            },
            (key, existing) =>
            {
                if (result.ProfitOrLoss > 0)
                    existing.TotalProfit += result.ProfitOrLoss;
                else if (result.ProfitOrLoss < 0)
                    existing.TotalLoss += -result.ProfitOrLoss;

                return existing;
            });
    }
}
