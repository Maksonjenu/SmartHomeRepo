# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files
COPY ["SmartHomeRepo/SmartHomeRepo.csproj", "SmartHomeRepo/"]
COPY ["SmartHome.Core/SmartHome.Core.csproj", "SmartHome.Core/"]
COPY ["SmartView/SmartView.csproj", "SmartView/"]

# Restore dependencies
RUN dotnet restore "SmartHomeRepo/SmartHomeRepo.csproj"

# Copy all source code
COPY . .

# Build
RUN dotnet build "SmartHomeRepo/SmartHomeRepo.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "SmartHomeRepo/SmartHomeRepo.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 5000
EXPOSE 5001

ENV ASPNETCORE_URLS=http://+:5000;https://+:5001
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "SmartHomeRepo.dll"]
