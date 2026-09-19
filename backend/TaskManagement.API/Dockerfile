FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["TaskManagement.API.csproj", "./"]
RUN dotnet restore "TaskManagement.API.csproj"
COPY . .
RUN dotnet build "TaskManagement.API.csproj" -c Release -o /app/build
RUN dotnet publish "TaskManagement.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "TaskManagement.API.dll"]
