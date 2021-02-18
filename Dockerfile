FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS=http://*:5000

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src

COPY ["src/Pjfm.Api/Pjfm.Api.csproj", "Pjfm.Api/"]
COPY ["src/Pjfm.Application/Pjfm.Application.csproj", "Pjfm.Application/"]
COPY ["src/Pjfm.Domain/Pjfm.Domain.csproj", "Pjfm.Domain/"]
COPY ["src/Pjfm.Infrastructure/Pjfm.Infrastructure.csproj", "Pjfm.Infrastructure/"]

RUN dotnet restore "Pjfm.Api/Pjfm.Api.csproj"
COPY . .
WORKDIR "/src/Pjfm.Api"
RUN dotnet build "Pjfm.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Pjfm.Api.csproj" -c Release -o /app/publish

FROM base AS runtime
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pjfm.Api.dll"]
