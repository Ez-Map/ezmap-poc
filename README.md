# EzMap PoC

EzMap PoC is a .NET 10 Web API for storing personal map data. It lets a user sign up, sign in, and manage three owned resource types:

- `Poi`: a point of interest with a `name` and `address`
- `Tag`: a label with a `name` and `description`
- `PoiCollection`: a saved collection of POIs and tags with a `viewType`

The application uses:

- ASP.NET Core Web API
- Entity Framework Core with SQL Server
- JWT bearer tokens for user identity
- FluentValidation for request validation
- Serilog with Seq for log shipping
- Elasticsearch for indexing create/update/delete operations

## Solution layout

- `EzMap.Api`: HTTP API, startup, controllers, middleware, infrastructure services
- `EzMap.Domain`: entities, DTOs, EF Core context/configuration, repositories, migrations
- `test/EzMap.IntegrationTest`: integration tests using `WebApplicationFactory`, SQLite in-memory, and a mocked Elasticsearch service

## Domain model

### User

- Identified by `Guid`
- Stores `DisplayName`, `UserName`, `Email`, and a BCrypt-hashed password
- Owns POIs, tags, and POI collections

### Poi

- Fields: `Id`, `Name`, `Address`, `UserId`
- Belongs to one user
- Can belong to many POI collections

### Tag

- Fields: `Id`, `Name`, `Description`, `UserId`, optional `ParentId`
- Belongs to one user
- Supports parent/child tag relationships
- Can belong to many POI collections

### PoiCollection

- Fields: `Id`, `Name`, `Description`, `ViewType`, `UserId`
- Belongs to one user
- Can contain many POIs and many tags
- `ViewType` enum values:
  - `Map = 1`
  - `List = 2`
  - `Grid = 3`

### Auditing and soft delete

All entities inherit from `EntityBase<Guid>` and track:

- `CreatedBy`, `CreatedDate`
- `LastModifiedBy`, `LastModifiedDate`
- `DeletedDate`

Deletes are soft deletes. EF query filters exclude rows where `DeletedDate != null`.

## How the app works

### Authentication flow

1. `POST /api/user/signup` creates a user.
2. Password complexity is checked in `IdentityService`.
3. Passwords are hashed with BCrypt.
4. `POST /api/user/signin` returns a JWT with the user id in the `NameIdentifier` claim.
5. Controllers use `IIdentityService` to read the current user id from the token claim.

### Data access pattern

- Controllers work through `IUnitOfWork`
- Repositories handle EF Core reads and writes
- `SaveAsync()` applies audit fields before persisting
- Create, update, and delete operations for POIs, tags, and collections wrap DB writes in a transaction and then mirror the change into Elasticsearch

## API summary

Swagger is enabled by default at `/swagger`.

### Auth

#### `POST /api/user/signup`

Creates a user.

Request body:

```json
{
  "userName": "alice",
  "password": "Strong!123",
  "email": "alice@example.com",
  "displayName": "Alice"
}
```

Password rules enforced by the app:

- required
- 8 to 16 characters
- at least one uppercase letter
- at least one lowercase letter
- at least one digit
- at least one special character matching ``[!@#$%^&*()\-+=<>?\|~`{}]``

#### `POST /api/user/signin`

Signs in and returns a JWT string.

Request body:

```json
{
  "username": "alice",
  "password": "Strong!123"
}
```

### Poi endpoints

All POI endpoints are intended to require a bearer token.

- `POST /api/poi`
  - body: `name`, `address`
- `PUT /api/poi/{id}`
  - body: `id`, `name`, `address`
- `DELETE /api/poi/{id}`
- `GET /api/poi/{id}`
- `GET /api/poi`
- `GET /api/poi/search?keyword=...`

Validation:

- `name`: required, max 50 chars
- `address`: required
- update `id`: required

Search behavior:

- Matches POIs owned by the current user
- Checks `name` or `address` using case-insensitive `Contains`

### Tag endpoints

All tag endpoints are intended to require a bearer token.

- `POST /api/tag`
  - body: `name`, `description`
- `PUT /api/tag/{id}`
  - body: `id`, `name`, `description`
- `DELETE /api/tag/{id}`
- `GET /api/tag/{id}`
- `GET /api/tag`
- `GET /api/tag/search?keyword=...`

Validation:

- `name`: required, max 50 chars
- `description`: required on create and update
- update `id`: required

Search behavior:

- Matches tags owned by the current user
- Current repository implementation effectively matches exact name equality through `keyword.ToLower().Contains(x.Name.ToLower())`

### PoiCollection endpoints

All collection endpoints are intended to require a bearer token.

- `POST /api/poicollection`
  - body: `name`, `description`
- `PUT /api/poicollection/{id}`
  - body: `id`, `name`, `description`, `viewType`, `pois`, `tags`
- `DELETE /api/poicollection/{id}`
- `GET /api/poicollection/{id}`
- `GET /api/poicollection`
- `GET /api/poicollection/search?keyword=...`

Validation:

- create `name`: required, max 100 chars
- create `description`: max 500 chars
- update `id`: required
- update `name`: required, max 100 chars
- update `description`: max 500 chars
- update `viewType`: required

Search behavior:

- Matches collections owned by the current user
- Searches collection `name`, `description`, POI names, and tag names

## Running locally

### Prerequisites

- .NET 10 SDK
- .NET 10 runtime if you want to execute the current integration tests locally
- SQL Server
- Elasticsearch 8.x
- Optional: Seq and Kibana

### App configuration

The API reads configuration from:

- `EzMap.Api/appsettings.json`
- `EzMap.Api/appsettings.{Environment}.json`
- environment variables

Important settings:

- `ConnectionStrings__myDb1`
- `AppSecret`
- `ELK__URl`

Example environment variables:

```bash
export ASPNETCORE_ENVIRONMENT=Development
export ConnectionStrings__myDb1="Server=localhost;Database=ezmap;User Id=sa;Password=Your_password123!;TrustServerCertificate=True"
export AppSecret="replace-with-a-long-random-secret"
export ELK__URl="http://localhost:9200"
```

### Run with `dotnet`

```bash
dotnet restore
dotnet run --project EzMap.Api
```

By launch profile, the development URLs are:

- `http://localhost:5156`
- `https://localhost:7124`

Swagger UI:

- `http://localhost:5156/swagger`
- `https://localhost:7124/swagger`

### Run with Docker Compose

The repo includes [`compose.yml`](/Users/bitum/RiderProjects/ezmap-poc/compose.yml) with:

- `api`
- `db` (SQL Server 2017)
- `seq`
- `es`
- `kibana`

Start the stack with:

```bash
docker compose -f compose.yml up --build
```

Published ports in the compose file:

- API HTTP: `5000`
- API HTTPS: `5003`
- Elasticsearch: `5001`
- Kibana: `5002`

## Testing

Run:

```bash
dotnet test ezmap-poc.sln
```

Test setup details:

- test environment is forced to `Test`
- SQL Server is replaced with in-memory SQLite
- Elasticsearch is replaced with a mock `IElasticSearchService`
- seed data includes one default user, POI, tag, and POI collection

Default seeded sign-in used by tests:

```json
{
  "username": "stringstring",
  "password": "stringstring"
}
```

## Current implementation notes

- The app targets `net10.0`.
- EF migrations are applied automatically on startup for non-test environments.
- Exception handling is centralized in custom middleware and returns a generic 500 payload.
- Deletes are soft deletes, not physical deletes.
- Integration tests were not re-run in this workspace after the migration because only the .NET 8 SDK/runtime is installed here; the solution now requires .NET 10 to build and run.
- The compose stack and production config are not fully aligned out of the box. `EzMap.Api/appsettings.Production.json` points `ELK:URl` at `http://localhost:5001/`, which is usually wrong from inside the API container. In Docker, this should typically be overridden to `http://es:9200`.
- The API is configured for JWT authentication, but the HTTP pipeline in [`Program.cs`](/Users/bitum/RiderProjects/ezmap-poc/EzMap.Api/Program.cs) does not currently call `UseAuthentication()` or `UseAuthorization()`.

## Useful source references

- Startup: [`Program.cs`](/Users/bitum/RiderProjects/ezmap-poc/EzMap.Api/Program.cs)
- Auth controller: [`UserController.cs`](/Users/bitum/RiderProjects/ezmap-poc/EzMap.Api/Controllers/UserController.cs)
- POI controller: [`PoiController.cs`](/Users/bitum/RiderProjects/ezmap-poc/EzMap.Api/Controllers/PoiController.cs)
- Tag controller: [`TagController.cs`](/Users/bitum/RiderProjects/ezmap-poc/EzMap.Api/Controllers/TagController.cs)
- Collection controller: [`PoiCollectionController.cs`](/Users/bitum/RiderProjects/ezmap-poc/EzMap.Api/Controllers/PoiCollectionController.cs)
- EF context: [`EzMapContext.cs`](/Users/bitum/RiderProjects/ezmap-poc/EzMap.Domain/Models/EzMapContext.cs)
- Test host: [`TestWebAppFactory.cs`](/Users/bitum/RiderProjects/ezmap-poc/test/EzMap.IntegrationTest/Infrastructure/TestWebAppFactory.cs)
