using AgroScan.API.Data;
using AgroScan.API.Models;
using BCrypt.Net;

namespace AgroScan.API.Services
{
    public static class SeedDataService
    {
        public static async Task SeedDataAsync(AgroScanDbContext context)
        {
            // Check if data already exists
            if (context.Users.Any())
            {
                return;
            }

            // Create sample users
            var users = new List<User>
            {
                new User
                {
                    FirstName = "Jane",
                    LastName = "Doe",
                    Role = "farmer",
                    Email = "jane.doe@example.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("password123"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
                    FirstName = "John",
                    LastName = "Smith",
                    Role = "admin",
                    Email = "john.smith@example.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
                    FirstName = "Alice",
                    LastName = "Johnson",
                    Role = "researcher",
                    Email = "alice.johnson@example.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("researcher123"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            context.Users.AddRange(users);
            await context.SaveChangesAsync();

            // Create sample inspections
            var inspections = new List<Inspection>
            {
                new Inspection
                {
                    PlantName = "Tomato Plant",
                    InspectionDate = new DateTime(2025, 9, 9),
                    Country = "Australia",
                    State = "NT",
                    City = "Darwin",
                    Notes = "Saw some yellowing on the lower leaves and a few dark spots. Plant seems a bit weaker than the others in the same row.",
                    UserId = users[0].Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Inspection
                {
                    PlantName = "Corn Stalk",
                    InspectionDate = new DateTime(2025, 9, 9),
                    Country = "Australia",
                    State = "NT",
                    City = "Darwin",
                    Notes = "Healthy corn plant with good growth.",
                    UserId = users[0].Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Inspection
                {
                    PlantName = "Bell Pepper",
                    InspectionDate = new DateTime(2025, 9, 9),
                    Country = "Australia",
                    State = "NT",
                    City = "Darwin",
                    Notes = "Pepper plant showing good health and fruit development.",
                    UserId = users[0].Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Inspection
                {
                    PlantName = "Tomato Plant",
                    InspectionDate = new DateTime(2025, 9, 11),
                    Country = "Australia",
                    State = "NT",
                    City = "Darwin",
                    Notes = "Another tomato plant inspection.",
                    UserId = users[0].Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            context.Inspections.AddRange(inspections);
            await context.SaveChangesAsync();

            // Create sample inspection analyses
            var analyses = new List<InspectionAnalysis>
            {
                new InspectionAnalysis
                {
                    InspectionId = inspections[0].Id,
                    Status = "At Risk",
                    ConfidenceScore = 92.5m,
                    Description = "Early blight is a common fungal disease that affects tomatoes. It's caused by the fungus Alternaria solani and typically appears on older leaves first as small, dark lesions which can enlarge and form a 'bull's-eye' pattern.",
                    TreatmentRecommendation = "1. Remove & Destroy: Immediately prune and destroy affected lower leaves to prevent the fungus from spreading. Do not compost them.\n2. Improve Airflow: Ensure adequate spacing between plants to promote air circulation, which helps leaves dry faster and reduces fungal growth.\n3. Fungicide Application: Apply a fungicide containing copper or chlorothalonil, following the product's instructions carefully, especially on new growth.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new InspectionAnalysis
                {
                    InspectionId = inspections[1].Id,
                    Status = "Healthy",
                    ConfidenceScore = 95.0m,
                    Description = "The corn plant appears to be in excellent health with no visible signs of disease or pest damage.",
                    TreatmentRecommendation = "Continue current care practices. Monitor regularly for any changes in plant health.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new InspectionAnalysis
                {
                    InspectionId = inspections[2].Id,
                    Status = "Healthy",
                    ConfidenceScore = 88.0m,
                    Description = "The bell pepper plant shows healthy growth with good fruit development and no signs of disease.",
                    TreatmentRecommendation = "Maintain current watering and fertilization schedule. Ensure adequate sunlight exposure.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new InspectionAnalysis
                {
                    InspectionId = inspections[3].Id,
                    Status = "Healthy",
                    ConfidenceScore = 90.0m,
                    Description = "This tomato plant appears healthy with no visible signs of disease or pest issues.",
                    TreatmentRecommendation = "Continue regular monitoring and maintain proper care practices.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            context.InspectionAnalyses.AddRange(analyses);
            await context.SaveChangesAsync();
        }

        public static async Task SeedAnalyticsDataAsync(AgroScanDbContext context, int userId = 5)
        {
            // Check if user exists
            var user = await context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException($"User with ID {userId} not found");
            }

            // Check if analytics data already exists for this user
            if (context.Inspections.Any(i => i.UserId == userId))
            {
                return; // Data already exists
            }

            var random = new Random();
            var plantNames = new[] { "Tomato", "Corn", "Bell Pepper", "Wheat", "Soybean", "Lettuce", "Carrot", "Potato", "Onion", "Cucumber" };
            var statuses = new[] { "Healthy", "At Risk", "Alert", "Diseased" };
            var countries = new[] { "Australia", "USA", "Canada", "UK", "Germany" };
            var states = new[] { "NT", "WA", "SA", "QLD", "NSW", "VIC", "TAS", "CA", "TX", "FL", "NY", "WA" };
            var cities = new[] { "Darwin", "Perth", "Adelaide", "Brisbane", "Sydney", "Melbourne", "Hobart", "Los Angeles", "Houston", "Miami", "New York", "Seattle" };

            var inspections = new List<Inspection>();
            var analyses = new List<InspectionAnalysis>();

            // Generate 200 inspection records
            for (int i = 0; i < 200; i++)
            {
                var inspectionDate = DateTime.UtcNow.AddDays(-random.Next(0, 365)); // Random date within last year
                var plantName = plantNames[random.Next(plantNames.Length)];
                var country = countries[random.Next(countries.Length)];
                var state = states[random.Next(states.Length)];
                var city = cities[random.Next(cities.Length)];

                var inspection = new Inspection
                {
                    PlantName = plantName,
                    InspectionDate = inspectionDate,
                    Country = country,
                    State = state,
                    City = city,
                    Notes = GenerateRandomNotes(plantName, random),
                    UserId = userId,
                    CreatedAt = inspectionDate,
                    UpdatedAt = inspectionDate
                };

                inspections.Add(inspection);
            }

            context.Inspections.AddRange(inspections);
            await context.SaveChangesAsync();

            // Generate corresponding analysis records
            foreach (var inspection in inspections)
            {
                var status = statuses[random.Next(statuses.Length)];
                var confidence = random.Next(60, 100);
                var analysis = new InspectionAnalysis
                {
                    InspectionId = inspection.Id,
                    Status = status,
                    ConfidenceScore = confidence,
                    Description = GenerateDescription(status, inspection.PlantName),
                    TreatmentRecommendation = GenerateTreatmentRecommendation(status),
                    CreatedAt = inspection.CreatedAt,
                    UpdatedAt = inspection.UpdatedAt
                };

                analyses.Add(analysis);
            }

            context.InspectionAnalyses.AddRange(analyses);
            await context.SaveChangesAsync();
        }

        private static string GenerateRandomNotes(string plantName, Random random)
        {
            var noteTemplates = new[]
            {
                $"Regular inspection of {plantName} plant. No visible issues detected.",
                $"Weekly check on {plantName}. Plant appears healthy and growing well.",
                $"Noticed some discoloration on {plantName} leaves. Monitoring closely.",
                $"{plantName} plant showing good growth and development.",
                $"Routine inspection of {plantName}. All systems normal.",
                $"Found minor pest activity on {plantName}. Treatment applied.",
                $"{plantName} plant responding well to recent care changes.",
                $"Seasonal inspection of {plantName}. Preparing for harvest.",
                $"Daily monitoring of {plantName}. Weather conditions favorable.",
                $"Comprehensive check of {plantName}. Overall health good."
            };

            return noteTemplates[random.Next(noteTemplates.Length)];
        }

        private static string GenerateDescription(string status, string plantName)
        {
            return status switch
            {
                "Healthy" => $"The {plantName} plant appears to be in excellent health with no visible signs of disease, pest damage, or nutrient deficiencies. Growth is vigorous and development is on track.",
                "At Risk" => $"The {plantName} plant shows early warning signs that require attention. Some minor issues have been detected that could develop into more serious problems if left untreated.",
                "Alert" => $"The {plantName} plant requires immediate attention. Significant issues have been identified that need prompt intervention to prevent further damage or loss.",
                "Diseased" => $"The {plantName} plant shows clear signs of disease or serious health problems. Immediate treatment is necessary to save the plant and prevent spread to other plants.",
                _ => $"The {plantName} plant has been inspected and analyzed for health status."
            };
        }

        private static string GenerateTreatmentRecommendation(string status)
        {
            return status switch
            {
                "Healthy" => "Continue current care practices. Maintain regular monitoring schedule. Ensure adequate water, nutrients, and sunlight.",
                "At Risk" => "1. Increase monitoring frequency to daily checks. 2. Apply preventive treatments as recommended. 3. Adjust environmental conditions if needed. 4. Consult with agricultural specialist if symptoms worsen.",
                "Alert" => "1. Isolate plant immediately to prevent spread. 2. Apply targeted treatment within 24 hours. 3. Remove affected areas carefully. 4. Monitor closely for improvement. 5. Consider professional consultation.",
                "Diseased" => "1. Quarantine plant immediately. 2. Apply emergency treatment protocol. 3. Remove severely affected parts. 4. Disinfect tools and equipment. 5. Seek professional agricultural advice. 6. Consider plant replacement if recovery unlikely.",
                _ => "Follow standard plant care guidelines and monitor regularly."
            };
        }
    }
}
