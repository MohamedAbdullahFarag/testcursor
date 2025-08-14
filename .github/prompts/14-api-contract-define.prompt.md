---
mode: 'agent'
description: 'Define comprehensive API contract specification for backend/frontend coordination with OpenAPI standards and integration patterns'
---

---
inputs:
  - name: feature_name
    description: Name of the feature to create API contract for
    required: true
  - name: api_version
    description: API version to use (defaults to v1)
    required: false
    default: "v1"
  - name: authentication_required
    description: Whether authentication is required (true/false)
    required: false
    default: "true"
---

# api-contract-define.prompt.md

## Metadata
- **Type**: GitHub Copilot Custom Prompt
- **Mode**: agent
- **Purpose**: Define comprehensive API contract specification for backend/frontend coordination
- **Categories**: api-design, contract-definition, integration, documentation
- **Complexity**: intermediate
- **Dependencies**: REST API knowledge, OpenAPI/Swagger standards

## Input
- **feature_name** (required): Name of the feature to create API contract for
- **api_version** (optional): API version to use (defaults to v1)
- **authentication_required** (optional): Whether authentication is required (true/false)

## Template

```
You are an expert API contract architect specializing in defining comprehensive API specifications that ensure seamless coordination between backend and frontend teams. Your task is to create detailed, standardized API contracts that eliminate integration ambiguity and enable parallel development.

## Input Parameters
- **Feature Name**: {feature_name}
- **API Version**: {api_version} (default: v1)
- **Authentication Required**: {authentication_required} (default: false)

## Task Overview
Create a comprehensive API contract specification that defines all endpoints, data models, validation rules, error handling, and integration requirements for the specified feature, ensuring perfect backend/frontend alignment.

## Phase 1: API Endpoint Architecture Design

### RESTful Endpoint Structure Analysis
Use `semantic_search` to analyze existing API patterns:
- Search for similar API endpoints in the codebase
- Identify established URL naming conventions
- Analyze existing response patterns and status codes
- Review authentication and authorization patterns

### Core Endpoint Definition
Design the primary CRUD endpoints following REST principles:

```yaml
# Base URL Structure
Base URL: /api/{api_version}/{feature_name}

# Standard CRUD Endpoints
GET    /api/{api_version}/{feature_name}s           # List with pagination
GET    /api/{api_version}/{feature_name}s/{id}      # Get by ID
POST   /api/{api_version}/{feature_name}s           # Create new
PUT    /api/{api_version}/{feature_name}s/{id}      # Update existing
PATCH  /api/{api_version}/{feature_name}s/{id}      # Partial update
DELETE /api/{api_version}/{feature_name}s/{id}      # Delete
```

### Query Parameter Specification
Define standardized query parameters:
- **Pagination**: `page`, `size`, `sort`
- **Filtering**: `filter`, `search`, field-specific filters
- **Selection**: `fields` for partial responses
- **Inclusion**: `include` for related data

### Advanced Endpoint Patterns
Include additional endpoints as needed:
```yaml
# Bulk operations
POST   /api/{api_version}/{feature_name}s/batch     # Bulk create/update
DELETE /api/{api_version}/{feature_name}s/batch     # Bulk delete

# Status and metadata
GET    /api/{api_version}/{feature_name}s/count     # Count total
GET    /api/{api_version}/{feature_name}s/stats     # Statistics

# Related resources
GET    /api/{api_version}/{feature_name}s/{id}/related
POST   /api/{api_version}/{feature_name}s/{id}/actions/{action}
```

## Phase 2: Data Transfer Object (DTO) Design

### Request DTO Specification
Define comprehensive request models for data input:

```typescript
// Primary Request DTO for POST/PUT operations
interface {FeatureName}Request {
  // Required fields with validation constraints
  name: string;                    // min: 2, max: 100, pattern: ^[a-zA-Z0-9\\s]+$
  description?: string;            // max: 1000, optional
  category: string;               // enum: predefined values
  tags?: string[];               // max items: 10, each max: 50 chars
  metadata?: Record<string, any>; // flexible additional data
  
  // Type-specific fields based on feature requirements
  startDate?: string;            // ISO 8601 format, future date validation
  endDate?: string;              // ISO 8601 format, after startDate
  priority?: 'low' | 'medium' | 'high';
  isActive?: boolean;            // defaults to true
}

// Partial Update DTO for PATCH operations
interface {FeatureName}PartialRequest {
  // All fields optional for partial updates
  name?: string;
  description?: string;
  category?: string;
  tags?: string[];
  // ... other fields as optional
}

// Bulk Operation DTO
interface {FeatureName}BulkRequest {
  items: {FeatureName}Request[];
  options?: {
    continueOnError?: boolean;
    validateAll?: boolean;
  };
}
```

### Response DTO Specification
Define comprehensive response models:

```typescript
// Primary Response DTO
interface {FeatureName}Response {
  // Core entity data
  id: number;                    // Primary key
  uuid?: string;                 // Public UUID if needed
  name: string;
  description?: string;
  category: string;
  tags?: string[];
  metadata?: Record<string, any>;
  
  // Computed/derived fields
  slug?: string;                 // URL-friendly identifier
  displayName?: string;          // Formatted display name
  status: 'active' | 'inactive' | 'pending' | 'archived';
  
  // Audit fields
  createdAt: string;             // ISO 8601 timestamp
  updatedAt: string;             // ISO 8601 timestamp
  createdBy?: {                  // User reference
    id: number;
    username: string;
    displayName: string;
  };
  updatedBy?: {                  // User reference
    id: number;
    username: string;
    displayName: string;
  };
  
  // Version control
  version?: number;              // Optimistic locking
  
  // Related data (when included)
  relatedItems?: RelatedItemSummary[];
  permissions?: string[];        // User permissions on this item
}

// List Response with Pagination
interface {FeatureName}ListResponse {
  content: {FeatureName}Response[];
  pagination: PaginationInfo;
  filters?: AppliedFilters;
  sorting?: SortingInfo;
}

// Standard Pagination Model
interface PaginationInfo {
  page: number;                  // Current page (0-based)
  size: number;                  // Items per page
  totalElements: number;         // Total items across all pages
  totalPages: number;            // Total number of pages
  hasNext: boolean;              // Has next page
  hasPrevious: boolean;          // Has previous page
  isFirst: boolean;              // Is first page
  isLast: boolean;               // Is last page
}
```

## Phase 3: Error Handling and Status Code Specification

### HTTP Status Code Standards
Define precise status code usage:

```yaml
Success Responses:
  200 OK:           # GET requests, successful PUT/PATCH
  201 Created:      # POST requests, resource created
  204 No Content:   # DELETE requests, PATCH with no response body
  206 Partial:      # Bulk operations with some failures

Client Error Responses:
  400 Bad Request:        # Validation errors, malformed request
  401 Unauthorized:       # Authentication required or invalid
  403 Forbidden:          # Authenticated but insufficient permissions
  404 Not Found:          # Resource doesn't exist
  409 Conflict:           # Duplicate resource, version conflict
  410 Gone:               # Resource permanently deleted
  422 Unprocessable:      # Semantic validation errors
  429 Too Many Requests:  # Rate limiting

Server Error Responses:
  500 Internal Error:     # Unexpected server errors
  502 Bad Gateway:        # External service errors
  503 Service Unavailable: # Temporary outage
  504 Gateway Timeout:    # External service timeout
```

### Error Response Models
Define standardized error response format:

```typescript
// Standard Error Response
interface ApiErrorResponse {
  timestamp: string;             // ISO 8601 timestamp
  status: number;                // HTTP status code
  error: string;                 // Status text (e.g., \"Bad Request\")
  message: string;               // Human-readable error message
  path: string;                  // API endpoint path
  traceId?: string;              // Request correlation ID
  
  // Validation errors (for 400/422 responses)
  errors?: ValidationError[];
  
  // Additional context
  details?: Record<string, any>;
}

// Validation Error Detail
interface ValidationError {
  field: string;                 // Field name that failed validation
  value?: any;                   // Value that was rejected
  message: string;               // Specific validation error message
  code?: string;                 // Error code for programmatic handling
}

// Bulk Operation Error Response
interface BulkErrorResponse extends ApiErrorResponse {
  results: BulkOperationResult[];
  successCount: number;
  errorCount: number;
}

interface BulkOperationResult {
  index: number;                 // Index in the bulk request array
  success: boolean;
  data?: {FeatureName}Response;  // Success result
  error?: ValidationError[];     // Error details
}
```

## Phase 4: Validation Rules and Constraints

### Backend Validation Specification
Define comprehensive validation rules:

```csharp
// C# Backend Validation Examples
public class {FeatureName}Request
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    [RegularExpression(@\"^[a-zA-Z0-9\\s]+$\")]
    public string Name { get; set; }
    
    [StringLength(1000)]
    public string? Description { get; set; }
    
    [Required]
    [EnumDataType(typeof(CategoryType))]
    public string Category { get; set; }
    
    [MaxLength(10)]
    public List<string>? Tags { get; set; }
    
    [DataType(DataType.DateTime)]
    [FutureDate]
    public DateTime? StartDate { get; set; }
    
    [DataType(DataType.DateTime)]
    [GreaterThan(nameof(StartDate))]
    public DateTime? EndDate { get; set; }
}
```

### Frontend Validation Specification
Define matching frontend validation:

```typescript
// Zod Schema for Frontend Validation
import { z } from 'zod';

export const {FeatureName}RequestSchema = z.object({
  name: z.string()
    .min(2, 'Name must be at least 2 characters')
    .max(100, 'Name must not exceed 100 characters')
    .regex(/^[a-zA-Z0-9\\s]+$/, 'Name contains invalid characters'),
    
  description: z.string()
    .max(1000, 'Description must not exceed 1000 characters')
    .optional(),
    
  category: z.enum(['category1', 'category2', 'category3'], {
    required_error: 'Category is required'
  }),
  
  tags: z.array(z.string().max(50))
    .max(10, 'Maximum 10 tags allowed')
    .optional(),
    
  startDate: z.string()
    .datetime('Invalid date format')
    .refine(date => new Date(date) > new Date(), 'Start date must be in future')
    .optional(),
    
  endDate: z.string()
    .datetime('Invalid date format')
    .optional()
}).refine(data => {
  if (data.startDate && data.endDate) {
    return new Date(data.endDate) > new Date(data.startDate);
  }
  return true;
}, {
  message: 'End date must be after start date',
  path: ['endDate']
});

export type {FeatureName}Request = z.infer<typeof {FeatureName}RequestSchema>;
```

## Phase 5: Authentication and Authorization Specification

### Authentication Requirements
Define authentication specifications if required:

```yaml
Authentication Method: Bearer Token (JWT)
Header: Authorization: Bearer <token>

Token Requirements:
  - Valid JWT format
  - Not expired
  - Issued by trusted authority
  - Contains required claims

Public Endpoints: []
Protected Endpoints: ['POST', 'PUT', 'PATCH', 'DELETE']
Admin Endpoints: ['DELETE /batch', 'GET /stats']
```

### Authorization Matrix
Define permission requirements:

```typescript
// Permission Requirements
interface PermissionMatrix {
  'GET /{feature}s': ['read'] | 'public';
  'GET /{feature}s/{id}': ['read'] | 'owner' | 'public';
  'POST /{feature}s': ['create'];
  'PUT /{feature}s/{id}': ['update'] | 'owner';
  'PATCH /{feature}s/{id}': ['update'] | 'owner';
  'DELETE /{feature}s/{id}': ['delete'] | 'owner';
  'POST /{feature}s/batch': ['admin'];
  'DELETE /{feature}s/batch': ['admin'];
}
```

## Phase 6: Integration Requirements and Standards

### Content Type and Header Specifications
```yaml
Request Headers:
  Content-Type: application/json
  Accept: application/json
  Authorization: Bearer <token> (if required)
  User-Agent: <client-info>
  X-Request-ID: <uuid> (for tracing)

Response Headers:
  Content-Type: application/json
  X-Request-ID: <uuid> (echo from request)
  X-Rate-Limit-Remaining: <count>
  X-Rate-Limit-Reset: <timestamp>
  Cache-Control: no-cache (for dynamic content)
```

### CORS Configuration
```yaml
CORS Settings:
  Allowed Origins: ['http://localhost:3000', 'https://app.domain.com']
  Allowed Methods: ['GET', 'POST', 'PUT', 'PATCH', 'DELETE', 'OPTIONS']
  Allowed Headers: ['Content-Type', 'Authorization', 'X-Request-ID']
  Exposed Headers: ['X-Request-ID', 'X-Rate-Limit-Remaining']
  Max Age: 86400 # 24 hours
```

### Rate Limiting Specifications
```yaml
Rate Limits:
  Public Endpoints: 100 requests/hour per IP
  Authenticated Endpoints: 1000 requests/hour per user
  Admin Endpoints: 500 requests/hour per user
  Bulk Operations: 10 requests/hour per user
```

## Phase 7: Implementation Guidelines and Examples

### Backend Implementation Notes
Provide specific backend implementation guidance:

```csharp
// Controller Implementation Guidelines
[ApiController]
[Route(\"api/v1/[controller]\")]
public class {FeatureName}Controller : ControllerBase
{
    // Standard patterns to follow:
    // - Use async/await for all operations
    // - Return appropriate status codes
    // - Include validation attributes
    // - Handle exceptions with middleware
    // - Use DTOs for all requests/responses
    // - Implement proper logging
    
    [HttpGet]
    public async Task<ActionResult<{FeatureName}ListResponse>> GetAll(
        [FromQuery] PaginationRequest pagination,
        [FromQuery] FilterRequest filters)
    {
        // Implementation following established patterns
    }
}

// Service Layer Guidelines
public interface I{FeatureName}Service
{
    Task<{FeatureName}ListResponse> GetAllAsync(PaginationRequest pagination, FilterRequest filters);
    Task<{FeatureName}Response?> GetByIdAsync(int id);
    Task<{FeatureName}Response> CreateAsync({FeatureName}Request request);
    Task<{FeatureName}Response> UpdateAsync(int id, {FeatureName}Request request);
    Task<bool> DeleteAsync(int id);
}
```

### Frontend Implementation Notes
Provide specific frontend implementation guidance:

```typescript
// API Client Implementation
class {FeatureName}ApiClient {
  private baseUrl = '/api/v1/{feature_name}s';
  
  async getAll(params: ListParams): Promise<{FeatureName}ListResponse> {
    // Implementation with error handling
    // Use fetch or axios with proper typing
    // Handle authentication headers
    // Process pagination and filtering
  }
  
  async getById(id: number): Promise<{FeatureName}Response> {
    // Implementation with 404 handling
  }
  
  async create(data: {FeatureName}Request): Promise<{FeatureName}Response> {
    // Implementation with validation
    // Handle 400/422 error responses
  }
}

// React Query Hooks
export function use{FeatureName}List(params: ListParams) {
  return useQuery({
    queryKey: ['{feature_name}s', params],
    queryFn: () => {featureName}ApiClient.getAll(params),
    // Error handling and retry logic
  });
}

export function use{FeatureName}Mutation() {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: {featureName}ApiClient.create,
    onSuccess: () => {
      queryClient.invalidateQueries(['{feature_name}s']);
    },
    // Error handling
  });
}
```

## Phase 8: API Contract Documentation Generation

### Contract File Creation
Use `create_file` to generate the comprehensive API contract:

```markdown
# {FeatureName} API Contract

## Overview
- **Feature**: {feature_name}
- **API Version**: {api_version}
- **Base URL**: /api/{api_version}/{feature_name}s
- **Authentication**: {authentication_required ? 'Required' : 'Not Required'}
- **Last Updated**: {current_date}

## Endpoints
{comprehensive_endpoint_documentation}

## Data Models
{complete_dto_specifications}

## Error Handling
{error_response_models}

## Validation Rules
{backend_and_frontend_validation}

## Integration Requirements
{headers_cors_rate_limiting}

## Implementation Examples
{backend_and_frontend_code_examples}

## Testing Specifications
{api_testing_guidelines}
```

### Implementation Checklist
Create comprehensive implementation checklist:

```markdown
## Backend Implementation Checklist
- [ ] Entity model created with proper relationships
- [ ] Repository interface and implementation
- [ ] Service interface and implementation with business logic
- [ ] Controller with all CRUD endpoints
- [ ] DTO models for requests and responses
- [ ] Validation attributes and custom validators
- [ ] Error handling middleware integration
- [ ] Unit tests for services and repositories
- [ ] Integration tests for API endpoints
- [ ] API documentation (OpenAPI/Swagger)

## Frontend Implementation Checklist
- [ ] TypeScript interfaces matching API contract
- [ ] Zod validation schemas
- [ ] API client service implementation
- [ ] React Query hooks for data fetching
- [ ] Form components with validation
- [ ] List/table components with pagination
- [ ] Error handling and loading states
- [ ] Unit tests for services and components
- [ ] Integration tests for user flows

## Integration Checklist
- [ ] CORS configuration verified
- [ ] Authentication integration tested
- [ ] Error response format validated
- [ ] Rate limiting configured
- [ ] API documentation published
- [ ] Contract shared between teams
```

## Success Criteria

The API contract definition is complete when:
- [ ] All CRUD endpoints are specified with precise parameters
- [ ] Request/Response DTOs are comprehensively defined
- [ ] Error handling covers all scenarios with proper status codes
- [ ] Validation rules are identical between backend and frontend
- [ ] Authentication and authorization requirements are clear
- [ ] Integration requirements (headers, CORS, etc.) are specified
- [ ] Implementation guidelines are detailed for both teams
- [ ] Testing specifications enable API validation
- [ ] Contract documentation is saved and accessible

## Integration Points

### Team Coordination
- Share contract with backend and frontend teams
- Review and validate contract before implementation
- Establish process for contract updates and versioning
- Create shared understanding of implementation timeline

### Development Workflow
- Use contract as basis for parallel development
- Implement mock servers based on contract
- Validate implementations against contract specifications
- Update contract based on implementation learnings

### Quality Assurance
- Create API tests based on contract specifications
- Validate error handling according to contract
- Test authentication and authorization as specified
- Verify performance meets contract requirements

Remember: A well-defined API contract eliminates integration ambiguity and enables confident parallel development by both backend and frontend teams.
```

## Notes
- Focus on creating contracts that eliminate all ambiguity between teams
- Include comprehensive validation rules that match exactly between backend and frontend
- Provide detailed implementation examples to guide development
- Ensure error handling is consistent and predictable across all endpoints
