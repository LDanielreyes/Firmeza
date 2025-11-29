# Use SDK image to build and run tests
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file and restore dependencies
COPY ["Firmeza.sln", "./"]
COPY ["Firmeza/Firmeza.csproj", "Firmeza/"]
COPY ["FirmezaAPI/FirmezaAPI.csproj", "FirmezaAPI/"]
COPY ["Firmeza.Tests/Firmeza.Tests.csproj", "Firmeza.Tests/"]

RUN dotnet restore "Firmeza.sln"

# Copy the rest of the code
COPY . .

# Build the test project
WORKDIR "/src/Firmeza.Tests"
RUN dotnet build "Firmeza.Tests.csproj" -c Release -o /app/build

# Run tests
# We use the SDK image as the final image because we need the 'dotnet test' capability
# and potentially other SDK tools.
FROM build AS testrunner
WORKDIR /src/Firmeza.Tests
ENTRYPOINT ["dotnet", "test", "--logger:trx"]
