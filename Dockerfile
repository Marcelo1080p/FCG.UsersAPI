FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/FCG.UsersAPI.Domain/FCG.UsersAPI.Domain.csproj src/FCG.UsersAPI.Domain/
COPY src/FCG.UsersAPI.Application/FCG.UsersAPI.Application.csproj src/FCG.UsersAPI.Application/
COPY src/FCG.UsersAPI.Infrastructure/FCG.UsersAPI.Infrastructure.csproj src/FCG.UsersAPI.Infrastructure/
COPY src/FCG.UsersAPI.API/FCG.UsersAPI.API.csproj src/FCG.UsersAPI.API/
RUN dotnet restore src/FCG.UsersAPI.API/FCG.UsersAPI.API.csproj

COPY src/ src/
RUN dotnet publish src/FCG.UsersAPI.API/FCG.UsersAPI.API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "FCG.UsersAPI.API.dll"]
