FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 54282
EXPOSE 44319

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["../CartAPI/CartAPI.csproj", "../CartAPI/"]
RUN dotnet restore "../CartAPI/CartAPI.csproj"
COPY . .
WORKDIR "/src/../CartAPI"
RUN dotnet build "CartAPI.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "CartAPI.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "CartAPI.dll"]