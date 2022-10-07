# Introduction
This is a simple boilerplate code template when getting started with .NET microservices.
In the code implementation you can find the following features: 

- Standalone JWT Authentication controller
- Ocelot API Gateway
- Sample REST API services
- Repository + UnitOfWork Design pattern implementation with RepoDB and PostgreSQL
- Blazor WASM test application

# Secrets
AuthenticationAPI project make use of local user secrets to connect to the database. Template:
```
{
  "DbSettings": {
    "ConnectionString": "Server=[YOUR_SERVER_NAME];Database=[YOUR_DB_NAME];Port=5432;User Id=[YOUR_USERNAME];Password=[YOUR_PASSWORD];Ssl Mode=Require;Trust Server Certificate=true"
  }
}
```

# Database
If you want to run the solution by yourself, in the *database* folder you will find the schema to create
all the necessary tables. The project is configured to run with PostgreSQL.

Add a new user to your database with the following values:
- username: 'user1'
- password: 'A6xnQhbz4Vx2HuGl4lXwZ5U2I8iziLRFnhP5eNfIRvQ='
- displayname: 'u1'
- isactive: 'true'

The password of that specified hash is -> 1234.