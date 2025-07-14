FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["SampleStartUp.csproj", "./"]
RUN dotnet restore "SampleStartUp.csproj"

COPY . ./
RUN dotnet build "SampleStartUp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SampleStartUp.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 80
ENTRYPOINT ["dotnet", "SampleStartUp.dll"]