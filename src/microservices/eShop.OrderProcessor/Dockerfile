FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src
COPY eShop.OrderProcessor.sln .
COPY OrderProcessor/OrderProcessor.csproj OrderProcessor/OrderProcessor.csproj 

RUN dotnet restore OrderProcessor/OrderProcessor.csproj

COPY OrderProcessor/ OrderProcessor/

WORKDIR /src/OrderProcessor

RUN dotnet publish OrderProcessor.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "OrderProcessor.dll"]