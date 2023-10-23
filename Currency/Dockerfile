#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM ubuntu:22.04

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Currency/Currency.csproj", "Currency/"]
RUN dotnet restore "Currency/Currency.csproj"
COPY . .
WORKDIR "/src/Currency"
RUN dotnet build "Currency.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Currency.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Currency.dll"]