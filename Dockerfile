FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR "/src/Junto"
COPY . .
RUN dotnet build "Junto.Api/Junto.Api.csproj" -c Release -o /app/build
RUN dotnet publish "Junto.Api/Junto.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim
WORKDIR /app
EXPOSE 80
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Junto.Api.dll"]