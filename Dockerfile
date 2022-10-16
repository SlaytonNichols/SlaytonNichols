FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /app

COPY ./api .
RUN dotnet nuget add source "https://nuget.pkg.github.com/SlaytonNichols/index.json" --name "release-github" --username "SlaytonNichols" --store-password-in-clear-text --password "${PAT}"
# CMD ["sh", "-c", "dotnet nuget add source 'https://nuget.pkg.github.com/SlaytonNichols/index.json' --name 'release-github' --username 'SlaytonNichols' --store-password-in-clear-text --password ${PAT}"]
RUN dotnet restore

WORKDIR /app/SlaytonNichols
RUN dotnet publish -c release -o /out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS runtime
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "SlaytonNichols.dll"]
