# ==============================================================
#  HappyFurnitureBE – ASP.NET Core 8.0 Backend
# ==============================================================

# ------- Build Stage -------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution & project files first (for layer caching)
COPY global.json ./
COPY HappyFurnitureBE.sln ./

COPY src/HappyFurnitureBE.API/HappyFurnitureBE.API.csproj           src/HappyFurnitureBE.API/
COPY src/HappyFurnitureBE.Application/HappyFurnitureBE.Application.csproj   src/HappyFurnitureBE.Application/
COPY src/HappyFurnitureBE.Domain/HappyFurnitureBE.Domain.csproj             src/HappyFurnitureBE.Domain/
COPY src/HappyFurnitureBE.Infrastructure/HappyFurnitureBE.Infrastructure.csproj src/HappyFurnitureBE.Infrastructure/

# Restore NuGet packages
RUN dotnet restore HappyFurnitureBE.sln

# Copy the rest of the source code
COPY . .

# Publish the API project in Release mode
RUN dotnet publish src/HappyFurnitureBE.API/HappyFurnitureBE.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ------- Runtime Stage -------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published output from build stage
COPY --from=build /app/publish .

# Expose the default ASP.NET Core port
EXPOSE 8080

# Run the application
ENTRYPOINT ["dotnet", "HappyFurnitureBE.API.dll"]
