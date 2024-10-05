# 使用官方的 .NET 8.0 SDK 镜像进行构建
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# 使用 .NET 8.0 SDK 镜像进行构建和发布
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY ["ParkingManagementAPI.csproj", "."]
RUN dotnet restore "ParkingManagementAPI.csproj"
COPY . .
RUN dotnet build "ParkingManagementAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ParkingManagementAPI.csproj" -c Release -o /app/publish

# 将发布的应用复制到基础镜像并运行
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ParkingManagementAPI.dll"]
