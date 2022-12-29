#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["CountryCurrency.Crawler.WebApi/CountryCurrency.Crawler.WebApi.csproj", "CountryCurrency.Crawler.WebApi/"]
COPY ["CountryCurrency.Crawler.Logic/CountryCurrency.Crawler.Logic.csproj", "CountryCurrency.Crawler.Logic/"]
COPY ["CountryCurrency.Crawler.Parse/CountryCurrency.Crawler.Parse.csproj", "CountryCurrency.Crawler.Parse/"]
COPY ["CountryCurrency.Crawler.Domain/CountryCurrency.Crawler.Domain.csproj", "CountryCurrency.Crawler.Domain/"]
COPY ["CountryCurrency.Crawler.Common/CountryCurrency.Crawler.Common.csproj", "CountryCurrency.Crawler.Common/"]
RUN dotnet restore "CountryCurrency.Crawler.WebApi/CountryCurrency.Crawler.WebApi.csproj"
COPY . .
WORKDIR "/src/CountryCurrency.Crawler.WebApi"
RUN dotnet build "CountryCurrency.Crawler.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CountryCurrency.Crawler.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CountryCurrency.Crawler.WebApi.dll"]