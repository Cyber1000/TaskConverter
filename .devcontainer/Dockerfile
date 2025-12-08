FROM mcr.microsoft.com/dotnet/sdk:10.0-noble

# Tools einmalig im Image backen
RUN dotnet tool install --global dotnet-outdated-tool \
    && dotnet tool install --global upgrade-assistant \
    && dotnet nuget locals all --clear