using AgroScan.API.DTOs;
using CsvHelper;
using CsvHelper.Configuration;
using ClosedXML.Excel;
using System.Globalization;
using System.Text;

namespace AgroScan.API.Services
{
    public class FileService : IFileService
    {
        private readonly ILogger<FileService> _logger;

        public FileService(ILogger<FileService> logger)
        {
            _logger = logger;
        }

        public async Task<byte[]> ExportInspectionsToCsvAsync(IEnumerable<InspectionDto> inspections)
        {
            try
            {
                using var memoryStream = new MemoryStream();
                using var writer = new StreamWriter(memoryStream, Encoding.UTF8);
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

                // Configure CSV mapping
                csv.Context.RegisterClassMap<InspectionDtoMap>();
                
                await csv.WriteRecordsAsync(inspections);
                await writer.FlushAsync();

                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting inspections to CSV");
                throw;
            }
        }

        public async Task<byte[]> ExportInspectionsToExcelAsync(IEnumerable<InspectionDto> inspections)
        {
            try
            {
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Inspections");

                // Add headers
                worksheet.Cell(1, 1).Value = "Id";
                worksheet.Cell(1, 2).Value = "Plant Name";
                worksheet.Cell(1, 3).Value = "Inspection Date";
                worksheet.Cell(1, 4).Value = "Country";
                worksheet.Cell(1, 5).Value = "City";
                worksheet.Cell(1, 6).Value = "Notes";
                worksheet.Cell(1, 7).Value = "Status";
                worksheet.Cell(1, 8).Value = "Confidence Score";

                // Style headers
                var headerRange = worksheet.Range(1, 1, 1, 8);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

                // Add data
                int row = 2;
                foreach (var inspection in inspections)
                {
                    worksheet.Cell(row, 1).Value = inspection.Id;
                    worksheet.Cell(row, 2).Value = inspection.PlantName;
                    worksheet.Cell(row, 3).Value = inspection.InspectionDate;
                    worksheet.Cell(row, 4).Value = inspection.Country;
                    worksheet.Cell(row, 5).Value = inspection.City;
                    worksheet.Cell(row, 6).Value = inspection.Notes;
                    worksheet.Cell(row, 7).Value = inspection.Analysis?.Status ?? "Unknown";
                    worksheet.Cell(row, 8).Value = inspection.Analysis?.ConfidenceScore ?? 0;
                    row++;
                }

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();

                using var memoryStream = new MemoryStream();
                workbook.SaveAs(memoryStream);
                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting inspections to Excel");
                throw;
            }
        }

        public async Task<IEnumerable<InspectionDto>> ImportInspectionsFromCsvAsync(Stream csvStream)
        {
            try
            {
                using var reader = new StreamReader(csvStream);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                csv.Context.RegisterClassMap<InspectionDtoMap>();
                var inspections = csv.GetRecords<InspectionDto>().ToList();

                _logger.LogInformation("Successfully imported {Count} inspections from CSV", inspections.Count);
                return inspections;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing inspections from CSV");
                throw;
            }
        }

        public async Task<byte[]> ExportUsersToCsvAsync(IEnumerable<UserDto> users)
        {
            try
            {
                using var memoryStream = new MemoryStream();
                using var writer = new StreamWriter(memoryStream, Encoding.UTF8);
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

                csv.Context.RegisterClassMap<UserDtoMap>();
                await csv.WriteRecordsAsync(users);
                await writer.FlushAsync();

                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting users to CSV");
                throw;
            }
        }

        public async Task<byte[]> ExportUsersToExcelAsync(IEnumerable<UserDto> users)
        {
            try
            {
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Users");

                // Add headers
                worksheet.Cell(1, 1).Value = "Id";
                worksheet.Cell(1, 2).Value = "Email";
                worksheet.Cell(1, 3).Value = "First Name";
                worksheet.Cell(1, 4).Value = "Last Name";
                worksheet.Cell(1, 5).Value = "Created At";

                // Style headers
                var headerRange = worksheet.Range(1, 1, 1, 5);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGreen;

                // Add data
                int row = 2;
                foreach (var user in users)
                {
                    worksheet.Cell(row, 1).Value = user.Id;
                    worksheet.Cell(row, 2).Value = user.Email;
                    worksheet.Cell(row, 3).Value = user.FirstName;
                    worksheet.Cell(row, 4).Value = user.LastName;
                    worksheet.Cell(row, 5).Value = user.CreatedAt;
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using var memoryStream = new MemoryStream();
                workbook.SaveAs(memoryStream);
                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting users to Excel");
                throw;
            }
        }

        public async Task<IEnumerable<UserDto>> ImportUsersFromCsvAsync(Stream csvStream)
        {
            try
            {
                using var reader = new StreamReader(csvStream);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                csv.Context.RegisterClassMap<UserDtoMap>();
                var users = csv.GetRecords<UserDto>().ToList();

                _logger.LogInformation("Successfully imported {Count} users from CSV", users.Count);
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing users from CSV");
                throw;
            }
        }
    }

    // CSV mapping configurations
    public class InspectionDtoMap : ClassMap<InspectionDto>
    {
        public InspectionDtoMap()
        {
            Map(m => m.Id).Name("Id");
            Map(m => m.PlantName).Name("Plant Name");
            Map(m => m.InspectionDate).Name("Inspection Date");
            Map(m => m.Country).Name("Country");
            Map(m => m.City).Name("City");
            Map(m => m.Notes).Name("Notes");
            Map(m => m.Analysis.Status).Name("Status");
            Map(m => m.Analysis.ConfidenceScore).Name("Confidence Score");
        }
    }

    public class UserDtoMap : ClassMap<UserDto>
    {
        public UserDtoMap()
        {
            Map(m => m.Id).Name("Id");
            Map(m => m.Email).Name("Email");
            Map(m => m.FirstName).Name("First Name");
            Map(m => m.LastName).Name("Last Name");
            Map(m => m.CreatedAt).Name("Created At");
        }
    }
}
