# Exam Monitoring PRP

## Overview

This PRP defines the real-time exam monitoring system for proctors and administrators to oversee active exam sessions, detect potential violations, and manage exam integrity.

## Core Features

### 1. Real-time Monitoring Dashboard
- Active session tracking
- Student activity monitoring
- Violation detection and alerts
- Performance metrics
- Resource utilization

### 2. Proctor Interface
- Session management
- Student assistance
- Violation handling
- Emergency controls
- Communication tools

### 3. Analytics and Reporting
- Real-time statistics
- Violation reports
- Performance analytics
- System health metrics
- Audit logging

### 4. Security Controls
- Anti-cheating measures
- Browser security
- Network monitoring
- Activity logging
- Emergency protocols

## Implementation Blueprint

### 1. Data Models

```csharp
public class ExamMonitoringSessionEntity : BaseEntity
{
    public int ExamId { get; set; }
    public int SessionId { get; set; }
    public int ProctorId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string Status { get; set; }
    public int ViolationCount { get; set; }
    public int WarningCount { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
}

public class MonitoringEventEntity : BaseEntity
{
    public int SessionId { get; set; }
    public string EventType { get; set; }
    public string Severity { get; set; }
    public string Description { get; set; }
    public DateTime Timestamp { get; set; }
    public Dictionary<string, object> EventData { get; set; }
    public bool RequiresAction { get; set; }
    public bool IsResolved { get; set; }
    public string Resolution { get; set; }
}

public class ProctorActionEntity : BaseEntity
{
    public int SessionId { get; set; }
    public int ProctorId { get; set; }
    public string ActionType { get; set; }
    public string Description { get; set; }
    public DateTime Timestamp { get; set; }
    public Dictionary<string, object> ActionData { get; set; }
    public string Outcome { get; set; }
}
```

### 2. Service Layer

```csharp
public interface IExamMonitoringService
{
    Task<IEnumerable<ActiveSessionDto>> GetActiveSessionsAsync();
    Task<MonitoringSessionDto> GetSessionDetailsAsync(int sessionId);
    Task<bool> RecordEventAsync(MonitoringEventDto eventDto);
    Task<bool> TakeActionAsync(ProctorActionDto actionDto);
    Task<IEnumerable<ViolationDto>> GetViolationsAsync(int sessionId);
    Task<bool> ResolveViolationAsync(int violationId, string resolution);
    Task<MonitoringStatsDto> GetRealTimeStatsAsync(int examId);
}

public class ExamMonitoringService : IExamMonitoringService
{
    private readonly IExamMonitoringRepository _monitoringRepository;
    private readonly IExamSessionRepository _sessionRepository;
    private readonly IProctorRepository _proctorRepository;
    private readonly IHubContext<MonitoringHub> _hubContext;
    private readonly ILogger<ExamMonitoringService> _logger;

    public async Task<IEnumerable<ActiveSessionDto>> GetActiveSessionsAsync()
    {
        using var scope = _logger.BeginScope("Retrieving active exam sessions");

        try
        {
            var sessions = await _monitoringRepository.GetActiveSessionsAsync();
            var dtos = _mapper.Map<IEnumerable<ActiveSessionDto>>(sessions);

            foreach (var dto in dtos)
            {
                dto.RealTimeStats = await GetSessionStatsAsync(dto.SessionId);
            }

            _logger.LogInformation("Retrieved {Count} active sessions", dtos.Count());
            return dtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active sessions");
            throw;
        }
    }

    public async Task<bool> RecordEventAsync(MonitoringEventDto eventDto)
    {
        using var scope = _logger.BeginScope("Recording monitoring event for session {SessionId}",
            eventDto.SessionId);

        try
        {
            var eventEntity = _mapper.Map<MonitoringEventEntity>(eventDto);
            await _monitoringRepository.AddEventAsync(eventEntity);

            if (eventDto.Severity == "High")
            {
                await NotifyProctorsAsync(eventDto);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording monitoring event");
            throw;
        }
    }

    private async Task NotifyProctorsAsync(MonitoringEventDto eventDto)
    {
        await _hubContext.Clients.Group($"exam_{eventDto.ExamId}")
            .SendAsync("ViolationDetected", eventDto);
    }
}
```

### 3. SignalR Hub for Real-time Updates

```csharp
public class MonitoringHub : Hub
{
    private readonly IExamMonitoringService _monitoringService;
    private readonly ILogger<MonitoringHub> _logger;

    public async Task JoinExamMonitoring(int examId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"exam_{examId}");
    }

    public async Task LeaveExamMonitoring(int examId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"exam_{examId}");
    }

    public async Task SendWarning(int sessionId, string message)
    {
        await Clients.Group($"session_{sessionId}").SendAsync("WarningReceived", message);
    }

    public async Task UpdateSessionStatus(int sessionId, string status)
    {
        await Clients.Group($"exam_{sessionId}").SendAsync("SessionStatusUpdated", sessionId, status);
    }
}
```

### 4. Frontend Components

```typescript
// Types
interface MonitoringSession {
  sessionId: number;
  examId: number;
  studentId: number;
  studentName: string;
  status: string;
  startTime: string;
  duration: number;
  progress: number;
  violationCount: number;
  warningCount: number;
  lastActivity: string;
}

interface MonitoringEvent {
  eventType: string;
  severity: 'Low' | 'Medium' | 'High';
  description: string;
  timestamp: string;
  requiresAction: boolean;
  isResolved: boolean;
}

// Components
const MonitoringDashboard: React.FC = () => {
  const { examId } = useParams<{ examId: string }>();
  const { data: sessions } = useActiveSessions(examId);
  const { data: stats } = useRealTimeStats(examId);

  return (
    <div className="flex h-screen">
      <SessionsList sessions={sessions} />
      <div className="flex-1">
        <MonitoringHeader stats={stats} />
        <SessionDetails />
        <ViolationLog />
      </div>
    </div>
  );
};

const SessionsList: React.FC<{ sessions: MonitoringSession[] }> = ({ sessions }) => {
  return (
    <div className="w-64 bg-gray-50 p-4 overflow-y-auto">
      {sessions?.map(session => (
        <SessionCard
          key={session.sessionId}
          session={session}
          onClick={() => selectSession(session.sessionId)}
        />
      ))}
    </div>
  );
};

const ViolationLog: React.FC<{ sessionId: number }> = ({ sessionId }) => {
  const { data: violations } = useViolations(sessionId);
  const { mutate: resolveViolation } = useResolveViolation();

  return (
    <div className="h-1/3 overflow-y-auto border-t">
      <div className="p-4">
        <h3 className="text-lg font-semibold mb-4">
          {t('monitoring.violations.title')}
        </h3>
        <div className="space-y-2">
          {violations?.map(violation => (
            <ViolationCard
              key={violation.id}
              violation={violation}
              onResolve={resolveViolation}
            />
          ))}
        </div>
      </div>
    </div>
  );
};
```

### 5. Real-time Monitoring Hooks

```typescript
const useMonitoringConnection = (examId: string) => {
  const connection = useRef<HubConnection>();

  useEffect(() => {
    connection.current = new HubConnectionBuilder()
      .withUrl('/hubs/monitoring')
      .withAutomaticReconnect()
      .build();

    const connect = async () => {
      try {
        await connection.current?.start();
        await connection.current?.invoke('JoinExamMonitoring', examId);
      } catch (error) {
        console.error('Error connecting to monitoring hub:', error);
      }
    };

    connect();

    return () => {
      connection.current?.stop();
    };
  }, [examId]);

  return connection.current;
};

const useViolationNotifications = (examId: string) => {
  const connection = useMonitoringConnection(examId);
  const queryClient = useQueryClient();

  useEffect(() => {
    connection?.on('ViolationDetected', (violation: MonitoringEvent) => {
      queryClient.invalidateQueries(['violations', examId]);
      showViolationAlert(violation);
    });

    return () => {
      connection?.off('ViolationDetected');
    };
  }, [connection, examId, queryClient]);
};
```

### Integration Points

```yaml
DATABASE:
  - tables:
    - ExamMonitoringSessions
    - MonitoringEvents
    - ProctorActions
    - ViolationLog
  - indexes:
    - IX_MonitoringEvents_SessionId
    - IX_MonitoringEvents_Timestamp
    - IX_ProctorActions_SessionId

REAL-TIME:
  - channels:
    - exam_{examId}
    - session_{sessionId}
    - proctor_{proctorId}
  - events:
    - ViolationDetected
    - SessionStatusUpdated
    - ProctorActionTaken

MONITORING:
  - metrics:
    - ActiveSessionCount
    - ViolationRate
    - ResponseLatency
    - SystemResources
  - alerts:
    - HighViolationRate
    - SystemOverload
    - ConnectionIssues

SECURITY:
  - features:
    - Proctor Authentication
    - Action Auditing
    - Access Control
    - Data Encryption
```

### Performance Considerations

1. **Real-time Updates**:
   - WebSocket connection management
   - Event batching
   - Selective updates
   - Connection recovery
   - Load balancing

2. **Data Management**:
   - Event streaming
   - Data aggregation
   - Caching strategy
   - Archive policies
   - Cleanup routines

### Error Handling

```typescript
const handleMonitoringError = async (error: Error) => {
  if (error instanceof ConnectionError) {
    await attemptReconnection();
  } else if (error instanceof DataSyncError) {
    await resyncData();
  } else {
    logError(error);
    showErrorNotification();
  }
};

const attemptReconnection = async () => {
  let attempts = 0;
  while (attempts < 3) {
    try {
      await reconnectMonitoring();
      break;
    } catch (error) {
      attempts++;
      await delay(1000 * Math.pow(2, attempts));
    }
  }
};
```

### Internationalization

```typescript
const monitoringTranslations = {
  en: {
    monitoring: {
      dashboard: {
        title: "Exam Monitoring Dashboard",
        activeSessions: "Active Sessions",
        violations: "Violations",
        stats: "Real-time Statistics"
      },
      session: {
        status: {
          active: "Active",
          suspended: "Suspended",
          completed: "Completed",
          terminated: "Terminated"
        },
        actions: {
          sendWarning: "Send Warning",
          suspend: "Suspend Session",
          terminate: "Terminate Session",
          assist: "Assist Student"
        }
      },
      violation: {
        severity: {
          low: "Low",
          medium: "Medium",
          high: "High"
        },
        status: {
          new: "New",
          investigating: "Investigating",
          resolved: "Resolved"
        }
      }
    }
  },
  ar: {
    monitoring: {
      dashboard: {
        title: "لوحة مراقبة الاختبار",
        activeSessions: "الجلسات النشطة",
        violations: "المخالفات",
        stats: "إحصائيات مباشرة"
      },
      session: {
        status: {
          active: "نشط",
          suspended: "معلق",
          completed: "مكتمل",
          terminated: "منتهي"
        },
        actions: {
          sendWarning: "إرسال تحذير",
          suspend: "تعليق الجلسة",
          terminate: "إنهاء الجلسة",
          assist: "مساعدة الطالب"
        }
      },
      violation: {
        severity: {
          low: "منخفض",
          medium: "متوسط",
          high: "مرتفع"
        },
        status: {
          new: "جديد",
          investigating: "قيد التحقيق",
          resolved: "تم الحل"
        }
      }
    }
  }
};
```
