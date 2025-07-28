# Use the official .NET SDK image as the base
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Expose port 80 for the web application
EXPOSE 80

# Set the working directory
WORKDIR /app

# Copy your .NET project files to the container
COPY ASP.net_7_RESTful_API/. .

# Restore NuGet packages and build your application
RUN dotnet build -c Debug -o out

# Specify the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime

# Set the working directory for the runtime image
WORKDIR /app

# Copy the built application from the build image
COPY --from=build /app/out ./

# Specify the entry point for your application
ENTRYPOINT ["dotnet", "ASP.net_7_RESTful_API.dll"]
