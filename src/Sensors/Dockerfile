FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Sensors.csproj", "./"]
RUN dotnet restore "./Sensors.csproj"
COPY . .
RUN dotnet publish "Sensors.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Sensors.dll"]
