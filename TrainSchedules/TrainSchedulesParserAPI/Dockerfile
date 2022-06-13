#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
FROM mcr.microsoft.com/dotnet/aspnet:6.0.0-focal-arm32v7 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TrainSchedulesParserAPI/TrainSchedulesParserAPI.csproj", "TrainSchedulesParserAPI/"]
RUN dotnet restore "TrainSchedulesParserAPI/TrainSchedulesParserAPI.csproj"
COPY . .
WORKDIR "/src/TrainSchedulesParserAPI"
RUN dotnet build "TrainSchedulesParserAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TrainSchedulesParserAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TrainSchedulesParserAPI.dll"]