namespace MCACapitalPortfolios.Application.Abstractions.Cron;

public interface ICronExpression {
    string GetDescritption();
    DateTime GetNextOccurrence(DateTime referenceDate);
}