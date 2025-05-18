namespace BettingSystem.Services;

public interface IBetWorkerManager
{
    void StartWorkers(int numberOfWorkers = 4);
    Task StopWorkersAsync();
}
