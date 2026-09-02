FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Imparsable.CLI/Imparsable.CLI.csproj", "Imparsable.CLI/"]
COPY ["Imparsable.Lang.Calculator/Imparsable.Lang.Calculator.csproj", "Imparsable.Lang.Calculator/"]
COPY ["Imparsable.Toolchain/Imparsable.Toolchain.csproj", "Imparsable.Toolchain/"]
COPY ["Imparsable.Toolchain.SourceGenerators/Imparsable.Toolchain.SourceGenerators.csproj", "Imparsable.Toolchain.SourceGenerators/"]
COPY ["Directory.Packages.props", "Directory.Packages.props"]
RUN dotnet restore "Imparsable.CLI/Imparsable.CLI.csproj"
COPY . .
WORKDIR "/src/Imparsable.CLI"
RUN dotnet build "./Imparsable.CLI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Imparsable.CLI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Imparsable.CLI.dll"]
