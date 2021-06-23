FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim

ARG source=./publish/out
EXPOSE 80

# Creates a folder in the container and makes it the woring folder
WORKDIR /app

# Copies application binaries
COPY ${source} .

EXPOSE 80
ENTRYPOINT ["dotnet", "Pjfm.Bff.dll"]