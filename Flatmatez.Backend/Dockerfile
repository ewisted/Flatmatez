FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["Flatmatez.Backend/Flatmatez.Backend.csproj", "Flatmatez.Backend/"]
RUN dotnet restore "Flatmatez.Backend/Flatmates.Backend.csproj"
COPY . .
WORKDIR "/src/Flatmatez.Backend"
RUN dotnet build "Flatmatez.Backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Flatmatez.Backend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Flatmatez.Backend.dll"]