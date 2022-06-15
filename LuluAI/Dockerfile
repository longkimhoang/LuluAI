#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["LuluAI/LuluAI.csproj", "LuluAI/"]
RUN dotnet restore "LuluAI/LuluAI.csproj"
COPY . .
WORKDIR "/src/LuluAI"
RUN dotnet build "LuluAI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LuluAI.csproj" -c Release -o /app/publish

FROM base AS final
ARG DISCORD_BOT_TOKEN
WORKDIR /app
COPY --from=publish /app/publish .
ENV DISCORD_BOT_TOKEN=$DISCORD_BOT_TOKEN
ENTRYPOINT ["dotnet", "LuluAI.dll", "--Discord:BotToken", $DISCORD_BOT_TOKEN]