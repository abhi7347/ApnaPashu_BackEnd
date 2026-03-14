# APNAPASHU API - Architecture & CRUD Implementation Guide

## 📋 Project Status
✅ **Build Status**: Successfully built (Release mode)
✅ **Architecture**: 7-Layer Layered Architecture implemented
✅ **CRUD Examples**: Complete implementations for Web & Mobile clients

---

## 🏗️ System Architecture

### Layered Structure
```
┌─────────────────────────────────────────────────┐
│  1. APNAPASHU.API (Web Layer)                   │
│     ├── Controllers/Web/                        │
│     ├── Controllers/Mobile/                     │
│     ├── Filters/ (Global Exception Handling)    │
│     ├── Middlewares/ (Error Handling, Auth)     │
│     └── Program.cs (DI Registration)            │
├─────────────────────────────────────────────────┤
│  2. APNAPASHU.ServiceContract (Interface Layer) │
│     ├── Web/IAnimalService, ICategoryService    │
│     └── Mobile/IAnimalService                   │
├─────────────────────────────────────────────────┤
│  3. APNAPASHU.Service (Business Logic Layer)    │
│     ├── Web/AnimalService, CategoryService      │
│     └── Mobile/AnimalService                    │
├─────────────────────────────────────────────────┤
│  4. APNAPASHU.RepositoryContract (Interface)    │
│     ├── Web/IAnimalRepository, ICategoryRepo    │
│     └── Mobile/IAnimalRepository                │
├─────────────────────────────────────────────────┤
│  5. APNAPASHU.Repository (Data Access Layer)    │
│     ├── Web/AnimalRepository, CategoryRepo      │
│     ├── Mobile/AnimalRepository                 │
│     └── BaseRepository (Connection Management)  │
├─────────────────────────────────────────────────┤
│  6. APNAPASHU.DataContract (Models & DTOs)      │
│     ├── Models/Animal/                          │
│     ├── Models/Category/                        │
│     ├── Enums/StatusEnum                        │
│     └── CommonModels.cs (JsonModel<T>)          │
├─────────────────────────────────────────────────┤
│  7. APNAPASHU.Common (Utilities & Helpers)      │
│     ├── Exceptions/CustomException              │
│     ├── Constants/ApplicationConstants          │
│     └── Messages/ResponseMessages               │
└─────────────────────────────────────────────────┘
```

### Request/Response Flow
```
Client Request
    ↓
Controller (Route validation)
    ↓
Service (Business Logic & Validation)
    ↓
Repository (Database Query)
    ↓
Database (SQL Server)
    ↓
Repository → Service → Controller
    ↓
JsonModel<T> Response
    ↓
Client Response
```

---

## 📱 Web vs Mobile Segregation

| Aspect | Web (/api/web/) | Mobile (/api/mobile/) |
|--------|-----|-------|
| **Base Route** | `/api/web/` | `/api/mobile/` |
| **Use Case** | Admin/Management interfaces | Mobile app users |
| **Queries** | Full dataset with search | Location-based, filtered |
| **Real Examples** | Category CRUD | Animal Listings, Location Search |
| **Response Detail** | All fields, admin controls | Essential fields only |

---

## 🔧 Examples Implemented

### 1. **Mobile CRUD API - Animal Listing** ✅ (Already Exists)
**Route**: `api/mobile/animal/*`

#### Endpoints:
- **GET** `/api/mobile/animal/{id}` - Get animal details by ID
- **GET** `/api/mobile/animal/nearby?location=...` - Find nearby animals
- **GET** `/api/mobile/animal/category/{category}` - Animals by category
- **POST** `/api/mobile/animal` - List new animal (Create)
- **PUT** `/api/mobile/animal` - Update animal listing

**Use Case**: Mobile users browsing and listing animals

---

### 2. **Web CRUD API - Category Management** ✅ (NEW)
**Route**: `api/web/category/*`

#### Endpoints:

##### **GET - Retrieve**
```
GET /api/web/category/{id}
Response: 
{
  "success": true,
  "data": {
    "categoryId": 1,
    "categoryName": "Dogs",
    "description": "All dog breeds",
    "iconUrl": "https://...",
    "createdDate": "2026-03-14T10:30:00Z",
    "updatedDate": "2026-03-14T10:30:00Z",
    "isActive": true
  },
  "message": "Category retrieved successfully",
  "statusCode": 200
}
```

##### **GET - List All (with pagination)**
```
GET /api/web/category?pageNumber=1&pageSize=10
Response:
{
  "success": true,
  "data": {
    "categories": [/* list */],
    "totalRecords": 45,
    "pageNumber": 1,
    "pageSize": 10
  },
  "message": "Categories retrieved successfully",
  "statusCode": 200
}
```

##### **GET - Search**
```
GET /api/web/category/search?searchTerm=dog
Response: List of matching categories
```

##### **POST - Create**
```
POST /api/web/category
Content-Type: application/json

{
  "categoryName": "Dogs",
  "description": "Domestic and wild dogs",
  "iconUrl": "https://example.com/dog-icon.png"
}

Response: 201 Created with full category object
```

##### **PUT - Update**
```
PUT /api/web/category
Content-Type: application/json

{
  "categoryId": 1,
  "categoryName": "Dogs - Updated",
  "description": "Updated description",
  "iconUrl": "https://..."
}

Response: 200 OK
```

##### **DELETE - Delete (Soft Delete)**
```
DELETE /api/web/category/{id}
Response: 200 OK { "success": true, "data": true }
```

---

## 📦 Database Schema

### Categories Table
```sql
CREATE TABLE Categories (
    CategoryId INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    IconUrl NVARCHAR(MAX),
    CreatedDate DATETIME DEFAULT GETDATE(),
    UpdatedDate DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1
);

CREATE INDEX IX_Categories_IsActive ON Categories(IsActive);
CREATE INDEX IX_Categories_Name ON Categories(CategoryName);
```

### Animals Table (Existing)
```sql
CREATE TABLE Animals (
    AnimalId INT PRIMARY KEY IDENTITY(1,1),
    AnimalName NVARCHAR(100) NOT NULL,
    Breed NVARCHAR(100),
    Category NVARCHAR(50),
    Age INT,
    Description NVARCHAR(MAX),
    Price DECIMAL(10, 2),
    Location NVARCHAR(200),
    ContactNumber NVARCHAR(20),
    CreatedDate DATETIME DEFAULT GETDATE(),
    UpdatedDate DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1
);
```

---

## 🛠️ How to Add More CRUD APIs

### Step 1: Create Data Models (DataContract)
```csharp
// APNAPASHU.DataContract/Models/YourEntity/YourEntityModels.cs
public class CreateYourEntityDto { /* fields */ }
public class UpdateYourEntityDto { /* fields */ }
public class YourEntityResponseDto { /* fields */ }
public class YourEntityListingResponseDto { /* fields */ }
```

### Step 2: Create Repository Interface (RepositoryContract)
```csharp
// APNAPASHU.RepositoryContract/(Web|Mobile)/IYourEntityRepository.cs
public interface IYourEntityRepository
{
    Task<YourEntityResponseDto> GetByIdAsync(int id);
    Task<List<YourEntityResponseDto>> GetAllAsync(int page, int size);
    Task<int> CreateAsync(CreateYourEntityDto dto);
    Task<bool> UpdateAsync(UpdateYourEntityDto dto);
    Task<bool> DeleteAsync(int id);
}
```

### Step 3: Implement Repository (Repository)
```csharp
// APNAPASHU.Repository/(Web|Mobile)/YourEntityRepository.cs
public class YourEntityRepository : BaseRepository, IYourEntityRepository
{
    public YourEntityRepository(IConfiguration config) : base(config) { }
    
    public async Task<YourEntityResponseDto> GetByIdAsync(int id)
    {
        using (IDbConnection conn = GetConnection())
        {
            // Dapper query here
        }
    }
    // ... other methods
}
```

### Step 4: Create Service Interface (ServiceContract)
```csharp
// APNAPASHU.ServiceContract/(Web|Mobile)/IYourEntityService.cs
public interface IYourEntityService
{
    Task<JsonModel<YourEntityResponseDto>> GetByIdAsync(int id);
    Task<JsonModel<YourEntityListingResponseDto>> GetAllAsync(int page, int size);
    Task<JsonModel<YourEntityResponseDto>> CreateAsync(CreateYourEntityDto dto);
    Task<JsonModel<bool>> UpdateAsync(UpdateYourEntityDto dto);
    Task<JsonModel<bool>> DeleteAsync(int id);
}
```

### Step 5: Implement Service (Service)
```csharp
// APNAPASHU.Service/(Web|Mobile)/YourEntityService.cs
public class YourEntityService : BaseService, IYourEntityService
{
    private readonly IYourEntityRepository _repository;
    
    public YourEntityService(IYourEntityRepository repo, IHttpContextAccessor accessor, IConfiguration config)
        : base(accessor, config)
    {
        _repository = repo;
    }
    
    public async Task<JsonModel<YourEntityResponseDto>> GetByIdAsync(int id)
    {
        try
        {
            if (id <= 0) return new JsonModel<YourEntityResponseDto>(null, "Invalid ID", 400);
            var item = await _repository.GetByIdAsync(id);
            if (item == null) return new JsonModel<YourEntityResponseDto>(null, "Not found", 404);
            return new JsonModel<YourEntityResponseDto>(item, "Retrieved", 200);
        }
        catch (Exception ex)
        {
            return new JsonModel<YourEntityResponseDto>(null, "Error", 500, ex.Message);
        }
    }
    // ... other methods
}
```

### Step 6: Create Controller (API)
```csharp
// APNAPASHU/Controllers/(Web|Mobile)/YourEntityController.cs
[Route("api/web/[controller]")]
[ApiController]
public class YourEntityController : BaseController
{
    private readonly IYourEntityService _service;
    
    public YourEntityController(IYourEntityService service, IHttpContextAccessor accessor, IConfiguration config)
        : base(accessor, config)
    {
        _service = service;
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }
    // ... other endpoints
}
```

### Step 7: Register in DI Container
```csharp
// APNAPASHU.ServiceContract/ServiceCollectionExtensionMethod.cs
public static IServiceCollection AddWebServices(this IServiceCollection services)
{
    // ... existing
    services.AddScoped<IYourEntityRepository, YourEntityRepository>();
    services.AddScoped<IYourEntityService, YourEntityService>();
    return services;
}
```

---

## 🔑 Key Features of the Architecture

### 1. **Soft Deletes**
- Uses `IsActive` BIT column instead of hard deletes
- Preserves audit trail and referential integrity

### 2. **Unified Response Format**
```csharp
public class JsonModel<T>
{
    public T Data { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
    public int StatusCode { get; set; }
    public string ErrorCode { get; set; }
}
```

### 3. **Exception Handling**
- **Global Filter**: `ApiExceptionFilterAttribute` catches exceptions at controller level
- **Middleware**: `ErrorHandlingMiddleware` catches exceptions at application level
- Consistent error response format

### 4. **Pagination Support**
- Page-based pagination with offset/fetch
- Includes total count for client-side pagination UI

### 5. **Dependency Injection**
- All services registered in DI container
- Loose coupling via interfaces
- Easy testing with mock implementations

### 6. **Security**
- JWT Bearer token authentication
- Base64 encoded sensitive data (encryption module available)
- CORS policy configuration

---

## 🚀 How to Use

### 1. **Setup Database**
```powershell
# Connect to SQL Server
sqlcmd -S localhost -U sa -P YourPassword -d APNAPASHU -i Database\CreateTables.sql
```

### 2. **Configure Connection String**
Edit `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=APNAPASHU;User Id=sa;Password=YourPassword;Encrypt=false;"
  }
}
```

### 3. **Run Application**
```powershell
cd APNAPASHU
dotnet run
```

### 4. **Test API**
- Swagger UI: `https://localhost:5001/swagger`
- Web Category API: `https://localhost:5001/api/web/category`
- Mobile Animal API: `https://localhost:5001/api/mobile/animal`

---

## 📊 API Response Examples

### Success Response (200)
```json
{
  "success": true,
  "data": { /* entity data */ },
  "message": "Operation successful",
  "statusCode": 200,
  "errorCode": null
}
```

### Created Response (201)
```json
{
  "success": true,
  "data": { /* created entity */ },
  "message": "Category created successfully",
  "statusCode": 201,
  "errorCode": null
}
```

### Validation Error (400)
```json
{
  "success": false,
  "data": null,
  "message": "Category name is required",
  "statusCode": 400,
  "errorCode": "REQUIRED_FIELD"
}
```

### Not Found Error (404)
```json
{
  "success": false,
  "data": null,
  "message": "Category not found",
  "statusCode": 404,
  "errorCode": "NOT_FOUND"
}
```

### Server Error (500)
```json
{
  "success": false,
  "data": null,
  "message": "Error retrieving category",
  "statusCode": 500,
  "errorCode": "System.NullReferenceException"
}
```

---

## 📝 Project Files Summary

### New Files Created
```
✅ APNAPASHU.DataContract/Models/Category/CategoryModels.cs
✅ APNAPASHU.RepositoryContract/Web/ICategoryRepository.cs
✅ APNAPASHU.Repository/Web/CategoryRepository.cs
✅ APNAPASHU.ServiceContract/Web/ICategoryService.cs
✅ APNAPASHU.Service/Web/CategoryService.cs
✅ APNAPASHU/Controllers/Web/CategoryController.cs
✅ Database/CreateTables.sql (updated with Categories table)
```

### Files Modified
```
✅ APNAPASHU.ServiceContract/ServiceCollectionExtensionMethod.cs (added DI registration)
```

### Build Issues Fixed
```
✅ Microsoft.AspNetCore.Http.Extensions: 8.0.0 → 2.3.9 (correct version)
✅ Npgsql: 8.0.1 → 8.0.3 (vulnerability fix)
```

---

## 🧪 Testing the APIs

### Using cURL

#### Create Category
```bash
curl -X POST https://localhost:5001/api/web/category \
  -H "Content-Type: application/json" \
  -d '{"categoryName":"Dogs","description":"Dog breeds","iconUrl":"..."}'
```

#### Get All Categories
```bash
curl https://localhost:5001/api/web/category?pageNumber=1&pageSize=10
```

#### Search Categories
```bash
curl "https://localhost:5001/api/web/category/search?searchTerm=dog"
```

#### Update Category
```bash
curl -X PUT https://localhost:5001/api/web/category \
  -H "Content-Type: application/json" \
  -d '{"categoryId":1,"categoryName":"Dogs Updated",...}'
```

#### Delete Category
```bash
curl -X DELETE https://localhost:5001/api/web/category/1
```

---

## 🎯 Summary

✅ **Architecture**: Proper 7-layer layered architecture implemented
✅ **Web CRUD**: Complete Category management API with full CRUD operations
✅ **Mobile CRUD**: Animal listing API with location-based search
✅ **Build Status**: Successfully compiled and ready to run
✅ **Database**: Schema prepared for both entities
✅ **Dependency Injection**: All services registered
✅ **Error Handling**: Global exception handling in place
✅ **Documentation**: Code well-commented and self-documenting

### Quick Start Commands
```powershell
# Clean build
dotnet clean

# Build project
dotnet build

# Run project
dotnet run

# Access API
# Swagger: https://localhost:5001/swagger
# Web API: https://localhost:5001/api/web/category
# Mobile API: https://localhost:5001/api/mobile/animal
```

---

**Created**: 2026-03-14  
**Status**: ✅ Production Ready  
**Framework**: .NET 8.0  
**Architecture**: Clean Layered Architecture with SOLID principles
