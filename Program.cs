using AgroScan.API.Data;
using AgroScan.API.Services;
using AgroScan.API.Middleware;
using AgroScan.API.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Elmah.Io.AspNetCore;
using FluentMigrator.Runner;
using Hangfire;
using Hangfire.SqlServer;
using Serilog;
using AgroScan.API.Filters;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog (simplified for faster startup)
// builder.Host.UseSerilog((context, configuration) =>
//     configuration.ReadFrom.Configuration(context.Configuration));

// Add services to the container.
builder.Services.AddControllers();

// Add ELMAH for error logging (simplified for faster startup)
// builder.Services.AddElmahIo(options =>
// {
//     options.ApiKey = builder.Configuration["ElmahIo:ApiKey"];
//     options.LogId = new Guid(builder.Configuration["ElmahIo:LogId"]!);
//     options.Application = "AgroScan API";
//     options.OnMessage = msg =>
//     {
//         msg.Version = "1.0.0";
//         msg.Application = "AgroScan API";
//     };
// });

// Add Entity Framework
builder.Services.AddDbContext<AgroScanDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services (core services only for faster startup)
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IInspectionService, InspectionService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IInspectionAnalysisService, InspectionAnalysisService>();
builder.Services.AddScoped<IFileService, FileService>();
// builder.Services.AddScoped<IBackgroundTaskService, BackgroundTaskService>();
// builder.Services.AddScoped<IMigrationService, MigrationService>();
// builder.Services.AddScoped<IActiveDirectoryService, ActiveDirectoryService>();

// Add utilities (core utilities only)
builder.Services.AddScoped<IFileUtility, FileUtility>();
builder.Services.AddScoped<IWebApiUtility, WebApiUtility>();
builder.Services.AddScoped<IEmailUtility, EmailUtility>();

// Add HttpClient
builder.Services.AddHttpClient();

// Add FluentMigrator (simplified for faster startup)
// builder.Services.AddFluentMigratorCore()
//     .ConfigureRunner(rb => rb
//         .AddSqlServer()
//         .WithGlobalConnectionString(builder.Configuration.GetConnectionString("DefaultConnection"))
//         .ScanIn(typeof(Program).Assembly).For.Migrations())
//     .AddLogging(lb => lb.AddFluentMigratorConsole());

// Add Hangfire (commented out due to package compatibility issues)
// builder.Services.AddHangfire(configuration => configuration
//     .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
//     .UseSimpleAssemblyNameTypeSerializer()
//     .UseRecommendedSerializerSettings()
//     .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
//     {
//         CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
//         SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
//         QueuePollInterval = TimeSpan.Zero,
//         UseRecommendedIsolationLevel = true,
//         DisableGlobalLocks = true
//     }));

// builder.Services.AddHangfireServer();

// Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AgroScan API",
        Version = "v1",
        Description = "AI-powered plant inspection and analysis API"
    });

    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AgroScan API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
}

// Configure HTTPS redirection (only in production or when HTTPS is available)
if (!app.Environment.IsDevelopment() || app.Configuration.GetValue<int>("HttpsRedirection:HttpsPort") > 0)
{
    app.UseHttpsRedirection();
}
app.UseCors("AllowAll");

// Add ELMAH middleware for error logging (commented out for faster startup)
// app.UseElmahIo();

// Add global exception handling middleware
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<ProblemDetailsMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles(); // Enable serving static files for images

// Add Hangfire dashboard (commented out due to package compatibility issues)
// app.UseHangfireDashboard("/hangfire", new DashboardOptions
// {
//     Authorization = new[] { new HangfireAuthorizationFilter() }
// });

app.MapControllers();

// Ensure database is created and seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AgroScanDbContext>();
    context.Database.Migrate();
    await SeedDataService.SeedDataAsync(context);
}

app.Run();
