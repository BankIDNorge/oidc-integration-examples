# sets up the base image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 as base
WORKDIR /app 
EXPOSE 80

#creates the build image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /example-aspnet
COPY . ./

COPY /app.csproj ./
RUN dotnet restore "app.csproj"
RUN dotnet build "app.csproj" -c Release -o /app/build

# publishes the app
FROM build as publish
RUN dotnet publish "app.csproj" -c Release -o /app/publish

# sets up the final image and copies the app
FROM base as final
WORKDIR /app
COPY --from=publish /app/publish .

# runs the app
ENTRYPOINT ["dotnet", "app.dll"]