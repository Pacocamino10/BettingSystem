using BettingSystem.Models;

namespace BettingSystem.Services;

public interface IBetProcessorService
{
    void AddBet(Bet bet);
    Task ShutdownSystemAsync();
}
