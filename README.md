# Dogshouseservice API

This is a REST API developed as a test assignment. The service provides functionality for managing a catalog of dogs using ASP.NET Core, EF Core, and a Code First approach.
The project demonstrates knowledge of REST principles, asynchronicity, design patterns (Repository, DI), unit testing, and error handling.

### Tech Stack
- C# / .NET 9 (ASP.NET Core Web API)
- Entity Framework Core (Code First)
- MS SQL Server (or other relational DB)
- xUnit & Moq: For Unit Testing
- Architectural Patterns: Repository Pattern, Dependency Injection (DI)

### Key Features
- API Endpoints: Methods for GET /ping, GET /dogs, and POST /dog.
- Sorting & Pagination: GET /dogs supports dynamic sorting (attribute, order) and pagination (pageNumber, pageSize).
- Advanced Validation: Server-side validation for POST /dog, including checks for unique names, negative values, and incorrect data.
- Rate Limiting: A built-in mechanism to protect the API, which returns 429 Too Many Requests in case of overload (as required by the task).
- Automatic Data Seeding: On first launch, the Dogs table is automatically populated with initial data ("Neo" and "Jessy") for demonstration.
- Test Coverage: Business logic (services) and API logic (controllers) are covered by Unit Tests.

## How to Run the Project
#### 1. Prerequisites
- .NET 9.0 SDK
- MS SQL Server (Express, LocalDB, or a full version)

#### 2. Clone the Repository
```
git clone https://[YOUR-REPO-URL].git
cd [PROJECT-FOLDER-NAME]
```

#### 3. Configure the Database Connection
- Open the file Dogshouseservice.WebAPI/appsettings.json.
- Find the ConnectionStrings section.
- Change the DefaultConnection to match your local SQL Server instance.

**Example for SQL Express (like yours):**
```JSON
"ConnectionStrings": {
  "DefaultConnection": "Server=IG-PC\\SQLEXPRESS;Database=DogsHouseDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

#### 4. Apply Migrations
This command will create the DogsHouseDb database and the Dogs table on your server.

**Open a terminal in the root folder of the solution and execute:**
```Bash
dotnet ef database update --project DogshouseService.DAL --startup-project DogshouseService.WebAPI
```

#### 5. Run the API
```Bash
cd DogshouseService.WebAPI
dotnet run
```
The service will be available at http://localhost:5123 or https://localhost:7123 (the address will be shown in the console).

## API Documentation (Endpoints)
### 1. Check Status
- **Method:** GET
- **Route:** /ping
- **Description:** Checks if the service is running.

**Success Response (200 OK):**
```
Dogshouseservice.Version1.0.1
```

#### 2. Get Dogs List
- **Method:** GET
- **Route:** /dogs
- **Description:** Retrieves a list of all dogs with support for sorting and pagination.

**Query Params:**
- attribute (string, optional): Field to sort by (e.g., weight, name).
- order (string, optional): Sort order (asc or desc).
- pageNumber (int, optional): Page number (default: 1).
- pageSize (int, optional): Number of items (default: 10).

**Example Request:** 
```curl
curl -X GET "https://localhost:7229/dogs?attribute=weight&order=desc"
```

**Success Response (200 OK):**
```JSON
[
  {
    "name": "Neo",
    "color": "red & amber",
    "tail_length": 22,
    "weight": 32
  },
  {
    "name": "Jessy",
    "color": "black & white",
    "tail_length": 7,
    "weight": 14
  }
]
```

#### 3. Create a Dog
- **Method:** POST
- **Route:** /dog
- **Description:** Creates a new dog.

**Request Body:**
```JSON
{
  "name": "Sparky",
  "color": "brown",
  "tail_length": 15,
  "weight": 20
}
```
**Success Response (201 Created):** Returns the created object.

**Errors:**
- 400 Bad Request: Invalid data (e.g., tail_length = -5 or missing name).
- 409 Conflict: A dog with this name already exists.


## Testing
The project includes Unit Tests for services and controllers.

**To run the tests, navigate to the test project folder and execute:**

```Bash
cd DogshouseService.Tests 
dotnet test
```


