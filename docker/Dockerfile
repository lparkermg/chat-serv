FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS dependencies

WORKDIR /build

COPY ./src/ChatServ/ChatServ.Api/ChatServ.Api.csproj ./ChatServ.Api/ChatServ.Api.csproj
COPY ./src/ChatServ/ChatServ.Core/ChatServ.Core.csproj ./ChatServ.Core/ChatServ.Core.csproj
COPY ./src/ChatServ/ChatServ.sln ./ChatServ.sln

RUN dotnet restore

FROM dependencies AS build

COPY ./src/ChatServ .
RUN dotnet publish ./ChatServ.Api/ChatServ.Api.csproj -c Release -o /build/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final

RUN apk update
RUN apk add icu-libs

COPY --from=build /build/out .

ENTRYPOINT ["dotnet", "ChatServ.Api.dll"]

