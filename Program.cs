using BettingSystem.Data;
using BettingSystem.Models;
using BettingSystem.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IBetProcessorService, BetProcessorService>();
builder.Services.AddSingleton<ISummaryService, SummaryService>();
builder.Services.AddSingleton<IBetWorkerManager, BetWorkerManager>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseCors("AllowAll");

var clients = new[] { "Ana", "Juan", "Marta", "Pedro", "Lucía" };
var rand = new Random();

for (int i = 1; i <= 100; i++)
{
    var bet = new Bet
    {
        Id = i,
        Amount = rand.Next(10, 100),
        Odds = rand.NextDouble() * 3 + 1,
        Client = clients[rand.Next(clients.Length)],
        Event = "Event_" + rand.Next(1, 5),
        Market = "Market_" + rand.Next(1, 3),
        Selection = "Selection_" + rand.Next(1, 4),
        Status = BetStatus.OPEN
    };
    InMemoryStorage.BetQueue.Enqueue(bet);

    var resolved = bet with
    {
        Status = (BetStatus)rand.Next(1, 4)
    };
    InMemoryStorage.BetQueue.Enqueue(resolved);
}

var workerManager = app.Services.GetRequiredService<IBetWorkerManager>();
workerManager.StartWorkers();

app.Run();
