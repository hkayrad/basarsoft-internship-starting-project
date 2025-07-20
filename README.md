# Basarsoft GIS API

**Start Date:** 16/07/25  
**Author:** Hakan Kayra Doğan

A .NET 9 Web API for managing geographical features with PostgreSQL backend. Built as an internship learning project focusing on geospatial data handling and clean architecture patterns.

## 🏗️ Architecture Overview

This project implements a clean, scalable architecture for geospatial data management with the following key components:

### Core Components
- **Single Controller**: `FeaturesController` handles all feature operations
- **Dual Service Implementation**: 
  - `PostgresqlFeatureServices` (production)
  - `ArrayFeatureServices` (development/testing)
- **Unified Response Format**: All endpoints return standardized `Response<T>` objects
- **Database Helper**: Static `DbHelper` manages PostgreSQL connections via `NpgsqlDataSource`

### Technology Stack
- **.NET 9** - Latest .NET framework
- **PostgreSQL** - Geospatial database with Npgsql driver
- **Swagger/OpenAPI** - API documentation
- **Raw SQL** - Direct database access without ORM

## 🚀 Getting Started

### Prerequisites
- .NET 9 SDK
- PostgreSQL database
- Visual Studio/VS Code (recommended)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/hkayrad/basarsoft-internship-starting-project.git
   cd Basarsoft-Start
   ```

2. **Configure database connection**
   
   Update `appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "PostgreSQL": "Host=localhost;Port=5432;Database=map_info;Username=your_username;Password=your_password"
     }
   }
   ```

3. **Set up PostgreSQL database**
   
   Create a `features` table:
   ```sql
   CREATE TABLE features (
       id SERIAL PRIMARY KEY,
       name VARCHAR(100) NOT NULL,
       wkt TEXT NOT NULL
   );
   ```

4. **Run the application**
   ```bash
   dotnet run --project API
   ```

   The API will be available at `https://localhost:443` with Swagger UI at `https://localhost:443/swagger`

## 📊 API Endpoints

All endpoints return a standardized response format:

```json
{
  "isSuccess": true,
  "message": "Operation completed successfully",
  "data": {...},
  "status": 200
}
```

### Available Endpoints

| Method | Endpoint | Description | Request Body |
|--------|----------|-------------|--------------|
| `POST` | `/api/features` | Add single feature | `AddFeatureDto` |
| `POST` | `/api/features/addMultiple` | Add multiple features (max 25) | `AddFeatureDto[]` |
| `GET` | `/api/features` | Get all features with optional pagination | Query: `pageNumber`, `pageSize` |
| `GET` | `/api/features/{id}` | Get feature by ID | - |
| `PUT` | `/api/features/{id}` | Update feature | `UpdateFeatureDto` |
| `DELETE` | `/api/features/{id}` | Delete feature | - |

### Data Models

**Feature Model:**
```csharp
{
  "id": 1,
  "name": "Sample Point",
  "wkt": "POINT(1 1)"
}
```

**Add/Update Feature DTO:**
```csharp
{
  "name": "Feature Name",    // 3-100 chars, alphanumeric + basic punctuation
  "wkt": "POINT(1 1)"       // Well-Known Text format
}
```

## 🔧 Key Features

### 1. Geospatial Data Support
- **WKT Format**: Uses Well-Known Text for geometry representation
- **Supported Geometries**: Points, Lines, Polygons, etc.
- **Validation**: Strict regex validation for WKT format compliance

### 2. Robust Validation System
- **Dual Validation**: Both DTOs and domain models implement `IValidatableObject`
- **Input Validation**: Regex patterns for Name (3-100 chars) and WKT format
- **Business Rules**: Domain model validation for business logic
- **Automatic Error Handling**: Validation errors wrapped in unified response format

### 3. Flexible Service Architecture
Switch between implementations in `Program.cs`:

```csharp
// Production: PostgreSQL
builder.Services.AddSingleton<IFeatureServices, PostgresqlFeatureServices>();

// Development: In-memory array
builder.Services.AddSingleton<IFeatureServices, ArrayFeatureServices>();
```

### 4. Optimized Database Operations
- **Parameterized Queries**: SQL injection prevention
- **Batch Operations**: Efficient multiple inserts using `NpgsqlBatch`
- **Async Resource Management**: Proper disposal of database connections
- **Connection Pooling**: `NpgsqlDataSource` for optimal performance

### 5. Development-Friendly Features
- **Swagger Integration**: Automatic API documentation
- **Environment-Specific Configuration**: Different settings for dev/prod
- **Comprehensive Error Messages**: Detailed validation and error responses

## 📁 Project Structure

```
API/
├── Controllers/
│   └── FeaturesController.cs          # Single controller for all feature operations
├── Models/
│   ├── Feature.cs                     # Domain model with validation
│   ├── Response.cs                    # Unified response wrapper
│   └── DTOs/Feature/
│       ├── BaseFeatureDto.cs          # Base DTO with validation
│       ├── AddFeatureDto.cs           # DTO for adding features
│       └── UpdateFeatureDto.cs        # DTO for updating features
├── Services/Feature/
│   ├── IFeatureServices.cs            # Service interface
│   ├── PostgresqlFeatureServices.cs   # PostgreSQL implementation
│   └── ArrayFeatureServices.cs        # In-memory implementation
├── Helpers/
│   └── DbHelper.cs                    # Database connection management
├── Validation/
│   └── ValidationResult.cs            # Custom validation result
└── Program.cs                         # Application configuration
```

## 🔄 Development Patterns

### Database Access Pattern
```csharp
await using var command = dataSource.CreateCommand(sql);
command.Parameters.AddWithValue("param", value);
using var reader = await command.ExecuteReaderAsync();
```

### Response Pattern
```csharp
// Success
return Response<T>.Success(data, "Success message", HttpStatusCode.OK);

// Failure
return Response<T>.Fail("Error message", HttpStatusCode.BadRequest);
```

### Validation Pattern
```csharp
public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
{
    if (condition)
        yield return new ValidationResult("Error message", [nameof(Property)]);
}
```

## 🧪 Testing

The project includes dual service implementations allowing for easy testing:

- **Production**: Use `PostgresqlFeatureServices` with real database
- **Development/Testing**: Use `ArrayFeatureServices` with in-memory storage

## 🛠️ Configuration

### Connection String
Required in `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Port=5432;Database=your_db;Username=user;Password=pass"
  }
}
```

### Service Registration
In `Program.cs`:
```csharp
// Choose your implementation
builder.Services.AddSingleton<IFeatureServices, PostgresqlFeatureServices>();
// OR
builder.Services.AddSingleton<IFeatureServices, ArrayFeatureServices>();
```

## 📝 Validation Rules

### Name Field
- **Length**: 3-100 characters
- **Allowed Characters**: Alphanumeric + space, comma, apostrophe, parentheses, forward slash, hyphen
- **Pattern**: `[^a-zA-Z0-9 ,'()/-]` (rejected characters)

### WKT Field
- **Format**: Valid Well-Known Text
- **Allowed Characters**: Uppercase letters, numbers, parentheses, space, comma, hyphen, period
- **Examples**: `POINT(1 1)`, `POLYGON((0 0,1 0,1 1,0 1,0 0))`

## 🤝 Contributing

This project follows clean architecture principles and coding standards. When contributing:

1. Follow the established patterns for database access
2. Maintain the unified response format
3. Add appropriate validation to both DTOs and domain models
4. Use parameterized queries for database operations
5. Include proper error handling and meaningful messages

## 📚 Learning Objectives

This project demonstrates:
- Clean architecture principles in .NET
- Geospatial data handling with PostgreSQL
- Raw SQL operations with Npgsql
- Comprehensive validation strategies
- API design best practices
- Dependency injection patterns
- Async/await patterns for database operations

---

*Built with ❤️ by HKD*