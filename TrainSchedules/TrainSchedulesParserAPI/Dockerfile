FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
#FROM mcr.microsoft.com/dotnet/aspnet:6.0.0-focal-arm32v7 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY TrainSchedulesParserAPI.csproj .

RUN dotnet restore "./TrainSchedulesParserAPI.csproj"
COPY . .
WORKDIR "/src/TrainSchedulesParserAPI"
RUN dotnet build "TrainSchedules/TrainSchedulesParserAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TrainSchedules/TrainSchedulesParserAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TrainSchedulesParserAPI.dll"]