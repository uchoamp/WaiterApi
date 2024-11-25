# WaiterApi
The Waiter API is a simple API for restaurant customers to place their orders.



## How To Run Migrations 

### First the environment
```psw
$Env:ASPNETCORE_ENVIRONMENT='Development'
```
### Add Migration

```psw
dotnet ef migrations add InitialCreate --project .\src\Waiter.Infra\Waiter.Infra.csproj  --startup-project .\src\Waiter.API\Waiter.API.csproj
```

### Update Database
```psw
dotnet ef database update --project .\src\Waiter.Infra\Waiter.Infra.csproj  --startup-project .\src\Waiter.API\Waiter.API.csproj
```