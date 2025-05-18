namespace BettingSystem.Models;

public class BetResult
{
    public int Id { get; set; }
    public string Client { get; set; } = string.Empty;
    public double Stake { get; set; }
    public double ProfitOrLoss { get; set; }
    public BetStatus Status { get; set; }
}
