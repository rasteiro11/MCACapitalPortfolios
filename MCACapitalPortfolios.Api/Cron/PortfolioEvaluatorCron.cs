using MCACapitalPortfolios.Application.Abstractions.Cron;
using MCACapitalPortfolios.Infrastructure.Cron;

namespace MCACapitalPortfolios.Api.Cron;

public class PortfolioEvaluatorCron : IHostedService
{
    private readonly ICron cron;

    public PortfolioEvaluatorCron(ICron cron)
    {
        this.cron = cron;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        cron.Start();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}