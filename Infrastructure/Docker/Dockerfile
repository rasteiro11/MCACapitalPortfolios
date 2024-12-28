FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY MCACapitalPortfolios.Api/MCACapitalPortfolios.Api.csproj ./MCACapitalPortfolios.Api/
RUN dotnet restore ./MCACapitalPortfolios.Api/MCACapitalPortfolios.Api.csproj

COPY . ./
WORKDIR /app/MCACapitalPortfolios.Api

RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /out ./

EXPOSE 5003

ENTRYPOINT ["dotnet", "MCACapitalPortfolios.Api.dll"]


