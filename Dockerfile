FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copia apenas os arquivos de projeto para que a camada de restore
# seja reaproveitada enquanto as dependências não mudarem
COPY src/Domain/Domain.csproj src/Domain/
COPY src/Application/Application.csproj src/Application/
COPY src/Infrastructure/Infrastructure.csproj src/Infrastructure/
COPY src/Api/Api.csproj src/Api/

RUN dotnet restore src/Api/Api.csproj

COPY src/ src/

RUN dotnet publish src/Api/Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080

COPY --from=build /app/publish .

# Usuário não privilegiado "app" referenciado pelo UID numérico (APP_UID = 1654),
# o que permite ao Kubernetes validar runAsNonRoot
USER $APP_UID

EXPOSE 8080

ENTRYPOINT ["dotnet", "Api.dll"]
