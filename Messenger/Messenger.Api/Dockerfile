#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 7000
EXPOSE 7001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Messenger.Api/Messenger.Api.csproj", "Messenger.Api/"]
COPY ["Messenger.Business/Messenger.Business.csproj", "Messenger.Business/"]
COPY ["Messenger.Client/Messanger.Image.Client.csproj", "Messenger.Client/"]
COPY ["Messenger.Shared/Messenger.Shared.csproj", "Messenger.Shared/"]
COPY ["Messenger.Infrastructure/Messenger.Infrastructure.csproj", "Messenger.Infrastructure/"]
RUN dotnet restore "./Messenger.Api/Messenger.Api.csproj"
COPY . .
WORKDIR "/src/Messenger.Api"
RUN dotnet build "./Messenger.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build


FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Messenger.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Messenger.Api.dll"]