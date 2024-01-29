# Stage 1: Build using the .NET 7.0 SDK
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS builder
WORKDIR /usr/src/comet/

# Set argument defaults
ENV COMET_BUILD_CONFIG "release"
ARG COMET_BUILD_CONFIG=$COMET_BUILD_CONFIG

# Copy and build servers and dependencies
COPY . ./
RUN dotnet restore
RUN dotnet publish ./src/Comet.Account -c $COMET_BUILD_CONFIG -o out/Comet.Account
RUN dotnet publish ./src/Comet.Game -c $COMET_BUILD_CONFIG -o out/Comet.Game

# Stage 2: Setup the runtime image
# Use the ASP.NET Core runtime image for applications that use ASP.NET Core
FROM mcr.microsoft.com/dotnet/aspnet:7.0

WORKDIR /usr/bin/comet/
COPY --from=builder /usr/src/comet/out .

# Copy the ini and map directories
# Adjust the paths according to where these directories are located in your source code
COPY ./src/Comet.Game/ini /usr/bin/comet/ini
COPY ./src/Comet.Game/map /usr/bin/comet/map

# Copy the wait-for-it script and give it execute permissions
COPY wait-for-it.sh /wait-for-it.sh
RUN chmod +x /wait-for-it.sh

# Set the entrypoint to the wait-for-it script
ENTRYPOINT ["/wait-for-it.sh"]
