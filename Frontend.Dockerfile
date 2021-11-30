FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /code

COPY ./Pizza.sln .
COPY ./src/Frontend/Frontend.csproj ./src/Frontend/
COPY ./src/Ingredients/Ingredients.csproj ./src/Ingredients/
COPY ./src/JaegerTracing/JaegerTracing.csproj ./src/JaegerTracing/
COPY ./src/Orders/Orders.csproj ./src/Orders/
COPY ./src/Orders.PubSub/Orders.PubSub.csproj ./src/Orders.PubSub/
COPY ./src/Ingredients.Data/Ingredients.Data.csproj ./src/Ingredients.Data/
COPY ./src/ShopConsole/ShopConsole.csproj ./src/ShopConsole/
COPY ./test/Ingredients.Tests/Ingredients.Tests.csproj ./test/Ingredients.Tests/
COPY ./test/GrpcTestHelper/GrpcTestHelper.csproj ./test/GrpcTestHelper/
COPY ./tools/CreateData/CreateData.csproj ./tools/CreateData/

RUN dotnet restore

COPY . .

RUN dotnet build -c Release --no-restore

RUN dotnet publish src/Frontend -c Release --no-build -o /app

FROM mcr.microsoft.com/dotnet/aspnet:6.0

WORKDIR /app

COPY --from=build /app .

ENTRYPOINT [ "./Frontend" ]
