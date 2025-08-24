using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.Core.Services.Interfaces
{
    public interface IQuestionImportService
    {
        // Import operations
        Task<QuestionImportBatchDto> ImportQuestionsAsync(ImportQuestionsDto dto);
        // Controller-friendly import entrypoints (file + options)
    // Accept ImportFileDto + options for controller-friendly calls (controller uploads will be saved first)
    Task<QuestionImportBatchDto> ImportFromExcelAsync(Ikhtibar.Shared.DTOs.ImportFileDto file, Ikhtibar.Shared.DTOs.QuestionImportOptionsDto options);
    Task<QuestionImportBatchDto> ImportFromCsvAsync(Ikhtibar.Shared.DTOs.ImportFileDto file, Ikhtibar.Shared.DTOs.QuestionImportOptionsDto options);
    Task<QuestionImportBatchDto> ImportFromJsonAsync(Ikhtibar.Shared.DTOs.ImportFileDto file, Ikhtibar.Shared.DTOs.QuestionImportOptionsDto options);
    // Note: controllers should convert IFormFile to shared ImportFileDto before calling core services.
    // Add an overload that accepts the controller-facing QuestionImportDto
    Task<QuestionImportBatchDto> ImportQuestionsAsync(QuestionImportDto dto);
        Task<QuestionImportBatchDto> GetImportBatchAsync(int batchId);
        Task<IEnumerable<QuestionImportBatchDto>> GetImportBatchesAsync(ImportBatchFilterDto filter);
    Task<bool> CancelImportAsync(int batchId);
    Task<bool> CancelImportBatchAsync(int batchId);
        Task<bool> RetryFailedImportsAsync(int batchId);
        
        // Import processing
        Task<ImportProgressDto> GetImportProgressAsync(int batchId);
        Task<bool> ProcessImportBatchAsync(int batchId);
    Task<ImportValidationResult> ValidateImportFileAsync(ImportFileDto dto);
    Task<ImportPreviewDto> PreviewImportAsync(ImportFileDto dto);
    // Controller-friendly validate/preview accepting controller DTOs
    Task<ImportPreviewDto> PreviewImportAsync(Ikhtibar.Shared.DTOs.QuestionImportValidationDto dto);
    Task<bool> ValidateImportDataAsync(Ikhtibar.Shared.DTOs.QuestionImportValidationDto dto);
        
        // Export operations
        Task<ExportResultDto> ExportQuestionsAsync(Ikhtibar.Shared.DTOs.QuestionExportDto dto);
        Task<ExportResultDto> ExportToExcelAsync(Ikhtibar.Shared.DTOs.QuestionExportOptionsDto options);
        Task<ExportResultDto> ExportToCsvAsync(Ikhtibar.Shared.DTOs.QuestionExportOptionsDto options);
        Task<ExportResultDto> ExportToJsonAsync(Ikhtibar.Shared.DTOs.QuestionExportOptionsDto options);
        Task<ExportResultDto> ExportQuestionBankAsync(int questionBankId, ExportFormat format);
        Task<ExportResultDto> ExportQuestionsByFilterAsync(QuestionFilterDto filter, ExportFormat format);
        Task<ExportTemplateDto> GetExportTemplateAsync(ExportFormat format);
        
        // File handling
        Task<string> SaveImportFileAsync(string filePath, int userId);
        Task<bool> DeleteImportFileAsync(string filePath);
    Task<IEnumerable<SupportedFormatDto>> GetSupportedFormatsAsync();
    Task<IEnumerable<SupportedFormatDto>> GetSupportedImportFormatsAsync();
        Task<FormatValidationResult> ValidateFileFormatAsync(string filePath);
        
        // Import/Export configuration
        Task<ImportConfigurationDto> GetImportConfigurationAsync();
        Task<bool> UpdateImportConfigurationAsync(UpdateImportConfigurationDto dto);
        Task<ExportConfigurationDto> GetExportConfigurationAsync();
        Task<bool> UpdateExportConfigurationAsync(UpdateExportConfigurationDto dto);
    // Additional helpers
    Task<bool> ValidateImportDataAsync(Ikhtibar.Shared.DTOs.ImportPreviewDto preview);
    Task<ImportStatisticsDto> GetImportStatisticsAsync();
    Task<IEnumerable<QuestionImportLogDto>> GetImportBatchLogsAsync(int batchId);
    Task<QuestionImportTemplateDto> GetImportTemplateAsync(string format);
    }
}
