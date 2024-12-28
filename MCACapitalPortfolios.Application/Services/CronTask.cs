using MCACapitalPortfolios.Application.Cron;
using Microsoft.VisualBasic;

namespace MCACapitalPortfolios.Application.Services;

public class CronTask : ICronTask
{
    public Task Execute()
    {
        Console.WriteLine("Running the cron task");
        return Task.CompletedTask;
    }
}