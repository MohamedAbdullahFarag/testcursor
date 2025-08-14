---
mode: agent
description: "Comprehensive security analysis with vulnerability assessment and mitigation strategies"
---

---
inputs:
  - name: security_scope
    description: Security analysis scope (authentication, authorization, input-validation, data-protection, all)
    required: false
    default: all
  - name: severity_level
    description: Minimum severity level to report (low, medium, high, critical)
    required: false
    default: medium
  - name: fix_issues
    description: Automatically fix identified issues (true, false)
    required: false
    default: false
  - name: compliance_standard
    description: Compliance standard to validate against (owasp, gdpr, hipaa, general)
    required: false
    default: owasp
---

---
command: "/security-analysis"
---
# Security Analysis Command for GitHub Copilot

## Command Usage
```
@copilot /security-analysis [security_scope] [severity_level] [fix_issues] [compliance_standard]
```

## Purpose
This command performs comprehensive security analysis of the entire codebase using GitHub Copilot's native tools to identify vulnerabilities, security misconfigurations, and compliance issues across both backend and frontend implementations with actionable remediation guidance.

**Input Parameters**:
- `security_scope` - Analysis scope: `authentication`, `authorization`, `input-validation`, `data-protection`, `all`
- `severity_level` - Report threshold: `low`, `medium`, `high`, `critical`
- `fix_issues` - Auto-fix capability: `true`, `false`
- `compliance_standard` - Standard validation: `owasp`, `gdpr`, `hipaa`, `general`

## How /security-analysis Works

### Phase 1: Security Discovery and Threat Modeling
```markdown
I'll perform comprehensive security analysis of your application. Let me start with discovering security-related components and potential attack vectors.

**Phase 1.1: Security Component Discovery**
```
I'll map all security-relevant components and configurations:
- semantic_search: "authentication authorization security" # Security component discovery
- file_search: "**/*Controller.cs" # API endpoint discovery
- file_search: "**/middleware/**" # Security middleware discovery
- grep_search: "\\[Authorize\\]|\\[AllowAnonymous\\]" # Authorization attribute discovery
- semantic_search: "password hashing encryption" # Cryptographic implementation discovery
- file_search: "**/appsettings*.json" # Configuration security analysis
- grep_search: "jwt|token|secret|key|password" # Security token discovery
```

**Phase 1.2: Attack Surface Analysis**
```
I'll analyze potential attack vectors and entry points:
- file_search: "**/*Dto.cs" # Input model discovery
- semantic_search: "user input validation" # Input validation analysis
- grep_search: "FromBody|FromQuery|FromForm" # Input source discovery
- semantic_search: "database queries sql" # SQL injection analysis
- file_search: "**/*.tsx" # Frontend security analysis
- grep_search: "dangerouslySetInnerHTML|eval\\(|innerHTML" # XSS vulnerability discovery
- semantic_search: "cors configuration security headers" # Security header analysis
```

**Phase 1.3: Data Flow Security Analysis**
```
I'll trace data flows and identify security boundaries:
- semantic_search: "sensitive data handling" # Sensitive data discovery
- grep_search: "password|ssn|credit.*card|api.*key" # PII and sensitive data patterns
- semantic_search: "data encryption at rest transit" # Encryption analysis
- file_search: "**/connection*.json" # Database connection security
- semantic_search: "logging sensitive information" # Information disclosure analysis
- grep_search: "Log.*\\(.*password|Log.*\\(.*token" # Logging security analysis
```
```

### Phase 2: Authentication and Authorization Security Analysis

#### Authentication Security Assessment
```markdown
**Phase 2.1: Authentication Implementation Analysis using GitHub Copilot Tools**
```
I'll analyze authentication mechanisms and identify security weaknesses:

## 🔐 Authentication Security Analysis (Tool-Enhanced)

### JWT Implementation Security Review
```powershell
# JWT security analysis using GitHub Copilot tools
semantic_search: "JWT token implementation security" # JWT pattern discovery
grep_search: "JwtSecurityToken|ValidateJwtSecurityToken" # JWT usage analysis
read_file: [JWT_CONFIG_FILES] # JWT configuration analysis
semantic_search: "token expiration refresh rotation" # Token lifecycle analysis
grep_search: "Bearer.*token|Authorization.*header" # Token transmission analysis
```

### Authentication Vulnerability Assessment
```typescript
// Authentication security analysis using GitHub Copilot tools
interface AuthenticationSecurityAnalysis {
  jwtSecurity: JWTSecurityIssue[];
  passwordSecurity: PasswordSecurityIssue[];
  sessionManagement: SessionSecurityIssue[];
  multiFactor: MFAImplementation[];
}

// Tool-discovered authentication vulnerabilities
const authSecurityAnalysis = {
  discoveryCommands: [
    'semantic_search: "password hashing bcrypt argon2"', // Password security analysis
    'grep_search: "password.*=.*|pwd.*=|secret.*="', // Hardcoded password discovery
    'semantic_search: "session management security"', // Session security analysis
    'grep_search: "cookie.*secure|cookie.*httponly"', // Cookie security analysis
    'semantic_search: "brute force protection rate limiting"' // Attack protection analysis
  ],
  vulnerabilities: {
    weakPasswordHashing: {
      severity: 'high',
      description: 'Analysis via password hashing algorithm discovery',
      location: 'Services/AuthenticationService.cs',
      recommendation: 'Implement Argon2 or bcrypt with proper salt',
      cweId: 'CWE-916'
    },
    missingRateLimit: {
      severity: 'high',
      description: 'No rate limiting on authentication endpoints',
      location: 'Controllers/AuthController.cs',
      recommendation: 'Implement rate limiting middleware',
      cweId: 'CWE-307'
    },
    tokenExposure: {
      severity: 'medium',
      description: 'JWT tokens logged in application logs',
      location: 'Middleware/LoggingMiddleware.cs',
      recommendation: 'Sanitize tokens from logs',
      cweId: 'CWE-532'
    }
  }
};
```

### Password Security Analysis
```csharp
// Password security assessment using GitHub Copilot tools
const passwordSecurityAnalysis = {
  discoveryCommands: [
    'semantic_search: "password validation complexity"', // Password policy analysis
    'grep_search: "PasswordHash|HashPassword|VerifyPassword"', // Password handling discovery
    'semantic_search: "password reset recovery"', // Password recovery analysis
    'grep_search: "password.*requirements|password.*policy"' // Password policy discovery
  ],
  securityIssues: {
    weakPasswordPolicy: {
      severity: 'medium',
      description: 'Insufficient password complexity requirements',
      currentPolicy: 'Minimum 6 characters, no complexity requirements',
      recommendation: 'Implement NIST 800-63B compliant password policy',
      impact: 'Susceptible to dictionary and brute force attacks'
    },
    passwordStorage: {
      severity: 'critical',
      description: 'Analysis via password hashing implementation',
      currentImplementation: 'Tool analysis of hashing algorithms',
      recommendation: 'Use Argon2id with appropriate work factors',
      impact: 'Password compromise in case of data breach'
    },
    passwordRecovery: {
      severity: 'high',
      description: 'Insecure password reset mechanism',
      vulnerability: 'Predictable reset tokens or insufficient expiration',
      recommendation: 'Implement cryptographically secure reset tokens',
      impact: 'Account takeover via password reset abuse'
    }
  }
};
```

### Multi-Factor Authentication Assessment
```typescript
// MFA implementation analysis using GitHub Copilot tools
const mfaAnalysis = {
  discoveryCommands: [
    'semantic_search: "multi factor authentication MFA 2FA"', // MFA implementation discovery
    'grep_search: "totp|authenticator|sms.*code"', // MFA method discovery
    'semantic_search: "backup codes recovery codes"', // Recovery mechanism analysis
    'semantic_search: "device trust remember device"' // Device management analysis
  ],
  implementation: {
    status: 'Missing - Critical security gap',
    recommendation: 'Implement TOTP-based MFA with backup codes',
    priority: 'high',
    effort: '1-2 weeks',
    standards: 'NIST 800-63B Level 2 compliance'
  }
};
```
```

#### Authorization Security Assessment
```markdown
**Phase 2.2: Authorization Implementation Analysis using GitHub Copilot Tools**
```
I'll analyze authorization mechanisms and access control vulnerabilities:

## 🛡️ Authorization Security Analysis (Tool-Enhanced)

### Role-Based Access Control (RBAC) Analysis
```powershell
# RBAC security analysis using GitHub Copilot tools
semantic_search: "role based access control RBAC" # RBAC implementation discovery
grep_search: "\\[Authorize\\(.*Roles|Policy.*=" # Authorization attribute analysis
semantic_search: "permission management authorization policies" # Policy analysis
file_search: "**/Authorization/**" # Authorization code discovery
list_code_usages: "AuthorizeAttribute" # Authorization usage analysis
```

### Authorization Vulnerability Assessment
```csharp
// Authorization security analysis using GitHub Copilot tools
interface AuthorizationSecurityAnalysis {
  rbacImplementation: RBACSecurityIssue[];
  accessControlVulnerabilities: AccessControlIssue[];
  privilegeEscalation: PrivilegeEscalationRisk[];
  dataAccessSecurity: DataAccessSecurityIssue[];
}

const authorizationSecurityAnalysis = {
  discoveryCommands: [
    'semantic_search: "authorization bypass privilege escalation"', // Authorization bypass analysis
    'grep_search: "User\\.IsInRole|ClaimsPrincipal"', // Claims-based auth analysis
    'semantic_search: "horizontal vertical privilege escalation"', // Privilege escalation analysis
    'grep_search: "where.*userId|filter.*user"' // Data isolation analysis
  ],
  vulnerabilities: {
    missingAuthorization: {
      severity: 'critical',
      description: 'Endpoints without authorization attributes',
      affectedEndpoints: [
        'GET /api/users/{id}/sensitive-data',
        'PUT /api/admin/settings',
        'DELETE /api/questions/{id}'
      ],
      recommendation: 'Add [Authorize] attributes to all sensitive endpoints',
      cweId: 'CWE-862'
    },
    insecureDirectObjectReference: {
      severity: 'high',
      description: 'Missing ownership validation in data access',
      vulnerability: 'Users can access other users\' data by changing IDs',
      location: 'Controllers/UsersController.cs:GetUserData',
      recommendation: 'Implement user context validation in data queries',
      cweId: 'CWE-639'
    },
    privilegeEscalation: {
      severity: 'high',
      description: 'Insufficient role validation',
      vulnerability: 'Users can modify their own roles',
      location: 'Controllers/UsersController.cs:UpdateUser',
      recommendation: 'Separate role management from user profile updates',
      cweId: 'CWE-269'
    }
  }
};
```

### Data Access Security Analysis
```csharp
// Data access authorization analysis using GitHub Copilot tools
const dataAccessSecurityAnalysis = {
  discoveryCommands: [
    'semantic_search: "data isolation multi tenant"', // Multi-tenancy analysis
    'grep_search: "where.*tenantId|filter.*tenant"', // Tenant isolation analysis
    'semantic_search: "row level security database"', // Database security analysis
    'grep_search: "SELECT.*FROM.*WHERE.*userId"' // User data isolation analysis
  ],
  dataIsolationIssues: {
    missingTenantIsolation: {
      severity: 'critical',
      description: 'Queries missing tenant isolation',
      affectedQueries: [
        'SELECT * FROM Questions WHERE CategoryId = @categoryId',
        'SELECT * FROM Exams WHERE Status = @status'
      ],
      recommendation: 'Add tenant context to all data queries',
      impact: 'Data leakage between organizations'
    },
    inadequateRowLevelSecurity: {
      severity: 'high',
      description: 'Missing user-level data access controls',
      vulnerability: 'Users can access data they don\'t own',
      recommendation: 'Implement consistent user context filtering',
      impact: 'Unauthorized data access'
    }
  }
};
```
```

### Phase 3: Input Validation and Injection Attack Analysis

#### SQL Injection Vulnerability Assessment
```markdown
**Phase 3.1: SQL Injection Analysis using GitHub Copilot Tools**
```
I'll analyze database queries and identify SQL injection vulnerabilities:

## 💉 SQL Injection Security Analysis (Tool-Enhanced)

### Database Query Security Assessment
```powershell
# SQL injection analysis using GitHub Copilot tools
semantic_search: "SQL injection prevention parameterized queries" # SQL security analysis
grep_search: "SELECT.*\\+|INSERT.*\\+|UPDATE.*\\+" # String concatenation discovery
semantic_search: "dapper sql query construction" # Dapper usage analysis
grep_search: "Query.*\\$|Execute.*\\$" # Dynamic query analysis
semantic_search: "stored procedures security" # Stored procedure analysis
```

### SQL Injection Vulnerability Assessment
```csharp
// SQL injection vulnerability analysis using GitHub Copilot tools
interface SQLInjectionAnalysis {
  vulnerableQueries: VulnerableQuery[];
  parameterizationIssues: ParameterizationIssue[];
  dynamicQueryRisks: DynamicQueryRisk[];
  mitigationStatus: MitigationStrategy[];
}

const sqlInjectionAnalysis = {
  discoveryCommands: [
    'grep_search: "Query\\(.*\\$|Execute\\(.*\\$"', // Dynamic query discovery
    'semantic_search: "string interpolation SQL queries"', // String interpolation analysis
    'grep_search: "WHERE.*\\+.*|SET.*\\+.*"', // Concatenated WHERE clauses
    'semantic_search: "parameterized queries dapper"' // Parameterization analysis
  ],
  vulnerabilities: {
    dynamicWhereClause: {
      severity: 'critical',
      description: 'Dynamic WHERE clause construction without parameterization',
      location: 'Repositories/QuestionRepository.cs:SearchQuestions',
      vulnerableCode: 'connection.Query($"SELECT * FROM Questions WHERE {searchCondition}")',
      recommendation: 'Use parameterized queries with dynamic conditions',
      cweId: 'CWE-89',
      exploitability: 'High - Direct user input in query construction'
    },
    orderByInjection: {
      severity: 'medium',
      description: 'ORDER BY clause using unsanitized user input',
      location: 'Repositories/BaseRepository.cs:GetPagedResults',
      vulnerableCode: 'ORDER BY ${sortColumn} ${sortDirection}',
      recommendation: 'Whitelist allowed sort columns and directions',
      cweId: 'CWE-89',
      exploitability: 'Medium - Limited to ORDER BY manipulation'
    }
  },
  mitigationStrategies: {
    parameterizedQueries: {
      implementation: 'Partial - 85% of queries use parameters',
      gaps: [
        'Dynamic search conditions in QuestionRepository',
        'Sorting parameters in pagination',
        'Dynamic column selection in reporting'
      ],
      recommendation: 'Achieve 100% parameterization coverage'
    },
    inputValidation: {
      implementation: 'Basic - Data annotations on DTOs',
      enhancement: 'Add custom validation for complex business rules',
      recommendation: 'Implement comprehensive input sanitization'
    }
  }
};
```

### Dapper Security Best Practices Analysis
```csharp
// Dapper-specific security analysis using GitHub Copilot tools
const dapperSecurityAnalysis = {
  discoveryCommands: [
    'semantic_search: "dapper security best practices"', // Dapper security patterns
    'grep_search: "DynamicParameters|new.*\\{.*\\}"', // Parameter usage analysis
    'semantic_search: "dapper sql injection prevention"', // Dapper SQL injection analysis
    'grep_search: "Query<.*>\\(|Execute\\("' // Dapper method usage analysis
  ],
  securityPatterns: {
    parameterUsage: {
      goodPractices: [
        'DynamicParameters usage for complex queries',
        'Anonymous objects for simple parameters',
        'Strongly-typed parameter objects'
      ],
      improvements: [
        'Implement parameter validation middleware',
        'Add query execution logging',
        'Implement query performance monitoring'
      ]
    },
    queryConstruction: {
      recommendations: [
        'Use const strings for base queries',
        'Implement query builder for complex conditions',
        'Add query result caching where appropriate'
      ]
    }
  }
};
```
```

#### Cross-Site Scripting (XSS) Analysis
```markdown
**Phase 3.2: XSS Vulnerability Analysis using GitHub Copilot Tools**
```
I'll analyze frontend code for XSS vulnerabilities:

## 🌐 XSS Vulnerability Analysis (Tool-Enhanced)

### Frontend XSS Security Assessment
```powershell
# XSS vulnerability analysis using GitHub Copilot tools
semantic_search: "XSS cross site scripting prevention" # XSS security analysis
grep_search: "dangerouslySetInnerHTML|innerHTML|eval\\(" # Dangerous HTML usage
semantic_search: "React security sanitization" # React security patterns
grep_search: "document\\.write|outerHTML" # DOM manipulation discovery
file_search: "**/*.tsx" # Component security analysis
```

### XSS Vulnerability Assessment
```typescript
// XSS vulnerability analysis using GitHub Copilot tools
interface XSSAnalysis {
  dangerousPatterns: DangerousPattern[];
  inputSanitization: SanitizationStatus[];
  outputEncoding: EncodingStatus[];
  cspImplementation: CSPAnalysis;
}

const xssAnalysis = {
  discoveryCommands: [
    'grep_search: "dangerouslySetInnerHTML.*__html"', // Dangerous HTML usage
    'semantic_search: "user input rendering sanitization"', // Input rendering analysis
    'grep_search: "\\{.*userInput.*\\}|\\{.*user\\."', // User data rendering
    'semantic_search: "content security policy CSP"' // CSP implementation analysis
  ],
  vulnerabilities: {
    dangerousHTML: {
      severity: 'high',
      description: 'Usage of dangerouslySetInnerHTML without sanitization',
      locations: [
        'components/RichTextDisplay.tsx:line 45',
        'components/QuestionRenderer.tsx:line 89'
      ],
      recommendation: 'Use DOMPurify for HTML sanitization',
      cweId: 'CWE-79',
      exploitability: 'High - Direct HTML injection possible'
    },
    unescapedUserInput: {
      severity: 'medium',
      description: 'User input rendered without proper escaping',
      locations: [
        'components/UserComment.tsx:line 23',
        'components/SearchResults.tsx:line 67'
      ],
      recommendation: 'Ensure React automatic escaping is not bypassed',
      cweId: 'CWE-79',
      exploitability: 'Medium - Requires specific injection techniques'
    },
    missingCSP: {
      severity: 'medium',
      description: 'Content Security Policy not implemented',
      impact: 'No defense against injected scripts',
      recommendation: 'Implement strict CSP headers',
      cweId: 'CWE-693'
    }
  }
};
```

### Content Security Policy Analysis
```typescript
// CSP implementation analysis using GitHub Copilot tools
const cspAnalysis = {
  discoveryCommands: [
    'semantic_search: "content security policy headers"', // CSP header analysis
    'grep_search: "Content-Security-Policy|CSP"', // CSP implementation discovery
    'file_search: "**/nginx.conf" "**/web.config"', // Web server config analysis
    'semantic_search: "script nonce inline scripts"' // Script security analysis
  ],
  currentImplementation: {
    status: 'Missing - Critical security gap',
    recommendation: 'Implement comprehensive CSP policy',
    priority: 'high',
    policy: {
      'default-src': "'self'",
      'script-src': "'self' 'nonce-{random}'",
      'style-src': "'self' 'unsafe-inline'",
      'img-src': "'self' data: https:",
      'connect-src': "'self'",
      'frame-ancestors': "'none'"
    }
  }
};
```
```

### Phase 4: Data Protection and Privacy Analysis

#### Data Encryption Assessment
```markdown
**Phase 4.1: Data Protection Analysis using GitHub Copilot Tools**
```
I'll analyze data encryption and privacy protection mechanisms:

## 🔒 Data Protection Security Analysis (Tool-Enhanced)

### Encryption Implementation Assessment
```powershell
# Data encryption analysis using GitHub Copilot tools
semantic_search: "data encryption at rest transit" # Encryption implementation discovery
grep_search: "encrypt|decrypt|AES|RSA" # Cryptographic usage analysis
semantic_search: "TLS SSL certificate configuration" # Transport security analysis
file_search: "**/appsettings*.json" # Configuration security analysis
semantic_search: "key management secrets" # Key management analysis
```

### Data Encryption Vulnerability Assessment
```csharp
// Data encryption analysis using GitHub Copilot tools
interface DataProtectionAnalysis {
  encryptionAtRest: EncryptionAtRestStatus;
  encryptionInTransit: EncryptionInTransitStatus;
  keyManagement: KeyManagementSecurity;
  piiProtection: PIIProtectionStatus;
}

const dataProtectionAnalysis = {
  discoveryCommands: [
    'semantic_search: "database encryption TDE"', // Database encryption analysis
    'grep_search: "connectionstring.*encrypt|ssl.*mode"', // Connection encryption
    'semantic_search: "sensitive data classification"', // Data classification analysis
    'grep_search: "PersonalData|SensitiveData"' // PII identification
  ],
  encryptionGaps: {
    databaseEncryption: {
      severity: 'high',
      description: 'Database not using Transparent Data Encryption (TDE)',
      impact: 'Sensitive data stored in plaintext',
      recommendation: 'Enable TDE on SQL Server database',
      compliance: 'Required for GDPR, HIPAA compliance'
    },
    configurationSecrets: {
      severity: 'critical',
      description: 'Sensitive configuration in plaintext',
      locations: [
        'appsettings.json:ConnectionStrings',
        'appsettings.json:JwtSettings:SecretKey'
      ],
      recommendation: 'Use Azure Key Vault or environment variables',
      impact: 'Credential exposure in source control'
    },
    piiEncryption: {
      severity: 'medium',
      description: 'PII fields not encrypted at application level',
      affectedFields: [
        'Users.Email',
        'Users.PhoneNumber',
        'StudentProfiles.PersonalData'
      ],
      recommendation: 'Implement field-level encryption for PII',
      compliance: 'GDPR Article 32 requirement'
    }
  }
};
```

### Key Management Security Analysis
```csharp
// Key management security analysis using GitHub Copilot tools
const keyManagementAnalysis = {
  discoveryCommands: [
    'semantic_search: "key management vault secrets"', // Key management discovery
    'grep_search: "SecretKey|PrivateKey|ApiKey"', // Key usage analysis
    'semantic_search: "key rotation lifecycle"', // Key rotation analysis
    'file_search: "**/secrets.json" "**/*.pfx"' // Secret file discovery
  ],
  securityIssues: {
    hardcodedKeys: {
      severity: 'critical',
      description: 'Cryptographic keys hardcoded in source code',
      locations: [
        'appsettings.json:JwtSettings:SecretKey',
        'Services/EncryptionService.cs:AES_KEY'
      ],
      recommendation: 'Move all keys to secure key management system',
      impact: 'Complete compromise of cryptographic security'
    },
    keyRotation: {
      severity: 'medium',
      description: 'No key rotation mechanism implemented',
      recommendation: 'Implement automated key rotation',
      impact: 'Extended exposure window for compromised keys'
    },
    keyAccess: {
      severity: 'high',
      description: 'Overly broad key access permissions',
      recommendation: 'Implement principle of least privilege for key access',
      impact: 'Unnecessary exposure of cryptographic keys'
    }
  }
};
```
```

#### Privacy and Compliance Analysis
```markdown
**Phase 4.2: Privacy and Compliance Analysis using GitHub Copilot Tools**
```
I'll analyze privacy controls and compliance with data protection regulations:

## 🛡️ Privacy and Compliance Analysis (Tool-Enhanced)

### GDPR Compliance Assessment
```powershell
# GDPR compliance analysis using GitHub Copilot tools
semantic_search: "GDPR data protection privacy" # GDPR implementation discovery
grep_search: "consent|right.*erasure|data.*portability" # GDPR rights analysis
semantic_search: "data retention deletion" # Data retention analysis
semantic_search: "privacy by design default" # Privacy principles analysis
file_search: "**/privacy-policy*" "**/terms*" # Legal document discovery
```

### Privacy Rights Implementation Analysis
```typescript
// Privacy rights analysis using GitHub Copilot tools
interface PrivacyAnalysis {
  gdprCompliance: GDPRComplianceStatus;
  dataRetention: DataRetentionPolicy;
  consentManagement: ConsentManagementStatus;
  dataPortability: DataPortabilityStatus;
}

const privacyAnalysis = {
  discoveryCommands: [
    'semantic_search: "user consent management"', // Consent mechanism analysis
    'grep_search: "delete.*user|anonymize|pseudonymize"', // Data deletion analysis
    'semantic_search: "data export user rights"', // Data portability analysis
    'semantic_search: "audit trail privacy"' // Privacy audit analysis
  ],
  gdprGaps: {
    rightToErasure: {
      severity: 'high',
      description: 'No implementation of right to be forgotten',
      requirement: 'GDPR Article 17 - Right to erasure',
      recommendation: 'Implement cascading user data deletion',
      impact: 'Non-compliance with GDPR erasure requests'
    },
    consentManagement: {
      severity: 'medium',
      description: 'Basic consent mechanism without granular controls',
      requirement: 'GDPR Article 7 - Conditions for consent',
      recommendation: 'Implement granular consent preferences',
      impact: 'Insufficient consent documentation'
    },
    dataPortability: {
      severity: 'medium',
      description: 'No data export functionality for users',
      requirement: 'GDPR Article 20 - Right to data portability',
      recommendation: 'Implement user data export in machine-readable format',
      impact: 'Cannot fulfill data portability requests'
    },
    privacyByDesign: {
      severity: 'low',
      description: 'Privacy considerations not systematically integrated',
      requirement: 'GDPR Article 25 - Data protection by design',
      recommendation: 'Implement privacy impact assessments',
      impact: 'Potential privacy violations in new features'
    }
  }
};
```

### Data Classification and Handling
```typescript
// Data classification analysis using GitHub Copilot tools
const dataClassificationAnalysis = {
  discoveryCommands: [
    'semantic_search: "sensitive data personal information"', // Sensitive data discovery
    'grep_search: "email|phone|address|ssn|credit"', // PII pattern analysis
    'semantic_search: "data anonymization pseudonymization"', // Anonymization analysis
    'grep_search: "public|internal|confidential|restricted"' // Data classification discovery
  ],
  dataCategories: {
    personalData: {
      classification: 'Highly Sensitive',
      fields: [
        'Users.Email',
        'Users.FirstName',
        'Users.LastName',
        'Users.PhoneNumber'
      ],
      protection: 'Encryption at rest and transit required',
      retention: '7 years after account deletion',
      compliance: 'GDPR, CCPA applicable'
    },
    academicData: {
      classification: 'Sensitive',
      fields: [
        'ExamResults.Score',
        'UserAnswers.SelectedOption',
        'StudentProfiles.AcademicRecord'
      ],
      protection: 'Access control and audit logging required',
      retention: 'Per institutional policy',
      compliance: 'FERPA applicable (if educational institution)'
    },
    systemData: {
      classification: 'Internal',
      fields: [
        'AuditLogs.Action',
        'SystemMetrics.Performance',
        'Configuration.Settings'
      ],
      protection: 'Standard access controls',
      retention: '3 years for audit purposes',
      compliance: 'SOX applicable (if public company)'
    }
  }
};
```
```

### Phase 5: Security Remediation and Hardening

#### Automated Security Fix Generation
```markdown
**Phase 5.1: Security Remediation using GitHub Copilot Tools**
```
I'll generate specific fixes for identified security vulnerabilities:

## 🔧 Security Remediation (Tool-Enhanced)

### Critical Vulnerability Fixes
```powershell
# Security fix generation using GitHub Copilot tools
semantic_search: "security fix patterns best practices" # Fix pattern discovery
read_file: [VULNERABLE_FILES] # Analyze vulnerable code
semantic_search: "secure coding guidelines" # Secure coding pattern analysis
create_file: [SECURITY_PATCHES] # Generate security fixes
```

### SQL Injection Fix Implementation
```csharp
// Generated SQL injection fix using GitHub Copilot analysis
// File: Repositories/QuestionRepository.cs

// ❌ VULNERABLE CODE (Before fix)
/*
public async Task<IEnumerable<QuestionEntity>> SearchQuestionsAsync(string searchTerm, string category)
{
    var sql = $"SELECT * FROM Questions WHERE Title LIKE '%{searchTerm}%' AND Category = '{category}'";
    return await _connection.QueryAsync<QuestionEntity>(sql);
}
*/

// ✅ SECURE CODE (After fix)
public async Task<IEnumerable<QuestionEntity>> SearchQuestionsAsync(string searchTerm, string category)
{
    const string sql = @"
        SELECT * FROM Questions 
        WHERE Title LIKE @SearchPattern 
        AND Category = @Category";
    
    var parameters = new DynamicParameters();
    parameters.Add("@SearchPattern", $"%{searchTerm}%");
    parameters.Add("@Category", category);
    
    return await _connection.QueryAsync<QuestionEntity>(sql, parameters);
}

// 🛡️ ENHANCED SECURE VERSION (Best practice)
public async Task<IEnumerable<QuestionEntity>> SearchQuestionsAsync(SearchCriteria criteria)
{
    // Input validation
    if (string.IsNullOrWhiteSpace(criteria.SearchTerm))
        throw new ArgumentException("Search term cannot be empty", nameof(criteria.SearchTerm));
    
    // Whitelist validation for category
    var allowedCategories = new[] { "Math", "Science", "Literature", "History" };
    if (!allowedCategories.Contains(criteria.Category))
        throw new ArgumentException("Invalid category", nameof(criteria.Category));
    
    const string sql = @"
        SELECT q.Id, q.Title, q.Content, q.Category, q.Difficulty, q.CreatedAt
        FROM Questions q
        WHERE (@SearchTerm IS NULL OR q.Title LIKE @SearchPattern)
        AND (@Category IS NULL OR q.Category = @Category)
        AND q.IsActive = 1
        ORDER BY q.CreatedAt DESC
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
    
    var parameters = new DynamicParameters();
    parameters.Add("@SearchTerm", criteria.SearchTerm);
    parameters.Add("@SearchPattern", $"%{criteria.SearchTerm}%");
    parameters.Add("@Category", criteria.Category);
    parameters.Add("@Offset", criteria.PageNumber * criteria.PageSize);
    parameters.Add("@PageSize", criteria.PageSize);
    
    return await _connection.QueryAsync<QuestionEntity>(sql, parameters);
}
```

### XSS Prevention Fix Implementation
```typescript
// Generated XSS prevention fix using GitHub Copilot analysis
// File: components/RichTextDisplay.tsx

import DOMPurify from 'dompurify';

// ❌ VULNERABLE CODE (Before fix)
/*
const RichTextDisplay: React.FC<{ content: string }> = ({ content }) => {
  return (
    <div dangerouslySetInnerHTML={{ __html: content }} />
  );
};
*/

// ✅ SECURE CODE (After fix)
const RichTextDisplay: React.FC<{ content: string }> = ({ content }) => {
  const sanitizedContent = useMemo(() => {
    return DOMPurify.sanitize(content, {
      ALLOWED_TAGS: ['b', 'i', 'u', 'strong', 'em', 'p', 'br', 'ul', 'ol', 'li'],
      ALLOWED_ATTR: ['class'],
      FORBID_SCRIPTS: true,
      FORBID_TAGS: ['script', 'iframe', 'object', 'embed'],
    });
  }, [content]);

  return (
    <div 
      dangerouslySetInnerHTML={{ __html: sanitizedContent }}
      className="rich-text-content"
    />
  );
};

// 🛡️ ENHANCED SECURE VERSION (Best practice with CSP)
const RichTextDisplay: React.FC<{ content: string; allowedElements?: string[] }> = ({ 
  content, 
  allowedElements = ['b', 'i', 'u', 'strong', 'em', 'p', 'br'] 
}) => {
  const sanitizedContent = useMemo(() => {
    if (!content) return '';
    
    const config = {
      ALLOWED_TAGS: allowedElements,
      ALLOWED_ATTR: ['class'],
      KEEP_CONTENT: true,
      FORBID_SCRIPTS: true,
      FORBID_TAGS: ['script', 'iframe', 'object', 'embed', 'form', 'input'],
      FORBID_ATTR: ['onerror', 'onload', 'onclick', 'onmouseover'],
    };
    
    return DOMPurify.sanitize(content, config);
  }, [content, allowedElements]);

  // Additional protection: CSP nonce for any remaining inline styles
  const nonce = useCSPNonce();

  return (
    <div 
      dangerouslySetInnerHTML={{ __html: sanitizedContent }}
      className="rich-text-content"
      data-testid="rich-text-display"
      {...(nonce && { nonce })}
    />
  );
};

export default memo(RichTextDisplay);
```

### Authorization Fix Implementation
```csharp
// Generated authorization fix using GitHub Copilot analysis
// File: Controllers/UsersController.cs

// ❌ VULNERABLE CODE (Before fix)
/*
[HttpGet("{id}")]
public async Task<ActionResult<UserDto>> GetUser(Guid id)
{
    var user = await _userService.GetUserByIdAsync(id);
    return user == null ? NotFound() : Ok(user);
}
*/

// ✅ SECURE CODE (After fix)
[HttpGet("{id}")]
[Authorize]
public async Task<ActionResult<UserDto>> GetUser(Guid id)
{
    // Get current user context
    var currentUserId = User.GetUserId();
    var currentUserRole = User.GetRole();
    
    // Authorization check: Users can only access their own data, admins can access any
    if (currentUserId != id && !User.IsInRole("Admin"))
    {
        _logger.LogWarning("Unauthorized access attempt. User {CurrentUserId} tried to access {TargetUserId}", 
            currentUserId, id);
        return Forbid("You can only access your own user data");
    }
    
    var user = await _userService.GetUserByIdAsync(id);
    if (user == null)
    {
        return NotFound();
    }
    
    // Data filtering based on role
    if (!User.IsInRole("Admin"))
    {
        // Remove sensitive fields for non-admin users
        user.Email = MaskEmail(user.Email);
        user.PhoneNumber = null;
        user.LastLoginAt = null;
    }
    
    return Ok(user);
}

// 🛡️ ENHANCED SECURE VERSION (Best practice with policy-based authorization)
[HttpGet("{id}")]
[Authorize(Policy = "CanViewUserData")]
public async Task<ActionResult<UserDto>> GetUser(Guid id)
{
    // Policy-based authorization handles complex authorization logic
    var authResult = await _authorizationService.AuthorizeAsync(User, id, "CanViewUserData");
    if (!authResult.Succeeded)
    {
        return Forbid();
    }
    
    try
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        
        // Apply data masking based on current user's permissions
        var maskedUser = await _dataProtectionService.ApplyDataMaskingAsync(user, User);
        
        return Ok(maskedUser);
    }
    catch (UnauthorizedAccessException)
    {
        return Forbid();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error retrieving user {UserId}", id);
        return StatusCode(500, "An error occurred while retrieving user data");
    }
}
```
```

#### Security Hardening Implementation
```markdown
**Phase 5.2: Security Hardening using GitHub Copilot Tools**
```
I'll implement comprehensive security hardening measures:

## 🔒 Security Hardening Implementation (Tool-Enhanced)

### Security Headers Configuration
```csharp
// Generated security headers middleware using GitHub Copilot analysis
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SecurityHeadersMiddleware> _logger;

    public SecurityHeadersMiddleware(RequestDelegate next, ILogger<SecurityHeadersMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Content Security Policy
        context.Response.Headers.Add("Content-Security-Policy", 
            "default-src 'self'; " +
            "script-src 'self' 'nonce-{nonce}'; " +
            "style-src 'self' 'unsafe-inline'; " +
            "img-src 'self' data: https:; " +
            "font-src 'self'; " +
            "connect-src 'self'; " +
            "frame-ancestors 'none'; " +
            "base-uri 'self'; " +
            "form-action 'self'");

        // Security headers
        context.Response.Headers.Add("X-Frame-Options", "DENY");
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
        context.Response.Headers.Add("Permissions-Policy", 
            "geolocation=(), microphone=(), camera=(), payment=()");

        // HSTS for HTTPS
        if (context.Request.IsHttps)
        {
            context.Response.Headers.Add("Strict-Transport-Security", 
                "max-age=31536000; includeSubDomains; preload");
        }

        // Remove server information disclosure
        context.Response.Headers.Remove("Server");
        context.Response.Headers.Remove("X-Powered-By");

        await _next(context);
    }
}
```

### Rate Limiting Implementation
```csharp
// Generated rate limiting configuration using GitHub Copilot analysis
public class RateLimitingConfiguration
{
    public static void ConfigureRateLimit(IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            // Global rate limit
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
                httpContext => RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.User.Identity?.Name ?? httpContext.Connection.RemoteIpAddress?.ToString(),
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1)
                    }));

            // Authentication endpoint rate limit
            options.AddPolicy("AuthPolicy", httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 5,
                        Window = TimeSpan.FromMinutes(15)
                    }));

            // API rate limit
            options.AddPolicy("ApiPolicy", httpContext =>
                RateLimitPartition.GetTokenBucketLimiter(
                    partitionKey: httpContext.User.Identity?.Name ?? "anonymous",
                    factory: partition => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = 100,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 10,
                        ReplenishmentPeriod = TimeSpan.FromSeconds(1),
                        TokensPerPeriod = 2,
                        AutoReplenishment = true
                    }));

            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = 429;
                
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    await context.HttpContext.Response.WriteAsync(
                        $"Too many requests. Please try again after {retryAfter.TotalMinutes} minute(s).",
                        cancellationToken: token);
                }
                else
                {
                    await context.HttpContext.Response.WriteAsync(
                        "Too many requests. Please try again later.", 
                        cancellationToken: token);
                }
            };
        });
    }
}

// Application usage
[EnableRateLimiting("AuthPolicy")]
[HttpPost("login")]
public async Task<ActionResult<AuthResult>> Login(LoginDto loginDto)
{
    // Authentication logic
}

[EnableRateLimiting("ApiPolicy")]
[HttpGet]
public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
{
    // API logic
}
```

### Security Monitoring and Alerting
```csharp
// Generated security monitoring using GitHub Copilot tools
public class SecurityMonitoringService : ISecurityMonitoringService
{
    private readonly ILogger<SecurityMonitoringService> _logger;
    private readonly IMetrics _metrics;
    private readonly IAlertingService _alertingService;

    public SecurityMonitoringService(
        ILogger<SecurityMonitoringService> logger,
        IMetrics metrics,
        IAlertingService alertingService)
    {
        _logger = logger;
        _metrics = metrics;
        _alertingService = alertingService;
    }

    public async Task LogSecurityEventAsync(SecurityEvent securityEvent)
    {
        // Structured logging for security events
        _logger.LogWarning("Security Event: {EventType} | User: {UserId} | IP: {IpAddress} | Details: {Details}",
            securityEvent.EventType,
            securityEvent.UserId,
            securityEvent.IpAddress,
            securityEvent.Details);

        // Metrics tracking
        _metrics.Increment($"security.events.{securityEvent.EventType.ToString().ToLower()}");

        // Alert on critical security events
        if (securityEvent.Severity >= SecurityEventSeverity.High)
        {
            await _alertingService.SendSecurityAlertAsync(securityEvent);
        }

        // Check for patterns indicating attacks
        await DetectSecurityPatternsAsync(securityEvent);
    }

    private async Task DetectSecurityPatternsAsync(SecurityEvent securityEvent)
    {
        switch (securityEvent.EventType)
        {
            case SecurityEventType.FailedLogin:
                await DetectBruteForceAttackAsync(securityEvent);
                break;
            case SecurityEventType.UnauthorizedAccess:
                await DetectAccessPatternAnomaliesAsync(securityEvent);
                break;
            case SecurityEventType.SuspiciousActivity:
                await DetectAnomalousUserBehaviorAsync(securityEvent);
                break;
        }
    }

    private async Task DetectBruteForceAttackAsync(SecurityEvent securityEvent)
    {
        var failedAttempts = await CountFailedAttemptsAsync(
            securityEvent.IpAddress, 
            TimeSpan.FromMinutes(15));

        if (failedAttempts >= 5)
        {
            await _alertingService.SendBruteForceAlertAsync(securityEvent.IpAddress, failedAttempts);
            
            // Implement IP blocking
            await BlockIpAddressAsync(securityEvent.IpAddress, TimeSpan.FromHours(1));
        }
    }
}
```
```

## Command Activation Process
When a user types:
```
@copilot /security-analysis [security_scope] [severity_level] [fix_issues] [compliance_standard]
```

The system should:
1. **Discovery Phase**: Use GitHub Copilot tools to map security components and attack surfaces
2. **Analysis Phase**: Analyze vulnerabilities across authentication, authorization, input validation, and data protection
3. **Assessment Phase**: Evaluate security posture against industry standards and compliance requirements
4. **Remediation Phase**: Generate specific security fixes and hardening measures using discovered patterns
5. **Monitoring Setup**: Implement security monitoring and alerting mechanisms
6. **Validation Framework**: Establish ongoing security validation and compliance checking

## Security Scope Coverage

### Authentication Security
- JWT token security and lifecycle management
- Password hashing and complexity requirements
- Multi-factor authentication implementation
- Session management and timeout controls

### Authorization Security
- Role-based access control (RBAC) implementation
- Policy-based authorization mechanisms
- Privilege escalation prevention
- Data access authorization and isolation

### Input Validation Security
- SQL injection prevention and parameterization
- Cross-site scripting (XSS) protection
- Cross-site request forgery (CSRF) prevention
- Input sanitization and validation

### Data Protection Security
- Encryption at rest and in transit
- Key management and rotation
- PII protection and privacy controls
- Data classification and handling

### Infrastructure Security
- Security headers configuration
- Rate limiting and DDoS protection
- Security monitoring and alerting
- Compliance validation (OWASP, GDPR, HIPAA)

## Notes
- All security analysis uses GitHub Copilot's native tools for comprehensive coverage
- Vulnerability assessment follows OWASP Top 10 and industry best practices
- Generated fixes include both immediate patches and long-term security improvements
- Compliance analysis adapts to specified standards (OWASP, GDPR, HIPAA)
- Security recommendations prioritize critical vulnerabilities and business impact
- All security implementations follow principle of defense in depth
