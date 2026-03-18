FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY SmartStock.slnx ./
COPY src/ ./src/

RUN dotnet restore ./SmartStock.slnx
RUN dotnet publish ./src/SmartStock.WebAPI/SmartStock.WebAPI.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish ./
EXPOSE 8080
ENTRYPOINT ["dotnet", "SmartStock.WebAPI.dll"]

