using System.Text.Json.Serialization;
using MCACapitalPortfolios.Api.Cron;
using MCACapitalPortfolios.Application.Abstractions.Cron;
using MCACapitalPortfolios.Application.Abstractions.Repository;
using MCACapitalPortfolios.Application.Abstractions.Services;
using MCACapitalPortfolios.Application.Services;
using MCACapitalPortfolios.Infrastructure.Cron;
using MCACapitalPortfolios.Infrastructure.Repositories;
using MCACapitalPortfolios.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) 
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers()
                .AddJsonOptions(options => {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

builder.Services.AddSingleton<IPortfoliosService, PortfolioService>();
builder.Services.AddSingleton<ICalendarService, CalendarService>();
builder.Services.AddSingleton<IPortfolioRepository, PortfolioRepository>();
builder.Services.AddSingleton<IPortfolioHistoryRepository, PortfolioHistoryRepository>();
builder.Services.AddSingleton<ISeriesService, SeriesService>();
builder.Services.AddSingleton<ICron>((provider) => {
    return new Cron(new CronExpression("0 18 * * *"), new CronTask(), provider.GetService<ILogger<Cron>>());
});

builder.Services.AddHostedService<PortfolioEvaluatorCron>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
