﻿FROM mcr.microsoft.com/dotnet/sdk:8.0@sha256:35792ea4ad1db051981f62b313f1be3b46b1f45cadbaa3c288cd0d3056eefb83 AS build-env
WORKDIR /Source
EXPOSE 80
EXPOSE 443

COPY ["DriveHubModel/*csproj", "DriveHubModel/"]
COPY ["Admin/*csproj", "Admin/"]
RUN dotnet restore "Admin/Admin.csproj"

COPY ["DriveHubModel/", "DriveHubModel/"]
COPY ["Admin/", "Admin/"]
WORKDIR "/Source/Admin"
RUN dotnet publish "Admin.csproj" -c Release -o /App

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0@sha256:6c4df091e4e531bb93bdbfe7e7f0998e7ced344f54426b7e874116a3dc3233ff
WORKDIR /App
COPY --from=build-env /App .
ENTRYPOINT ["dotnet", "Admin.dll"]