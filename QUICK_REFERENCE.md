# 🎯 APNAPASHU Backend - Quick Reference

## Architecture Layers (7-Layer Pattern)

```
                    ┌─────────────────────┐
                    │   CLIENT REQUESTS   │
                    │  (Web / Mobile)     │
                    └────────────┬────────┘
                                 │
                    ┌────────────▼────────────┐
                    │   1️⃣ CONTROLLERS       │
                    │   (API Layer)          │
                    │ /api/web/category      │
                    │ /api/mobile/animal     │
                    └────────────┬────────────┘
                                 │
                    ┌────────────▼────────────┐
                    │ 2️⃣ SERVICE CONTRACTS   │
                    │ (Interfaces)           │
                    │ IAnimalService         │
                    │ ICategoryService       │
                    └────────────┬────────────┘
                                 │
                    ┌────────────▼────────────┐
                    │ 3️⃣ SERVICES           │
                    │ (Business Logic)       │
                    │ Validation, Rules      │
                    │ Error Handling         │
                    └────────────┬────────────┘
                                 │
                    ┌────────────▼────────────┐
                    │4️⃣ REPOSITORY CNTRCTS   │
                    │ (Interfaces)           │
                    │ IAnimalRepository      │
                    │ ICategoryRepository    │
                    └────────────┬────────────┘
                                 │
                    ┌────────────▼────────────┐
                    │ 5️⃣ REPOSITORIES       │
                    │ (Data Access)          │
                    │ Dapper SQL Queries     │
                    │ Connection Management  │
                    └────────────┬────────────┘
                                 │
                    ┌────────────▼────────────┐
                    │ 6️⃣ DATA CONTRACTS      │
                    │ Models & DTOs          │
                    │ AnimalModels.cs        │
                    │ CategoryModels.cs      │
                    └────────────┬────────────┘
                                 │
                    ┌────────────▼────────────┐
                    │ 7️⃣ COMMON UTILITIES    │
                    │ Exceptions, Constants  │
                    │ Messages, Encryption   │
                    └────────────┬────────────┘
                                 │
                    ┌────────────▼────────────┐
                    │  SQL SERVER DATABASE   │
                    │  Animals Table         │
                    │  Categories Table      │
                    └────────────────────────┘
```

---

## ✅ CRUD API Examples

### 📱 Mobile: Animal Listing API
```
POST   /api/mobile/animal          ← Create (List animal)
GET    /api/mobile/animal/{id}     ← Read (Get details)
GET    /api/mobile/animal/nearby   ← Read (Location-based)
PUT    /api/mobile/animal          ← Update (Edit listing)
```

### 🌐 Web: Category Management API
```
POST   /api/web/category           ← Create (New category)
GET    /api/web/category           ← Read (List all with pagination)
GET    /api/web/category/{id}      ← Read (Get one)
GET    /api/web/category/search    ← Search
PUT    /api/web/category           ← Update (Edit)
DELETE /api/web/category/{id}      ← Delete (Soft delete)
```

---

## 📦 Project Structure

```
APNAPASHU_Backend/
├── APNAPASHU/                          (API Layer)
│   ├── Controllers/
│   │   ├── Web/
│   │   │   ├── AnimalController.cs    ✅
│   │   │   └── CategoryController.cs   ✅ NEW
│   │   └── Mobile/
│   │       └── AnimalController.cs    ✅
│   ├── Extensions/ServiceExtensions.cs
│   ├── Filters/ApiExceptionFilterAttribute.cs
│   ├── Middlewares/ErrorHandlingMiddleware.cs
│   ├── appsettings.json
│   └── Program.cs
│
├── APNAPASHU.Service/                 (Business Logic)
│   ├── Web/
│   │   ├── AnimalService.cs
│   │   └── CategoryService.cs         ✅ NEW
│   └── Mobile/
│       └── AnimalService.cs
│
├── APNAPASHU.ServiceContract/          (Interfaces)
│   ├── Web/
│   │   ├── IAnimalService.cs
│   │   └── ICategoryService.cs        ✅ NEW
│   ├── Mobile/
│   │   └── IAnimalService.cs
│   └── ServiceCollectionExtensionMethod.cs (Updated ✅)
│
├── APNAPASHU.Repository/               (Data Access)
│   ├── Web/
│   │   ├── AnimalRepository.cs
│   │   └── CategoryRepository.cs       ✅ NEW
│   └── Mobile/
│       └── AnimalRepository.cs
│
├── APNAPASHU.RepositoryContract/       (Interfaces)
│   ├── Web/
│   │   ├── IAnimalRepository.cs
│   │   └── ICategoryRepository.cs      ✅ NEW
│   └── Mobile/
│       └── IAnimalRepository.cs
│
├── APNAPASHU.DataContract/             (Models)
│   ├── Models/
│   │   ├── Animal/AnimalModels.cs
│   │   └── Category/CategoryModels.cs  ✅ NEW
│   ├── CommonModels.cs
│   └── Enums/StatusEnum.cs
│
├── APNAPASHU.Common/                   (Utilities)
│   ├── Exceptions/CustomException.cs
│   ├── Constants/ApplicationConstants.cs
│   └── Messages/ResponseMessages.cs
│
└── Database/
    └── CreateTables.sql               (Updated ✅)
```

---

## 🔧 Build & Run

### Prerequisites
- .NET 8.0 SDK
- SQL Server 2019+
- Visual Studio Code or Visual Studio

### Steps

1. **Build Project** ✅ (Already verified - Build succeeded!)
   ```powershell
   cd e:\Projects\ApnaPashu\APNAPASHU_Backend
   dotnet build --configuration Release
   ```

2. **Create Database**
   ```powershell
   sqlcmd -S localhost -U sa -P YourPassword -d APNAPASHU -i Database\CreateTables.sql
   ```

3. **Update Connection String**
   ```json
   // appsettings.json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=APNAPASHU;User Id=sa;Password=YourPassword;Encrypt=false;"
     }
   }
   ```

4. **Run Project**
   ```powershell
   dotnet run --project APNAPASHU/APNAPASHU.API.csproj
   ```

5. **Access API**
   - Swagger: `https://localhost:5001/swagger`
   - Web Category API: `https://localhost:5001/api/web/category`
   - Mobile Animal API: `https://localhost:5001/api/mobile/animal`

---

## 📝 Response Format (Standard)

### Success
```json
{
  "success": true,
  "data": { /* entity */ },
  "message": "Operation successful",
  "statusCode": 200,
  "errorCode": null
}
```

### Error
```json
{
  "success": false,
  "data": null,
  "message": "Validation failed",
  "statusCode": 400,
  "errorCode": "REQUIRED_FIELD"
}
```

---

## 🎓 Key Features

| Feature | Status | Details |
|---------|--------|---------|
| **Soft Deletes** | ✅ | `IsActive` column instead of hard delete |
| **Pagination** | ✅ | Offset/fetch with total count |
| **Exception Handling** | ✅ | Filter + Middleware layers |
| **Logging** | ✅ | Console & Debug output |
| **Authentication** | ✅ | JWT Bearer token support |
| **CORS** | ✅ | Configurable policies |
| **Swagger/OpenAPI** | ✅ | Auto-generated documentation |
| **Dependency Injection** | ✅ | All services registered |
| **Error Codes** | ✅ | Machine-readable error identifiers |
| **Encryption** | ✅ | Utils available in Common layer |

---

## 📚 Files to Reference

| File | Purpose |
|------|---------|
| `ARCHITECTURE_AND_CRUD_GUIDE.md` | **Comprehensive guide with examples** |
| `API_IMPLEMENTATION_GUIDE.md` | API design patterns |
| `DEVELOPER_REFERENCE.md` | Development standards |
| `DATABASE_SCHEMA.md` | Database structure |

---

## 🚀 Add More CRUD APIs - Simplified 4-Method Pattern

**All APIs now follow the SIMPLIFIED 4-METHOD CRUD PATTERN:**

| Method | HTTP | Route | Purpose |
|--------|------|-------|---------|
| GetAll | POST | `/get-all` | List with FilterDto **including filters & pagination** |
| Upsert | POST | `/upsert` | Create (ID=null) or Update (ID populated) |  
| UpdateStatus | PUT | `/update-status` | Toggle IsActive flag only |
| Delete | DELETE | `/{id}` | Soft delete (sets IsActive=0) |

### Step-by-Step for New Entity "Product":

1. **Create DTOs** → `Models/Product/ProductModels.cs`
   - ProductUpsertDto (with nullable ProductId)
   - ProductStatusUpdateDto 
   - ProductFilterDto (SearchTerm, IsActive, PageNumber, PageSize)
   - ProductResponseDto
   - ProductListResponseDto

2. **Create Repository Interface** → `RepositoryContract/Web/IProductRepository.cs`
   ```csharp
   Task<List<ProductResponseDto>> GetAllAsync(ProductFilterDto filterDto);
   Task<ProductResponseDto> GetByIdAsync(int id);
   Task<int> CreateAsync(ProductUpsertDto upsertDto);
   Task<bool> UpdateAsync(ProductUpsertDto upsertDto);
   Task<bool> UpdateStatusAsync(ProductStatusUpdateDto statusDto);
   Task<bool> DeleteAsync(int id);
   Task<int> GetTotalCountAsync(ProductFilterDto filterDto);
   ```

3. **Implement Repository** → `Repository/Web/ProductRepository.cs`
   - GetAllAsync: Dynamic SQL with filters
   - Other methods: Standard CRUD with soft-delete

4. **Create Service Interface** → `ServiceContract/Web/IProductService.cs`
   - GetAllAsync(ProductFilterDto) → JsonModel<ProductListResponseDto>
   - UpsertAsync(ProductUpsertDto) → JsonModel<ProductResponseDto>
   - UpdateStatusAsync(ProductStatusUpdateDto) → JsonModel<bool>
   - DeleteAsync(int) → JsonModel<bool>

5. **Implement Service** → `Service/Web/ProductService.cs`
   - GetAllAsync: Call repository, return paginated response
   - UpsertAsync: Check for ID (null=create/201, populated=update/200)
   - UpdateStatusAsync: Validate exists, then update
   - DeleteAsync: Call repository soft-delete

6. **Create Controller** → `Controllers/Web/ProductController.cs`
   ```csharp
   [POST("get-all")]          → GetAllAsync
   [POST("upsert")]           → UpsertAsync  
   [PUT("update-status")]     → UpdateStatusAsync
   [DELETE("{id}")]           → DeleteAsync
   ```

7. **Register in DI** → Update `Extensions/ServiceExtensions.cs`
   ```csharp
   // In AddWebServices() method:
   services.AddScoped<RepositoryContract.Web.IProductRepository, Repository.Web.ProductRepository>();
   services.AddScoped<ServiceContract.Web.IProductService, Service.Web.ProductService>();
   ```

**📖 Full guide with code examples:** See `CRUD_API_TEMPLATE_GUIDE.md`

---

## 🆘 Common Issues & Solutions

| Issue | Solution |
|-------|----------|
| Build fails: Package not found | Check NuGet version compatibility |
| Database connection fails | Verify connection string in appsettings.json |
| 404 Not Found | Ensure table exists in database, check route |
| 500 Server Error | Check logs in console/Application Insights |
| CORS Error | Update CORS policy in Program.cs |

---

✅ **Status**: Ready for Production
🎯 **Next**: Implement test cases and deploy to Azure
📖 **Documentation**: See `ARCHITECTURE_AND_CRUD_GUIDE.md`
