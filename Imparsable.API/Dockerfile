FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Imparsable.API/Imparsable.API.csproj", "Imparsable.API/"]
COPY ["Imparsable.Lang.Calculator.LSP/Imparsable.Lang.Calculator.LSP.csproj", "Imparsable.Lang.Calculator.LSP/"]
COPY ["Imparsable.Toolchain.LSP/Imparsable.Toolchain.LSP.csproj", "Imparsable.Toolchain.LSP/"]
COPY ["Imparsable.Lang.Calculator/Imparsable.Lang.Calculator.csproj", "Imparsable.Lang.Calculator/"]
COPY ["Imparsable.Toolchain/Imparsable.Toolchain.csproj", "Imparsable.Toolchain/"]
COPY ["Imparsable.Toolchain.SourceGenerators/Imparsable.Toolchain.SourceGenerators.csproj", "Imparsable.Toolchain.SourceGenerators/"]
COPY ["Directory.Packages.props", "Directory.Packages.props"]
RUN dotnet restore "Imparsable.API/Imparsable.API.csproj"
COPY . .
WORKDIR "/src/Imparsable.API"
RUN dotnet build "./Imparsable.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Imparsable.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Imparsable.API.dll"]
