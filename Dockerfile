# Compilación
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiamos los archivos de proyecto primero para aprovechar
# la caché de Docker y no reinstalar paquetes si no cambian
COPY Core/Core.csproj Core/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY API/API.csproj API/

# Restauramos las dependencias de todos los proyectos
RUN dotnet restore API/API.csproj

# Copiamos el resto del código fuente
COPY Core/ Core/
COPY Infrastructure/ Infrastructure/
COPY API/ API/

# Compilamos y publicamos en modo Release
WORKDIR /src/API
RUN dotnet publish -c Release -o /app/publish

# Imagen final de producción
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copiamos solo el resultado de la compilación
COPY --from=build /app/publish .

# Railway asigna el puerto dinámicamente mediante la variable PORT
ENV ASPNETCORE_URLS=http://+:${PORT}

ENTRYPOINT ["dotnet", "API.dll"]