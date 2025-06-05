FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY CarInsuranceSales.sln .
COPY v3.csproj .
RUN dotnet restore CarInsuranceSales.sln

COPY InsurancePolicy.cs .
COPY MindeeService.cs .
COPY Program.cs .
COPY UserSession.cs .
COPY Utilities.cs .
COPY BotHandlers.cs .
COPY appsettings.json .

RUN dotnet publish v3.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 80

ENTRYPOINT ["dotnet", "v3.dll"]


