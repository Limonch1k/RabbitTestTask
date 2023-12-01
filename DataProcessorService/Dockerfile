FROM mcr.microsoft.com/dotnet/sdk:8.0 as build

WORKDIR /source

# copy csproj and restore as distinct layers
COPY DataProcessorService.csproj app/
COPY Models.cs app/
COPY Migrations app/
COPY Mapper app/
COPY DbLayer app/
COPY Services app/
COPY Program.cs app/
COPY FileLogger app/


RUN dotnet restore "app/DataProcessorService.csproj"


WORKDIR /source/app
RUN dotnet publish -c release -o /build  /restore





# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=build /build .
RUN mkdir /db
RUN mkdir /app/log

ENTRYPOINT  [ "dotnet", "DataProcessorService.dll", "rabbit", "guest", "guest", "/app/log/customer.log", "/db/ModuleCategory.db" ]
