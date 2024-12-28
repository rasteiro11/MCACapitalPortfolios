namespace MCACapitalPortfolios.Application.Abstractions.Cron;

public interface ICron {
    void Start();
    void ScheduleNextExecution();
}