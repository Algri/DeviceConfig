#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WebApi/IoT.RPiController.WebApi/IoT.RPiController.WebApi.csproj", "WebApi/IoT.RPiController.WebApi/"]
RUN dotnet restore "WebApi/IoT.RPiController.WebApi/IoT.RPiController.WebApi.csproj"
COPY . .
WORKDIR "/src/WebApi/IoT.RPiController.WebApi"
RUN dotnet build "IoT.RPiController.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IoT.RPiController.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IoT.RPiController.WebApi.dll"]