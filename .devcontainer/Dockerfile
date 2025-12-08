FROM mcr.microsoft.com/dotnet/sdk:10.0-noble

RUN apt-get update && apt-get install -y openssh-client git \
    && rm -rf /var/lib/apt/lists/*

# Tools einmalig im Image backen
RUN dotnet tool install --global dotnet-outdated-tool \
    && dotnet tool install --global upgrade-assistant \
    && dotnet nuget locals all --clear