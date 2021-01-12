FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS=http://*:5000

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src

COPY ["Pjfm.WebClient/Pjfm.WebClient.csproj", "Pjfm.WebClient/"]
COPY ["Pjfm.Application/Pjfm.Application.csproj", "Pjfm.Application/"]
COPY ["Pjfm.Domain/Pjfm.Domain.csproj", "Pjfm.Domain/"]
COPY ["Pjfm.Infrastructure/Pjfm.Infrastructure.csproj", "Pjfm.Infrastructure/"]

RUN dotnet restore "Pjfm.WebClient/Pjfm.WebClient.csproj"
COPY . .
WORKDIR "/src/Pjfm.WebClient"
RUN dotnet build "Pjfm.WebClient.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Pjfm.WebClient.csproj" -c Release -o /app/publish

FROM base AS runtime
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pjfm.WebClient.dll"]
