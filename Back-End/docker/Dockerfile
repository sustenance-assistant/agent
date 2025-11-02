# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./BackEndService.sln ./
COPY ./src/BackEndService.Core/FoodOrderingService.Core.csproj ./src/BackEndService.Core/
COPY ./src/BackEndService.Gateway/FoodOrderingService.Gateway.csproj ./src/BackEndService.Gateway/
COPY ./src/BackEndService.Workflows/FoodOrderingService.Workflows.csproj ./src/BackEndService.Workflows/
COPY ./src/BackEndService.Infrastructure/FoodOrderingService.Infrastructure.csproj ./src/BackEndService.Infrastructure/
COPY ./src/BackEndService.API/FoodOrderingService.API.csproj ./src/BackEndService.API/

RUN dotnet restore ./BackEndService.sln

COPY ./src ./src
RUN dotnet publish ./src/BackEndService.API/FoodOrderingService.API.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
RUN mkdir -p /app/logs
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "FoodOrderingService.API.dll"]


