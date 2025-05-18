namespace BettingSystem.Models;

public record Bet
{
    public int Id { get; set; }
    public double Amount { get; set; }
    public double Odds { get; set; }
    public string Client { get; set; } = string.Empty;
    public string Event { get; set; } = string.Empty;
    public string Market { get; set; } = string.Empty;
    public string Selection { get; set; } = string.Empty;
    public BetStatus Status { get; set; }
}
