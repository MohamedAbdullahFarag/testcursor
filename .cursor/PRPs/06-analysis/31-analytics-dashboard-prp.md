# Analytics Dashboard System PRP

## 🎯 Goal
Implement a comprehensive analytics dashboard system that provides real-time insights into exam performance, grading metrics, and educational outcomes through interactive visualizations and detailed reports.

## 🧠 Why
A robust analytics dashboard is essential because:
1. Data-driven decision making requires clear insights
2. Performance trends need to be identified
3. Quality metrics must be monitored
4. Stakeholders need actionable information
5. Continuous improvement requires measurement

## 📋 Context

### Current State
- Auto-grading system operational
- Manual grading workflow implemented
- Results finalization system in place
- Basic data collection implemented

### Analytics Requirements
1. **Performance Metrics**
   - Score distributions
   - Success rates
   - Time analytics
   - Progress tracking

2. **Grading Analytics**
   - Grader performance
   - Consistency metrics
   - Workload distribution
   - Quality indicators

3. **Educational Insights**
   - Learning outcomes
   - Question effectiveness
   - Topic performance
   - Skill mastery

4. **Interactive Features**
   - Data filtering
   - Custom views
   - Export capabilities
   - Real-time updates

## 🔨 Implementation Blueprint

### Task 1: Create Analytics Models

\`\`\`csharp
public class AnalyticsDashboardEntity : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<AnalyticsWidget> Widgets { get; set; }
    public Dictionary<string, object> Filters { get; set; }
    public DateTime LastUpdated { get; set; }
    public bool IsPublic { get; set; }
    public List<string> SharedWith { get; set; }
}

public class AnalyticsWidget
{
    public string Type { get; set; } // Chart, Table, Metric, etc.
    public string Title { get; set; }
    public string Description { get; set; }
    public Dictionary<string, object> Configuration { get; set; }
    public AnalyticsDataSource DataSource { get; set; }
    public Dictionary<string, object> Styling { get; set; }
    public List<string> Interactions { get; set; }
}

public class AnalyticsDataSource
{
    public string Type { get; set; } // SQL, API, Aggregation, etc.
    public string Query { get; set; }
    public Dictionary<string, object> Parameters { get; set; }
    public string RefreshInterval { get; set; }
    public DateTime LastRefreshed { get; set; }
}

public class PerformanceMetricsEntity : BaseEntity
{
    public int ExamSessionId { get; set; }
    public DateTime CalculatedAt { get; set; }
    public double AverageScore { get; set; }
    public double MedianScore { get; set; }
    public double StandardDeviation { get; set; }
    public Dictionary<string, int> GradeDistribution { get; set; }
    public Dictionary<string, double> TopicPerformance { get; set; }
    public Dictionary<string, double> SkillMastery { get; set; }
    public TimeAnalytics TimeMetrics { get; set; }
}

public class GradingAnalyticsEntity : BaseEntity
{
    public DateTime Period { get; set; }
    public List<GraderPerformance> GraderMetrics { get; set; }
    public double AverageGradingTime { get; set; }
    public double ConsistencyScore { get; set; }
    public Dictionary<string, int> WorkloadDistribution { get; set; }
    public List<QualityIndicator> QualityMetrics { get; set; }
}
\`\`\`

### Task 2: Create Analytics Service

\`\`\`csharp
public interface IAnalyticsDashboardService
{
    Task<AnalyticsDashboardDto> CreateDashboardAsync(CreateDashboardDto dto);
    Task<AnalyticsDashboardDto> UpdateDashboardAsync(int dashboardId, UpdateDashboardDto dto);
    Task<AnalyticsDashboardDto> GetDashboardAsync(int dashboardId);
    Task<List<AnalyticsDashboardDto>> GetUserDashboardsAsync();
    Task<WidgetDataDto> GetWidgetDataAsync(int widgetId, Dictionary<string, object> filters);
    Task<PerformanceMetricsDto> GetExamPerformanceAsync(int examSessionId);
    Task<GradingAnalyticsDto> GetGradingAnalyticsAsync(DateRange range);
    Task<byte[]> ExportDashboardDataAsync(int dashboardId, string format);
}

public class AnalyticsDashboardService : IAnalyticsDashboardService
{
    private readonly IAnalyticsDashboardRepository _dashboardRepository;
    private readonly IPerformanceMetricsRepository _metricsRepository;
    private readonly IGradingAnalyticsRepository _gradingRepository;
    private readonly ILogger<AnalyticsDashboardService> _logger;

    // Implementation methods...
}
\`\`\`

### Task 3: Create Analytics Repository

\`\`\`csharp
public interface IAnalyticsDashboardRepository : IBaseRepository<AnalyticsDashboardEntity>
{
    Task<List<AnalyticsDashboardEntity>> GetUserDashboardsAsync(int userId);
    Task<WidgetData> GetWidgetDataAsync(AnalyticsWidget widget, Dictionary<string, object> filters);
    Task<List<PerformanceMetricsEntity>> GetPerformanceTrendAsync(DateRange range);
    Task<List<GradingAnalyticsEntity>> GetGradingAnalyticsAsync(DateRange range);
}

public class AnalyticsDashboardRepository : BaseRepository<AnalyticsDashboardEntity>, IAnalyticsDashboardRepository
{
    // Implementation methods...
}
\`\`\`

### Task 4: Create API Controllers

\`\`\`csharp
[ApiController]
[Route("api/[controller]")]
public class AnalyticsDashboardController : ControllerBase
{
    private readonly IAnalyticsDashboardService _dashboardService;
    private readonly ILogger<AnalyticsDashboardController> _logger;

    [HttpPost("dashboards")]
    public async Task<ActionResult<AnalyticsDashboardDto>> CreateDashboard(CreateDashboardDto dto)
    {
        // Implementation...
    }

    [HttpGet("dashboards/{dashboardId}")]
    public async Task<ActionResult<AnalyticsDashboardDto>> GetDashboard(int dashboardId)
    {
        // Implementation...
    }

    [HttpGet("widgets/{widgetId}/data")]
    public async Task<ActionResult<WidgetDataDto>> GetWidgetData(
        int widgetId,
        [FromQuery] Dictionary<string, object> filters)
    {
        // Implementation...
    }

    // Additional endpoints...
}
\`\`\`

### Task 5: Create Frontend Types and Services

\`\`\`typescript
// Types
export interface AnalyticsDashboard {
  id: string;
  name: string;
  description: string;
  widgets: AnalyticsWidget[];
  filters: Record<string, any>;
  lastUpdated: string;
  isPublic: boolean;
  sharedWith: string[];
}

export interface AnalyticsWidget {
  type: WidgetType;
  title: string;
  description: string;
  configuration: Record<string, any>;
  dataSource: AnalyticsDataSource;
  styling: Record<string, any>;
  interactions: string[];
}

export interface PerformanceMetrics {
  examSessionId: string;
  calculatedAt: string;
  averageScore: number;
  medianScore: number;
  standardDeviation: number;
  gradeDistribution: Record<string, number>;
  topicPerformance: Record<string, number>;
  skillMastery: Record<string, number>;
  timeMetrics: TimeAnalytics;
}

// Service
export const analyticsDashboardService = {
  createDashboard: async (dto: CreateDashboardDto): Promise<AnalyticsDashboard> => {
    const response = await axios.post<AnalyticsDashboard>(
      '/api/AnalyticsDashboard/dashboards',
      dto
    );
    return response.data;
  },

  getWidgetData: async (
    widgetId: string,
    filters: Record<string, any>
  ): Promise<WidgetData> => {
    const response = await axios.get<WidgetData>(
      \`/api/AnalyticsDashboard/widgets/\${widgetId}/data\`,
      { params: filters }
    );
    return response.data;
  },

  // Additional methods...
};
\`\`\`

### Task 6: Create Frontend Hooks

\`\`\`typescript
export const useAnalyticsDashboard = (dashboardId: string) => {
  const [dashboard, setDashboard] = useState<AnalyticsDashboard | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const refreshDashboard = useCallback(async () => {
    try {
      setLoading(true);
      const response = await analyticsDashboardService.getDashboard(dashboardId);
      setDashboard(response);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load dashboard');
    } finally {
      setLoading(false);
    }
  }, [dashboardId]);

  const refreshWidgetData = useCallback(async (
    widgetId: string,
    filters: Record<string, any>
  ) => {
    try {
      const data = await analyticsDashboardService.getWidgetData(widgetId, filters);
      setDashboard(prev => {
        if (!prev) return null;
        return {
          ...prev,
          widgets: prev.widgets.map(w =>
            w.id === widgetId ? { ...w, data } : w
          )
        };
      });
    } catch (err) {
      console.error('Failed to refresh widget data:', err);
    }
  }, []);

  // Additional hooks...

  return {
    dashboard,
    loading,
    error,
    refreshDashboard,
    refreshWidgetData,
  };
};
\`\`\`

### Task 7: Create Frontend Components

\`\`\`typescript
export const AnalyticsDashboardComponent: React.FC<{
  dashboard: AnalyticsDashboard;
  onRefreshWidget: (widgetId: string) => Promise<void>;
}> = ({ dashboard, onRefreshWidget }) => {
  const { t } = useTranslation();
  
  return (
    <div className="bg-white shadow-lg rounded-lg p-6">
      <div className="flex justify-between items-center mb-6">
        <div>
          <h2 className="text-2xl font-bold">
            {dashboard.name}
          </h2>
          <p className="text-gray-500">
            {dashboard.description}
          </p>
        </div>
        
        <div className="flex items-center space-x-4">
          <span className="text-sm text-gray-500">
            {t('analytics.lastUpdated', {
              time: formatDateTime(dashboard.lastUpdated)
            })}
          </span>
          
          <button
            type="button"
            className="px-4 py-2 text-sm bg-blue-600 text-white rounded 
                     hover:bg-blue-700"
            onClick={() => onRefreshWidget('all')}
          >
            {t('analytics.actions.refresh')}
          </button>
        </div>
      </div>
      
      {/* Filters */}
      <div className="mb-6">
        <AnalyticsFilters
          filters={dashboard.filters}
          onChange={handleFilterChange}
        />
      </div>
      
      {/* Widgets Grid */}
      <div className="grid grid-cols-2 gap-6">
        {dashboard.widgets.map(widget => (
          <AnalyticsWidget
            key={widget.id}
            widget={widget}
            onRefresh={() => onRefreshWidget(widget.id)}
          />
        ))}
      </div>
    </div>
  );
};

export const AnalyticsWidget: React.FC<{
  widget: AnalyticsWidget;
  onRefresh: () => Promise<void>;
}> = ({ widget, onRefresh }) => {
  const { t } = useTranslation();
  
  const renderChart = () => {
    switch (widget.type) {
      case 'LineChart':
        return (
          <LineChart
            data={widget.dataSource.data}
            config={widget.configuration}
            styling={widget.styling}
          />
        );
      case 'BarChart':
        return (
          <BarChart
            data={widget.dataSource.data}
            config={widget.configuration}
            styling={widget.styling}
          />
        );
      case 'MetricCard':
        return (
          <MetricCard
            value={widget.dataSource.data.value}
            trend={widget.dataSource.data.trend}
            config={widget.configuration}
          />
        );
      default:
        return null;
    }
  };
  
  return (
    <div className="bg-white rounded-lg shadow p-4">
      <div className="flex justify-between items-start mb-4">
        <div>
          <h3 className="font-medium">{widget.title}</h3>
          <p className="text-sm text-gray-500">{widget.description}</p>
        </div>
        
        <button
          type="button"
          className="p-2 text-gray-400 hover:text-gray-600"
          onClick={onRefresh}
        >
          <RefreshIcon className="w-4 h-4" />
        </button>
      </div>
      
      <div className="relative">
        {renderChart()}
        
        <div className="absolute bottom-0 right-0 p-2">
          <span className="text-xs text-gray-400">
            {t('analytics.lastRefreshed', {
              time: formatTime(widget.dataSource.lastRefreshed)
            })}
          </span>
        </div>
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
dotnet test Ikhtibar.Tests/Core/Services/AnalyticsDashboardServiceTests.cs
dotnet test Ikhtibar.Tests/Infrastructure/Repositories/AnalyticsDashboardRepositoryTests.cs
dotnet test Ikhtibar.Tests/Api/Controllers/AnalyticsDashboardControllerTests.cs
\`\`\`

### Frontend Validation

\`\`\`bash
# Type checking
cd frontend
npm run type-check

# Lint check
npm run lint

# Unit tests
npm run test src/modules/analytics/hooks/useAnalyticsDashboard.test.ts
npm run test src/modules/analytics/components/AnalyticsDashboardComponent.test.ts
\`\`\`

### Integration Testing

\`\`\`bash
# Create dashboard
curl -X POST https://localhost:7001/api/AnalyticsDashboard/dashboards -d '{dashboardData}'

# Get widget data
curl -X GET https://localhost:7001/api/AnalyticsDashboard/widgets/{widgetId}/data

# Export dashboard data
curl -X GET https://localhost:7001/api/AnalyticsDashboard/dashboards/{dashboardId}/export
\`\`\`

## 📋 Acceptance Criteria

1. **Performance Analytics**
   - [x] Score distribution charts
   - [x] Time analytics
   - [x] Progress tracking
   - [x] Trend analysis

2. **Grading Insights**
   - [x] Grader performance metrics
   - [x] Consistency analysis
   - [x] Workload visualization
   - [x] Quality indicators

3. **Educational Metrics**
   - [x] Learning outcome analysis
   - [x] Question effectiveness
   - [x] Topic performance
   - [x] Skill mastery tracking

4. **User Experience**
   - [x] Interactive filters
   - [x] Real-time updates
   - [x] Custom views
   - [x] Export capabilities

5. **Performance**
   - [x] Fast data loading
   - [x] Efficient updates
   - [x] Responsive charts
   - [x] Optimized queries

## 🔄 Development Iterations

### Phase 1: Core Analytics Engine
- Implement data models
- Create analytics services
- Build basic dashboards
- Add core metrics

### Phase 2: Visualization System
- Add chart components
- Create interactive filters
- Build custom views
- Implement real-time updates

### Phase 3: Advanced Analytics
- Add trend analysis
- Implement predictions
- Create benchmarks
- Build recommendations

### Phase 4: UI/UX Enhancement
- Improve interactivity
- Add animations
- Enhance responsiveness
- Implement customization

## 📚 Reference Materials
- Section 6.2 in requirements document for analytics specifications
- Dashboard design guidelines
- Data visualization best practices
- Performance optimization techniques
