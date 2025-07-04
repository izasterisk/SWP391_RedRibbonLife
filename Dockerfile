# Use the official .NET 8.0 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy solution file and project files
COPY *.sln .
COPY SWP391_RedRibbonLife/*.csproj ./SWP391_RedRibbonLife/
COPY BLL/*.csproj ./BLL/
COPY DAL/*.csproj ./DAL/

# Restore dependencies
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish SWP391_RedRibbonLife/SWP391_RedRibbonLife.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy published app
COPY --from=build-env /app/out .

# Expose port
EXPOSE 8080

# Set environment variable for ASP.NET Core
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

# Run the application
ENTRYPOINT ["dotnet", "SWP391_RedRibbonLife.dll"]