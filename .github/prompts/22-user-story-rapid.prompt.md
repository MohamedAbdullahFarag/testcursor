---
mode: agent
description: "Rapid user story analysis with contract-first API design and full-stack implementation planning"
---

---
inputs:
  - name: user_story
    description: User story description to analyze
    required: true
  - name: analysis_focus
    description: Analysis focus (requirements, implementation, testing)
    required: false
    default: "implementation"
---
# user-story-rapid.prompt.md

## Metadata
- **Type**: GitHub Copilot Custom Prompt
- **Mode**: agent
- **Purpose**: Rapidly analyze user stories and create detailed implementation plans for separate backend and frontend projects
- **Categories**: user-story-analysis, implementation-planning, api-design, full-stack-development
- **Complexity**: advanced
- **Dependencies**: user story analysis, API design, backend/frontend architecture patterns

## Input
- **user_story** (required): The user story to analyze and plan implementation for
- **tech_stack** (optional): Specific technology stack requirements (default: Java/Spring + React/TypeScript)
- **planning_depth** (optional): Level of planning detail (rapid/detailed/comprehensive)

## Template

```
You are an expert user story analyst and implementation planner focused on rapidly creating comprehensive implementation plans for full-stack development projects. Your goal is to parse user stories, design API contracts, plan backend and frontend implementations, and create detailed execution strategies.

## Input Parameters
- **User Story**: {user_story}
- **Tech Stack**: {tech_stack} (default: Java/Spring Boot + React/TypeScript)
- **Planning Depth**: {planning_depth} (default: detailed)

## Task Overview
Analyze the provided user story to extract requirements, design API contracts first, create detailed implementation plans for both backend and frontend projects, plan integration strategies, and provide comprehensive validation approaches with risk mitigation strategies.

## Phase 1: Comprehensive User Story Analysis

### User Story Parsing and Requirements Extraction
```typescript
// Comprehensive user story analysis structure
interface UserStoryAnalysis {
  story_components: {
    user_persona: string;
    desired_feature: string;
    business_benefit: string;
    story_complexity: 'simple' | 'medium' | 'complex' | 'epic';
  };
  
  acceptance_criteria: {
    explicit_criteria: Array<{
      criterion: string;
      validation_method: string;
      priority: 'must_have' | 'should_have' | 'could_have';
    }>;
    implicit_criteria: Array<{
      criterion: string;
      reasoning: string;
      validation_approach: string;
    }>;
  };
  
  non_functional_requirements: {
    performance_requirements: Array<{
      metric: string;
      target_value: string;
      measurement_method: string;
    }>;
    security_requirements: Array<{
      requirement: string;
      implementation_approach: string;
      validation_method: string;
    }>;
    usability_requirements: Array<{
      requirement: string;
      design_considerations: string[];
      accessibility_needs: string[];
    }>;
    scalability_requirements: Array<{
      requirement: string;
      technical_implications: string[];
      implementation_strategy: string;
    }>;
  };
  
  success_metrics: Array<{
    metric_name: string;
    measurement_approach: string;
    target_value: string;
    business_impact: string;
  }>;
  
  domain_context: {
    business_domain: string;
    key_entities: string[];
    business_rules: string[];
    integration_points: string[];
  };
}

// Parse and analyze user story comprehensively
function analyzeUserStory(userStory: string): UserStoryAnalysis {
  // Extract story components using natural language processing
  // Identify explicit and implicit acceptance criteria
  // Derive non-functional requirements from context
  // Define measurable success metrics
}
```

### Requirements Prioritization and Scope Definition
Use `semantic_search` to understand similar implementations in codebase:

```typescript
// Define scope and priorities for implementation
interface ScopeDefinition {
  mvp_scope: {
    core_features: string[];
    essential_apis: string[];
    minimum_ui_components: string[];
    critical_validations: string[];
  };
  
  extended_scope: {
    enhanced_features: string[];
    additional_apis: string[];
    advanced_ui_components: string[];
    comprehensive_validations: string[];
  };
  
  future_scope: {
    potential_features: string[];
    scalability_enhancements: string[];
    integration_opportunities: string[];
    optimization_areas: string[];
  };
  
  implementation_phases: Array<{
    phase_number: number;
    phase_name: string;
    deliverables: string[];
    success_criteria: string[];
    estimated_effort: string;
  }>;
}
```

## Phase 2: API Contract Design (Contract-First Approach)

### Comprehensive API Design and Documentation
```typescript
// Design comprehensive API contract
interface APIContract {
  api_metadata: {
    version: string;
    base_url: string;
    authentication: string;
    rate_limiting: string;
  };
  
  endpoints: Array<{
    method: 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE';
    path: string;
    summary: string;
    description: string;
    
    parameters: Array<{
      name: string;
      location: 'path' | 'query' | 'header' | 'body';
      type: string;
      required: boolean;
      validation: string[];
    }>;
    
    request_body?: {
      content_type: string;
      schema: any;
      examples: any[];
    };
    
    responses: Array<{
      status_code: number;
      description: string;
      schema: any;
      examples: any[];
    }>;
    
    security_requirements: string[];
    rate_limit: string;
  }>;
  
  data_models: Array<{
    model_name: string;
    description: string;
    properties: Array<{
      name: string;
      type: string;
      required: boolean;
      validation_rules: string[];
      description: string;
    }>;
    relationships: Array<{
      related_model: string;
      relationship_type: 'one_to_one' | 'one_to_many' | 'many_to_many';
      foreign_key?: string;
    }>;
  }>;
  
  error_handling: {
    error_format: any;
    common_errors: Array<{
      error_code: string;
      http_status: number;
      description: string;
      resolution_guidance: string;
    }>;
  };
}

// Generate comprehensive API contract
function designAPIContract(requirements: UserStoryAnalysis): APIContract {
  // Design RESTful endpoints following best practices
  // Create comprehensive data models with validation
  // Plan error handling and security requirements
  // Include examples and documentation
}
```

### OpenAPI Specification Generation
Use `create_file` to generate OpenAPI specification:

```yaml
# Generate comprehensive OpenAPI specification
openapi: 3.0.3
info:
  title: "{feature_name} API"
  description: "API for {user_story_summary}"
inputs:
  - name: user_story
    description: User story description to analyze
    required: true
  - name: analysis_focus
    description: Analysis focus (requirements, implementation, testing)
    required: false
    default: "implementation"
  version: "1.0.0"
  contact:
    name: "Development Team"
    email: "dev@company.com"

servers:
  - url: "http://localhost:8080/api/v1"
    description: "Development server"
inputs:
  - name: user_story
    description: User story description to analyze
    required: true
  - name: analysis_focus
    description: Analysis focus (requirements, implementation, testing)
    required: false
    default: "implementation"
  - url: "https://api.company.com/v1"
    description: "Production server"

inputs:
  - name: user_story
    description: User story description to analyze
    required: true
  - name: analysis_focus
    description: Analysis focus (requirements, implementation, testing)
    required: false
    default: "implementation"
security:
  - bearerAuth: []

paths:
  /{resource}:
    get:
      summary: "List {resources} with pagination"
      description: "Retrieve a paginated list of {resources}"
inputs:
  - name: user_story
    description: User story description to analyze
    required: true
  - name: analysis_focus
    description: Analysis focus (requirements, implementation, testing)
    required: false
    default: "implementation"
      parameters:
        - name: page
          in: query
          schema:
            type: integer
            minimum: 0
            default: 0
        - name: size
          in: query
          schema:
            type: integer
            minimum: 1
            maximum: 100
            default: 20
        - name: sort
          in: query
          schema:
            type: string
            default: "id,asc"
      responses:
        '200':
          description: "Successful response"
inputs:
  - name: user_story
    description: User story description to analyze
    required: true
  - name: analysis_focus
    description: Analysis focus (requirements, implementation, testing)
    required: false
    default: "implementation"
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/PagedResponse'
        '400':
          $ref: '#/components/responses/BadRequest'
        '401':
          $ref: '#/components/responses/Unauthorized'
        '500':
          $ref: '#/components/responses/InternalServerError'

    post:
      summary: "Create new {resource}"
      description: "Create a new {resource} with validation"
inputs:
  - name: user_story
    description: User story description to analyze
    required: true
  - name: analysis_focus
    description: Analysis focus (requirements, implementation, testing)
    required: false
    default: "implementation"
      requestBody:
        required: true
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/Create{Resource}Request'
      responses:
        '201':
          description: "Resource created successfully"
inputs:
  - name: user_story
    description: User story description to analyze
    required: true
  - name: analysis_focus
    description: Analysis focus (requirements, implementation, testing)
    required: false
    default: "implementation"
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/{Resource}Response'
        '400':
          $ref: '#/components/responses/BadRequest'
        '401':
          $ref: '#/components/responses/Unauthorized'
        '409':
          $ref: '#/components/responses/Conflict'
        '500':
          $ref: '#/components/responses/InternalServerError'

  /{resource}/{id}:
    get:
      summary: "Get {resource} by ID"
      description: "Retrieve a specific {resource} by its ID"
inputs:
  - name: user_story
    description: User story description to analyze
    required: true
  - name: analysis_focus
    description: Analysis focus (requirements, implementation, testing)
    required: false
    default: "implementation"
      parameters:
        - name: id
          in: path
          required: true
          schema:
            type: string
            format: uuid
      responses:
        '200':
          description: "Successful response"
inputs:
  - name: user_story
    description: User story description to analyze
    required: true
  - name: analysis_focus
    description: Analysis focus (requirements, implementation, testing)
    required: false
    default: "implementation"
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/{Resource}Response'
        '404':
          $ref: '#/components/responses/NotFound'
        '401':
          $ref: '#/components/responses/Unauthorized'
        '500':
          $ref: '#/components/responses/InternalServerError'

components:
  securitySchemes:
    bearerAuth:
      type: http
      scheme: bearer
      bearerFormat: JWT

  schemas:
    {Resource}Response:
      type: object
      required: {required_fields}
      properties:
        {comprehensive_property_definitions}

    Create{Resource}Request:
      type: object
      required: {required_fields}
      properties:
        {comprehensive_property_definitions_with_validation}

    PagedResponse:
      type: object
      properties:
        content:
          type: array
          items:
            $ref: '#/components/schemas/{Resource}Response'
        page:
          $ref: '#/components/schemas/PageInfo'

    PageInfo:
      type: object
      properties:
        number:
          type: integer
        size:
          type: integer
        totalElements:
          type: integer
        totalPages:
          type: integer

    ErrorResponse:
      type: object
      required:
        - timestamp
        - status
        - error
        - message
        - path
      properties:
        timestamp:
          type: string
          format: date-time
        status:
          type: integer
        error:
          type: string
        message:
          type: string
        path:
          type: string
        validation_errors:
          type: array
          items:
            $ref: '#/components/schemas/ValidationError'

    ValidationError:
      type: object
      properties:
        field:
          type: string
        message:
          type: string
        rejected_value:
          type: string

  responses:
    BadRequest:
      description: "Bad request"
inputs:
  - name: user_story
    description: User story description to analyze
    required: true
  - name: analysis_focus
    description: Analysis focus (requirements, implementation, testing)
    required: false
    default: "implementation"
      content:
        application/json:
          schema:
            $ref: '#/components/schemas/ErrorResponse'

    Unauthorized:
      description: "Unauthorized"
inputs:
  - name: user_story
    description: User story description to analyze
    required: true
  - name: analysis_focus
    description: Analysis focus (requirements, implementation, testing)
    required: false
    default: "implementation"
      content:
        application/json:
          schema:
            $ref: '#/components/schemas/ErrorResponse'

    NotFound:
      description: "Resource not found"
inputs:
  - name: user_story
    description: User story description to analyze
    required: true
  - name: analysis_focus
    description: Analysis focus (requirements, implementation, testing)
    required: false
    default: "implementation"
      content:
        application/json:
          schema:
            $ref: '#/components/schemas/ErrorResponse'

    Conflict:
      description: "Resource conflict"
inputs:
  - name: user_story
    description: User story description to analyze
    required: true
  - name: analysis_focus
    description: Analysis focus (requirements, implementation, testing)
    required: false
    default: "implementation"
      content:
        application/json:
          schema:
            $ref: '#/components/schemas/ErrorResponse'

    InternalServerError:
      description: "Internal server error"
inputs:
  - name: user_story
    description: User story description to analyze
    required: true
  - name: analysis_focus
    description: Analysis focus (requirements, implementation, testing)
    required: false
    default: "implementation"
      content:
        application/json:
          schema:
            $ref: '#/components/schemas/ErrorResponse'
```

## Phase 3: Backend Implementation Planning

### Java/Spring Boot Backend Architecture
```typescript
// Comprehensive backend implementation plan
interface BackendImplementationPlan {
  project_structure: {
    base_package: string;
    
    packages: Array<{
      package_name: string;
      purpose: string;
      classes: Array<{
        class_name: string;
        class_type: 'entity' | 'repository' | 'service' | 'controller' | 'dto' | 'mapper' | 'exception' | 'config';
        responsibilities: string[];
        dependencies: string[];
      }>;
    }>;
  };
  
  implementation_layers: {
    entity_layer: Array<{
      entity_name: string;
      table_name: string;
      jpa_annotations: string[];
      relationships: string[];
      validation_constraints: string[];
    }>;
    
    repository_layer: Array<{
      repository_name: string;
      extends: string;
      custom_queries: string[];
      query_methods: string[];
    }>;
    
    service_layer: Array<{
      service_name: string;
      business_methods: string[];
      transaction_boundaries: string[];
      validation_logic: string[];
    }>;
    
    controller_layer: Array<{
      controller_name: string;
      base_path: string;
      endpoints: string[];
      security_configuration: string[];
    }>;
    
    dto_layer: Array<{
      dto_name: string;
      dto_type: 'request' | 'response' | 'internal';
      validation_annotations: string[];
      mapping_strategy: string;
    }>;
  };
  
  cross_cutting_concerns: {
    exception_handling: string[];
    security_configuration: string[];
    logging_strategy: string[];
    validation_strategy: string[];
    transaction_management: string[];
  };
  
  testing_strategy: {
    unit_tests: string[];
    integration_tests: string[];
    test_data_setup: string[];
    mocking_strategy: string[];
  };
}
```

### Backend Implementation Order and Dependencies
```bash
# Backend implementation sequence
create_backend_implementation_plan() {
    echo "🏗️ Creating Backend Implementation Plan"
    
    cat > "backend-implementation-plan.md" << 'EOF'
# Backend Implementation Plan: {feature_name}

## Implementation Order

### Phase 1: Foundation Setup
1. **Create Entity Classes**
   ```java
   @Entity
   @Table(name = "{table_name}")
   public class {EntityName} {
       @Id
       @GeneratedValue(strategy = GenerationType.UUID)
       private UUID id;
       
       // Properties with JPA annotations
       // Validation constraints
       // Relationships
   }
   ```

2. **Create Repository Interfaces**
   ```java
   @Repository
   public interface {EntityName}Repository extends JpaRepository<{EntityName}, UUID> {
       // Custom query methods
       // Derived query methods
       // Native queries if needed
   }
   ```

3. **Create DTO Classes**
   ```java
   public record Create{EntityName}Request(
       @NotBlank String name,
       @Email String email,
       // Validation annotations
   ) {}
   
   public record {EntityName}Response(
       UUID id,
       String name,
       // Response fields
   ) {}
   ```

### Phase 2: Business Logic
4. **Create Mapper Interfaces**
   ```java
   @Mapper(componentModel = "spring")
   public interface {EntityName}Mapper {
       {EntityName}Response toResponse({EntityName} entity);
       {EntityName} toEntity(Create{EntityName}Request request);
   }
   ```

5. **Create Service Classes**
   ```java
   @Service
   @Transactional
   public class {EntityName}Service {
       // Business logic methods
       // Validation logic
       // Transaction boundaries
   }
   ```

### Phase 3: API Layer
6. **Create Controller Classes**
   ```java
   @RestController
   @RequestMapping("/api/v1/{resources}")
   @Validated
   public class {EntityName}Controller {
       // REST endpoints
       // OpenAPI documentation
       // Security configuration
   }
   ```

7. **Create Exception Handling**
   ```java
   @ControllerAdvice
   public class GlobalExceptionHandler {
       // Exception mapping
       // Error response formatting
       // Logging
   }
   ```

### Phase 4: Testing
8. **Create Unit Tests**
   - Repository tests with @DataJpaTest
   - Service tests with @ExtendWith(MockitoExtension.class)
   - Controller tests with @WebMvcTest

9. **Create Integration Tests**
   - @SpringBootTest with TestContainers
   - Full API workflow tests
   - Database integration tests
EOF
}
```

## Phase 4: Frontend Implementation Planning

### React/TypeScript Frontend Architecture
```typescript
// Comprehensive frontend implementation plan
interface FrontendImplementationPlan {
  project_structure: {
    feature_directory: string;
    
    subdirectories: Array<{
      directory_name: string;
      purpose: string;
      files: Array<{
        file_name: string;
        file_type: 'component' | 'hook' | 'api' | 'schema' | 'type' | 'test' | 'story';
        responsibilities: string[];
        dependencies: string[];
      }>;
    }>;
  };
  
  implementation_layers: {
    type_definitions: Array<{
      type_name: string;
      source: 'api_contract' | 'ui_specific' | 'form_data';
      properties: string[];
      relationships: string[];
    }>;
    
    validation_schemas: Array<{
      schema_name: string;
      validation_library: 'zod' | 'yup' | 'joi';
      validation_rules: string[];
      error_messages: string[];
    }>;
    
    api_layer: Array<{
      api_function_name: string;
      http_method: string;
      endpoint: string;
      request_type?: string;
      response_type: string;
      error_handling: string[];
    }>;
    
    hooks_layer: Array<{
      hook_name: string;
      hook_type: 'data_fetching' | 'form_management' | 'state_management' | 'side_effect';
      dependencies: string[];
      return_values: string[];
    }>;
    
    component_layer: Array<{
      component_name: string;
      component_type: 'page' | 'layout' | 'form' | 'display' | 'interactive';
      props_interface: string;
      state_management: string[];
      styling_approach: string;
    }>;
  };
  
  ui_ux_considerations: {
    responsive_design: string[];
    accessibility_requirements: string[];
    loading_states: string[];
    error_states: string[];
    empty_states: string[];
  };
  
  testing_strategy: {
    component_tests: string[];
    hook_tests: string[];
    integration_tests: string[];
    e2e_scenarios: string[];
  };
}
```

### Frontend Implementation Order and Structure
```bash
# Frontend implementation sequence
create_frontend_implementation_plan() {
    echo "🎨 Creating Frontend Implementation Plan"
    
    cat > "frontend-implementation-plan.md" << 'EOF'
# Frontend Implementation Plan: {feature_name}

## Project Structure
```
src/features/{feature}/
├── api/
│   ├── {feature}Api.ts      # API client functions
│   └── index.ts             # API exports
├── components/
│   ├── {Feature}Card.tsx    # Display component
│   ├── {Feature}Form.tsx    # Form component
│   ├── {Feature}List.tsx    # List component
│   └── index.ts             # Component exports
├── hooks/
│   ├── use{Feature}.ts      # Data fetching hook
│   ├── use{Feature}Form.ts  # Form management hook
│   └── index.ts             # Hook exports
├── schemas/
│   ├── {feature}Schema.ts   # Zod validation schemas
│   └── index.ts             # Schema exports
├── types/
│   ├── {feature}Types.ts    # TypeScript interfaces
│   └── index.ts             # Type exports
├── __tests__/
│   ├── components/          # Component tests
│   ├── hooks/               # Hook tests
│   └── api/                 # API tests
└── index.ts                 # Feature exports
```

## Implementation Order

### Phase 1: Type Definitions and Validation
1. **Create TypeScript Types**
   ```typescript
   // Based on API contract
   export interface {Feature} {
     id: string;
     name: string;
     // Properties matching API response
   }
   
   export interface Create{Feature}Request {
     name: string;
     // Properties matching API request
   }
   
   export interface {Feature}ListResponse {
     content: {Feature}[];
     page: PageInfo;
   }
   ```

2. **Create Zod Validation Schemas**
   ```typescript
   import { z } from 'zod';
   
   export const create{Feature}Schema = z.object({
     name: z.string().min(1, 'Name is required'),
     // Validation rules matching backend
   });
   
   export type Create{Feature}FormData = z.infer<typeof create{Feature}Schema>;
   ```

### Phase 2: API Integration
3. **Create API Client Functions**
   ```typescript
   import { axiosInstance } from '@/shared/api';
   
   export const {feature}Api = {
     getAll: (params: ListParams): Promise<{Feature}ListResponse> =>
       axiosInstance.get('/{resources}', { params }).then(res => res.data),
     
     getById: (id: string): Promise<{Feature}> =>
       axiosInstance.get(`/{resources}/${id}`).then(res => res.data),
     
     create: (data: Create{Feature}Request): Promise<{Feature}> =>
       axiosInstance.post('/{resources}', data).then(res => res.data),
     
     update: (id: string, data: Update{Feature}Request): Promise<{Feature}> =>
       axiosInstance.put(`/{resources}/${id}`, data).then(res => res.data),
     
     delete: (id: string): Promise<void> =>
       axiosInstance.delete(`/{resources}/${id}`).then(res => res.data),
   };
   ```

### Phase 3: Data Management Hooks
4. **Create Data Fetching Hooks**
   ```typescript
   import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
   
   export const use{Feature}List = (params: ListParams) => {
     return useQuery({
       queryKey: ['{feature}', 'list', params],
       queryFn: () => {feature}Api.getAll(params),
       staleTime: 5 * 60 * 1000, // 5 minutes
     });
   };
   
   export const useCreate{Feature} = () => {
     const queryClient = useQueryClient();
     
     return useMutation({
       mutationFn: {feature}Api.create,
       onSuccess: () => {
         queryClient.invalidateQueries({ queryKey: ['{feature}'] });
       },
     });
   };
   ```

5. **Create Form Management Hooks**
   ```typescript
   import { useForm } from 'react-hook-form';
   import { zodResolver } from '@hookform/resolvers/zod';
   
   export const use{Feature}Form = (defaultValues?: Partial<Create{Feature}FormData>) => {
     return useForm<Create{Feature}FormData>({
       resolver: zodResolver(create{Feature}Schema),
       defaultValues,
     });
   };
   ```

### Phase 4: UI Components
6. **Create Display Components**
   ```tsx
   interface {Feature}CardProps {
     {feature}: {Feature};
     onEdit?: (id: string) => void;
     onDelete?: (id: string) => void;
   }
   
   export const {Feature}Card: React.FC<{Feature}CardProps> = memo(({
     {feature},
     onEdit,
     onDelete
   }) => {
     return (
       <div className="border rounded-lg p-4">
         {/* Component implementation */}
       </div>
     );
   });
   ```

7. **Create Form Components**
   ```tsx
   interface {Feature}FormProps {
     defaultValues?: Partial<Create{Feature}FormData>;
     onSubmit: (data: Create{Feature}FormData) => void;
     isLoading?: boolean;
   }
   
   export const {Feature}Form: React.FC<{Feature}FormProps> = ({
     defaultValues,
     onSubmit,
     isLoading
   }) => {
     const form = use{Feature}Form(defaultValues);
     
     return (
       <form onSubmit={form.handleSubmit(onSubmit)}>
         {/* Form implementation */}
       </form>
     );
   };
   ```

8. **Create List Components**
   ```tsx
   interface {Feature}ListProps {
     params?: ListParams;
     onItemSelect?: (item: {Feature}) => void;
   }
   
   export const {Feature}List: React.FC<{Feature}ListProps> = ({
     params,
     onItemSelect
   }) => {
     const { data, isLoading, error } = use{Feature}List(params || {});
     
     if (isLoading) return <LoadingSpinner />;
     if (error) return <ErrorMessage error={error} />;
     
     return (
       <div className="space-y-4">
         {/* List implementation */}
       </div>
     );
   };
   ```

### Phase 5: Testing
9. **Create Component Tests**
   ```typescript
   import { render, screen } from '@testing-library/react';
   import { {Feature}Card } from '../{Feature}Card';
   
   describe('{Feature}Card', () => {
     it('should render {feature} information', () => {
       // Test implementation
     });
   });
   ```

10. **Create Hook Tests**
    ```typescript
    import { renderHook } from '@testing-library/react';
    import { use{Feature}List } from '../use{Feature}';
    
    describe('use{Feature}List', () => {
      it('should fetch {feature} list', () => {
        // Test implementation
      });
    });
    ```
EOF
}
```

## Phase 5: Integration and Validation Planning

### Integration Strategy and Configuration
```bash
# Comprehensive integration planning
create_integration_plan() {
    echo "🔗 Creating Integration Plan"
    
    cat > "integration-plan.md" << 'EOF'
# Integration Plan: Backend + Frontend

## CORS Configuration (Backend)
```java
@Configuration
@EnableWebMvc
public class WebConfig implements WebMvcConfigurer {
    
    @Override
    public void addCorsMappings(CorsRegistry registry) {
        registry.addMapping("/api/**")
                .allowedOrigins("http://localhost:3000", "https://app.company.com")
                .allowedMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                .allowedHeaders("*")
                .allowCredentials(true)
                .maxAge(3600);
    }
}
```

## Environment Configuration (Frontend)
```typescript
// .env.development
VITE_API_BASE_URL=http://localhost:8080/api/v1
VITE_API_TIMEOUT=10000

// .env.production
VITE_API_BASE_URL=https://api.company.com/v1
VITE_API_TIMEOUT=10000

// API configuration
export const apiConfig = {
  baseURL: import.meta.env.VITE_API_BASE_URL,
  timeout: parseInt(import.meta.env.VITE_API_TIMEOUT),
  headers: {
    'Content-Type': 'application/json',
  },
};
```

## Error Response Handling
```typescript
// API interceptors for consistent error handling
axiosInstance.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Handle authentication errors
      authService.logout();
      window.location.href = '/login';
    }
    
    if (error.response?.status === 400) {
      // Handle validation errors
      const validationErrors = error.response.data.validation_errors;
      // Process and display validation errors
    }
    
    return Promise.reject(error);
  }
);
```

## Loading States Management
```typescript
// Global loading state management
export const useGlobalLoading = () => {
  const [loadingStates, setLoadingStates] = useState<Record<string, boolean>>({});
  
  const setLoading = (key: string, isLoading: boolean) => {
    setLoadingStates(prev => ({
      ...prev,
      [key]: isLoading
    }));
  };
  
  const isAnyLoading = Object.values(loadingStates).some(Boolean);
  
  return { loadingStates, setLoading, isAnyLoading };
};
```

## Optimistic Updates
```typescript
// Optimistic updates for better UX
export const useOptimistic{Feature}Updates = () => {
  const queryClient = useQueryClient();
  
  const optimisticCreate = useMutation({
    mutationFn: {feature}Api.create,
    onMutate: async (newItem) => {
      // Cancel outgoing queries
      await queryClient.cancelQueries({ queryKey: ['{feature}'] });
      
      // Snapshot the previous value
      const previousItems = queryClient.getQueryData(['{feature}']);
      
      // Optimistically update
      queryClient.setQueryData(['{feature}'], (old: any) => ({
        ...old,
        content: [...old.content, { ...newItem, id: 'temp-id' }]
      }));
      
      return { previousItems };
    },
    onError: (err, newItem, context) => {
      // Rollback on error
      queryClient.setQueryData(['{feature}'], context?.previousItems);
    },
    onSettled: () => {
      // Refresh data
      queryClient.invalidateQueries({ queryKey: ['{feature}'] });
    },
  });
  
  return { optimisticCreate };
};
```
EOF
}
```

### Comprehensive Validation Commands
```bash
# Validation and testing commands
create_validation_commands() {
    echo "🧪 Creating Validation Commands"
    
    cat > "validation-commands.md" << 'EOF'
# Validation Commands

## Backend Validation (Java/Spring Boot)
```bash
# Build and test
./gradlew clean build test

# Run with coverage
./gradlew test jacocoTestReport

# Integration tests
./gradlew integrationTest

# Start application
./gradlew bootRun

# Health check
curl -f http://localhost:8080/actuator/health

# API documentation check
curl -f http://localhost:8080/swagger-ui/index.html
```

## Frontend Validation (React/TypeScript)
```bash
# Type checking
npm run type-check

# Linting
npm run lint

# Unit tests with coverage
npm run test:coverage

# Component tests
npm run test:components

# Integration tests
npm run test:integration

# Build verification
npm run build

# Start development server
npm run dev

# E2E tests
npm run test:e2e
```

## Integration Validation
```bash
# Start backend
cd backend && ./gradlew bootRun &

# Wait for backend to be ready
while ! curl -f http://localhost:8080/actuator/health; do
  echo "Waiting for backend..."
  sleep 2
done

# Start frontend
cd frontend && npm run dev &

# Wait for frontend to be ready
while ! curl -f http://localhost:3000; do
  echo "Waiting for frontend..."
  sleep 2
done

# Run integration tests
npm run test:integration

# Run E2E tests
npm run test:e2e

# Manual validation checklist:
# [ ] User can see list of {resources}
# [ ] User can create new {resource}
# [ ] User can edit existing {resource}
# [ ] User can delete {resource}
# [ ] Validation errors display correctly
# [ ] Loading states work properly
# [ ] Error handling works as expected
```
EOF
}
```

## Phase 6: Risk Mitigation and Success Planning

### Comprehensive Risk Assessment
```typescript
// Risk mitigation strategies
interface RiskMitigationPlan {
  technical_risks: Array<{
    risk_description: string;
    probability: 'low' | 'medium' | 'high';
    impact: 'low' | 'medium' | 'high';
    mitigation_strategies: string[];
    contingency_plans: string[];
  }>;
  
  integration_risks: Array<{
    integration_point: string;
    potential_issues: string[];
    prevention_measures: string[];
    fallback_approaches: string[];
  }>;
  
  timeline_risks: Array<{
    risk_factor: string;
    impact_on_delivery: string;
    mitigation_approach: string[];
    early_warning_signs: string[];
  }>;
  
  quality_risks: Array<{
    quality_concern: string;
    prevention_strategies: string[];
    detection_methods: string[];
    resolution_procedures: string[];
  }>;
}
```

### Implementation Plan File Generation
Use `create_file` to generate the comprehensive implementation plan:

```markdown
# Implementation Plan: {feature_name}

## 📋 User Story Analysis
**Original Story**: {user_story}

### Story Components
- **User Persona**: {identified_user_persona}
- **Desired Feature**: {extracted_feature_description}
- **Business Benefit**: {identified_business_value}

### Acceptance Criteria
#### Explicit Criteria
{extracted_explicit_criteria}

#### Implicit Criteria
{derived_implicit_criteria}

### Non-Functional Requirements
#### Performance Requirements
{performance_requirements_with_targets}

#### Security Requirements
{security_requirements_and_validations}

#### Usability Requirements
{usability_and_accessibility_requirements}

## 🔗 API Contract Design

### API Endpoints
{comprehensive_api_endpoint_definitions}

### Data Models
{detailed_data_model_specifications}

### Error Handling
{error_response_format_and_handling}

## 🏗️ Backend Implementation Plan

### Project Structure
{java_spring_boot_project_structure}

### Implementation Order
{detailed_backend_implementation_sequence}

### Testing Strategy
{backend_testing_approach_and_coverage}

## 🎨 Frontend Implementation Plan

### Project Structure
{react_typescript_project_structure}

### Implementation Order
{detailed_frontend_implementation_sequence}

### UI/UX Considerations
{responsive_design_and_accessibility_requirements}

## 🔗 Integration Strategy

### CORS Configuration
{cors_setup_and_configuration}

### Environment Setup
{environment_variables_and_configuration}

### Error Handling
{client_server_error_handling_integration}

## 🧪 Validation and Testing

### Backend Validation
{backend_testing_commands_and_procedures}

### Frontend Validation
{frontend_testing_commands_and_procedures}

### Integration Validation
{integration_testing_procedures_and_checklists}

## ⚠️ Risk Mitigation

### Technical Risks
{identified_technical_risks_and_mitigation}

### Integration Risks
{integration_challenges_and_solutions}

### Timeline Risks
{schedule_risks_and_contingencies}

## 📊 Success Metrics

### Technical Success Criteria
{measurable_technical_success_indicators}

### Business Success Criteria
{business_value_achievement_metrics}

### User Experience Success Criteria
{user_satisfaction_and_usability_metrics}

## 🚀 Implementation Timeline

### Phase 1: Foundation (Estimated: X days)
{setup_and_infrastructure_tasks}

### Phase 2: Backend Development (Estimated: X days)
{backend_implementation_tasks}

### Phase 3: Frontend Development (Estimated: X days)
{frontend_implementation_tasks}

### Phase 4: Integration & Testing (Estimated: X days)
{integration_and_validation_tasks}

### Phase 5: Deployment & Monitoring (Estimated: X days)
{deployment_and_monitoring_setup}

---
*This implementation plan provides comprehensive guidance for full-stack development of the user story with systematic validation and risk mitigation.*
```

## Success Criteria

The user story rapid analysis and planning is complete when:
- [ ] User story is comprehensively parsed and analyzed
- [ ] API contract is designed with complete documentation
- [ ] Backend implementation plan covers all layers
- [ ] Frontend implementation plan includes all components
- [ ] Integration strategy addresses all connection points
- [ ] Validation commands ensure quality at all levels
- [ ] Risk mitigation strategies are comprehensive
- [ ] Success metrics are measurable and clear
- [ ] Implementation timeline is realistic and detailed

## Integration Points

### Development Workflow Integration
- Connect with existing development and review processes
- Integrate with project management and tracking systems
- Align with team communication and collaboration practices
- Ensure compatibility with CI/CD pipelines

### Quality Assurance Integration
- Leverage existing testing frameworks and procedures
- Integrate with code quality tools and validation systems
- Connect to monitoring and observability infrastructure
- Align with organizational quality standards

### Knowledge Management Integration
- Contribute planning approaches to organizational knowledge base
- Share API design patterns with development teams
- Document implementation strategies for reuse
- Update planning procedures based on lessons learned

Remember: Focus on contract-first API design, systematic implementation planning, and comprehensive validation strategies for successful full-stack development.
```

## Notes
- Emphasize contract-first API design approach
- Focus on comprehensive full-stack planning
- Include detailed validation and testing strategies
- Provide systematic implementation sequences for both backend and frontend
