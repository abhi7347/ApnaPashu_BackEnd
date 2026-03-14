# APNAPASHU Backend - Developer Quick Reference

## 🎯 Checklist: Adding a New API

Use this checklist when adding a new feature/API:

### Phase 1: Data Layer
- [ ] Create DTOs in `APNAPASHU.DataContract/Models/{Feature}/`
  - `Create{Feature}Dto.cs`
  - `Update{Feature}Dto.cs`
  - `{Feature}ResponseDto.cs`
  - `{Feature}ListingResponseDto.cs` (if paginated)

- [ ] Create Repository Interface in `APNAPASHU.RepositoryContract/Web/`
  - `I{Feature}Repository.cs`
  - Define all async methods
  
- [ ] Create Repository Interface in `APNAPASHU.RepositoryContract/Mobile/`
  - `I{Feature}Repository.cs`
  - Can have mobile-specific methods

### Phase 2: Data Access
- [ ] Create Repository Implementation in `APNAPASHU.Repository/Web/`
  - `{Feature}Repository.cs` : `BaseRepository, IRepositoryContract.Web.I{Feature}Repository`
  - Use Dapper with SQL queries
  - Handle exceptions properly
  
- [ ] Create Repository Implementation in `APNAPASHU.Repository/Mobile/`
  - `{Feature}Repository.cs` : `BaseRepository, IRepositoryContract.Mobile.I{Feature}Repository`
  - Mobile-specific queries (e.g., location-based)

### Phase 3: Business Logic
- [ ] Create Service Interface in `APNAPASHU.ServiceContract/Web/`
  - `I{Feature}Service.cs`
  - Return `JsonModel<T>` for all methods
  
- [ ] Create Service Interface in `APNAPASHU.ServiceContract/Mobile/`
  - `I{Feature}Service.cs`
  - Mobile-specific business logic

- [ ] Create Service Implementation in `APNAPASHU.Service/Web/`
  - `{Feature}Service.cs` : `BaseService, IServiceContract.Web.I{Feature}Service`
  - Handle business logic validation
  - Call repository methods
  - Wrap in try-catch with proper error responses
  
- [ ] Create Service Implementation in `APNAPASHU.Service/Mobile/`
  - `{Feature}Service.cs` : `BaseService, IServiceContract.Mobile.I{Feature}Service`

### Phase 4: Presentation Layer
- [ ] Create Controller in `APNAPASHU/Controllers/Web/`
  - `{Feature}Controller.cs` : `BaseController, [Route("api/web/[controller]")]`
  - Add XML documentation comments
  - Use proper HTTP verbs and status codes
  - Add `[HttpGet]`, `[HttpPost]`, etc. attributes
  
- [ ] Create Controller in `APNAPASHU/Controllers/Mobile/`
  - `{Feature}Controller.cs` : `BaseController, [Route("api/mobile/[controller]")]`
  - Mobile-specific endpoints

### Phase 5: Database
- [ ] Add SQL table creation script in `Database/CreateTables.sql`
  - Include all necessary columns
  - Add primary key
  - Add CreatedDate, UpdatedDate, IsActive columns
  - Create indexes for frequently queried columns

### Phase 6: Dependency Injection
- [ ] Register services in `APNAPASHU.ServiceContract/ServiceCollectionExtensionMethod.cs`
  - Add to `AddWebServices()` method
  - Add to `AddMobileServices()` method

## 📋 Code Templates

### Repository Template (Web)
```csharp
using Dapper;
using APNAPASHU.DataContract.Models.{Feature};

namespace APNAPASHU.Repository.Web
{
    public class {Feature}Repository : BaseRepository, RepositoryContract.Web.I{Feature}Repository
    {
        public {Feature}Repository(IConfiguration configuration) : base(configuration) { }

        public async Task<{Feature}ResponseDto> Get{Feature}ByIdAsync(int id)
        {
            try
            {
                string query = @"
                    SELECT * FROM {Features}
                    WHERE Id = @Id AND IsActive = 1";

                using (IDbConnection conn = GetConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Id", id);
                    return await conn.QueryFirstOrDefaultAsync<{Feature}ResponseDto>(query, parameters);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}", ex);
            }
        }
    }
}
```

### Service Template (Web)
```csharp
using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.{Feature};

namespace APNAPASHU.Service.Web
{
    public class {Feature}Service : BaseService, ServiceContract.Web.I{Feature}Service
    {
        private readonly RepositoryContract.Web.I{Feature}Repository _repository;

        public {Feature}Service(RepositoryContract.Web.I{Feature}Repository repository,
            IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
            : base(httpContextAccessor, configuration)
        {
            _repository = repository;
        }

        public async Task<JsonModel<{Feature}ResponseDto>> Get{Feature}ByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return new JsonModel<{Feature}ResponseDto>(null, "Invalid ID", 400, "INVALID_ID");

                var result = await _repository.Get{Feature}ByIdAsync(id);
                if (result == null)
                    return new JsonModel<{Feature}ResponseDto>(null, "Not found", 404, "NOT_FOUND");

                return new JsonModel<{Feature}ResponseDto>(result, "Success", 200);
            }
            catch (Exception ex)
            {
                return new JsonModel<{Feature}ResponseDto>(null, "Error", 500, ex.Message);
            }
        }
    }
}
```

### Controller Template
```csharp
using Microsoft.AspNetCore.Mvc;
using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.{Feature};
using APNAPASHU.ServiceContract.Web;

namespace APNAPASHU.API.Controllers.Web
{
    /// <summary>
    /// {Feature} Management API
    /// </summary>
    [Route("api/web/[controller]")]
    [ApiController]
    public class {Feature}Controller : BaseController
    {
        private readonly I{Feature}Service _{featureLower}Service;
        private readonly ILogger<{Feature}Controller> _logger;

        public {Feature}Controller(I{Feature}Service {featureLower}Service, IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration, ILogger<{Feature}Controller> logger)
            : base(httpContextAccessor, configuration)
        {
            _{featureLower}Service = {featureLower}Service;
            _logger = logger;
        }

        /// <summary>
        /// Get {feature} by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get{Feature}ById(int id)
        {
            _logger.LogInformation($"Getting {feature} with ID: {id}");
            var result = await _{featureLower}Service.Get{Feature}ByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}
```

## 🔗 Dependency Chain

```
Controller
    ↓ (depends on)
Service (Interface: IService)
    ↓
Repository (Interface: IRepository)
    ↓
DataContract Models
    ↓
Database
```

## ✅ Response Examples

### Success Response
```json
{
  "data": { "id": 1, "name": "Example" },
  "message": "Operation successful",
  "statusCode": 200,
  "appError": ""
}
```

### Error Response
```json
{
  "data": null,
  "message": "Animal not found",
  "statusCode": 404,
  "appError": "NOT_FOUND"
}
```

## 🔐 Common Error Codes

| Code | Meaning | HTTP |
|------|---------|------|
| INVALID_ID | ID is null/empty/negative | 400 |
| INVALID_DATA | Input validation failed | 400 |
| NOT_FOUND | Resource doesn't exist | 404 |
| ALREADY_EXISTS | Duplicate record | 400 |
| UNAUTHORIZED | Auth failed | 401 |
| FORBIDDEN | Permission denied | 403 |
| INTERNAL_ERROR | Server error | 500 |
| INVALID_PARAMS | Query params invalid | 400 |

## 📊 Database Naming Convention

**Tables:** PascalCase plural
- Animals
- Categories
- Users
- Orders

**Columns:** PascalCase
- AnimalId (PK)
- CategoryId (FK)
- AnimalName
- CreatedDate
- UpdatedDate
- IsActive

## 🧪 Testing Endpoints

### Web Animal API
```bash
# Get all
curl -X GET "https://localhost:7001/api/web/animal" \
  -H "Content-Type: application/json"

# Get by ID
curl -X GET "https://localhost:7001/api/web/animal/1" \
  -H "Content-Type: application/json"

# Create
curl -X POST "https://localhost:7001/api/web/animal" \
  -H "Content-Type: application/json" \
  -d '{"animalName":"Dog","breed":"Labrador",...}'

# Update
curl -X PUT "https://localhost:7001/api/web/animal/1" \
  -H "Content-Type: application/json" \
  -d '{"animalName":"Updated Dog",...}'

# Delete
curl -X DELETE "https://localhost:7001/api/web/animal/1"
```

### Mobile Animal API
```bash
# Nearby
curl -X GET "https://localhost:7001/api/mobile/animal/nearby?location=Mumbai"

# Category
curl -X GET "https://localhost:7001/api/mobile/animal/category/Pet"

# List
curl -X POST "https://localhost:7001/api/mobile/animal/list" \
  -H "Content-Type: application/json" \
  -d '{"animalName":"Cat","breed":"Persian",...}'
```

## 🐛 Debug Tips

**Enable Detailed Logging:**
Edit `appsettings.Development.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Information"
    }
  }
}
```

**Check Database Connection:**
```csharp
// In Program.cs
var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Connection: {connStr}");
```

**View SQL Queries:**
Use SQL Profiler or enable query logging in Dapper options.

## 📚 File Organization

```
New Feature: Category Management
├── Create{Feature}Dto.cs ......................... DataContract/Models/Category/
├── I{Feature}Repository.cs (Web) ............... RepositoryContract/Web/
├── I{Feature}Repository.cs (Mobile) ........... RepositoryContract/Mobile/
├── {Feature}Repository.cs (Web) ............... Repository/Web/
├── {Feature}Repository.cs (Mobile) ........... Repository/Mobile/
├── I{Feature}Service.cs (Web) ................. ServiceContract/Web/
├── I{Feature}Service.cs (Mobile) ............. ServiceContract/Mobile/
├── {Feature}Service.cs (Web) ................. Service/Web/
├── {Feature}Service.cs (Mobile) ............. Service/Mobile/
├── {Feature}Controller.cs (Web) .............. Controllers/Web/
├── {Feature}Controller.cs (Mobile) .......... Controllers/Mobile/
└── CreateTables.sql ............................ Database/
```

## 🚀 Deployment Checklist

- [ ] Update connection strings for production
- [ ] Update JWT secret key (use Azure Key Vault)
- [ ] Configure CORS for production domains
- [ ] Enable HTTPS only
- [ ] Set up logging pipeline
- [ ] Configure backup strategy for database
- [ ] Set up monitoring/alerts
- [ ] Document API endpoints
- [ ] Create API key for clients
- [ ] Test all endpoints in staging

---

**Happy Coding! 🚀**