FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/Sabemi.Webhooks.API/Sabemi.Webhooks.API.csproj Sabemi.Webhooks.API/
RUN dotnet restore Sabemi.Webhooks.API/Sabemi.Webhooks.API.csproj

COPY src/Sabemi.Webhooks.API/ Sabemi.Webhooks.API/
WORKDIR /src/Sabemi.Webhooks.API
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Sabemi.Webhooks.API.dll"]
