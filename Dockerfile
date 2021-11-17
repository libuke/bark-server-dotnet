#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
COPY ["/src/BarkServerNet/BarkServerNet.csproj", "BarkServerNet/"]
RUN dotnet restore "BarkServerNet/BarkServerNet.csproj"
COPY . .
WORKDIR "/src/BarkServerNet"
RUN dotnet build "BarkServerNet.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BarkServerNet.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BarkServerNet.dll"]
