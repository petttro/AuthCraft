FROM mcr.microsoft.com/dotnet/sdk:6.0.403-alpine3.16

# this value is now set in AWS console as environment variable
ENV ASPNETCORE_ENVIRONMENT DockerDev

RUN mkdir -p /opt/src /opt/test /mnt/test \
	&& apk add --no-cache parallel

WORKDIR /opt

COPY AuthCraft.sln ./AuthCraft.sln
COPY NuGet.Config ./NuGet.config
COPY src/ ./src/
COPY test/ ./test/
# we need this so that the solution build succeeds

# we need to build the entire solution to make sure that the bamboo job fails if any of the test assemblies contain a compile error
# also removing integration tests so they don't get in the way
RUN dotnet restore && dotnet build --configuration Release

CMD find /opt/test/ -mindepth 1 -maxdepth 1 -type d -name "*.Test" | parallel -j $(grep -c ^processor /proc/cpuinfo) 'cd "{}" && dotnet test -c Release --no-build -r /mnt/test/ --logger "trx;LogFileName=$(basename {}).trx"'
