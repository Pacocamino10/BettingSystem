using BettingSystem.Data;
using BettingSystem.Models;
using BettingSystem.Services;
using Xunit;

namespace BettingSystem.Tests
{
    public class SummaryServiceTests
    {
        [Fact]
        public void GetSummary_ReturnsCorrectSummary()
        {
            // Arrange
            InMemoryStorage.Results.Clear();
            InMemoryStorage.ClientStats.Clear();
            InMemoryStorage.TotalBetsProcessed = 2;

            InMemoryStorage.Results.Add(new BetResult
            {
                Id = 1,
                Client = "Juan",
                Stake = 50,
                ProfitOrLoss = 100,
                Status = BetStatus.WINNER
            });

            InMemoryStorage.Results.Add(new BetResult
            {
                Id = 2,
                Client = "Ana",
                Stake = 30,
                ProfitOrLoss = -30,
                Status = BetStatus.LOSER
            });

            InMemoryStorage.ClientStats["Juan"] = new ClientStats { Client = "Juan", TotalProfit = 100 };
            InMemoryStorage.ClientStats["Ana"] = new ClientStats { Client = "Ana", TotalLoss = 30 };

            var service = new SummaryService();

            // Act
            var summary = service.GetSummary();

            // Assert
            Assert.Contains("Total bets processed: 2", summary);
            Assert.Contains("Total amount bet: 80", summary);
            Assert.Contains("Total profit/loss: 70", summary);
            Assert.Contains("Juan", summary);
            Assert.Contains("Ana", summary);
        }
    }
}
