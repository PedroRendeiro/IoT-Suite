#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["IoTSuite/Server/IoTSuite.Server.csproj", "IoTSuite/Server/"]
COPY ["IoTSuite/Shared/IoTSuite.Shared.csproj", "IoTSuite/Shared/"]
COPY ["IoTSuite/Client/IoTSuite.Client.csproj", "IoTSuite/Client/"]
RUN dotnet restore "IoTSuite/Server/IoTSuite.Server.csproj"
COPY . .
WORKDIR "/src/IoTSuite/Server"
RUN dotnet build "IoTSuite.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IoTSuite.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IoTSuite.Server.dll"]