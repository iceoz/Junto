FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Junto/Junto.csproj", "Junto/"]
RUN dotnet restore "Junto/Junto.csproj"
COPY . .
WORKDIR "/src/Junto"
RUN dotnet build "Junto.csproj" -c Release -o /app/build
RUN dotnet publish "Junto.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim
WORKDIR /app
EXPOSE 80
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Junto.dll"]