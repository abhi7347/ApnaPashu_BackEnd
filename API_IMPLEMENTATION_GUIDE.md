# APNAPASHU Backend - Complete API Implementation Guide

## Overview
This is a complete reference implementation following a **layered architecture** with **Web/Mobile segregation**. The Animal API is fully implemented end-to-end covering all layers.

## Project Architecture

```
┌──────────────────────────────────────────────┐
│   APNAPASHU.API (Controllers & Filters)      │
│   - Web: /api/web/animal                     │
│   - Mobile: /api/mobile/animal               │
└──────────────────┬───────────────────────────┘
                   │
┌──────────────────▼───────────────────────────┐
│   APNAPASHU.Service (Business Logic)         │
│   - Web: AnimalService                       │
│   - Mobile: AnimalService                    │
└──────────────────┬───────────────────────────┘
                   │
┌──────────────────▼───────────────────────────┐
│   APNAPASHU.ServiceContract (Interfaces)     │
│   - Web: IAnimalService                      │
│   - Mobile: IAnimalService                   │
└──────────────────┬───────────────────────────┘
                   │
┌──────────────────▼───────────────────────────┐
│   APNAPASHU.Repository (Data Access)         │
│   - Web: AnimalRepository                    │
│   - Mobile: AnimalRepository                 │
└──────────────────┬───────────────────────────┘
                   │
┌──────────────────▼───────────────────────────┐
│   APNAPASHU.RepositoryContract (Interfaces)  │
│   - Web: IAnimalRepository                   │
│   - Mobile: IAnimalRepository                │
└──────────────────┬───────────────────────────┘
                   │
┌──────────────────▼───────────────────────────┐
│   APNAPASHU.DataContract (Models/DTOs)       │
│   - AnimalModels.cs                          │
│   - CommonModels.cs (JsonModel, etc)         │
└──────────────────┬───────────────────────────┘
                   │
┌──────────────────▼───────────────────────────┐
│   APNAPASHU.Common (Utilities/Constants)     │
│   - Exceptions, Constants, Messages          │
└──────────────────────────────────────────────┘
```

## Database Setup

1. **Create Database:**
   ```sql
   CREATE DATABASE APNAPASHU;
   ```

2. **Run SQL Scripts:**
   - Execute `/Database/CreateTables.sql` to create the Animals table and indexes

3. **Update Connection String:**
   - Edit `appsettings.json`
   - Update `DefaultConnection` with your SQL Server details

## Global Exception Handling

### Exception Filter (ApiExceptionFilterAttribute)
- Catches all exceptions in controllers
- Converts to standardized JsonModel response
- Returns appropriate HTTP status codes

### Error Handling Middleware (ErrorHandlingMiddleware)
- Catches exceptions outside controllers
- Logs errors for debugging
- Returns JSON error responses

### Custom Exceptions
- `CustomException` - For application-specific errors with status codes

## Web API Endpoints

### Base: `http://localhost:5001/api/web/animal`

```http
GET     /
         - Get all animals with pagination
         - Query: pageNumber, pageSize

GET     /{id}
         - Get animal by ID

GET     /search?searchTerm=xxx
         - Search animals by name, breed, or category

POST    /
         - Create new animal
         - Body: CreateAnimalDto

PUT     /{id}
         - Update animal
         - Body: UpdateAnimalDto

DELETE  /{id}
         - Delete animal (soft delete)
```

## Mobile API Endpoints

### Base: `http://localhost:5001/api/mobile/animal`

```http
GET     /{id}
         - Get animal details

GET     /nearby?location=xxx
         - Get nearby animals by location with pagination

GET     /category/{category}
         - Get animals by category

POST    /list
         - List animal by user
         - Body: CreateAnimalDto

PUT     /update/{id}
         - Update animal listing
         - Body: UpdateAnimalDto
```

## Response Format

All API responses follow this standard format:

```json
{
  "data": {},
  "message": "Success message",
  "statusCode": 200,
  "appError": "",
  "accessToken": null
}
```

### Response Codes:
- **200** - OK
- **201** - Created
- **400** - Bad Request / Invalid Data
- **401** - Unauthorized
- **404** - Not Found
- **500** - Internal Server Error

## Step-by-Step Guide: Adding a New API

### Example: Add Category Management API

#### 1. Create Models (`APNAPASHU.DataContract/Models/Category/`)

```csharp
public class CreateCategoryDto
{
    public string CategoryName { get; set; }
    public string Description { get; set; }
}

public class CategoryResponseDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public string Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
}
```

#### 2. Create Repository Interface (`APNAPASHU.RepositoryContract/Web/`)

```csharp
namespace APNAPASHU.RepositoryContract.Web
{
    public interface ICategoryRepository
    {
        Task<CategoryResponseDto> GetCategoryByIdAsync(int categoryId);
        Task<List<CategoryResponseDto>> GetAllCategoriesAsync();
        Task<int> CreateCategoryAsync(CreateCategoryDto dto);
        Task<bool> UpdateCategoryAsync(int categoryId, UpdateCategoryDto dto);
        Task<bool> DeleteCategoryAsync(int categoryId);
    }
}
```

#### 3. Create Repository Implementation (`APNAPASHU.Repository/Web/`)

```csharp
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace APNAPASHU.Repository.Web
{
    public class CategoryRepository : BaseRepository, RepositoryContract.Web.ICategoryRepository
    {
        public CategoryRepository(IConfiguration configuration) : base(configuration) { }

        public async Task<CategoryResponseDto> GetCategoryByIdAsync(int categoryId)
        {
            string query = @"
                SELECT CategoryId, CategoryName, Description, CreatedDate, IsActive
                FROM Categories
                WHERE CategoryId = @CategoryId AND IsActive = 1";

            using (IDbConnection conn = GetConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@CategoryId", categoryId);
                return await conn.QueryFirstOrDefaultAsync<CategoryResponseDto>(query, parameters);
            }
        }

        // Implement other methods similarly...
    }
}
```

#### 4. Create Service Interface (`APNAPASHU.ServiceContract/Web/`)

```csharp
namespace APNAPASHU.ServiceContract.Web
{
    public interface ICategoryService
    {
        Task<JsonModel<CategoryResponseDto>> GetCategoryByIdAsync(int categoryId);
        Task<JsonModel<List<CategoryResponseDto>>> GetAllCategoriesAsync();
        Task<JsonModel<CategoryResponseDto>> CreateCategoryAsync(CreateCategoryDto dto);
        Task<JsonModel<bool>> UpdateCategoryAsync(int categoryId, UpdateCategoryDto dto);
        Task<JsonModel<bool>> DeleteCategoryAsync(int categoryId);
    }
}
```

#### 5. Create Service Implementation (`APNAPASHU.Service/Web/`)

```csharp
namespace APNAPASHU.Service.Web
{
    public class CategoryService : BaseService, ServiceContract.Web.ICategoryService
    {
        private readonly RepositoryContract.Web.ICategoryRepository _categoryRepository;

        public CategoryService(RepositoryContract.Web.ICategoryRepository categoryRepository, 
            IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
            : base(httpContextAccessor, configuration)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<JsonModel<CategoryResponseDto>> GetCategoryByIdAsync(int categoryId)
        {
            try
            {
                if (categoryId <= 0)
                    return new JsonModel<CategoryResponseDto>(null, "Invalid ID", 400, "INVALID_ID");

                var category = await _categoryRepository.GetCategoryByIdAsync(categoryId);
                if (category == null)
                    return new JsonModel<CategoryResponseDto>(null, "Not found", 404, "NOT_FOUND");

                return new JsonModel<CategoryResponseDto>(category, "Success", 200);
            }
            catch (Exception ex)
            {
                return new JsonModel<CategoryResponseDto>(null, "Error", 500, ex.Message);
            }
        }

        // Implement other methods similarly...
    }
}
```

#### 6. Create Controller (`APNAPASHU/Controllers/Web/`)

```csharp
namespace APNAPASHU.API.Controllers.Web
{
    [Route("api/web/[controller]")]
    [ApiController]
    public class CategoryController : BaseController
    {
        private readonly ServiceContract.Web.ICategoryService _categoryService;

        public CategoryController(ServiceContract.Web.ICategoryService categoryService, 
            IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
            : base(httpContextAccessor, configuration)
        {
            _categoryService = categoryService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var result = await _categoryService.GetCategoryByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await _categoryService.GetAllCategoriesAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _categoryService.CreateCategoryAsync(createDto);
            return StatusCode(result.StatusCode, result);
        }

        // Implement other endpoints similarly...
    }
}
```

#### 7. Register in Dependency Injection (`ServiceCollectionExtensionMethod.cs`)

```csharp
public static IServiceCollection AddWebServices(this IServiceCollection services)
{
    // Existing services...
    
    // Category
    services.AddScoped<RepositoryContract.Web.ICategoryRepository, Web.CategoryRepository>();
    services.AddScoped<ServiceContract.Web.ICategoryService, Web.CategoryService>();

    return services;
}
```

#### 8. Create Database Table (`Database/CreateTables.sql`)

```sql
CREATE TABLE Categories (
    CategoryId INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX),
    CreatedDate DATETIME DEFAULT GETDATE(),
    UpdatedDate DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1
);
```

## Key Features Implemented

✅ **Global Exception Handling** - Filters + Middleware  
✅ **Dependency Injection** - Service registration  
✅ **CORS Configuration** - Multiple policies  
✅ **JWT Authentication** - Ready to integrate bearer token  
✅ **Pagination Support** - BasePaginationDto  
✅ **Logging** - Console and Debug  
✅ **Swagger Documentation** - Auto-generated API docs  
✅ **Structured Response Models** - Standardized JsonModel  
✅ **Database Connection** - SQL Server with Dapper ORM  
✅ **Middleware** - Error handling, logging  

## Testing the API

### Using Swagger
1. Run the application: `dotnet run`
2. Navigate to: `https://localhost:7001/swagger`
3. Test endpoints directly from the UI

### Using cURL

```bash
# Get all animals
curl -X GET "https://localhost:7001/api/web/animal?pageNumber=1&pageSize=10"

# Get animal by ID
curl -X GET "https://localhost:7001/api/web/animal/1"

# Create animal (Web)
curl -X POST "https://localhost:7001/api/web/animal" \
  -H "Content-Type: application/json" \
  -d '{
    "animalName": "Dog",
    "breed": "Labrador",
    "category": "Pet",
    "age": 3,
    "description": "Friendly dog",
    "price": 5000,
    "location": "Mumbai",
    "contactNumber": "9876543210"
  }'

# Mobile endpoints
curl -X GET "https://localhost:7001/api/mobile/animal/nearby?location=Mumbai"
```

## Notes

- All repositories implement **soft delete** (IsActive = 0)
- Timestamp fields auto-update with GETDATE()
- Implement pagination to avoid large data loads
- Use Dapper for efficient database operations
- Follow the same pattern for all new APIs
- Always use dependency injection
- Validate input in services, not controllers
- Return proper HTTP status codes

## Next Steps

1. Configure real SQL Server connection string
2. Run database migration scripts
3. Test all endpoints with Swagger
4. Implement more APIs following this pattern
5. Add unit tests
6. Deploy to cloud (Azure, AWS, etc.)