FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /app


COPY ./api .
RUN --mount=type=secret,id=github_token \
    github_token=$(cat /run/secrets/github_token) \
    dotnet nuget add source "https://nuget.pkg.github.com/SlaytonNichols/index.json" --name "github" \
    --username "SlaytonNichols" \
    --store-password-in-clear-text --password $(cat /run/secrets/github_token)
RUN dotnet restore

WORKDIR /app/SlaytonNichols
RUN dotnet publish -c release -o /out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS runtime
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "SlaytonNichols.dll"]
