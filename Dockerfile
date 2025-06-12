FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY *.sln .
COPY quiz_application.Api/*.csproj ./quiz_application.Api/
COPY quiz_application.Application/*.csproj ./quiz_application.Application/
COPY quiz_application.Domain/*.csproj ./quiz_application.Domain/
COPY quiz_application.Infrastructure/*.csproj ./quiz_application.Infrastructure/
COPY quiz_application.Tests/*.csproj ./quiz_application.Tests/

# Restore dependencies
RUN dotnet restore

# Copy all source code
COPY . .

# Build the application
RUN dotnet build -c Release --no-restore

# Run tests
RUN dotnet test -c Release --no-build --verbosity normal

# Publish the application
RUN dotnet publish quiz_application.Api/quiz_application.Api.csproj -c Release -o /app/publish --no-build

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 5000

ENTRYPOINT ["dotnet", "quiz_application.Api.dll"]
