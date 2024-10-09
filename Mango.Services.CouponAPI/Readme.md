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
```
7. Run the project