using BettingSystem.Data;
using BettingSystem.Models;
using BettingSystem.Services;
using Xunit;
using System.Threading.Tasks;

namespace BettingSystem.Tests
{
    public class BetWorkerManagerAdditionalTests
    {
        [Fact]
        public async Task ProcessesMultipleBets_CorrectlyUpdatesStorage()
        {
            // Arrange
            InMemoryStorage.BetQueue.Clear();
            InMemoryStorage.Results.Clear();
            InMemoryStorage.TotalBetsProcessed = 0;
            InMemoryStorage.ClientStats.Clear();

            var manager = new BetWorkerManager();

            for (int i = 1; i <= 5; i++)
            {
                InMemoryStorage.BetQueue.Enqueue(new Bet
                {
                    Id = i,
                    Client = "Client" + i,
                    Amount = 50,
                    Odds = 2,
                    Status = BetStatus.OPEN
                });
            }

            Assert.Equal(5, InMemoryStorage.BetQueue.Count); // Verifica que están en cola

            // Act
            manager.StartWorkers();

            await Task.Delay(2000); // Más tiempo para procesar

            await manager.StopWorkersAsync();

            Assert.True(InMemoryStorage.BetQueue.IsEmpty); // Verifica que la cola quedó vacía

            // Assert
            Assert.Equal(5, InMemoryStorage.TotalBetsProcessed);
            Assert.Equal(5, InMemoryStorage.Results.Count);
            foreach (var clientStat in InMemoryStorage.ClientStats.Values)
            {
                Assert.True(clientStat.TotalProfit >= 0 || clientStat.TotalLoss >= 0);
            }
        }


        [Fact]
        public async Task StopWorkersAsync_StopsWorkerGracefully()
        {
            // Arrange
            var manager = new BetWorkerManager();

            // Act
            manager.StartWorkers();
            var stopTask = manager.StopWorkersAsync();

            // Assert
            await stopTask;
            Assert.True(true); // Simplemente que no falle y termine
        }

        [Fact]
        public async Task ProcessesEmptyQueue_WithoutErrors()
        {
            // Arrange
            InMemoryStorage.BetQueue.Clear();
            InMemoryStorage.BetQueue.Clear();
            InMemoryStorage.Results.Clear();
            InMemoryStorage.TotalBetsProcessed = 0;
            InMemoryStorage.ClientStats.Clear();

            var manager = new BetWorkerManager();

            // Act
            manager.StartWorkers();

            await Task.Delay(500); // esperar un poco para procesar (cola vacía)

            await manager.StopWorkersAsync();

            // Assert
            Assert.Empty(InMemoryStorage.Results);
            Assert.Equal(0, InMemoryStorage.TotalBetsProcessed);
        }
    }
}
