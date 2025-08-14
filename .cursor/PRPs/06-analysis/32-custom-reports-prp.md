# Custom Reports System PRP

## 🎯 Goal
Implement a flexible and powerful custom reports system that allows users to create, save, schedule, and distribute detailed reports with customizable templates, data sources, and output formats.

## 🧠 Why
A robust custom reports system is essential because:
1. Different stakeholders need different report formats
2. Custom data views require flexible aggregation
3. Reports need to be automated and scheduled
4. Data must be accessible in various formats
5. Report generation needs to be efficient

## 📋 Context

### Current State
- Analytics dashboard implemented
- Results finalization system in place
- Data storage and access layers exist
- Basic export capabilities available

### Reporting Requirements
1. **Report Design**
   - Template creation
   - Data source selection
   - Layout customization
   - Format options

2. **Data Integration**
   - Multiple sources
   - Custom aggregation
   - Filtering options
   - Cross-referencing

3. **Distribution System**
   - Scheduling
   - Access control
   - Delivery methods
   - Format conversion

4. **Management Features**
   - Version control
   - Template library
   - Batch processing
   - Archive system

## 🔨 Implementation Blueprint

### Task 1: Create Report Models

\`\`\`csharp
public class CustomReportEntity : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public ReportTemplate Template { get; set; }
    public List<ReportDataSource> DataSources { get; set; }
    public ReportSchedule Schedule { get; set; }
    public List<ReportDelivery> DeliveryOptions { get; set; }
    public Dictionary<string, object> Parameters { get; set; }
    public string Status { get; set; }
    public DateTime LastGenerated { get; set; }
    public Guid CreatedBy { get; set; }
    public List<string> SharedWith { get; set; }
    public string Version { get; set; }
}

public class ReportTemplate
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Format { get; set; } // PDF, Excel, HTML, etc.
    public string Content { get; set; }
    public Dictionary<string, object> Styling { get; set; }
    public List<ReportSection> Sections { get; set; }
    public Dictionary<string, string> Localization { get; set; }
}

public class ReportDataSource
{
    public string Name { get; set; }
    public string Type { get; set; } // SQL, API, File, etc.
    public string Query { get; set; }
    public Dictionary<string, object> Parameters { get; set; }
    public string RefreshStrategy { get; set; }
    public DataTransformation[] Transformations { get; set; }
}

public class ReportSchedule
{
    public string Frequency { get; set; } // Daily, Weekly, Monthly, etc.
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string TimeZone { get; set; }
    public List<string> Recipients { get; set; }
    public bool IsEnabled { get; set; }
    public Dictionary<string, object> RecurrenceRule { get; set; }
}

public class ReportGeneration : BaseEntity
{
    public Guid ReportId { get; set; }
    public DateTime GeneratedAt { get; set; }
    public string Status { get; set; }
    public Dictionary<string, object> Parameters { get; set; }
    public List<string> OutputFiles { get; set; }
    public TimeSpan Duration { get; set; }
    public string Error { get; set; }
}
\`\`\`

### Task 2: Create Report Service

\`\`\`csharp
public interface ICustomReportService
{
    Task<CustomReportDto> CreateReportAsync(CreateReportDto dto);
    Task<CustomReportDto> UpdateReportAsync(Guid reportId, UpdateReportDto dto);
    Task<ReportGenerationDto> GenerateReportAsync(Guid reportId, Dictionary<string, object> parameters);
    Task<List<CustomReportDto>> GetUserReportsAsync();
    Task<bool> ScheduleReportAsync(Guid reportId, ReportScheduleDto schedule);
    Task<byte[]> GetReportOutputAsync(Guid generationId, string format);
    Task<List<ReportTemplateDto>> GetTemplatesAsync();
    Task<bool> ShareReportAsync(Guid reportId, List<string> users);
}

public class CustomReportService : ICustomReportService
{
    private readonly ICustomReportRepository _reportRepository;
    private readonly IReportTemplateRepository _templateRepository;
    private readonly IReportGenerationRepository _generationRepository;
    private readonly IReportDataService _dataService;
    private readonly IReportRenderer _renderer;
    private readonly ILogger<CustomReportService> _logger;

    // Implementation methods...
}
\`\`\`

### Task 3: Create Report Repository

\`\`\`csharp
public interface ICustomReportRepository : IBaseRepository<CustomReportEntity>
{
    Task<List<CustomReportEntity>> GetUserReportsAsync(Guid userId);
    Task<List<ReportTemplate>> GetTemplatesAsync();
    Task<List<ReportGeneration>> GetGenerationHistoryAsync(Guid reportId);
    Task<ReportDataResult> ExecuteDataSourceAsync(ReportDataSource source);
}

public class CustomReportRepository : BaseRepository<CustomReportEntity>, ICustomReportRepository
{
    // Implementation methods...
}
\`\`\`

### Task 4: Create API Controllers

\`\`\`csharp
[ApiController]
[Route("api/[controller]")]
public class CustomReportController : ControllerBase
{
    private readonly ICustomReportService _reportService;
    private readonly ILogger<CustomReportController> _logger;

    [HttpPost("reports")]
    public async Task<ActionResult<CustomReportDto>> CreateReport(CreateReportDto dto)
    {
        // Implementation...
    }

    [HttpPost("reports/{reportId}/generate")]
    public async Task<ActionResult<ReportGenerationDto>> GenerateReport(
        Guid reportId,
        [FromBody] Dictionary<string, object> parameters)
    {
        // Implementation...
    }

    [HttpGet("reports/{reportId}/output")]
    public async Task<FileResult> GetReportOutput(
        Guid reportId,
        [FromQuery] string format)
    {
        // Implementation...
    }

    // Additional endpoints...
}
\`\`\`

### Task 5: Create Frontend Types and Services

\`\`\`typescript
// Types
export interface CustomReport {
  id: string;
  name: string;
  description: string;
  template: ReportTemplate;
  dataSources: ReportDataSource[];
  schedule: ReportSchedule;
  deliveryOptions: ReportDelivery[];
  parameters: Record<string, any>;
  status: string;
  lastGenerated: string;
  createdBy: string;
  sharedWith: string[];
  version: string;
}

export interface ReportTemplate {
  name: string;
  description: string;
  format: ReportFormat;
  content: string;
  styling: Record<string, any>;
  sections: ReportSection[];
  localization: Record<string, string>;
}

// Service
export const customReportService = {
  createReport: async (dto: CreateReportDto): Promise<CustomReport> => {
    const response = await axios.post<CustomReport>(
      '/api/CustomReport/reports',
      dto
    );
    return response.data;
  },

  generateReport: async (
    reportId: string,
    parameters: Record<string, any>
  ): Promise<ReportGeneration> => {
    const response = await axios.post<ReportGeneration>(
      \`/api/CustomReport/reports/\${reportId}/generate\`,
      parameters
    );
    return response.data;
  },

  // Additional methods...
};
\`\`\`

### Task 6: Create Frontend Hooks

\`\`\`typescript
export const useCustomReport = (reportId: string) => {
  const [report, setReport] = useState<CustomReport | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const generateReport = useCallback(async (parameters: Record<string, any>) => {
    try {
      setLoading(true);
      const generation = await customReportService.generateReport(reportId, parameters);
      return generation;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to generate report');
      return null;
    } finally {
      setLoading(false);
    }
  }, [reportId]);

  // Additional hooks...

  return {
    report,
    loading,
    error,
    generateReport,
  };
};
\`\`\`

### Task 7: Create Frontend Components

\`\`\`typescript
export const ReportDesignerComponent: React.FC<{
  report: CustomReport;
  onSave: (updates: UpdateReportDto) => Promise<void>;
}> = ({ report, onSave }) => {
  const { t } = useTranslation();
  
  return (
    <div className="bg-white shadow-lg rounded-lg p-6">
      <div className="flex justify-between items-center mb-6">
        <div>
          <h2 className="text-2xl font-bold">
            {report.name}
          </h2>
          <p className="text-gray-500">
            {report.description}
          </p>
        </div>
        
        <div className="flex items-center space-x-4">
          <span className="text-sm text-gray-500">
            {t('reports.version')} {report.version}
          </span>
          
          <button
            type="button"
            className="px-4 py-2 text-sm bg-blue-600 text-white rounded 
                     hover:bg-blue-700"
            onClick={() => onSave(getUpdates())}
          >
            {t('reports.actions.save')}
          </button>
        </div>
      </div>
      
      {/* Template Designer */}
      <div className="mb-8">
        <h3 className="text-lg font-medium mb-4">
          {t('reports.template.title')}
        </h3>
        <TemplateDesigner
          template={report.template}
          onChange={handleTemplateChange}
        />
      </div>
      
      {/* Data Sources */}
      <div className="mb-8">
        <h3 className="text-lg font-medium mb-4">
          {t('reports.dataSources.title')}
        </h3>
        <DataSourcesEditor
          dataSources={report.dataSources}
          onChange={handleDataSourcesChange}
        />
      </div>
      
      {/* Schedule */}
      <div className="mb-8">
        <h3 className="text-lg font-medium mb-4">
          {t('reports.schedule.title')}
        </h3>
        <ScheduleEditor
          schedule={report.schedule}
          onChange={handleScheduleChange}
        />
      </div>
      
      {/* Delivery Options */}
      <div>
        <h3 className="text-lg font-medium mb-4">
          {t('reports.delivery.title')}
        </h3>
        <DeliveryOptionsEditor
          options={report.deliveryOptions}
          onChange={handleDeliveryOptionsChange}
        />
      </div>
    </div>
  );
};

export const ReportGeneratorComponent: React.FC<{
  report: CustomReport;
  onGenerate: (parameters: Record<string, any>) => Promise<void>;
}> = ({ report, onGenerate }) => {
  const { t } = useTranslation();
  const [parameters, setParameters] = useState<Record<string, any>>({});
  
  return (
    <div className="bg-white rounded-lg shadow p-6">
      <h3 className="text-lg font-medium mb-4">
        {t('reports.generate.title')}
      </h3>
      
      {/* Parameters Form */}
      <div className="space-y-4 mb-6">
        {Object.entries(report.parameters).map(([key, config]) => (
          <div key={key}>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              {t(\`reports.parameters.\${key}\`)}
            </label>
            <ParameterInput
              config={config as ParameterConfig}
              value={parameters[key]}
              onChange={value => setParameters(prev => ({
                ...prev,
                [key]: value
              }))}
            />
          </div>
        ))}
      </div>
      
      {/* Format Selection */}
      <div className="mb-6">
        <label className="block text-sm font-medium text-gray-700 mb-1">
          {t('reports.format.label')}
        </label>
        <select
          className="w-full border rounded-md py-2 px-3"
          value={parameters.format}
          onChange={e => setParameters(prev => ({
            ...prev,
            format: e.target.value
          }))}
        >
          {report.template.availableFormats.map(format => (
            <option key={format} value={format}>
              {t(\`reports.format.\${format}\`)}
            </option>
          ))}
        </select>
      </div>
      
      {/* Generation Button */}
      <div className="flex justify-end">
        <button
          type="button"
          className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
          onClick={() => onGenerate(parameters)}
        >
          {t('reports.actions.generate')}
        </button>
      </div>
    </div>
  );
};
\`\`\`

## 🧪 Validation Loop

### Backend Validation

\`\`\`bash
# Build the backend
cd backend
dotnet build

# Run unit tests
dotnet test Ikhtibar.Tests/Core/Services/CustomReportServiceTests.cs
dotnet test Ikhtibar.Tests/Infrastructure/Repositories/CustomReportRepositoryTests.cs
dotnet test Ikhtibar.Tests/Api/Controllers/CustomReportControllerTests.cs
\`\`\`

### Frontend Validation

\`\`\`bash
# Type checking
cd frontend
npm run type-check

# Lint check
npm run lint

# Unit tests
npm run test src/modules/reports/hooks/useCustomReport.test.ts
npm run test src/modules/reports/components/ReportDesignerComponent.test.ts
\`\`\`

### Integration Testing

\`\`\`bash
# Create report
curl -X POST http://localhost:5000/api/CustomReport/reports -d '{reportData}'

# Generate report
curl -X POST http://localhost:5000/api/CustomReport/reports/{reportId}/generate -d '{parameters}'

# Get report output
curl -X GET http://localhost:5000/api/CustomReport/reports/{reportId}/output?format=pdf
\`\`\`

## 📋 Acceptance Criteria

1. **Report Design**
   - [x] Template customization
   - [x] Data source configuration
   - [x] Layout flexibility
   - [x] Format options

2. **Data Management**
   - [x] Multiple data sources
   - [x] Custom aggregation
   - [x] Filtering options
   - [x] Data transformation

3. **Distribution System**
   - [x] Scheduling capability
   - [x] Access control
   - [x] Multiple formats
   - [x] Delivery options

4. **Management Features**
   - [x] Version control
   - [x] Template library
   - [x] Batch processing
   - [x] History tracking

5. **Performance**
   - [x] Fast generation
   - [x] Efficient delivery
   - [x] Resource optimization
   - [x] Concurrent processing

## 🔄 Development Iterations

### Phase 1: Core Reporting Engine
- Implement report models
- Create template system
- Build data sources
- Add basic generation

### Phase 2: Designer System
- Create template designer
- Add data source config
- Build parameter system
- Implement preview

### Phase 3: Distribution System
- Add scheduling
- Create delivery options
- Implement notifications
- Build access control

### Phase 4: Management Features
- Add version control
- Create template library
- Build batch processing
- Implement archiving

## 📚 Reference Materials
- Section 7.1 in requirements document for reporting specifications
- Template design guidelines
- Data source integration patterns
- Report generation optimization techniques
