# ---------- Etapa 1: compilación (imagen SDK) ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restaurar primero solo con el .csproj aprovecha la caché de capas de Docker
COPY TaskBoard.Api.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o /app/publish --no-restore /p:UseAppHost=false

# ---------- Etapa 2: ejecución (imagen runtime, más liviana) ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# curl se usa para el healthcheck del contenedor
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/publish ./

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

# Usuario sin privilegios incluido en las imágenes oficiales de .NET 8
USER $APP_UID

ENTRYPOINT ["dotnet", "TaskBoard.Api.dll"]
