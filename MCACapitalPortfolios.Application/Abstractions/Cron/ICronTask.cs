namespace MCACapitalPortfolios.Application.Cron;

public interface ICronTask {
    Task Execute();
}