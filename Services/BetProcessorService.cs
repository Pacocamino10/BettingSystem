using BettingSystem.Data;
using BettingSystem.Models;

namespace BettingSystem.Services;

public class BetProcessorService : IBetProcessorService
{
    private readonly IBetWorkerManager _workerManager;

    public BetProcessorService(IBetWorkerManager workerManager)
    {
        _workerManager = workerManager;
        _workerManager.StartWorkers();
    }

    public void AddBet(Bet bet)
    {
        if (!InMemoryStorage.AcceptingBets)
            return;

        InMemoryStorage.BetQueue.Enqueue(bet);
    }

    public async Task ShutdownSystemAsync()
    {
        InMemoryStorage.AcceptingBets = false;
        await _workerManager.StopWorkersAsync();
    }
}
