using BettingSystem.Models;
using BettingSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace BettingSystem.Controllers;

[ApiController]
[Route("[controller]")]
public class BetController : ControllerBase
{
    private readonly IBetProcessorService _betProcessor;
    private readonly ISummaryService _summaryService;

    public BetController(IBetProcessorService betProcessor, ISummaryService summaryService)
    {
        _betProcessor = betProcessor;
        _summaryService = summaryService;
    }

    [HttpPost]
    public IActionResult AddBet([FromBody] Bet bet)
    {
        _betProcessor.AddBet(bet);
        return Ok("Bet received");
    }

    [HttpPost("shutdown")]
    public async Task<IActionResult> Shutdown()
    {
        await _betProcessor.ShutdownSystemAsync();
        var summary = _summaryService.GetSummary();
        return Ok(summary);
    }

    [HttpGet("summary")]
    public IActionResult GetSummary()
    {
        var summary = _summaryService.GetSummary();
        return Ok(summary);
    }

}
