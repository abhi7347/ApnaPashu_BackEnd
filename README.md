# APNAPASHU Backend

A complete, enterprise-grade backend API for the APNAPASHU platform built with **.NET 8.0** following **layered architecture** with **Web/Mobile segregation**.

## 🚀 Features

✅ **Layered Architecture** - Clean separation of concerns  
✅ **Web/Mobile Segregation** - Separate APIs for different clients  
✅ **Global Exception Handling** - Centralized error management  
✅ **Middleware Stack** - Error handling, logging, CORS  
✅ **JWT Authentication** - Secure API access  
✅ **Swagger Documentation** - Auto-generated API docs  
✅ **Dependency Injection** - Full DI setup  
✅ **Soft Deletes** - Audit trail maintenance  
✅ **Pagination Support** - Efficient data loading  
✅ **Dapper ORM** - Fast database queries  

## 📁 Project Structure

```
APNAPASHU_Backend/
├── APNAPASHU/                      (API Layer - Controllers, Filters, Middleware)
│   ├── Controllers/
│   │   ├── Web/                    (Web endpoints)
│   │   │   └── AnimalController.cs ✅
│   │   └── Mobile/                 (Mobile endpoints)
│   │       └── AnimalController.cs ✅
│   ├── Filters/
│   │   └── ApiExceptionFilterAttribute.cs ✅
│   ├── Middlewares/
│   │   └── ErrorHandlingMiddleware.cs ✅
│   ├── Extensions/
│   ├── Properties/
│   └── Program.cs ✅
│
├── APNAPASHU.Service/              (Business Logic Layer)
│   ├── BaseService.cs ✅
│   ├── Web/
│   │   └── AnimalService.cs ✅
│   └── Mobile/
│       └── AnimalService.cs ✅
│
├── APNAPASHU.ServiceContract/      (Service Interfaces)
│   ├── ServiceCollectionExtensionMethod.cs ✅
│   ├── Web/
│   │   └── IAnimalService.cs ✅
│   └── Mobile/
│       └── IAnimalService.cs ✅
│
├── APNAPASHU.Repository/           (Data Access Layer)
│   ├── BaseRepository.cs ✅
│   ├── Web/
│   │   └── AnimalRepository.cs ✅
│   └── Mobile/
│       └── AnimalRepository.cs ✅
│
├── APNAPASHU.RepositoryContract/   (Repository Interfaces)
│   ├── Web/
│   │   └── IAnimalRepository.cs ✅
│   └── Mobile/
│       └── IAnimalRepository.cs ✅
│
├── APNAPASHU.DataContract/         (Models & DTOs)
│   ├── Models/
│   │   ├── CommonModels.cs ✅
│   │   └── Animal/
│   │       └── AnimalModels.cs ✅
│   ├── Enums/
│   └── CustomRegularExpression/
│
├── APNAPASHU.Common/               (Shared Utilities)
│   ├── CommonFunctions.cs
│   ├── EncryptionDecryption.cs
│   ├── JwtSetting.cs
│   ├── Constants/
│   │   └── ApplicationConstants.cs ✅
│   ├── Exceptions/
│   │   └── CustomException.cs ✅
│   └── Messages/
│       └── ResponseMessages.cs ✅
│
├── Database/
│   └── CreateTables.sql ✅
│
├── API_IMPLEMENTATION_GUIDE.md ✅
├── appsettings.json ✅
├── appsettings.Development.json ✅
└── APNAPASHU.sln

✅ = Fully Implemented with Reference Example
```

## 🎯 Complete Implementation Example: Animal API

The **Animal API** is fully implemented end-to-end serving as a reference for other APIs:

### Web Endpoints: `/api/web/animal`
```
GET     /                    - Get all animals (paginated)
GET     /{id}                - Get animal by ID
GET     /search?term=xxx     - Search animals
POST    /                    - Create animal
PUT     /{id}                - Update animal
DELETE  /{id}                - Delete animal
```

### Mobile Endpoints: `/api/mobile/animal`
```
GET     /{id}                - Get animal details
GET     /nearby?location=xxx - Get nearby animals
GET     /category/{cat}      - Get by category
POST    /list                - List animal
PUT     /update/{id}         - Update listing
```

## 🔧 Quick Start

### 1. Prerequisites
- .NET 8.0 SDK
- SQL Server 2019+
- Visual Studio 2022+ or VS Code

### 2. Setup Database
```bash
# Create database and tables
sqlcmd -S localhost -E -i Database/CreateTables.sql
```

### 3. Configure Connection String
Edit `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=APNAPASHU;User Id=sa;Password=YourPassword;Encrypt=false;"
  },
  "JwtSettings": {
    "Secret": "your-256-bit-secret-key-min-32-char!!!",
    "Issuer": "APNAPASHU",
    "Audience": "APNAPASHU_Users",
    "ExpiryMinutes": 60
  }
}
```

### 4. Build & Run
```bash
cd APNAPASHU
dotnet build
dotnet run
```

### 5. Test with Swagger
Navigate to: `https://localhost:7001/swagger`

## 📝 Adding a New API

Follow the **Animal API** as your template. Complete step-by-step guide available in:
📖 **[API_IMPLEMENTATION_GUIDE.md](API_IMPLEMENTATION_GUIDE.md)**

Quick pattern:
1. Create DTOs in `APNAPASHU.DataContract/Models/`
2. Create Repository Interface in `APNAPASHU.RepositoryContract/Web/`
3. Create Repository Implementation in `APNAPASHU.Repository/Web/`
4. Create Service Interface in `APNAPASHU.ServiceContract/Web/`
5. Create Service Implementation in `APNAPASHU.Service/Web/`
6. Create Controller in `APNAPASHU/Controllers/Web/`
7. Register DI in `ServiceCollectionExtensionMethod.cs`
8. Create database table in `Database/CreateTables.sql`

## 🛡️ Security Features

- **Global Exception Handling** - Prevents sensitive data leakage
- **JWT Authentication** - Bearer token validation
- **CORS Protection** - Configurable origin policies
- **Input Validation** - FluentValidation integration
- **Soft Deletes** - Audit trail preservation
- **SQL Injection Prevention** - Parameterized queries with Dapper

## 🔌 Global Middleware Pipeline

```
Request
  ↓
Error Handling Middleware
  ↓
CORS Middleware
  ↓
Authentication Middleware
  ↓
Authorization Middleware
  ↓
Controllers & Filters (Exception Filter)
  ↓
Response
```

## 📦 NuGet Packages (.NET 8.0 Latest)

- **Dapper** 2.1.15 - ORM
- **FluentValidation** 11.8.1 - Data validation
- **JWT Bearer** 8.0.0 - Authentication
- **Swagger** 6.8.0 - API documentation
- **ClosedXML** 0.102.0 - Excel support
- **MailKit** 4.3.0 - Email support

## 📊 Response Format

All APIs return standardized responses:

```json
{
  "data": { /* Payload */ },
  "message": "Human-readable message",
  "statusCode": 200,
  "appError": "Technical error code",
  "accessToken": null
}
```

## 🚦 HTTP Status Codes

- **200** OK - Successful GET/PUT
- **201** Created - Successful POST
- **400** Bad Request - Validation error
- **401** Unauthorized - Authentication failed
- **404** Not Found - Resource not found
- **500** Internal Error - Server error

## 📚 Built With

- **.NET 8.0** LTS - Latest stable framework
- **ASP.NET Core 8.0** - High-performance web framework
- **Entity Framework / Dapper** - Data access
- **Dependency Injection** - IoC container
- **Swagger OpenAPI** - API documentation
- **Microsoft.Extensions** - Configuration, Logging

## 🤝 Contributing

When adding new APIs:
1. Follow the Animal API pattern exactly
2. Implement both Web and Mobile segregation
3. Add proper error handling and validation
4. Include XML documentation comments
5. Update API_IMPLEMENTATION_GUIDE.md

## ⚙️ Configuration Files

- `appsettings.json` - Production settings
- `appsettings.Development.json` - Development settings
- `launchSettings.json` - Launch profiles

## 🐛 Troubleshooting

**Database Connection Issues:**
- Verify SQL Server is running
- Check connection string syntax
- Ensure user has proper permissions

**JWT Errors:**
- Verify secret key length (min 32 chars)
- Check issuer and audience match

**CORS Errors:**
- Add origin to AllowSpecific policy in Program.cs
- Verify credentials setting

## 📖 Documentation

- [API Implementation Guide](API_IMPLEMENTATION_GUIDE.md) - Add new APIs
- Swagger Docs - Auto-generated at `/swagger`
- XML Comments - Inline code documentation

## 📞 Support

For questions on implementation patterns, refer to the Animal API implementation in the project.

---

**Built with ❤️ for APNAPASHU Platform**