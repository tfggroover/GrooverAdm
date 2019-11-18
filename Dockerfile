FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src
COPY ["GrooverAdmSPA/GrooverAdmSPA.csproj", "GrooverAdmSPA/"]
# Setup NodeJs
RUN apt-get update && \
    apt-get install -y wget && \
    apt-get install -y gnupg2 && \
    wget -qO- https://deb.nodesource.com/setup_10.x | bash - && \
    apt-get install -y build-essential nodejs
RUN dotnet restore "GrooverAdmSPA/GrooverAdmSPA.csproj"
COPY . .
WORKDIR "/src/GrooverAdmSPA"
RUN dotnet build "GrooverAdmSPA.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GrooverAdmSPA.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Setup NodeJs
RUN apt-get update && \
    apt-get install -y wget && \
    apt-get install -y gnupg2 && \
    wget -qO- https://deb.nodesource.com/setup_10.x | bash - && \
    apt-get install -y build-essential nodejs
ENV GOOGLE_APPLICATION_CREDENTIALS="./serviceKey.json"
ENTRYPOINT ["dotnet", "GrooverAdmSPA.dll"]
