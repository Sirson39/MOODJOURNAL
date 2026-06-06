FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file first to restore dependencies with better layer caching.
COPY Web/MOODJOURNAL.Web/MOODJOURNAL.Web.csproj Web/MOODJOURNAL.Web/
RUN dotnet restore Web/MOODJOURNAL.Web/MOODJOURNAL.Web.csproj

# Copy the remaining source code and publish the web app.
COPY . .
RUN dotnet publish Web/MOODJOURNAL.Web/MOODJOURNAL.Web.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://0.0.0.0:10000
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 10000

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MOODJOURNAL.Web.dll"]
