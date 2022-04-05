#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/Presentation/Octopus.Conductor.WebApi/Octopus.Conductor.WebApi.csproj", "src/Presentation/Octopus.Conductor.WebApi/"]
COPY ["src/Core/Octopus.Conductor.Application/Octopus.Conductor.Application.csproj", "src/Core/Octopus.Conductor.Application/"]
COPY ["src/Core/Octopus.Conductor.Domain/Octopus.Conductor.Domain.csproj", "src/Core/Octopus.Conductor.Domain/"]
COPY ["src/Infrastructure/Octopus.Conductor.Infrastructure.RabbitMQ/Octopus.Conductor.Infrastructure.RabbitMQ.csproj", "src/Infrastructure/Octopus.Conductor.Infrastructure.RabbitMQ/"]
COPY ["src/Infrastructure/Octopus.Conductor.Infrastructure.RelationalDB/Octopus.Conductor.Infrastructure.RelationalDB.csproj", "src/Infrastructure/Octopus.Conductor.Infrastructure.RelationalDB/"]
COPY ["src/Infrastructure/Octopus.Conductor.Infrastructure.WorkerService/Octopus.Conductor.Infrastructure.WorkerService.csproj", "src/Infrastructure/Octopus.Conductor.Infrastructure.WorkerService/"]
RUN dotnet restore "src/Presentation/Octopus.Conductor.WebApi/Octopus.Conductor.WebApi.csproj"
COPY . .
WORKDIR "/src/src/Presentation/Octopus.Conductor.WebApi"
RUN dotnet build "Octopus.Conductor.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Octopus.Conductor.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Octopus.Conductor.WebApi.dll"]