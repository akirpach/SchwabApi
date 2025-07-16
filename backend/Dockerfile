FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["SchwabSaaS.csproj", "./"]
RUN dotnet restore "SchwabSaaS.csproj"

COPY . ./
RUN dotnet build "SchwabSaaS.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SchwabSaaS.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment for config loading
ENV ASPNETCORE_ENVIRONMENT=Docker

EXPOSE 80
ENTRYPOINT ["dotnet", "SchwabSaaS.dll"]