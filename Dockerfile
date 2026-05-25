# Step 1: Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Using JSON array syntax to safely handle the space in the filename
COPY ["Student Portal.sln", "./"]
COPY StudentPortalApi/ ./StudentPortalApi/

# Restore dependencies using the solution file
RUN dotnet restore "Student Portal.sln"

# Build and publish a release from the project folder
RUN dotnet publish StudentPortalApi/StudentPortalApi.csproj -c Release -o out

# Step 2: Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Tell ASP.NET Core to bind to the port dynamically assigned by Render
ENV ASPNETCORE_URLS=http://+:10000

# Start the application using your project's DLL
ENTRYPOINT ["dotnet", "StudentPortalApi.dll"]
