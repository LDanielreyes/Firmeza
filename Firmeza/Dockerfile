FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Firmeza.sln", "./"]
COPY ["Firmeza/Firmeza.csproj", "Firmeza/"]
COPY ["FirmezaAPI/FirmezaAPI.csproj", "FirmezaAPI/"]
COPY ["Firmeza.Tests/Firmeza.Tests.csproj", "Firmeza.Tests/"]
RUN dotnet restore "Firmeza/Firmeza.csproj"
COPY . .
WORKDIR "/src/Firmeza"
RUN dotnet build "Firmeza.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Firmeza.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Firmeza.dll"]
