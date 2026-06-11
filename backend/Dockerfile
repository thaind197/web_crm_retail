# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

# Copy solution and project files first to restore dependencies (cache optimization)
COPY SalesCRM.slnx ./
COPY src/SalesCRM.Shared/SalesCRM.Shared.csproj src/SalesCRM.Shared/
COPY src/SalesCRM.Domain/SalesCRM.Domain.csproj src/SalesCRM.Domain/
COPY src/SalesCRM.Application/SalesCRM.Application.csproj src/SalesCRM.Application/
COPY src/SalesCRM.Infrastructure/SalesCRM.Infrastructure.csproj src/SalesCRM.Infrastructure/
COPY src/SalesCRM.WebAPI/SalesCRM.WebAPI.csproj src/SalesCRM.WebAPI/

RUN dotnet restore src/SalesCRM.WebAPI/SalesCRM.WebAPI.csproj

# Copy the rest of the source code
COPY src/ ./src/

# Build and publish release output
RUN dotnet publish src/SalesCRM.WebAPI/SalesCRM.WebAPI.csproj -c Release -o /app/out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build-env /app/out .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "SalesCRM.WebAPI.dll"]
