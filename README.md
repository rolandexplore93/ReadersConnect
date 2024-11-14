# ReadersConnect API
ReadersConnect API is a .NET Core Web API designed for managing a library system for Educational Development Trust . This API enables users to perform various operations, such as registering as a user, borrowing/requesting a book, managing user roles, and more. The API includes role-based authorization to secure access to specific endpoints.

# Prerequisites
- Visual Studio 2022 Editor
- .NET SDK: .NET 8 SDK or later
- SQL Server: Database for storing application data
- Entity Framework Core: Used for database migrations and data access
- Git: For cloning the repository

# Technologies & Architecture
- C#
- .NET 8
- Entity Framework
- Clean Architecture
- Repository Patterns
- SwaggerUI for testing
- Serilog library for logs tracking
- 

# Getting Started
1. Clone this Repository
- Choose the Directory to clone to on your computer and clone this project Repository using the command below
- git clone https://github.com/rolandexplore93/ReadersConnect.git
- Open Visual Studio 2022, click on open solution, location the directory and open ReadersConnect.sln

# Configure Database and App Settings
The project uses appsettings.json and appsettings.secrets.json for configuration. Update these files with your database connection string and JWT secret. appsettings.json already came with the cloning. So create secrets folder at the root of ReadersConnect.Web and add appsettings.secrets.json inside the folder (ReadersConnect.Web/secrets/appsettings.secrets)

##### appsettings.json
```

{
  "ConnectionStrings": {
    "DefaultConnection": "Your SQL Server Connection String Here"
  },
  "AppSettings": {
    "Secret": "YourJWTSecretKeyHere"
  }
}
```

#### appsettings.secrets.json
```

{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    },
    "ApplicationInsights": {
      "LogLevel": {
        "Default": "Information"
      }
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "defaultConnection": "Server= SQL server string, Database=GiveANameToTheDatabase; UserId=sa;Password=Password;Encrypt=True;TrustServerCertificate=True;"
  },
  "SuperAdmin": {
    "FirstName": "ReadersConnect",
    "LastName": "SuperAdmin",
    "UserName": "staff.ReadersConnect",
    "Email": "ReadersConnect@gmail.com",
    "Password": "Admin@123*"
  },
  "JWTsettings": {
    "Secret": "dotNetRollyJWT200dotNetRollyJWT200dotNetRollyJWT200",
    "ValidIssuer": "https://readersconnect-api.com",
    "ValidAudience": "https://test-readersconnect-api.com"
  }
}
```
NOTE: Note: When you run first this application, it will create a SuperAdmin user with  "UserName": "staff.ReadersConnect" and "Password": "Admin@123*" for you to test the APIs.

# Setup Database
- Ensure your SQL Server is running and accessible.
- Open package console manager on Visual studio and Apply migrations to set up the initial database schema: `update-database`
- Ensure you run the `update-database` from the Infrastructure project

  ![image](https://github.com/user-attachments/assets/fef4ca2b-c636-4277-84cc-6e6581b5d347)


# Seed Initial Data into Database
I have added a seed file to populate the database with initial data, including roles, users, and books. Locate the SeedDataIntoDb class in the ReadersConnect.Infrastructure/DbInitializer. The call to seed the data into database  at application startup has been added to program.cs. 
![image](https://github.com/user-attachments/assets/2699beb2-1ea2-4c63-ab29-c4df30c9e901)


Please comment out the data seeding call once your data has been added to the database. (i.e, // var seedDataIntoDB = scope.ServeProvider.GetRequiredService<SeedDataIntoDb>() and // await seedDataIntoDB.SeedAsync() as shown below)
![image](https://github.com/user-attachments/assets/8433c007-867d-4422-8bbe-855eff4c8491)

## start the application
The API should be running on https://localhost:7002/index.html or http://localhost:5005/index.html by default.
![image](https://github.com/user-attachments/assets/96f0f5f2-f3fe-464a-a021-f45ec3321bdd)

## API Testing via Swagger
The API includes Swagger for easy testing of endpoints. Open a browser and navigate to https://localhost:7002/ to access Swagger UI. Use the Swagger interface to explore available endpoints and test them.
![image](https://github.com/user-attachments/assets/76827ecc-dc10-4b94-b52f-ad4277b96878)

