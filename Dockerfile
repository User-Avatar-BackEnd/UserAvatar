#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["UserAvatar.API/UserAvatar.API.csproj", "UserAvatar.API/"]
COPY ["UserAvatar.BLL/UserAvatar.BLL.csproj", "UserAvatar.BLL/"]
COPY ["UserAvatar.DAL/UserAvatar.DAL.csproj", "UserAvatar.AL/"]
RUN dotnet restore "UserAvatar.API/UserAvatar.API.csproj"
COPY . .
WORKDIR "/src/UserAvatar.API"
RUN dotnet build "UserAvatar.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserAvatar.API.csproj" -c Release -o  /app/publish 

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserAvatar.API.dll"]