# Use an official .NET SDK runtime as a parent image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env

# Set working directory in the container
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj /app/
RUN dotnet restore

# Copy everything else and build app
COPY . ./

# Expose port 80 and run the app
EXPOSE 80
ENTRYPOINT ["dotnet", "run", "--environment", "Development"]
