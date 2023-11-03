#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM base as kostil
WORKDIR /app
COPY Roboto-Regular.ttf /app/Roboto-Regular.ttf

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ServerViewYa/ServerViewYa.csproj", "ServerViewYa/"]
RUN dotnet restore "ServerViewYa/ServerViewYa.csproj"
COPY . .
WORKDIR "/src/ServerViewYa"
RUN dotnet build "ServerViewYa.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ServerViewYa.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ServerViewYa.dll"]