FROM mcr.microsoft.com/dotnet/sdk:8.0 as build

WORKDIR /source

# copy csproj and restore as distinct layers
COPY FileParserService.csproj app/
COPY Models app/
COPY Services app/
COPY Program.cs app/
COPY FileLogger app/


RUN dotnet restore "app/FileParserService.csproj"

WORKDIR /source/app
RUN dotnet clean
RUN dotnet publish -c release -o /build  /restore





# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=build /build .
RUN mkdir /db
RUN mkdir /app/log


ENTRYPOINT  [ "dotnet", "FileParserService.dll", "/app/xml_file", "rabbit", "guest", "guest", "/app/log/producer.log" ]
