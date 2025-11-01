# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./FoodOrderingService.sln ./
COPY ./src/FoodOrderingService.Core/FoodOrderingService.Core.csproj ./src/FoodOrderingService.Core/
COPY ./src/FoodOrderingService.Gateway/FoodOrderingService.Gateway.csproj ./src/FoodOrderingService.Gateway/
COPY ./src/FoodOrderingService.Workflows/FoodOrderingService.Workflows.csproj ./src/FoodOrderingService.Workflows/
COPY ./src/FoodOrderingService.Infrastructure/FoodOrderingService.Infrastructure.csproj ./src/FoodOrderingService.Infrastructure/
COPY ./src/FoodOrderingService.API/FoodOrderingService.API.csproj ./src/FoodOrderingService.API/

RUN dotnet restore ./FoodOrderingService.sln

COPY ./src ./src
RUN dotnet publish ./src/FoodOrderingService.API/FoodOrderingService.API.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
RUN mkdir -p /app/logs
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "FoodOrderingService.API.dll"]


