FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["apps/Nameless.WebApplication/Nameless.WebApplication.csproj", "apps/Nameless.WebApplication/"]
RUN dotnet restore "apps/Nameless.WebApplication/Nameless.WebApplication.csproj"
COPY . .
WORKDIR "/src/apps/Nameless.WebApplication"
RUN dotnet build "Nameless.WebApplication.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Nameless.WebApplication.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Nameless.WebApplication.dll"]