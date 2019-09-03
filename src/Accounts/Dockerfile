# Note that we set context from the docker-compose file to allow docker to have a context above the docker file

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY ./Accounts.csproj .
RUN dotnet restore

# copy everything else and build app
COPY . .
WORKDIR /app

# using debug as this means we will use the docker dynamo db instance
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS runtime
WORKDIR /app
COPY --from=build /app/out ./
EXPOSE 5000
ENTRYPOINT ["dotnet", "Accounts.dll"]
