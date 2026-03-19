# Use the official .NET 10 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY TeamTrack.Api.sln .
COPY src/TeamTrack.Api/TeamTrack.Api.csproj src/TeamTrack.Api/

# Restore dependencies
RUN dotnet restore TeamTrack.Api.sln

# Copy the rest of the source
COPY . .

# Build the project
RUN dotnet publish TeamTrack.Api.sln -c Release -o /app/publish

# Use the official .NET 10 runtime image for running
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Run the Web API
ENTRYPOINT ["dotnet", "TeamTrack.Api.dll"]
