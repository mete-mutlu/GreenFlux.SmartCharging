#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/GreenFlux.SmartCharging.API/GreenFlux.SmartCharging.API.csproj", "src/GreenFlux.SmartCharging.API/"]
RUN dotnet restore "src/GreenFlux.SmartCharging.API/GreenFlux.SmartCharging.API.csproj"
COPY . .
WORKDIR "/src/src/GreenFlux.SmartCharging.API"
RUN dotnet build "GreenFlux.SmartCharging.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GreenFlux.SmartCharging.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GreenFlux.SmartCharging.API.dll"]