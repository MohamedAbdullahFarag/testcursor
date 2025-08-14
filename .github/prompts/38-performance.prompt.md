---
mode: agent
description: "Analyze application performance with bottleneck identification and optimization recommendations"
---

---
inputs:
  - name: action
    description: Performance action to perform (analyze, optimize, benchmark, monitor)
    required: true
  - name: scope
    description: Performance scope (frontend, backend, database, api, full-stack)
    required: true
  - name: target
    description: Specific component or area to focus on
    required: false
---

---
command: "/performance"
---
# Performance Optimization Command for GitHub Copilot

## Command Usage
```
@copilot /performance [action] [scope] [target]
```

## Purpose
This command provides comprehensive performance analysis and optimization for the Ikhtibar examination management system. It covers both ASP.NET Core backend and React.js frontend performance, following modern performance best practices and project-specific optimization patterns.

**Input Parameters**: 
- `action` - Performance action: `analyze`, `optimize`, `benchmark`, or `monitor`
- `scope` - Performance scope: `frontend`, `backend`, `database`, `api`, or `full-stack`
- `target` - Specific component, service, or area to focus on (optional)

## How /performance Works

### Phase 1: Performance Context Discovery and Analysis
```markdown
I'll help you with comprehensive performance optimization for the Ikhtibar project. Let me analyze your request and gather the necessary context.

**Phase 1.1: Parse Performance Request**
```
Performance Request Analysis:
- **Action**: [ANALYZE/OPTIMIZE/BENCHMARK/MONITOR]
- **Scope**: [FRONTEND/BACKEND/DATABASE/API/FULL-STACK]
- **Target**: [SPECIFIC_COMPONENT_OR_AREA]
- **Project Context**: ASP.NET Core + React.js with TypeScript
- **Performance Goals**: Sub-3s page loads, <100ms API responses, 60fps UI
```

**Phase 1.2: Performance Baseline Analysis using GitHub Copilot Tools**
```
I'll establish performance baseline using GitHub Copilot's native tools:
- semantic_search: "[SCOPE] performance patterns" # Find existing optimizations
- file_search: "**/*[TARGET]*" # Find related files
- grep_search: "async|await|cache|lazy|memo" # Find performance patterns
- read_file: [PERFORMANCE_CRITICAL_FILES] # Read key implementations
- run_in_terminal: [PERFORMANCE_COMMANDS] # Execute performance tests
- get_errors: [PERFORMANCE_FILES] # Check for performance issues
```

**Phase 1.3: Performance Architecture Analysis**
```
Performance Context Analysis:
- [ ] **Frontend Stack**: React 18, TypeScript, Vite, TanStack Query, Tailwind CSS
- [ ] **Backend Stack**: ASP.NET Core 8, Dapper/EF Core, SQL Server, Redis Cache
- [ ] **Bundling**: Vite with code splitting and tree shaking
- [ ] **Caching**: Browser cache, Redis, Response caching, Query caching
- [ ] **CDN**: Static asset optimization and delivery
- [ ] **Database**: Query optimization, indexing, connection pooling
- [ ] **Monitoring**: Application Insights, performance profiling
- [ ] **Targets**: <3s initial load, <100ms API response, 60fps interactions
```
```

### Phase 2: Action-Specific Performance Implementation

#### For Action: `analyze`
```markdown
**Phase 2.1: Performance Analysis using GitHub Copilot Tools**
```
I'll perform comprehensive performance analysis:

## 📊 Performance Analysis (Tool-Enhanced)

### Frontend Performance Analysis
```powershell
# Comprehensive frontend analysis using GitHub Copilot tools
run_in_terminal: "npm run build" # Build for production analysis
run_in_terminal: "npx vite-bundle-analyzer dist" # Bundle analysis
run_in_terminal: "npx lighthouse http://localhost:5173 --output json" # Lighthouse audit
semantic_search: "performance bottlenecks" # Find known issues
grep_search: "useEffect|useState|useMemo|useCallback" # Find hook usage
```

#### Bundle Analysis (Tool-Generated)
```
Bundle Analysis Results:
┌─────────────────────────────────────────────────────────────┐
│ Bundle Size Analysis:                                       │
│ • Total Bundle Size: [SIZE]MB                               │
│ • Main Chunk: [SIZE]KB                                      │
│ • Vendor Chunks: [SIZE]KB                                   │
│ • Async Chunks: [COUNT] chunks, [SIZE]KB                    │
│ • Largest Dependencies: [LIBRARY_LIST]                      │
├─────────────────────────────────────────────────────────────┤
│ Code Splitting Analysis:                                    │
│ • Route-based Splitting: [STATUS]                           │
│ • Component-level Splitting: [STATUS]                       │
│ • Library Splitting: [STATUS]                               │
│ • Dynamic Imports: [COUNT] found                            │
└─────────────────────────────────────────────────────────────┘
```

#### Lighthouse Performance Audit (Tool-Executed)
```json
{
  "performance": {
    "score": [PERFORMANCE_SCORE],
    "firstContentfulPaint": "[FCP_TIME]ms",
    "largestContentfulPaint": "[LCP_TIME]ms",
    "cumulativeLayoutShift": "[CLS_SCORE]",
    "firstInputDelay": "[FID_TIME]ms",
    "totalBlockingTime": "[TBT_TIME]ms"
  },
  "opportunities": [
    {
      "title": "[OPTIMIZATION_OPPORTUNITY]",
      "description": "[DETAILED_DESCRIPTION]",
      "savings": "[POTENTIAL_SAVINGS]ms"
    }
  ]
}
```

### Backend Performance Analysis
```powershell
# Comprehensive backend analysis using GitHub Copilot tools
run_in_terminal: "dotnet build --configuration Release" # Optimized build
semantic_search: "database query performance" # Find query patterns
grep_search: "async|await|Task|ConfigureAwait" # Find async patterns
grep_search: "cache|Cache|IMemoryCache" # Find caching usage
```

#### API Performance Analysis (Tool-Measured)
```
API Performance Metrics:
┌─────────────────────────────────────────────────────────────┐
│ Endpoint Performance:                                       │
├─────────────────────────────────────────────────────────────┤
│ Endpoint: GET /api/[ENDPOINT]                               │
│ • Average Response Time: [TIME]ms                           │
│ • 95th Percentile: [TIME]ms                                 │
│ • Throughput: [RPS] req/sec                                 │
│ • Error Rate: [PERCENTAGE]%                                 │
├─────────────────────────────────────────────────────────────┤
│ Database Query Analysis:                                    │
│ • Average Query Time: [TIME]ms                              │
│ • Slow Queries (>100ms): [COUNT]                            │
│ • Missing Indexes: [COUNT]                                  │
│ • N+1 Query Issues: [COUNT]                                 │
└─────────────────────────────────────────────────────────────┘
```

### Database Performance Analysis
```sql
-- Generated based on database schema analysis
-- Top 10 slowest queries
SELECT TOP 10
    total_elapsed_time / execution_count AS avg_time_ms,
    execution_count,
    total_elapsed_time,
    (total_elapsed_time / execution_count) / 1000.0 AS avg_time_seconds,
    SUBSTRING(qt.text, (qs.statement_start_offset/2)+1, 
        ((CASE qs.statement_end_offset
            WHEN -1 THEN DATALENGTH(qt.text)
            ELSE qs.statement_end_offset
        END - qs.statement_start_offset)/2)+1) AS individual_query
FROM sys.dm_exec_query_stats qs
CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) qt
ORDER BY avg_time_ms DESC;

-- Missing index analysis
SELECT 
    mid.database_id,
    mid.object_id,
    mid.equality_columns,
    mid.inequality_columns,
    mid.included_columns,
    migs.user_seeks,
    migs.avg_total_user_cost,
    migs.avg_user_impact
FROM sys.dm_db_missing_index_details mid
INNER JOIN sys.dm_db_missing_index_groups mig ON mid.index_handle = mig.index_handle
INNER JOIN sys.dm_db_missing_index_group_stats migs ON mig.index_group_handle = migs.group_handle
ORDER BY migs.avg_total_user_cost * migs.avg_user_impact DESC;
```

### Performance Issues Identified (Tool-Discovered)
#### Critical Issues (Fix Immediately)
1. **Issue**: [CRITICAL_PERFORMANCE_ISSUE]
   - **Location**: [FILE_PATH:LINE_NUMBER]
   - **Impact**: [PERFORMANCE_IMPACT]
   - **Tool Detection**: [ANALYSIS_METHOD]
   - **Estimated Fix Time**: [TIME_ESTIMATE]

#### Major Issues (Fix Soon)
1. **Issue**: [MAJOR_PERFORMANCE_ISSUE]
   - **Location**: [FILE_PATH:LINE_NUMBER]
   - **Impact**: [PERFORMANCE_IMPACT]
   - **Tool Detection**: [ANALYSIS_METHOD]
   - **Estimated Fix Time**: [TIME_ESTIMATE]
```

#### For Action: `optimize`
```markdown
**Phase 2.2: Performance Optimization using GitHub Copilot Tools**
```
I'll implement comprehensive performance optimizations:

## ⚡ Performance Optimization Implementation (Tool-Enhanced)

### Frontend Optimizations (Tool-Implemented)

#### React Performance Optimizations
```typescript
// Generated based on component analysis using semantic_search
import React, { memo, useMemo, useCallback, lazy, Suspense } from 'react';
import { useQuery } from '@tanstack/react-query';

// 1. Component Memoization (applied to components found via analysis)
const [COMPONENT_NAME] = memo<[COMPONENT_NAME]Props>(({ 
  // Props from interface analysis
}) => {
  // 2. Memoize expensive calculations
  const expensiveValue = useMemo(() => {
    // Expensive calculation identified via performance analysis
    return [EXPENSIVE_CALCULATION];
  }, [DEPENDENCY_ARRAY]);

  // 3. Memoize event handlers
  const handleClick = useCallback((id: string) => {
    // Event handler implementation
  }, [DEPENDENCY_ARRAY]);

  // 4. Optimize re-renders with proper dependencies
  const { data, isLoading } = useQuery({
    queryKey: ['[QUERY_KEY]', [DEPENDENCIES]],
    queryFn: () => [API_CALL],
    staleTime: 5 * 60 * 1000, // 5 minutes cache
    cacheTime: 10 * 60 * 1000, // 10 minutes cache
  });

  return (
    <div>
      {/* Optimized rendering with proper conditionals */}
      {isLoading ? (
        <div>Loading...</div>
      ) : (
        <div>
          {/* Component content */}
        </div>
      )}
    </div>
  );
});

// 5. Lazy load components for code splitting
const [HEAVY_COMPONENT] = lazy(() => 
  import('./[HEAVY_COMPONENT]').then(module => ({
    default: module.[HEAVY_COMPONENT]
  }))
);

// 6. Suspense boundaries for better UX
const [CONTAINER_COMPONENT] = () => (
  <Suspense fallback={<div>Loading component...</div>}>
    <[HEAVY_COMPONENT] />
  </Suspense>
);
```

#### Bundle Optimization (Tool-Configured)
```typescript
// vite.config.ts optimizations based on bundle analysis
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import { splitVendorChunkPlugin } from 'vite';

export default defineConfig({
  plugins: [
    react(),
    splitVendorChunkPlugin()
  ],
  build: {
    rollupOptions: {
      output: {
        manualChunks: {
          // Split large dependencies identified via bundle analysis
          'react-vendor': ['react', 'react-dom'],
          'query-vendor': ['@tanstack/react-query'],
          'router-vendor': ['react-router-dom'],
          'ui-vendor': ['@headlessui/react', '@heroicons/react'],
        }
      }
    },
    // Optimize chunk size
    chunkSizeWarningLimit: 1000,
  },
  // Optimize dev server
  server: {
    open: true,
    hmr: {
      overlay: false
    }
  }
});
```

### Backend Optimizations (Tool-Enhanced)

#### API Response Optimization
```csharp
// Generated based on controller analysis
[ApiController]
[Route("api/[controller]")]
public class [OPTIMIZED_CONTROLLER] : ControllerBase
{
    private readonly I[SERVICE] _service;
    private readonly IMemoryCache _cache;
    private readonly ILogger<[OPTIMIZED_CONTROLLER]> _logger;

    public [OPTIMIZED_CONTROLLER](
        I[SERVICE] service,
        IMemoryCache cache,
        ILogger<[OPTIMIZED_CONTROLLER]> logger)
    {
        _service = service;
        _cache = cache;
        _logger = logger;
    }

    [HttpGet]
    [ResponseCache(Duration = 300)] // 5 minutes response cache
    public async Task<ActionResult<IEnumerable<[DTO]>>> GetAll()
    {
        const string cacheKey = "[ENTITY_TYPE]_all";
        
        // 1. Check memory cache first
        if (_cache.TryGetValue(cacheKey, out IEnumerable<[DTO]>? cachedData))
        {
            _logger.LogInformation("Returning cached data for {CacheKey}", cacheKey);
            return Ok(cachedData);
        }

        // 2. Get from service with optimized query
        var data = await _service.GetAllOptimizedAsync();
        
        // 3. Cache for future requests
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            SlidingExpiration = TimeSpan.FromMinutes(2),
            Priority = CacheItemPriority.Normal
        };
        
        _cache.Set(cacheKey, data, cacheOptions);
        
        return Ok(data);
    }

    [HttpGet("paginated")]
    public async Task<ActionResult<PagedResult<[DTO]>>> GetPaginated(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null)
    {
        // Input validation
        pageSize = Math.Min(pageSize, 100); // Limit max page size
        
        var result = await _service.GetPaginatedAsync(page, pageSize, search);
        return Ok(result);
    }
}
```

#### Database Query Optimization
```csharp
// Generated based on repository analysis
public class [OPTIMIZED_REPOSITORY] : I[OPTIMIZED_REPOSITORY]
{
    private readonly string _connectionString;
    private readonly ILogger<[OPTIMIZED_REPOSITORY]> _logger;

    public [OPTIMIZED_REPOSITORY](string connectionString, ILogger<[OPTIMIZED_REPOSITORY]> logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }

    // 1. Optimized query with proper indexing hints
    public async Task<IEnumerable<[ENTITY]>> GetAllOptimizedAsync()
    {
        const string sql = @"
            SELECT [COLUMN_LIST] -- Only select needed columns
            FROM [TABLE_NAME] WITH (INDEX([INDEX_NAME])) -- Use specific index
            WHERE [COMMON_FILTER_CONDITIONS]
            ORDER BY [INDEXED_COLUMN]
            OFFSET 0 ROWS FETCH NEXT 1000 ROWS ONLY"; -- Limit result set

        using var connection = new SqlConnection(_connectionString);
        
        return await connection.QueryAsync<[ENTITY]>(sql);
    }

    // 2. Bulk operations for better performance
    public async Task<int> BulkInsertAsync(IEnumerable<[ENTITY]> entities)
    {
        const string sql = @"
            INSERT INTO [TABLE_NAME] ([COLUMN_LIST])
            VALUES (@Property1, @Property2, @Property3)";

        using var connection = new SqlConnection(_connectionString);
        
        // Use bulk operation instead of individual inserts
        return await connection.ExecuteAsync(sql, entities);
    }

    // 3. Async enumerable for large result sets
    public async IAsyncEnumerable<[ENTITY]> GetLargeDataSetAsync()
    {
        const string sql = @"
            SELECT [COLUMN_LIST]
            FROM [TABLE_NAME]
            ORDER BY Id";

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand(sql, connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            yield return new [ENTITY]
            {
                // Map columns to properties
            };
        }
    }
}
```

### Caching Strategy Implementation (Tool-Configured)
```csharp
// Redis caching implementation based on usage patterns
public class [CACHING_SERVICE] : I[CACHING_SERVICE]
{
    private readonly IDistributedCache _distributedCache;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<[CACHING_SERVICE]> _logger;

    public [CACHING_SERVICE](
        IDistributedCache distributedCache,
        IMemoryCache memoryCache,
        ILogger<[CACHING_SERVICE]> logger)
    {
        _distributedCache = distributedCache;
        _memoryCache = memoryCache;
        _logger = logger;
    }

    // Multi-level caching strategy
    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        // Level 1: Memory cache (fastest)
        if (_memoryCache.TryGetValue(key, out T? memoryValue))
        {
            _logger.LogDebug("Cache hit in memory for key: {Key}", key);
            return memoryValue;
        }

        // Level 2: Distributed cache (Redis)
        var distributedValue = await _distributedCache.GetStringAsync(key);
        if (distributedValue != null)
        {
            _logger.LogDebug("Cache hit in Redis for key: {Key}", key);
            var deserializedValue = JsonSerializer.Deserialize<T>(distributedValue);
            
            // Populate memory cache for next time
            _memoryCache.Set(key, deserializedValue, TimeSpan.FromMinutes(5));
            
            return deserializedValue;
        }

        _logger.LogDebug("Cache miss for key: {Key}", key);
        return null;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromHours(1)
        };

        var serializedValue = JsonSerializer.Serialize(value);
        
        // Set in both caches
        await _distributedCache.SetStringAsync(key, serializedValue, options);
        _memoryCache.Set(key, value, expiration ?? TimeSpan.FromMinutes(5));
        
        _logger.LogDebug("Cached value for key: {Key}", key);
    }
}
```
```

#### For Action: `benchmark`
```markdown
**Phase 2.3: Performance Benchmarking using GitHub Copilot Tools**
```
I'll create comprehensive performance benchmarks:

## 🏁 Performance Benchmarking (Tool-Enhanced)

### Frontend Benchmarking
```typescript
// Generated based on component analysis
import { performance, PerformanceObserver } from 'perf_hooks';

// 1. Component rendering benchmarks
export const benchmarkComponentRendering = (componentName: string, renderFn: () => void) => {
  const iterations = 1000;
  const times: number[] = [];

  for (let i = 0; i < iterations; i++) {
    const start = performance.now();
    renderFn();
    const end = performance.now();
    times.push(end - start);
  }

  const average = times.reduce((a, b) => a + b, 0) / times.length;
  const min = Math.min(...times);
  const max = Math.max(...times);
  const p95 = times.sort((a, b) => a - b)[Math.floor(times.length * 0.95)];

  console.log(`${componentName} Rendering Benchmark:`, {
    average: `${average.toFixed(2)}ms`,
    min: `${min.toFixed(2)}ms`,
    max: `${max.toFixed(2)}ms`,
    p95: `${p95.toFixed(2)}ms`
  });
};

// 2. Bundle size tracking
export const trackBundleSize = () => {
  // This would be integrated with build process
  const bundleStats = {
    mainBundle: '[SIZE]KB',
    vendorBundle: '[SIZE]KB',
    asyncChunks: '[SIZE]KB',
    totalSize: '[SIZE]KB'
  };
  
  // Track over time for regressions
  console.log('Bundle Size Tracking:', bundleStats);
};

// 3. Core Web Vitals monitoring
export const trackCoreWebVitals = () => {
  // First Contentful Paint
  new PerformanceObserver((list) => {
    const entries = list.getEntries();
    entries.forEach((entry) => {
      if (entry.name === 'first-contentful-paint') {
        console.log('FCP:', entry.startTime);
      }
    });
  }).observe({ entryTypes: ['paint'] });

  // Largest Contentful Paint
  new PerformanceObserver((list) => {
    const entries = list.getEntries();
    const lastEntry = entries[entries.length - 1];
    console.log('LCP:', lastEntry.startTime);
  }).observe({ entryTypes: ['largest-contentful-paint'] });

  // Cumulative Layout Shift
  new PerformanceObserver((list) => {
    let clsValue = 0;
    for (const entry of list.getEntries()) {
      if (!entry.hadRecentInput) {
        clsValue += (entry as any).value;
      }
    }
    console.log('CLS:', clsValue);
  }).observe({ entryTypes: ['layout-shift'] });
};
```

### Backend Benchmarking
```csharp
// Generated based on API analysis
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

[MemoryDiagnoser]
[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net80)]
public class [SERVICE_NAME]Benchmarks
{
    private I[SERVICE] _service;
    private [TEST_DATA_TYPE] _testData;

    [GlobalSetup]
    public void Setup()
    {
        // Initialize service and test data
        _service = new [SERVICE](/* dependencies */);
        _testData = new [TEST_DATA_TYPE] { /* test data */ };
    }

    [Benchmark]
    [Arguments(10)]
    [Arguments(100)]
    [Arguments(1000)]
    public async Task<[RETURN_TYPE]> ProcessDataAsync(int itemCount)
    {
        return await _service.ProcessAsync(_testData, itemCount);
    }

    [Benchmark]
    public async Task<[RETURN_TYPE]> GetDataWithCacheAsync()
    {
        return await _service.GetWithCacheAsync();
    }

    [Benchmark]
    public async Task<[RETURN_TYPE]> GetDataWithoutCacheAsync()
    {
        return await _service.GetWithoutCacheAsync();
    }

    [Benchmark]
    public async Task<[RETURN_TYPE]> DatabaseQueryOptimizedAsync()
    {
        return await _service.OptimizedQueryAsync();
    }

    [Benchmark]
    public async Task<[RETURN_TYPE]> DatabaseQueryUnoptimizedAsync()
    {
        return await _service.UnoptimizedQueryAsync();
    }
}

// Database query benchmarks
[MemoryDiagnoser]
public class DatabaseBenchmarks
{
    private string _connectionString;

    [GlobalSetup]
    public void Setup()
    {
        _connectionString = "/* connection string */";
    }

    [Benchmark]
    public async Task<IEnumerable<[ENTITY]>> QueryWithIndexAsync()
    {
        // Query using proper indexes
        using var connection = new SqlConnection(_connectionString);
        return await connection.QueryAsync<[ENTITY]>(@"
            SELECT * FROM [TABLE] WITH (INDEX([INDEX_NAME]))
            WHERE [INDEXED_COLUMN] = @Value",
            new { Value = "test" });
    }

    [Benchmark]
    public async Task<IEnumerable<[ENTITY]>> QueryWithoutIndexAsync()
    {
        // Query without indexes for comparison
        using var connection = new SqlConnection(_connectionString);
        return await connection.QueryAsync<[ENTITY]>(@"
            SELECT * FROM [TABLE]
            WHERE [NON_INDEXED_COLUMN] = @Value",
            new { Value = "test" });
    }
}
```

### Load Testing Scripts
```javascript
// Artillery.js load testing configuration
// Generated based on API endpoints analysis
module.exports = {
  config: {
    target: 'http://localhost:5000',
    phases: [
      { duration: 60, arrivalRate: 10 }, // Ramp up
      { duration: 300, arrivalRate: 50 }, // Sustained load
      { duration: 60, arrivalRate: 100 }, // Peak load
    ],
    defaults: {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer {{token}}'
      }
    }
  },
  scenarios: [
    {
      name: 'API Load Test',
      flow: [
        {
          post: {
            url: '/api/auth/login',
            json: {
              email: 'test@example.com',
              password: 'testpassword'
            },
            capture: {
              json: '$.token',
              as: 'token'
            }
          }
        },
        {
          get: {
            url: '/api/[ENDPOINT]',
            headers: {
              'Authorization': 'Bearer {{ token }}'
            }
          }
        },
        {
          post: {
            url: '/api/[ENDPOINT]',
            json: {
              // Request body based on DTO analysis
            }
          }
        }
      ]
    }
  ]
};
```
```

### Phase 3: Performance Monitoring and Alerting

```markdown
**Phase 3.1: Performance Monitoring Setup using GitHub Copilot Tools**
```
I'll set up comprehensive performance monitoring:

## 📈 Performance Monitoring Implementation (Tool-Enhanced)

### Application Performance Monitoring
```csharp
// Generated based on project structure analysis
public static class PerformanceMonitoring
{
    public static void ConfigurePerformanceMonitoring(this IServiceCollection services, IConfiguration configuration)
    {
        // Application Insights integration
        services.AddApplicationInsightsTelemetry(configuration["ApplicationInsights:ConnectionString"]);
        
        // Custom performance tracking
        services.AddSingleton<IPerformanceTracker, PerformanceTracker>();
        
        // Health checks
        services.AddHealthChecks()
            .AddSqlServer(configuration.GetConnectionString("DefaultConnection"))
            .AddRedis(configuration.GetConnectionString("Redis"))
            .AddApplicationInsightsPublisher();
    }
}

public class PerformanceTracker : IPerformanceTracker
{
    private readonly TelemetryClient _telemetryClient;
    private readonly ILogger<PerformanceTracker> _logger;

    public PerformanceTracker(TelemetryClient telemetryClient, ILogger<PerformanceTracker> logger)
    {
        _telemetryClient = telemetryClient;
        _logger = logger;
    }

    public async Task<T> TrackDurationAsync<T>(string operationName, Func<Task<T>> operation)
    {
        using var activity = _telemetryClient.StartOperation<DependencyTelemetry>(operationName);
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var result = await operation();
            activity.Telemetry.Success = true;
            return result;
        }
        catch (Exception ex)
        {
            activity.Telemetry.Success = false;
            _telemetryClient.TrackException(ex);
            throw;
        }
        finally
        {
            stopwatch.Stop();
            activity.Telemetry.Duration = stopwatch.Elapsed;
            
            _telemetryClient.TrackMetric($"{operationName}.Duration", stopwatch.ElapsedMilliseconds);
            
            if (stopwatch.ElapsedMilliseconds > 1000) // Alert on slow operations
            {
                _logger.LogWarning("Slow operation detected: {OperationName} took {Duration}ms", 
                    operationName, stopwatch.ElapsedMilliseconds);
            }
        }
    }
}
```

### Frontend Performance Monitoring
```typescript
// Generated based on React app analysis
export class FrontendPerformanceMonitor {
  private static instance: FrontendPerformanceMonitor;
  private performanceData: PerformanceEntry[] = [];

  public static getInstance(): FrontendPerformanceMonitor {
    if (!FrontendPerformanceMonitor.instance) {
      FrontendPerformanceMonitor.instance = new FrontendPerformanceMonitor();
    }
    return FrontendPerformanceMonitor.instance;
  }

  public init(): void {
    this.trackCoreWebVitals();
    this.trackResourceTiming();
    this.trackUserInteractions();
    this.setupPerformanceObserver();
  }

  private trackCoreWebVitals(): void {
    // Track FCP, LCP, FID, CLS
    new PerformanceObserver((list) => {
      for (const entry of list.getEntries()) {
        this.sendMetric('core-web-vitals', {
          name: entry.name,
          value: entry.startTime,
          timestamp: Date.now()
        });
      }
    }).observe({ entryTypes: ['paint', 'largest-contentful-paint'] });
  }

  private trackResourceTiming(): void {
    new PerformanceObserver((list) => {
      for (const entry of list.getEntries()) {
        if (entry.duration > 1000) { // Track slow resources
          this.sendMetric('slow-resource', {
            name: entry.name,
            duration: entry.duration,
            size: (entry as PerformanceResourceTiming).transferSize
          });
        }
      }
    }).observe({ entryTypes: ['resource'] });
  }

  private trackUserInteractions(): void {
    // Track click-to-response time
    document.addEventListener('click', (event) => {
      const start = performance.now();
      requestAnimationFrame(() => {
        const duration = performance.now() - start;
        this.sendMetric('interaction-timing', {
          target: (event.target as Element).tagName,
          duration,
          timestamp: Date.now()
        });
      });
    });
  }

  private sendMetric(type: string, data: any): void {
    // Send to analytics service
    fetch('/api/analytics/performance', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ type, data })
    }).catch(error => console.error('Failed to send performance metric:', error));
  }
}
```

### Performance Alerting Configuration
```yaml
# Azure Monitor alerts configuration
# Generated based on performance requirements
alerts:
  - name: "High API Response Time"
    condition: "avg(requests/duration) > 100ms"
    frequency: "1m"
    severity: "warning"
    action: "email-team"
    
  - name: "Database Query Performance"
    condition: "avg(database/query-duration) > 500ms"
    frequency: "1m"
    severity: "critical"
    action: "page-oncall"
    
  - name: "Frontend Core Web Vitals"
    condition: "avg(browser/lcp) > 2500ms"
    frequency: "5m"
    severity: "warning"
    action: "slack-notification"
    
  - name: "Memory Usage High"
    condition: "avg(memory/usage) > 80%"
    frequency: "1m"
    severity: "critical"
    action: "auto-scale"

dashboards:
  - name: "Performance Overview"
    panels:
      - "API Response Times (95th percentile)"
      - "Database Query Performance"
      - "Frontend Core Web Vitals"
      - "Cache Hit Rates"
      - "Error Rates"
      - "Throughput (RPS)"
```
```

## Command Activation Process
When a user types:
```
@copilot /performance [action] [scope] [target]
```

The system should:
1. **Analyze Current State**: Use GitHub Copilot tools to assess performance baseline
2. **Identify Bottlenecks**: Discover performance issues through comprehensive analysis
3. **Implement Optimizations**: Apply scope-specific performance improvements
4. **Benchmark Results**: Measure performance improvements with before/after metrics
5. **Setup Monitoring**: Configure ongoing performance monitoring and alerting

## Notes
- All performance optimizations follow modern best practices and project-specific patterns
- Frontend optimizations include React 18 features, bundle optimization, and Core Web Vitals
- Backend optimizations cover caching, database queries, and API response times
- Monitoring includes both synthetic and real user monitoring (RUM)
- Performance budgets are enforced: <3s page loads, <100ms API responses
- All optimizations are measured and validated with comprehensive benchmarking
- Continuous monitoring ensures performance regressions are caught early
- Performance recommendations are prioritized by impact and implementation effort
