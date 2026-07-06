# Shared multi-stage build for every Cartify service.
# docker-compose passes PROJECT (e.g. Cartify.Gateway) per service.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG PROJECT
WORKDIR /src
COPY . .
RUN dotnet restore "$PROJECT/$PROJECT.csproj"
RUN dotnet publish "$PROJECT/$PROJECT.csproj" -c Release -o /app /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
ARG PROJECT
WORKDIR /app
COPY --from=build /app .
ENV APP_DLL="${PROJECT}.dll"
ENV ASPNETCORE_URLS="http://+:8080"
EXPOSE 8080
ENTRYPOINT ["/bin/sh", "-c", "dotnet $APP_DLL"]
