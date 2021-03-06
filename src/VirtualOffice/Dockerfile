#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
RUN dotnet tool install -g Microsoft.Web.LibraryManager.Cli
ENV PATH="$PATH:/root/.dotnet/tools"
WORKDIR /src
COPY ["VirtualOffice/VirtualOffice.csproj", "VirtualOffice/"]
RUN dotnet restore "VirtualOffice/VirtualOffice.csproj"
COPY . .
WORKDIR "/src/VirtualOffice"
RUN libman restore
RUN dotnet build "VirtualOffice.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VirtualOffice.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VirtualOffice.dll"]