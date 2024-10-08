#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["UserAvatar.Api/UserAvatar.Api.csproj", "UserAvatar.Api/"]
COPY ["UserAvatar.Bll.TaskManager/UserAvatar.Bll.TaskManager.csproj", "UserAvatar.Bll.TaskManager/"]
COPY ["UserAvatar.Bll.Gamification/UserAvatar.Bll.Gamification.csproj", "UserAvatar.Bll.Gamification/"]
COPY ["UserAvatar.Dal/UserAvatar.Dal.csproj", "UserAvatar.Dal/"]
RUN dotnet restore "UserAvatar.Api/UserAvatar.Api.csproj"
COPY . .
WORKDIR "/src/UserAvatar.Api"
RUN dotnet build "UserAvatar.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserAvatar.Api.csproj" -c Release -o  /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserAvatar.Api.dll"]
