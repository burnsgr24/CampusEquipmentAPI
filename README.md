
# Campus Equipment API

## Overview
This project is a REST API for managing campus equipment. It was built using .NET 10, Entity Framework Core 10, and SQLite.

The API supports creating, viewing, updating, and deleting equipment records.

## Technologies
-.NET 10
-ASP.NET Core Minimal API
-Entity Framework Core 10
-SQLite
-Visual Studio Code REST Client

## Equipment Model
Each equipment record contains:

-EquipmentItemId: Integer primary key
-Name: String, maximum 100 characters
-Category: String, maximum 50 characters
-IsAvailable: Boolean indicating availability

The API uses a request DTO to receive input. The database-generated ID is not included in the request DTO.

## API Endpoints
-GET /equipment - Get all equipment
-GET /equipment/{id} - Get equipment by ID
-POST /equipment - Create equipment
-PUT /equipment/{id} - Update equipment
-DELETE /equipment/{id} - Delete equipment

## Validation
-Name is required and cannot be whitespace.
-Name cannot exceed 100 characters.
-Category is required and cannot be whitespace.
-Category cannot exceed 50 characters.
-IsAvailable must be explicitly provided.

Invalid requests return HTTP 400 Bad Request.

## Database and Seeding
The project uses SQLite with the database file campus-equipment.db.

The database is seeded with three records if the equipment table is empty:

1.Dell Latitude Laptop - Computers - Available
2.Epson Projector - Presentation - Available
3.Canon Camera - Photography - Unavailable

Seeding is repeat-safe and does not add duplicate records when the database update command is run again.

## Running the Project

Requirements:
-.NET 10 SDK
-Entity Framework Core tools

Commands:

dotnet --version
dotnet tool restore
dotnet restore
dotnet build
dotnet ef database update
dotnet run --no-launch-profile --urls http://localhost:5050

The API will be available at http://localhost:5050.

## Testing
HTTP requests are included in CampusEquipmentAPI.http and can be executed using the REST Client extension in Visual Studio Code.

Tests cover:
-Retrieving all equipment
-Retrieving equipment by ID
-Handling nonexistent equipment
-Creating equipment
-Updating equipment
-Deleting equipment
-Rejecting invalid POST requests
-Rejecting invalid PUT requests
-Verifying invalid updates do not modify existing records
-Verifying deleted records return 404
-Verifying database persistence after restarting the API
-Verifying repeated database updates do not duplicate seeded records

## Implementation Notes
The API uses dependency injection to provide an EquipmentDbContext to endpoint handlers.

Entity Framework Core tracks changes to equipment records. The PUT endpoint updates the retrieved entity and uses SaveChangesAsync() to persist changes to SQLite.

Migrations are generated with dotnet ef migrations add and applied with dotnet ef database update. Generating a migration does not automatically apply it to the database.

The request DTO separates incoming API data from the database entity and prevents clients from supplying the database-generated primary key.

## Implementation Questions

1.How do a request DTO and an entity differ, and why is EquipmentItemId absent from the DTO?

-The DTO represents the information a client sends to the API, but the entity represents the equipment record stored in the database. For EquipmentItemID, it is left out of the DTO because the database generates the ID when a new record is created.

2.How do DI, DbContext, and SaveChangesAsync work together for a PUT?

-Dependency injection provides the endpoint with an EquipmentDbContext. The DbContext finds and tracks the equipment record. The PUT endpoint changes its properties, and SaveChangesAsync saves those changes to the SQLite database.

3.How does generating a migration differ from applying it?

-Generating a migration creates files describing changes to the database structure. Applying a migration uses those files to make the actual changes to the database. The commands are dotnet ef migrations add and dotnet ef database update.

4.Why does a second database update not duplicate seed rows?

-The seeding code checks whether the equipment table already contains records before adding anything. If records already exist, it skips seeding, preventing duplicate rows when the database is updated again.

5.What does your restart check prove that one successful GET does not?

-A successful GET only proves that the API can retrieve data while it is running. Retrieving the same data after restarting the API proves that the information was saved in the SQLite database and was not just stored temporarily in memory.

6.Why must this API validate string lengths even though the EF model declares maximum lengths?

-The API needs to check string lengths before saving data so it can reject invalid requests with a clear HTTP 400 response. The EF model defines database limits, but it does not replace the API's own input validation.