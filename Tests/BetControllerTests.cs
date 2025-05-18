using BettingSystem.Controllers;
using BettingSystem.Models;
using BettingSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BettingSystem.Tests
{
    public class BetControllerTests
    {
        [Fact]
        public async Task Shutdown_ReturnsSummaryText()
        {
            var mockProcessor = new Mock<IBetProcessorService>();
            var mockSummary = new Mock<ISummaryService>();

            mockProcessor.Setup(p => p.ShutdownSystemAsync()).Returns(Task.CompletedTask);
            mockSummary.Setup(s => s.GetSummary()).Returns("summary text");

            var controller = new BetController(mockProcessor.Object, mockSummary.Object);

            var result = await controller.Shutdown();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("summary text", okResult.Value);
        }

        [Fact]
        public void AddBet_ReturnsOk_WhenValidBet()
        {
            var mockProcessor = new Mock<IBetProcessorService>();
            var mockSummary = new Mock<ISummaryService>();

            var controller = new BetController(mockProcessor.Object, mockSummary.Object);
            var bet = new Bet { Id = 1, Client = "Test", Amount = 50, Status = BetStatus.OPEN };

            var result = controller.AddBet(bet);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Bet received", okResult.Value);
            mockProcessor.Verify(p => p.AddBet(bet), Times.Once);
        }

        [Fact]
        public void GetSummary_ReturnsSummaryText()
        {
            var mockProcessor = new Mock<IBetProcessorService>();
            var mockSummary = new Mock<ISummaryService>();
            mockSummary.Setup(s => s.GetSummary()).Returns("summary text");

            var controller = new BetController(mockProcessor.Object, mockSummary.Object);

            var result = controller.GetSummary();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("summary text", okResult.Value);
        }
    }

}
