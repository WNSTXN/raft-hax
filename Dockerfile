FROM mcr.microsoft.com/dotnet/sdk:6.0

WORKDIR /

COPY . ./

ARG project_name="raft-hax"
RUN dotnet restore ${project_name}
RUN dotnet build ${project_name}
RUN dotnet build submodules/SharpMonoInjectorCore/SharpMonoInjector
