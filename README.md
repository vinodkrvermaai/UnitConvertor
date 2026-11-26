# Unit Conversion API

ASP.NET Core 8 REST API that converts numeric values between **length**, **temperature**, and **weight** units. Built for a technical assessment with **Clean Architecture**, **SOLID**, and common **design patterns** (Strategy, Factory, Dependency Inversion).

Unit symbols use full lowercase names (e.g. `meter`, `celsius`, `kilogram`).

## Solution structure

```
UnitConversion/
├── src/
│   ├── UnitConversion.Domain/          # Entities, enums (no dependencies)
│   ├── UnitConversion.Application/     # Use cases, strategies, validation, DTOs
│   ├── UnitConversion.Infrastructure/  # Hardcoded unit catalog (IUnitCatalog)
│   └── UnitConversion.API/             # HTTP, middleware, composition root
├── tests/
│   ├── UnitConversion.UnitTests/       # NUnit
│   └── UnitConversion.IntegrationTests/  # WebApplicationFactory + NUnit
└── UnitConversion.sln
```

**Dependency flow:** API → Application + Infrastructure → Application → Domain.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

## Run locally

```powershell
cd D:\UnitConversion
dotnet restore
dotnet run --project src\UnitConversion.API
```

- Swagger (Development): see console URL (e.g. `https://localhost:7xxx/swagger`)
- Health: `GET /health`

## API endpoints

| Method | Route | Description |
|--------|--------|-------------|
| GET | `/api/v1/conversion?value=&from=&to=` | Convert units |
| GET | `/api/v1/units` | List units (optional `?category=Length`) |
| GET | `/api/v1/categories` | List categories |

### Examples

```http
GET /api/v1/conversion?value=1&from=meter&to=foot
GET /api/v1/conversion?value=100&from=celsius&to=fahrenheit
```

## Design highlights

| Principle / pattern | Implementation |
|---------------------|----------------|
| **DIP** | `IUnitCatalog` in Application; `HardcodedUnitCatalog` in Infrastructure |
| **Strategy** | `LengthConversionStrategy`, `WeightConversionStrategy`, `TemperatureConversionStrategy` |
| **Factory** | `ConversionStrategyFactory` selects strategy by `MeasurementCategory` |
| **Validation** | `ConversionRequestValidator` (FluentValidation) → HTTP **400** |
| **Errors** | `GlobalExceptionMiddleware` → unexpected **500** + trace id |
| **Logging** | Serilog → console + daily file; conversion audit via `ILogger` in `UnitConversionService` |

## Logging

- **Console** while running `dotnet run --project src\UnitConversion.API`
- **Daily files:** `src/UnitConversion.API/logs/unitconversion-YYYYMMDD.log` (31-day retention)
- **Logged:** conversion start/complete (value, units, result, duration); unhandled errors (`GlobalExceptionMiddleware`)
- **Not logged as conversion audit:** FluentValidation **400** (unknown unit, cross-category, etc.)
- **`GET /health`** is separate from log files

**Conversion math:** length/weight use linear factors to base (meter, kg). Temperature converts via a **Celsius hub** (affine coefficients). Results rounded to **6** decimal places.

**No database** — units are hardcoded in [`HardcodedUnitCatalog`](src/UnitConversion.Infrastructure/Data/HardcodedUnitCatalog.cs). To scale to hundreds of units, replace or complement `IUnitCatalog` with a JSON/DB-backed implementation without changing Application services.

## Tests

```powershell
dotnet test
```

- **Unit tests:** strategies, factory, conversion service, validator, catalog
- **Integration tests:** GET conversion, validation failures, health, units list

## Supported units (summary)

| Category | Symbols (ascending / display order) |
|----------|-------------------------------------|
| Length | micrometer … nautical mile |
| Weight | milligram … metricton |
| Temperature | celsius, fahrenheit, kelvin |

## Technology

.NET 8, ASP.NET Core Web API, FluentValidation, Swagger, Serilog (console + daily file), NUnit, `Microsoft.AspNetCore.Mvc.Testing`.

**Not used:** Entity Framework, SQL Server, MediatR, AutoMapper.
