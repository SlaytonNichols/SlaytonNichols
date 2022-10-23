FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /app

# Please select the corresponding download for your Linux containers https://github.com/DataDog/dd-trace-dotnet/releases/latest

# Download and install the Tracer
RUN mkdir -p /opt/datadog \
    && mkdir -p /var/log/datadog \
    && TRACER_VERSION=$(curl -s https://api.github.com/repos/DataDog/dd-trace-dotnet/releases/latest | grep tag_name | cut -d '"' -f 4 | cut -c2-) \
    && curl -LO https://github.com/DataDog/dd-trace-dotnet/releases/download/v${TRACER_VERSION}/datadog-dotnet-apm_${TRACER_VERSION}_amd64.deb \
    && dpkg -i ./datadog-dotnet-apm_${TRACER_VERSION}_amd64.deb \
    && rm ./datadog-dotnet-apm_${TRACER_VERSION}_amd64.deb

# Enable the tracer
ENV CORECLR_ENABLE_PROFILING=1
ENV CORECLR_PROFILER={846F5F1C-F9AE-4B07-969E-05C26BC060D8}
ENV CORECLR_PROFILER_PATH=/opt/datadog/Datadog.Trace.ClrProfiler.Native.so
ENV DD_DOTNET_TRACER_HOME=/opt/datadog
ENV DD_INTEGRATIONS=/opt/datadog/integrations.json

COPY ./api .
RUN --mount=type=secret,id=github_token \
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
