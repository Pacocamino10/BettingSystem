using BettingSystem.Models;
using System.Collections.Concurrent;

namespace BettingSystem.Data;

public static class InMemoryStorage
{
    public static ConcurrentQueue<Bet> BetQueue = new();
    public static ConcurrentDictionary<int, Bet> LastBetById = new();
    public static ConcurrentBag<BetResult> Results = new();
    public static ConcurrentDictionary<string, ClientStats> ClientStats = new();
    public static ConcurrentBag<Bet> BetsForReview = new();
    public static long TotalBetsProcessed = 0;
    public static bool AcceptingBets = true;
}
