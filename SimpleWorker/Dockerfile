FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

# Install the Datadog .NET Tracer
RUN mkdir -p /opt/datadog
RUN apt-get update && apt-get install -y curl
RUN curl -LO https://github.com/DataDog/dd-trace-dotnet/releases/download/v2.47.0/datadog-dotnet-apm_2.47.0_amd64.deb \
    && dpkg -i ./datadog-dotnet-apm_2.47.0_amd64.deb \
    && rm ./datadog-dotnet-apm_2.47.0_amd64.deb

# Set Datadog environment variables
ENV CORECLR_ENABLE_PROFILING=1
ENV CORECLR_PROFILER={846F5F1C-F9AE-4B07-969E-05C26BC060D8}
ENV CORECLR_PROFILER_PATH=/opt/datadog/Datadog.Trace.ClrProfiler.Native.so
ENV DD_DOTNET_TRACER_HOME=/opt/datadog
ENV DD_TRACE_ENABLED=true
ENV DD_SERVICE=SimpleWorker
ENV DD_ENV=dev
ENV DD_VERSION=1.0.0
ENV DD_AGENT_HOST=localhost

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SimpleWorker.csproj", "./"]
RUN dotnet restore "SimpleWorker.csproj"
COPY . .
RUN dotnet build "SimpleWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SimpleWorker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimpleWorker.dll"] 