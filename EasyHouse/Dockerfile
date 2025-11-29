FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

COPY EasyHouse/*.csproj ./EasyHouse/
RUN dotnet restore EasyHouse/EasyHouse.csproj

COPY . ./
RUN dotnet publish EasyHouse/EasyHouse.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build-env /app/out .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "EasyHouse.dll"]