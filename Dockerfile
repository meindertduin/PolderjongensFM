FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS=http://*:5000

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /sln


COPY ["./pjfm.sln", "./"]
COPY ["./src/Pjfm.Api/Pjfm.Api.csproj", "./src/Pjfm.Api/"]
COPY ["./src/Pjfm.Application/Pjfm.Application.csproj", "./src/Pjfm.Application/"]
COPY ["./src/Pjfm.Domain/Pjfm.Domain.csproj", "./src/Pjfm.Domain/"]
COPY ["./src/Pjfm.Infrastructure/Pjfm.Infrastructure.csproj", "./src/Pjfm.Infrastructure/"]
COPY ["./tests/Pjfm.WebClient.UnitTests/Pjfm.WebClient.UnitTests.csproj", "./tests/Pjfm.WebClient.UnitTests/"]

RUN echo ls ./tests/Pjfm.WebClient.UnitTests/

RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./src/Pjfm.Api/Pjfm.Api.csproj" -c Release -o /app/publish

FROM base AS runtime
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pjfm.Api.dll"]
