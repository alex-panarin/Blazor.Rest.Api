#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Blazor.Rest.Api/Blazor.Rest.Api.csproj", "Blazor.Rest.Api/"]
RUN dotnet restore "Blazor.Rest.Api/Blazor.Rest.Api.csproj"
COPY . .
WORKDIR "/src/Blazor.Rest.Api"
RUN dotnet build "Blazor.Rest.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Blazor.Rest.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Blazor.Rest.Api.dll"]
