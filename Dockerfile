FROM mcr.microsoft.com/dotnet/sdk:8.0 as build


WORKDIR build
COPY ./src .
RUN dotnet restore ./Waiter.API/Waiter.API.csproj
RUN dotnet publish -c Release -o out ./Waiter.API/Waiter.API.csproj


FROM mcr.microsoft.com/dotnet/sdk:8.0 as app
WORKDIR app
COPY --from=build /build/out .
ENTRYPOINT ["dotnet", "Waiter.API.dll"]
