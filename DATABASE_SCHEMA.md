# Database Schema

## Animals Table

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

CREATE INDEX IX_Animals_IsActive ON Animals(IsActive);
CREATE INDEX IX_Animals_Category ON Animals(Category);
CREATE INDEX IX_Animals_Location ON Animals(Location);
```

### Schema Diagram

```
┌─────────────────────────────────┐
│         Animals                 │
├─────────────────────────────────┤
│ AnimalId (PK) INT               │ ← Primary Key
├─────────────────────────────────┤
│ AnimalName NVARCHAR(100) NOT NULL     │
│ Breed NVARCHAR(100)             │
│ Category NVARCHAR(50)           │ ← Indexed
│ Age INT                         │
│ Description NVARCHAR(MAX)       │
│ Price DECIMAL(10, 2)            │
│ Location NVARCHAR(200)          │ ← Indexed
│ ContactNumber NVARCHAR(20)      │
├─────────────────────────────────┤
│ CreatedDate DATETIME            │ ← Auto-set
│ UpdatedDate DATETIME            │ ← Auto-update
│ IsActive BIT (1)                │ ← Soft Delete
└─────────────────────────────────┘
```

## Data Flow Examples

### Create Animal (Web)
```
POST /api/web/animal
{
  "animalName": "Golden Retriever",
  "breed": "Golden Retriever",
  "category": "Pet/Dog",
  "age": 2,
  "description": "Beautiful and friendly golden retriever",
  "price": 15000,
  "location": "Mumbai",
  "contactNumber": "9876543210"
}
↓
AnimalController.CreateAnimal()
↓
AnimalService.CreateAnimalAsync()
↓
AnimalRepository.CreateAnimalAsync()
↓
SQL: INSERT INTO Animals (...)
↓
Response:
{
  "data": {
    "animalId": 1,
    "animalName": "Golden Retriever",
    ...
  },
  "message": "Animal created successfully",
  "statusCode": 201
}
```

### Get Animals with Pagination
```
GET /api/web/animal?pageNumber=1&pageSize=10
↓
AnimalController.GetAllAnimals()
↓
AnimalService.GetAllAnimalsAsync()
↓
AnimalRepository.GetAllAnimalsAsync()
↓
SQL: SELECT ... OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY
↓
AnimalRepository.GetTotalAnimalsCountAsync()
↓
Response:
{
  "data": {
    "animals": [
      {"animalId": 1, "animalName": "Dog", ...},
      ...
    ],
    "totalRecords": 50,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 5
  },
  "message": "Animals retrieved successfully",
  "statusCode": 200
}
```

### Search Animals
```
GET /api/web/animal/search?searchTerm=dog
↓
AnimalController.SearchAnimals()
↓
AnimalService.SearchAnimalsAsync()
↓
AnimalRepository.SearchAnimalsAsync()
↓
SQL: SELECT ... WHERE 
     AnimalName LIKE '%dog%' OR 
     Breed LIKE '%dog%' OR 
     Category LIKE '%dog%'
↓
Response: List of matching animals
```

### Get Nearby Animals (Mobile)
```
GET /api/mobile/animal/nearby?location=Mumbai&pageNumber=1&pageSize=10
↓
AnimalController.GetNearbyAnimals()
↓
AnimalService.GetNearbyAnimalsAsync()
↓
AnimalRepository.GetNearbyAnimalsAsync()
↓
SQL: SELECT ... 
     WHERE IsActive = 1 AND Location LIKE '%Mumbai%'
↓
Response: Animals in Mumbai with pagination
```

### Update Animal
```
PUT /api/web/animal/1
{
  "animalName": "Updated Name",
  ...
}
↓
AnimalController.UpdateAnimal()
↓
AnimalService.UpdateAnimalAsync()
  - Validate ID
  - Check if exists
↓
AnimalRepository.UpdateAnimalAsync()
↓
SQL: UPDATE Animals 
     SET AnimalName = '@Name', ..., UpdatedDate = GETDATE()
     WHERE AnimalId = @Id
↓
Response: success/failure
```

### Delete Animal (Soft Delete)
```
DELETE /api/web/animal/1
↓
AnimalController.DeleteAnimal()
↓
AnimalService.DeleteAnimalAsync()
↓
AnimalRepository.DeleteAnimalAsync()
↓
SQL: UPDATE Animals 
     SET IsActive = 0, UpdatedDate = GETDATE()
     WHERE AnimalId = @Id
↓
Data is NOT physically deleted, only marked inactive
```

## Query Performance

### With Indexes
```sql
-- Fast (uses index)
SELECT * FROM Animals WHERE Category = 'Pet'
SELECT * FROM Animals WHERE Location LIKE '%Mumbai%'
SELECT * FROM Animals WHERE IsActive = 1

-- Cost: Low
```

### Without Indexes
```sql
-- Slow (table scan)
SELECT * FROM Animals WHERE Description LIKE '%puppy%'

-- Cost: High
```

## Soft Delete Strategy

### Why Soft Delete?
- Audit trail
- Data recovery
- Historical analysis
- Compliance

### Getting Active Records
```sql
SELECT * FROM Animals WHERE IsActive = 1
```

### Getting Deleted Records
```sql
SELECT * FROM Animals WHERE IsActive = 0
```

### Restore Deleted Record
```sql
UPDATE Animals SET IsActive = 1 WHERE AnimalId = @Id
```

## Future Schema Additions

### Users Table
```sql
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    Email NVARCHAR(100) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(256) NOT NULL,
    FullName NVARCHAR(100),
    PhoneNumber NVARCHAR(20),
    CreatedDate DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1
);
```

### Categories Table
```sql
CREATE TABLE Categories (
    CategoryId INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX),
    CreatedDate DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1
);
```

### Animal-Category Relationship
```sql
ALTER TABLE Animals
ADD CategoryId INT FOREIGN KEY REFERENCES Categories(CategoryId);
```

---

**Updated: 2026-03-14**