# AgroScan API

A comprehensive plant health analysis API built with .NET 9, featuring AI-powered plant inspection, background processing, and enterprise-grade features.

## 🚀 Features

### Core Features
- **Plant Health Analysis**: Upload images for AI-powered plant health diagnosis
- **User Management**: JWT-based authentication and authorization
- **Inspection Management**: Track and manage plant inspections
- **Image Processing**: Handle and store plant images

### Enterprise Features
- **File I/O Operations**: CSV/Excel import/export for data management
- **Background Processing**: Queue-based task processing with Hangfire
- **Structured Logging**: Comprehensive logging with Serilog
- **Database Migrations**: FluentMigrator for schema management
- **Active Directory Integration**: LDAP authentication support
- **Email Services**: Automated email notifications
- **Web API Utilities**: HTTP client services for external integrations

## 🛠️ Technology Stack

- **.NET 9**: Latest .NET framework
- **Entity Framework Core**: ORM for database operations
- **SQL Server**: Primary database
- **JWT Authentication**: Secure API access
- **Hangfire**: Background job processing
- **FluentMigrator**: Database migrations
- **Serilog**: Structured logging
- **Swagger/OpenAPI**: API documentation
- **Elmah.Io**: Error logging and monitoring

## 📋 Prerequisites

- .NET 9 SDK
- SQL Server 2019 or later
- Visual Studio 2022 or VS Code
- Git

## 🚀 Getting Started

### 1. Clone the Repository
```bash
git clone https://github.com/your-org/agroscan-api.git
cd agroscan-api
```

### 2. Database Setup

#### Option A: SQL Server (Recommended)
1. Install SQL Server 2019 or later
2. Create a database named `AgroScanDB`
3. Update connection string in `appsettings.json`

#### Option B: SQLite (Development)
1. No additional setup required
2. Database file will be created automatically

### 3. Configuration

#### Update appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AgroScanDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "Jwt": {
    "Key": "YourSecretKeyHere",
    "Issuer": "AgroScanAPI",
    "Audience": "AgroScanUsers"
  },
  "Email": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": "587",
    "EnableSsl": "true",
    "Username": "your-email@gmail.com",
    "Password": "your-app-password",
    "FromAddress": "noreply@agroscan.com"
  },
  "ActiveDirectory": {
    "Domain": "your-domain.com",
    "Container": "CN=Users,DC=your-domain,DC=com"
  },
  "ElmahIo": {
    "ApiKey": "your-elmah-api-key",
    "LogId": "your-log-id"
  }
}
```

### 4. Install Dependencies
```bash
dotnet restore
```

### 5. Run Database Migrations
```bash
# Using Entity Framework migrations
dotnet ef database update

# Using FluentMigrator (alternative)
dotnet run --migrate
```

### 6. Run the Application
```bash
dotnet run
```

The API will be available at:
- **API**: `https://localhost:7001`
- **Swagger UI**: `https://localhost:7001/swagger`
- **Hangfire Dashboard**: `https://localhost:7001/hangfire`

## 📁 Project Structure

```
AgroScan.API/
├── Controllers/           # API Controllers
├── Data/                 # Database context and models
├── DTOs/                 # Data Transfer Objects
├── Services/             # Business logic services
├── Utilities/            # Utility classes
├── Middleware/           # Custom middleware
├── Migrations/           # Database migrations
├── Models/               # Entity models
├── Exceptions/           # Custom exceptions
└── Filters/              # Action filters
```

## 🔧 Development Setup

### Environment Configuration

#### Development
```bash
# Set environment
export ASPNETCORE_ENVIRONMENT=Development

# Run with hot reload
dotnet watch run
```

#### Production
```bash
# Set environment
export ASPNETCORE_ENVIRONMENT=Production

# Build and run
dotnet build --configuration Release
dotnet run --configuration Release
```

### Database Migrations

#### Entity Framework Migrations
```bash
# Add new migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Remove last migration
dotnet ef migrations remove
```

#### FluentMigrator Migrations
```bash
# Run migrations
dotnet run --migrate

# Rollback migration
dotnet run --rollback --version 20250101000001
```

## 🧪 Testing

### Unit Tests
```bash
dotnet test
```

### API Testing
Use the provided `test-api.http` file or import the OpenAPI specification into your preferred API testing tool.

### Load Testing
```bash
# Install NBomber
dotnet tool install -g NBomber

# Run load tests
nbomber run --scenario load-test
```

## 📊 Monitoring and Logging

### Structured Logging
- **Console**: Development logging
- **File**: Daily log files in `logs/` directory
- **Seq**: Centralized logging (optional)

### Error Monitoring
- **Elmah.Io**: Error tracking and monitoring
- **Hangfire Dashboard**: Background job monitoring

### Health Checks
```bash
# Health check endpoint
curl https://localhost:7001/health
```

## 🚀 Deployment

### Docker Deployment
```bash
# Build Docker image
docker build -t agroscan-api .

# Run container
docker run -p 7001:7001 agroscan-api
```

### IIS Deployment
1. Publish the application
2. Configure IIS with .NET Core hosting bundle
3. Set up application pool
4. Configure web.config

### Azure Deployment
```bash
# Deploy to Azure App Service
az webapp deployment source config-zip --resource-group myResourceGroup --name myAppName --src ./publish.zip
```

## 🔐 Security

### Authentication
- JWT Bearer tokens
- Active Directory integration
- Role-based authorization

### Security Headers
- HTTPS enforcement
- CORS configuration
- Request validation

### Data Protection
- Password hashing with BCrypt
- SQL injection prevention
- XSS protection

## 📈 Performance

### Caching
- In-memory caching for frequently accessed data
- Response caching for static content

### Background Processing
- Hangfire for long-running tasks
- Queue-based processing
- Retry mechanisms

### Database Optimization
- Indexed queries
- Connection pooling
- Query optimization

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests
5. Submit a pull request

### Code Style
- Follow C# coding conventions
- Use meaningful variable names
- Add XML documentation
- Write unit tests

## 📝 API Documentation

### Authentication
All endpoints require JWT authentication except:
- `POST /api/auth/login`
- `POST /api/auth/register`
- `GET /api/health`

### Rate Limiting
- 100 requests per minute per user
- 1000 requests per hour per IP

### Error Handling
All errors return consistent problem detail responses:
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.6.1",
  "title": "An error occurred",
  "status": 400,
  "detail": "Error description",
  "instance": "/api/endpoint",
  "traceId": "trace-id",
  "timestamp": "2025-01-01T00:00:00Z"
}
```

## 🆘 Troubleshooting

### Common Issues

#### Database Connection
```bash
# Check connection string
# Verify SQL Server is running
# Check firewall settings
```

#### Authentication Issues
```bash
# Verify JWT configuration
# Check token expiration
# Validate user credentials
```

#### Background Jobs
```bash
# Check Hangfire dashboard
# Verify database connection
# Check job queue status
```

### Logs Location
- **Application Logs**: `logs/agroscan-{date}.txt`
- **Error Logs**: Elmah.Io dashboard
- **Background Jobs**: Hangfire dashboard

## 📞 Support

- **Documentation**: [API Docs](https://localhost:7001/swagger)
- **Issues**: [GitHub Issues](https://github.com/your-org/agroscan-api/issues)
- **Email**: support@agroscan.com

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- .NET team for the excellent framework
- Entity Framework team for the ORM
- Hangfire team for background processing
- Serilog team for structured logging