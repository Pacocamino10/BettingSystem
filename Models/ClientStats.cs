namespace BettingSystem.Models;

public class ClientStats
{
    public string Client { get; set; } = string.Empty;
    public double TotalProfit { get; set; }
    public double TotalLoss { get; set; }
}
