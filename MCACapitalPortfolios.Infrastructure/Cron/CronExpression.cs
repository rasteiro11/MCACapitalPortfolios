using CronExpressionDescriptor;
using MCACapitalPortfolios.Application.Abstractions.Cron;
using NCrontab;

namespace MCACapitalPortfolios.Infrastructure.Cron;

public class CronExpression : ICronExpression
{

    private readonly string expression;

    public CronExpression(string expression)
    {
        this.expression = expression;
    }

    public string GetDescritption()
    {
        return ExpressionDescriptor.GetDescription(expression, new Options { Use24HourTimeFormat = true });
    }

    public DateTime GetNextOccurrence(DateTime referenceDate)
    {
        return CrontabSchedule.Parse(expression).GetNextOccurrence(referenceDate);
    }
}