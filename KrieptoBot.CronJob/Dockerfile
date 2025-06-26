# Use the official .NET SDK image to build and publish the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["KrieptoBot.CronJob/KrieptoBot.CronJob.csproj", "KrieptoBot.CronJob/"]
RUN dotnet restore "KrieptoBot.CronJob/KrieptoBot.CronJob.csproj"
COPY . .
WORKDIR "/src/KrieptoBot.CronJob"
RUN dotnet publish "KrieptoBot.CronJob.csproj" -c Release -o /app/publish

# Use the official .NET runtime image for the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "KrieptoBot.CronJob.dll"]

