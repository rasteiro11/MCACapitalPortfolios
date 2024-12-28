using MCACapitalPortfolios.Application.Abstractions.Cron;
using MCACapitalPortfolios.Application.Cron;
using Microsoft.Extensions.Logging;

namespace MCACapitalPortfolios.Infrastructure.Cron;

public class Cron : ICron {
    private readonly ICronExpression expression;
    private readonly ICronTask cronTask;
    private Timer? _timer;
    private readonly ILogger<Cron> logger;

    public Cron(ICronExpression expression, ICronTask cronTask, ILogger<Cron> logger)
    {
        this.expression = expression;
        this.cronTask = cronTask;
        this.logger = logger;
        logger.LogInformation($"Cron registered: {expression.GetDescritption()}");
    }

    public void ScheduleNextExecution()
    {
        DateTime now = DateTime.Now;
        DateTime nextOccurrence = expression.GetNextOccurrence(now);

        TimeSpan delay = nextOccurrence - now;
        if (delay < TimeSpan.Zero)
        {
            throw new InvalidOperationException("Next occurrence is in the past. Cron expression might be invalid.");
        }

        logger.LogInformation($"Task scheduled at {nextOccurrence} (in {delay.Hours} hours {delay.Minutes} minutes {delay.TotalSeconds} seconds).");

        _timer = new Timer(async _ =>
        {
            await cronTask.Execute();
            ScheduleNextExecution(); 
        }, null, delay, Timeout.InfiniteTimeSpan);
    }

    public void Start()
    {
        ScheduleNextExecution();
    }
}
