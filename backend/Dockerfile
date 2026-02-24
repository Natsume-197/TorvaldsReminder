FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app
COPY src/Domain/TorvaldsReminder.Domain.csproj                 src/Domain/
COPY src/Application/TorvaldsReminder.Application.csproj       src/Application/
COPY src/Infrastructure/TorvaldsReminder.Infrastructure.csproj src/Infrastructure/
COPY src/API/TorvaldsReminder.API.csproj                       src/API/
RUN dotnet restore src/API/TorvaldsReminder.API.csproj
COPY . .
RUN dotnet publish src/API/TorvaldsReminder.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "TorvaldsReminder.API.dll"]
