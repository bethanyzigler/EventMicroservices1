FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 61236
EXPOSE 44385

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["../OrderAPI/OrderAPI.csproj", "../OrderAPI/"]
RUN dotnet restore "../OrderAPI/OrderAPI.csproj"
COPY . .
WORKDIR "/src/../OrderAPI"
RUN dotnet build "OrderAPI.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "OrderAPI.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "OrderAPI.dll"]

