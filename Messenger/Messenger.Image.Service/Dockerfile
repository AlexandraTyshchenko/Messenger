#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8090
EXPOSE 8091
EXPOSE 5010

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Messenger.Image.Service/Messenger.Image.Api.csproj", "Messenger.Image.Service/"]
COPY ["Messenger.Shared/Messenger.Shared.csproj", "Messenger.Shared/"]
RUN dotnet restore "./Messenger.Image.Service/Messenger.Image.Api.csproj"
COPY . .
WORKDIR "/src/Messenger.Image.Service"
RUN dotnet build "./Messenger.Image.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Messenger.Image.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Messenger.Image.Api.dll"]