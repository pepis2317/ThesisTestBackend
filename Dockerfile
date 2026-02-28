FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ThesisTestAPI/ThesisTestAPI.csproj", "ThesisTestAPI/"]
COPY ["ThesisTestAPI.Entities/ThesisTestAPI.Entities.csproj", "ThesisTestAPI.Entities/"]
RUN dotnet restore "ThesisTestAPI/ThesisTestAPI.csproj"
COPY . .
WORKDIR "/src/ThesisTestAPI"
RUN dotnet build "./ThesisTestAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ThesisTestAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ThesisTestAPI.dll"]
