FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy only the sample subtree from repo root context.
COPY sample/ ./sample/

WORKDIR /src/sample
RUN dotnet restore src/SampleApi/SampleApi.csproj --locked-mode
RUN dotnet publish src/SampleApi/SampleApi.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "SampleApi.dll"]
