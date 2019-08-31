FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Flatmatez.Backend/Flatmatez.Backend.csproj", "Flatmatez.Backend/"]
RUN dotnet restore "Flatmatez.Backend/Flatmatez.Backend.csproj"
COPY . .
WORKDIR "/src/Flatmatez.Backend"
RUN dotnet build "Flatmatez.Backend.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Flatmatez.Backend.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Flatmatez.Backend.dll"]