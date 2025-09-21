# AgroScan API

A .NET Web API backend for the AgroScan plant inspection and analysis application.

## Features

- **User Management**: Registration, authentication, and user profile management
- **Plant Inspections**: Create, read, update, and delete plant inspections
- **Image Upload**: Upload and manage plant images with local storage
- **AI Analysis**: Store and manage AI-powered plant disease analysis results
- **JWT Authentication**: Secure API endpoints with JWT tokens
- **Swagger UI**: Interactive API documentation

## Database Schema

### Users Table
- `id` (Primary Key)
- `first_name`
- `last_name`
- `role` (farmer, admin, researcher, student)
- `email`
- `password` (hashed)
- `created_at`
- `updated_at`

### Inspections Table
- `id` (Primary Key)
- `plant_name`
- `inspection_date`
- `country`
- `state`
- `city`
- `notes`
- `created_at`
- `updated_at`
- `user_id` (Foreign Key)

### Inspection Images Table
- `id` (Primary Key)
- `inspection_id` (Foreign Key)
- `image` (filename)
- `created_at`
- `updated_at`

### Inspection Analysis Table
- `inspection_id` (Primary Key, Foreign Key)
- `status`
- `confidence_score`
- `description`
- `treatment_recommendation`
- `created_at`
- `updated_at`

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### Installation

1. Clone the repository
2. Navigate to the project directory
3. Update the connection string in `appsettings.json` if needed
4. Run the application:

```bash
dotnet run
```

The API will be available at `https://localhost:7000` (or the configured port).

### Swagger UI

Once the application is running, navigate to the root URL to access the Swagger UI for interactive API documentation.

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login and get JWT token

### Users
- `GET /api/users` - Get all users (Admin only)
- `GET /api/users/{id}` - Get user by ID
- `GET /api/users/me` - Get current user profile
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user

### Inspections
- `GET /api/inspections` - Get all inspections with optional filtering
- `GET /api/inspections/my-inspections` - Get current user's inspections
- `GET /api/inspections/{id}` - Get inspection by ID
- `POST /api/inspections` - Create new inspection
- `PUT /api/inspections/{id}` - Update inspection
- `DELETE /api/inspections/{id}` - Delete inspection

### Images
- `POST /api/images/upload` - Upload plant image
- `GET /api/images/{id}` - Get image metadata
- `GET /api/images/inspection/{inspectionId}` - Get images for inspection
- `GET /api/images/file/{imageName}` - Download image file
- `DELETE /api/images/{id}` - Delete image

### Analysis
- `GET /api/inspectionanalysis/inspection/{inspectionId}` - Get analysis for inspection
- `POST /api/inspectionanalysis` - Create analysis
- `PUT /api/inspectionanalysis/inspection/{inspectionId}` - Update analysis
- `DELETE /api/inspectionanalysis/inspection/{inspectionId}` - Delete analysis

## Configuration

### JWT Settings
Update the JWT configuration in `appsettings.json`:

```json
{
  "Jwt": {
    "Key": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "AgroScanAPI",
    "Audience": "AgroScanUsers"
  }
}
```

### Database Connection
Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AgroScanDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

## Sample Data

The application includes seed data with:
- Sample users (farmer, admin, researcher)
- Sample plant inspections
- Sample AI analysis results

## Security

- Passwords are hashed using BCrypt
- JWT tokens for authentication
- CORS enabled for frontend integration
- Input validation and sanitization

## File Storage

Images are stored locally in the `wwwroot/images/inspections/` directory. The filename is stored in the database for reference.

## Development

### Adding New Features

1. Create models in the `Models` folder
2. Create DTOs in the `DTOs` folder
3. Create services in the `Services` folder
4. Create controllers in the `Controllers` folder
5. Update the DbContext if needed
6. Register services in `Program.cs`

### Database Migrations

To create a new migration:
```bash
dotnet ef migrations add MigrationName
```

To update the database:
```bash
dotnet ef database update
```

## License

This project is licensed under the MIT License.
