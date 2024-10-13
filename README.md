## Steps to fire up the Shopping cart project

1. Clone the repository
2. Open the solution in Visual Studio
3. Build the solution
4. Use the docker-compose file inside Mango.Services.CouponAPI to run the project
```bash
docker-compose -f docker-compose.yml up -d
```
5. Update the connection string in appsettings.json file
6. Run the migration for each project
```bash
dotnet ef database update -p Mango.Services.CouponAPI.Mango.Services.CouponAPI.csproj 
dotnet ef database update -p Mango.Services.ProductAPI.Mango.Services.ProductAPI.csproj
dotnet ef database update -p Mango.Services.CartAPI.Mango.Services.CartAPI.csproj
dotnet ef database update -p Mango.Services.AuthAPI.Mango.Services.AuthAPI.csproj
dotnet ef database update -p Mango.Services.EmailAPI.Mango.Services.EmailAPI.csproj
```
7. If migration is not available, create a migration
```bash
dotnet ef migrations add InitialCreate -p Mango.Services.CouponAPI.Mango.Services.CouponAPI.csproj -o Data/Migrations
dotnet ef migrations add InitialCreate -p Mango.Services.ProductAPI.Mango.Services.ProductAPI.csproj -o Data/Migrations
dotnet ef migrations add InitialCreate -p Mango.Services.CartAPI.Mango.Services.CartAPI.csproj -o Data/Migrations
dotnet ef migrations add InitialCreate -p Mango.Services.AuthAPI.Mango.Services.AuthAPI.csproj -o Data/Migrations
dotnet ef migrations add InitialCreate -p Mango.Services.EmailAPI.Mango.Services.EmailAPI.csproj -o Data/Migrations
```

8. Run the project
