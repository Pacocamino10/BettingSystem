using BettingSystem.Data;
using BettingSystem.Models;
using BettingSystem.Services;
using Moq;
using Xunit;

namespace BettingSystem.Tests
{
    public class BetProcessorServiceTests
    {
        [Fact]
        public void AddBet_EnqueuesBet()
        {
            // Arrange
            InMemoryStorage.BetQueue.Clear();
            var mockWorkerManager = new Mock<IBetWorkerManager>();
            var service = new BetProcessorService(mockWorkerManager.Object);

            var bet = new Bet { Id = 1, Client = "Test", Amount = 50, Status = BetStatus.OPEN };

            // Act
            service.AddBet(bet);

            // Assert
            Assert.False(InMemoryStorage.BetQueue.IsEmpty);
        }

        [Fact]
        public async Task ShutdownSystemAsync_CallsStopWorkers()
        {
            // Arrange
            var mockWorkerManager = new Mock<IBetWorkerManager>();
            mockWorkerManager.Setup(w => w.StopWorkersAsync()).Returns(Task.CompletedTask).Verifiable();

            var service = new BetProcessorService(mockWorkerManager.Object);

            // Act
            await service.ShutdownSystemAsync();

            // Assert
            mockWorkerManager.Verify(w => w.StopWorkersAsync(), Times.Once);
        }
    }

    public class BetWorkerManagerTests
    {
        [Fact]
        public async Task WorkerProcessesBetsAndStops()
        {
            // Arrange
            InMemoryStorage.BetQueue.Clear();
            InMemoryStorage.Results.Clear();
            InMemoryStorage.TotalBetsProcessed = 0;
            InMemoryStorage.ClientStats.Clear();

            Assert.Empty(InMemoryStorage.BetQueue);
            Assert.Empty(InMemoryStorage.Results);
            Assert.Equal(0, InMemoryStorage.TotalBetsProcessed);

            var manager = new BetWorkerManager();
            var bet = new Bet
            {
                Id = 1,
                Client = "Tester",
                Amount = 100,
                Odds = 2.0,
                Status = BetStatus.OPEN
            };

            InMemoryStorage.BetQueue.Enqueue(bet);
            manager.StartWorkers();

            // Let the worker process the bet
            await Task.Delay(500);

            await manager.StopWorkersAsync();

            foreach (var result in InMemoryStorage.Results)
            {
                Console.WriteLine($"Result: Client={result.Client}, Id={result.Id}, ProfitOrLoss={result.ProfitOrLoss}");
            }

            // Assert processed correctly
            Assert.Single(InMemoryStorage.Results);
            Assert.Equal(1, InMemoryStorage.TotalBetsProcessed);
        }

    }

    public class SummaryServiceEdgeTests
    {
        [Fact]
        public void GetSummary_EmptyStorage_ReturnsCorrectString()
        {
            // Arrange
            InMemoryStorage.Results.Clear();
            InMemoryStorage.ClientStats.Clear();
            InMemoryStorage.TotalBetsProcessed = 0;

            var service = new SummaryService();

            // Act
            var result = service.GetSummary();

            // Assert
            Assert.Contains("Total bets processed: 0", result);
            Assert.Contains("Total amount bet: 0", result);
            Assert.Contains("Total profit/loss: 0", result);
        }
    }
}
