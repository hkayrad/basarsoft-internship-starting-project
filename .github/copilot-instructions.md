# Copilot Instructions for Basarsoft GIS API

## Project Overview
.NET 9 Web API for managing geographical features with PostgreSQL backend. Built as an internship learning project focusing on geospatial data handling and clean architecture patterns.

## Architecture Components

### Core Structure
- **Single Controller**: `FeaturesController` handles all feature operations
- **Dual Services**: `PostgresqlFeatureServices` (production) and `ArrayFeatureServices` (dev/testing)
- **Response Wrapper**: All endpoints return standardized `Response<T>` objects
- **Database Helper**: Static `DbHelper` manages PostgreSQL connections via `NpgsqlDataSource`

### Critical Design Patterns

#### 1. Unified Response Format
Every API call returns `Response<T>` with consistent structure:
```csharp
Response<T>.Success(data, "message", HttpStatusCode.OK)
Response<T>.Fail("error message", HttpStatusCode.BadRequest)
```
Validation errors automatically wrapped in `Program.cs` via `ApiBehaviorOptions`.

#### 2. Raw SQL Database Access
- No ORM - direct PostgreSQL integration using Npgsql
- Parameterized queries to prevent SQL injection
- Batch operations with `NpgsqlBatch` for multiple inserts (max 25)
- Pattern: `await using var command = DbHelper.DataSource.CreateCommand(sql)`

#### 3. Validation Strategy
Both DTOs and domain models implement `IValidatableObject`:
- **DTOs**: Input validation with regex for Name/WKT format
- **Domain Models**: Business rule validation
- Use `yield return new ValidationResult()` pattern consistently

#### 4. Service Swapping
Switch between implementations in `Program.cs`:
```csharp
// Production: PostgreSQL
builder.Services.AddSingleton<IFeatureServices, PostgresqlFeatureServices>();
// Development: In-memory array
builder.Services.AddSingleton<IFeatureServices, ArrayFeatureServices>();
```

## Development Patterns

### Running & Testing
- Start API: `dotnet run --project API` (runs on HTTPS:443)
- Swagger UI automatically available in development
- Connection string required in `appsettings.Development.json`

### Geospatial Data Handling
- Uses WKT (Well-Known Text) format for geometry: `"POINT(1 1)"`, `"POLYGON(...)"`
- Strict regex validation for WKT format compliance
- Name validation: 3-100 chars, alphanumeric + basic punctuation only

### Async Resource Management
All database operations follow pattern:
```csharp
await using var command = dataSource.CreateCommand(sql);
using var reader = await command.ExecuteReaderAsync();
```

## Key Files & Naming
- **Consistent Naming**: Controller and models both use "Features" naming convention
- **DTO Inheritance**: `BaseFeatureDto` → `AddFeatureDto`/`UpdateFeatureDto`
- **Validation**: Both DTOs and domain models validate independently

## Endpoints
- `POST /api/features` - Single feature
- `POST /api/features/addMultiple` - Batch add (limit: 25)
- `GET /api/features` - List with optional pagination
- `PUT /api/features/{id}` - Update
- `DELETE /api/features/{id}` - Remove
