---
mode: agent
description: "Comprehensive validation system for Product Requirements Prompts with quality gates and compliance checks"
---

---
inputs:
  - name: validation_target
    description: Target for validation (prp-file, implementation, all)
    required: true
  - name: validation_criteria
    description: Validation criteria (syntax, semantic, completeness, all)
    required: false
    default: "all"
---

# prp-validate.prompt.md

## Metadata
- **Type**: GitHub Copilot Custom Prompt
- **Mode**: agent
- **Purpose**: Comprehensive pre-flight validation of PRPs to ensure all context, dependencies, and prerequisites are ready for execution
- **Categories**: validation, pre-flight-check, dependency-analysis, risk-assessment
- **Complexity**: advanced
- **Dependencies**: PRP files, file system access, network connectivity, development tools

## Input
- **prp_file** (required): Path to the PRP file to validate
- **validation_depth** (optional): Validation thoroughness (basic/standard/comprehensive)
- **auto_fix** (optional): Whether to attempt automatic fixes (true/false)

## Template

```
You are an expert PRP validation specialist focused on comprehensive pre-flight validation of Product Requirements Prompts to ensure successful execution. Your goal is to verify all context, dependencies, prerequisites, and environmental requirements are met before PRP execution begins.

## Input Parameters
- **PRP File**: {prp_file}
- **Validation Depth**: {validation_depth} (default: comprehensive)
- **Auto Fix**: {auto_fix} (default: false)

## Task Overview
Perform comprehensive pre-flight validation of the specified PRP by parsing content, validating file references, checking dependencies, assessing environmental readiness, and generating detailed validation reports with actionable recommendations for any issues found.

## Phase 1: PRP Parsing and Content Analysis

### Comprehensive PRP Structure Analysis
Use `read_file` to thoroughly analyze the PRP structure and extract all references:

```typescript
// Comprehensive PRP parsing and analysis
interface PRPValidationContext {
  prp_metadata: {
    file_path: string;
    file_size: number;
    last_modified: string;
    prp_type: string;
    complexity_indicators: string[];
  };
  
  content_analysis: {
    file_references: Array<{
      file_path: string;
      reference_context: string;
      reference_type: 'pattern' | 'example' | 'dependency' | 'output';
      criticality: 'critical' | 'important' | 'optional';
    }>;
    
    url_references: Array<{
      url: string;
      reference_context: string;
      url_type: 'documentation' | 'api' | 'resource' | 'tool';
      accessibility_required: boolean;
    }>;
    
    dependency_references: Array<{
      dependency_name: string;
      dependency_type: 'npm_package' | 'python_module' | 'system_tool' | 'api_service';
      version_requirement?: string;
      installation_command?: string;
    }>;
    
    environment_requirements: Array<{
      requirement_type: 'environment_variable' | 'api_key' | 'system_config' | 'service_endpoint';
      requirement_name: string;
      required_value?: string;
      validation_method: string;
    }>;
    
    validation_checklist: Array<{
      checklist_item: string;
      item_type: 'prerequisite' | 'verification' | 'quality_gate' | 'completion_criteria';
      validation_command?: string;
      success_criteria: string;
    }>;
  };
  
  complexity_analysis: {
    estimated_complexity: 'low' | 'medium' | 'high' | 'very_high';
    risk_factors: string[];
    critical_dependencies: string[];
    potential_failure_points: string[];
    mitigation_strategies: string[];
  };
}

// Parse and analyze the complete PRP file
async function parsePRPForValidation(filePath: string): Promise<PRPValidationContext> {
  // Read the PRP file content
  // Extract all file references using regex patterns
  // Identify URL references and their contexts
  // Parse dependency requirements and environment needs
  // Analyze complexity indicators and risk factors
}
```

### Reference Extraction and Categorization
Use `grep_search` to systematically find all references:

```bash
# Comprehensive reference extraction
extract_prp_references() {
    local prp_file=$1
    local validation_results_dir=".prp-validation-$(date +%Y%m%d-%H%M%S)"
    
    mkdir -p "$validation_results_dir"
    
    echo "🔍 Extracting PRP References from: $prp_file"
    
    # Extract file references
    echo "📁 File References..."
    grep -n -E "(file:|path:|example:|pattern:)" "$prp_file" > "$validation_results_dir/file_refs.txt"
    grep -n -E "(\\.tsx?|\\.jsx?|\\.py|\\.json|\\.md|\\.yml|\\.yaml)" "$prp_file" >> "$validation_results_dir/file_refs.txt"
    
    # Extract URL references
    echo "🌐 URL References..."
    grep -n -E "(https?://|url:|docs:|documentation:)" "$prp_file" > "$validation_results_dir/url_refs.txt"
    
    # Extract dependency references
    echo "📦 Dependency References..."
    grep -n -E "(npm install|pip install|import |from |require\\()" "$prp_file" > "$validation_results_dir/dep_refs.txt"
    
    # Extract environment requirements
    echo "🔧 Environment Requirements..."
    grep -n -E "(API_KEY|_TOKEN|_SECRET|process\\.env|environment|config)" "$prp_file" > "$validation_results_dir/env_refs.txt"
    
    # Extract validation checklist items
    echo "✅ Validation Checklist..."
    grep -n -E "(\\[ \\]|\\[x\\]|- \\[ \\]|- \\[x\\]|checklist:|validate:|verify:)" "$prp_file" > "$validation_results_dir/checklist_refs.txt"
    
    echo "✅ Reference extraction completed in: $validation_results_dir"
}
```

## Phase 2: File System and Context Validation

### Comprehensive File Reference Validation
Use `file_search` and `list_dir` to validate all file references:

```typescript
// Validate all file references in the PRP
interface FileValidationResult {
  file_path: string;
  exists: boolean;
  accessible: boolean;
  last_modified?: string;
  file_size?: number;
  content_summary?: string;
  validation_issues: string[];
  recommendations: string[];
}

async function validateFileReferences(references: string[]): Promise<FileValidationResult[]> {
  const results: FileValidationResult[] = [];
  
  for (const filePath of references) {
    const result: FileValidationResult = {
      file_path: filePath,
      exists: false,
      accessible: false,
      validation_issues: [],
      recommendations: []
    };
    
    try {
      // Check if file exists using file_search
      const searchResults = await searchForFile(filePath);
      
      if (searchResults.length > 0) {
        result.exists = true;
        result.accessible = true;
        
        // Get file metadata
        const fileInfo = await getFileInfo(filePath);
        result.last_modified = fileInfo.lastModified;
        result.file_size = fileInfo.size;
        
        // Analyze file content for currency and relevance
        const contentAnalysis = await analyzeFileContent(filePath);
        result.content_summary = contentAnalysis.summary;
        
        // Check for potential issues
        if (contentAnalysis.outdated) {
          result.validation_issues.push('File content appears outdated');
          result.recommendations.push('Review and update file content');
        }
        
        if (contentAnalysis.complexity > 8) {
          result.validation_issues.push('High complexity file may need additional context');
          result.recommendations.push('Consider adding detailed documentation');
        }
        
      } else {
        result.exists = false;
        result.validation_issues.push('Referenced file not found');
        result.recommendations.push(`Create missing file: ${filePath}`);
        result.recommendations.push('Update PRP to reference existing file');
      }
      
    } catch (error) {
      result.validation_issues.push(`File validation error: ${error.message}`);
      result.recommendations.push('Verify file path and permissions');
    }
    
    results.push(result);
  }
  
  return results;
}
```

### URL Accessibility and Validation
```bash
# Comprehensive URL validation
validate_url_references() {
    local validation_results_dir=$1
    local url_refs_file="$validation_results_dir/url_refs.txt"
    
    echo "🌐 Validating URL References"
    
    # Extract unique URLs from references
    urls=$(grep -oE 'https?://[^[:space:]]+' "$url_refs_file" | sort -u)
    
    > "$validation_results_dir/url_validation.txt"
    
    for url in $urls; do
        echo "Checking: $url"
        
        # Check URL accessibility with comprehensive testing
        response_code=$(curl -s -o /dev/null -w "%{http_code}" --max-time 10 "$url")
        response_time=$(curl -s -o /dev/null -w "%{time_total}" --max-time 10 "$url")
        
        if [[ $response_code -eq 200 ]]; then
            echo "✅ $url - OK (${response_time}s)" | tee -a "$validation_results_dir/url_validation.txt"
        elif [[ $response_code -eq 404 ]]; then
            echo "❌ $url - NOT FOUND (404)" | tee -a "$validation_results_dir/url_validation.txt"
        elif [[ $response_code -eq 403 ]]; then
            echo "⚠️  $url - FORBIDDEN (403)" | tee -a "$validation_results_dir/url_validation.txt"
        elif [[ $response_code -eq 0 ]]; then
            echo "❌ $url - TIMEOUT/CONNECTION ERROR" | tee -a "$validation_results_dir/url_validation.txt"
        else
            echo "⚠️  $url - HTTP $response_code" | tee -a "$validation_results_dir/url_validation.txt"
        fi
        
        # Additional checks for documentation URLs
        if [[ $url == *"docs"* ]] || [[ $url == *"documentation"* ]]; then
            # Check if documentation is current (look for version indicators)
            content_check=$(curl -s "$url" | grep -i "version\|updated\|last modified" | head -1)
            if [[ -n "$content_check" ]]; then
                echo "   📚 Documentation info: $content_check" | tee -a "$validation_results_dir/url_validation.txt"
            fi
        fi
        
        sleep 0.5  # Rate limiting
    done
}
```

### Codebase Pattern Analysis
Use `semantic_search` to validate that referenced patterns exist:

```typescript
// Validate that referenced patterns and examples exist in codebase
interface PatternValidationResult {
  pattern_name: string;
  pattern_type: string;
  found_examples: Array<{
    file_path: string;
    relevance_score: number;
    pattern_match_quality: 'exact' | 'similar' | 'related' | 'none';
    usage_context: string;
  }>;
  pattern_currency: 'current' | 'outdated' | 'deprecated';
  recommendations: string[];
}

async function validatePatternReferences(prpContent: string): Promise<PatternValidationResult[]> {
  const results: PatternValidationResult[] = [];
  
  // Extract pattern references from PRP
  const patternMatches = prpContent.match(/pattern:\s*([^\n]+)/g) || [];
  const exampleMatches = prpContent.match(/example:\s*([^\n]+)/g) || [];
  
  const allPatterns = [...patternMatches, ...exampleMatches];
  
  for (const patternRef of allPatterns) {
    const patternName = patternRef.replace(/^(pattern|example):\s*/, '').trim();
    
    // Use semantic_search to find similar patterns in codebase
    const searchResults = await searchForPatterns(patternName);
    
    const result: PatternValidationResult = {
      pattern_name: patternName,
      pattern_type: patternRef.startsWith('pattern') ? 'pattern' : 'example',
      found_examples: [],
      pattern_currency: 'current',
      recommendations: []
    };
    
    // Analyze search results for pattern quality
    for (const searchResult of searchResults) {
      const example = {
        file_path: searchResult.filePath,
        relevance_score: searchResult.relevance,
        pattern_match_quality: determinePatternMatchQuality(searchResult),
        usage_context: searchResult.context
      };
      
      result.found_examples.push(example);
    }
    
    // Determine pattern currency and provide recommendations
    if (result.found_examples.length === 0) {
      result.pattern_currency = 'deprecated';
      result.recommendations.push(`Pattern '${patternName}' not found in current codebase`);
      result.recommendations.push('Update PRP with current pattern reference');
    } else if (result.found_examples.every(e => e.relevance_score < 0.3)) {
      result.pattern_currency = 'outdated';
      result.recommendations.push(`Pattern '${patternName}' may be outdated`);
      result.recommendations.push('Review and update pattern reference');
    }
    
    results.push(result);
  }
  
  return results;
}
```

## Phase 3: Dependency and Environment Validation

### Comprehensive Dependency Validation
```bash
# Validate all dependencies systematically
validate_dependencies() {
    local validation_results_dir=$1
    local dep_refs_file="$validation_results_dir/dep_refs.txt"
    
    echo "📦 Validating Dependencies"
    
    > "$validation_results_dir/dependency_validation.txt"
    
    # Node.js dependencies
    echo "🟨 Node.js Dependencies..." | tee -a "$validation_results_dir/dependency_validation.txt"
    if command -v npm >/dev/null 2>&1; then
        echo "✅ npm available" | tee -a "$validation_results_dir/dependency_validation.txt"
        
        # Check package.json exists
        if [[ -f "package.json" ]]; then
            echo "✅ package.json found" | tee -a "$validation_results_dir/dependency_validation.txt"
            
            # Run npm audit for security issues
            npm audit --audit-level moderate > "$validation_results_dir/npm_audit.txt" 2>&1
            if [[ $? -eq 0 ]]; then
                echo "✅ npm audit passed" | tee -a "$validation_results_dir/dependency_validation.txt"
            else
                echo "⚠️  npm audit found issues - check npm_audit.txt" | tee -a "$validation_results_dir/dependency_validation.txt"
            fi
            
            # Check for outdated packages
            npm outdated > "$validation_results_dir/npm_outdated.txt" 2>&1
            if [[ -s "$validation_results_dir/npm_outdated.txt" ]]; then
                echo "⚠️  Outdated packages found - check npm_outdated.txt" | tee -a "$validation_results_dir/dependency_validation.txt"
            else
                echo "✅ All npm packages up to date" | tee -a "$validation_results_dir/dependency_validation.txt"
            fi
        else
            echo "❌ package.json not found" | tee -a "$validation_results_dir/dependency_validation.txt"
        fi
    else
        echo "❌ npm not available" | tee -a "$validation_results_dir/dependency_validation.txt"
    fi
    
    # Python dependencies
    echo "🐍 Python Dependencies..." | tee -a "$validation_results_dir/dependency_validation.txt"
    if command -v python3 >/dev/null 2>&1; then
        echo "✅ Python3 available: $(python3 --version)" | tee -a "$validation_results_dir/dependency_validation.txt"
        
        # Check requirements.txt
        if [[ -f "requirements.txt" ]]; then
            echo "✅ requirements.txt found" | tee -a "$validation_results_dir/dependency_validation.txt"
            
            # Validate Python imports mentioned in PRP
            python3 << 'EOF' | tee -a "$validation_results_dir/dependency_validation.txt"
import re
import sys
import importlib

# Read PRP file and extract Python import statements
try:
    with open(sys.argv[1], 'r') as f:
        content = f.read()
    
    # Find import statements in code blocks
    import_patterns = [
        r'^import\s+([a-zA-Z_][a-zA-Z0-9_]*)',
        r'^from\s+([a-zA-Z_][a-zA-Z0-9_]*)',
    ]
    
    imports = set()
    for pattern in import_patterns:
        matches = re.findall(pattern, content, re.MULTILINE)
        imports.update(matches)
    
    # Test each import
    failed_imports = []
    for module in imports:
        try:
            importlib.import_module(module)
            print(f'✅ Python module available: {module}')
        except ImportError:
            failed_imports.append(module)
            print(f'❌ Python module missing: {module}')
    
    if failed_imports:
        print(f'🔧 Install missing modules: pip install {" ".join(failed_imports)}')
        sys.exit(1)
    else:
        print('✅ All Python dependencies satisfied')

except Exception as e:
    print(f'⚠️  Python dependency check failed: {e}')
EOF
        fi
    else
        echo "❌ Python3 not available" | tee -a "$validation_results_dir/dependency_validation.txt"
    fi
    
    # System tools and utilities
    echo "🛠️  System Tools..." | tee -a "$validation_results_dir/dependency_validation.txt"
    local required_tools=("git" "curl" "jq")
    for tool in "${required_tools[@]}"; do
        if command -v "$tool" >/dev/null 2>&1; then
            echo "✅ $tool available: $(command -v $tool)" | tee -a "$validation_results_dir/dependency_validation.txt"
        else
            echo "❌ $tool not available" | tee -a "$validation_results_dir/dependency_validation.txt"
        fi
    done
}
```

### Environment and API Validation
```bash
# Comprehensive environment validation
validate_environment() {
    local validation_results_dir=$1
    local env_refs_file="$validation_results_dir/env_refs.txt"
    
    echo "🔧 Validating Environment Configuration"
    
    > "$validation_results_dir/environment_validation.txt"
    
    # Check for common API keys and environment variables
    echo "🔑 API Keys and Environment Variables..." | tee -a "$validation_results_dir/environment_validation.txt"
    
    # Extract environment variable references from PRP
    env_vars=$(grep -oE '\$[A-Z_]+|\${[A-Z_]+}|process\.env\.[A-Z_]+' "$prp_file" | sort -u)
    
    for env_var in $env_vars; do
        # Clean up the environment variable name
        clean_var=$(echo "$env_var" | sed -E 's/\$\{?([A-Z_]+)\}?/\1/' | sed 's/process\.env\.//')
        
        if [[ -n "${!clean_var}" ]]; then
            echo "✅ Environment variable set: $clean_var" | tee -a "$validation_results_dir/environment_validation.txt"
        else
            echo "❌ Environment variable missing: $clean_var" | tee -a "$validation_results_dir/environment_validation.txt"
        fi
    done
    
    # Check common API services
    echo "🌐 API Service Connectivity..." | tee -a "$validation_results_dir/environment_validation.txt"
    
    # OpenAI API
    if grep -q "openai\|gpt\|GPT" "$prp_file"; then
        if [[ -n "$OPENAI_API_KEY" ]]; then
            echo "✅ OpenAI API key configured" | tee -a "$validation_results_dir/environment_validation.txt"
            
            # Test API connectivity
            response=$(curl -s -w "%{http_code}" -o /dev/null \
                -H "Authorization: Bearer $OPENAI_API_KEY" \
                "https://api.openai.com/v1/models")
            
            if [[ $response -eq 200 ]]; then
                echo "✅ OpenAI API accessible" | tee -a "$validation_results_dir/environment_validation.txt"
            else
                echo "⚠️  OpenAI API not accessible (HTTP $response)" | tee -a "$validation_results_dir/environment_validation.txt"
            fi
        else
            echo "❌ OpenAI API key not configured" | tee -a "$validation_results_dir/environment_validation.txt"
        fi
    fi
    
    # GitHub API
    if grep -q "github\|GitHub" "$prp_file"; then
        if [[ -n "$GITHUB_TOKEN" ]]; then
            echo "✅ GitHub token configured" | tee -a "$validation_results_dir/environment_validation.txt"
            
            # Test GitHub API connectivity
            response=$(curl -s -w "%{http_code}" -o /dev/null \
                -H "Authorization: token $GITHUB_TOKEN" \
                "https://api.github.com/user")
            
            if [[ $response -eq 200 ]]; then
                echo "✅ GitHub API accessible" | tee -a "$validation_results_dir/environment_validation.txt"
            else
                echo "⚠️  GitHub API not accessible (HTTP $response)" | tee -a "$validation_results_dir/environment_validation.txt"
            fi
        else
            echo "⚠️  GitHub token not configured" | tee -a "$validation_results_dir/environment_validation.txt"
        fi
    fi
    
    # Docker (if mentioned in PRP)
    if grep -q "docker\|Docker\|container" "$prp_file"; then
        if command -v docker >/dev/null 2>&1; then
            echo "✅ Docker available: $(docker --version)" | tee -a "$validation_results_dir/environment_validation.txt"
            
            # Check Docker daemon
            if docker info >/dev/null 2>&1; then
                echo "✅ Docker daemon running" | tee -a "$validation_results_dir/environment_validation.txt"
            else
                echo "❌ Docker daemon not running" | tee -a "$validation_results_dir/environment_validation.txt"
            fi
        else
            echo "❌ Docker not available" | tee -a "$validation_results_dir/environment_validation.txt"
        fi
    fi
}
```

## Phase 4: Risk Assessment and Complexity Analysis

### Comprehensive Risk Assessment
```typescript
// Perform comprehensive risk assessment of the PRP
interface RiskAssessment {
  overall_risk_level: 'low' | 'medium' | 'high' | 'very_high';
  complexity_score: number; // 1-10 scale
  
  risk_factors: Array<{
    factor: string;
    category: 'technical' | 'environmental' | 'dependency' | 'complexity' | 'timeline';
    impact: 'low' | 'medium' | 'high' | 'critical';
    probability: 'low' | 'medium' | 'high';
    mitigation_strategies: string[];
  }>;
  
  critical_dependencies: Array<{
    dependency: string;
    criticality: 'blocking' | 'important' | 'optional';
    availability_status: 'available' | 'missing' | 'outdated';
    resolution_effort: 'low' | 'medium' | 'high';
  }>;
  
  potential_failure_points: Array<{
    failure_scenario: string;
    impact: 'low' | 'medium' | 'high' | 'critical';
    early_detection: string[];
    recovery_procedures: string[];
  }>;
  
  readiness_score: number; // 0-100 scale
  recommendations: Array<{
    priority: 'immediate' | 'high' | 'medium' | 'low';
    action: string;
    effort_estimate: string;
    impact: string;
  }>;
}

// Analyze risk factors and complexity
function assessPRPRisk(
  validationContext: PRPValidationContext,
  fileValidation: FileValidationResult[],
  dependencyStatus: any[]
): RiskAssessment {
  // Calculate complexity score based on various factors
  // Identify risk factors and their mitigation strategies
  // Analyze critical dependencies and their status
  // Generate recommendations for risk mitigation
}
```

### Automated Fix Generation
```bash
# Generate automated fixes for common validation issues
generate_auto_fixes() {
    local validation_results_dir=$1
    
    echo "🔧 Generating Auto-Fix Suggestions"
    
    > "$validation_results_dir/auto_fixes.sh"
    echo "#!/bin/bash" >> "$validation_results_dir/auto_fixes.sh"
    echo "# Auto-generated fixes for PRP validation issues" >> "$validation_results_dir/auto_fixes.sh"
    echo "" >> "$validation_results_dir/auto_fixes.sh"
    
    # Generate npm dependency fixes
    if [[ -f "$validation_results_dir/npm_audit.txt" ]] && grep -q "vulnerabilities" "$validation_results_dir/npm_audit.txt"; then
        echo "echo 'Fixing npm security vulnerabilities...'" >> "$validation_results_dir/auto_fixes.sh"
        echo "npm audit fix" >> "$validation_results_dir/auto_fixes.sh"
        echo "" >> "$validation_results_dir/auto_fixes.sh"
    fi
    
    # Generate Python dependency fixes
    missing_python_modules=$(grep "❌ Python module missing:" "$validation_results_dir/dependency_validation.txt" | cut -d':' -f2 | tr -d ' ')
    if [[ -n "$missing_python_modules" ]]; then
        echo "echo 'Installing missing Python modules...'" >> "$validation_results_dir/auto_fixes.sh"
        echo "pip install $missing_python_modules" >> "$validation_results_dir/auto_fixes.sh"
        echo "" >> "$validation_results_dir/auto_fixes.sh"
    fi
    
    # Generate environment variable setup
    missing_env_vars=$(grep "❌ Environment variable missing:" "$validation_results_dir/environment_validation.txt" | cut -d':' -f2 | tr -d ' ')
    if [[ -n "$missing_env_vars" ]]; then
        echo "echo 'Setting up missing environment variables...'" >> "$validation_results_dir/auto_fixes.sh"
        for var in $missing_env_vars; do
            echo "echo 'Please set $var: export $var=your_value_here'" >> "$validation_results_dir/auto_fixes.sh"
        done
        echo "" >> "$validation_results_dir/auto_fixes.sh"
    fi
    
    # Make the fix script executable
    chmod +x "$validation_results_dir/auto_fixes.sh"
    
    echo "✅ Auto-fix script generated: $validation_results_dir/auto_fixes.sh"
}
```

## Phase 5: Comprehensive Validation Report Generation

### Detailed Validation Report Creation
Use `create_file` to generate comprehensive validation report:

```markdown
# PRP Validation Report: {prp_name}

## 📋 Validation Summary
- **PRP File**: {prp_file}
- **Validation Date**: {validation_timestamp}
- **Validation Depth**: {validation_depth}
- **Overall Status**: {overall_status}
- **Readiness Score**: {readiness_score}/100

## 🔍 Context Validation Results

### File References
{file_validation_detailed_results}

### URL Accessibility
{url_validation_detailed_results}

### Pattern Currency
{pattern_validation_detailed_results}

## 📦 Dependency Analysis

### System Dependencies
{system_dependency_results}

### Package Dependencies
{package_dependency_results}

### Environment Configuration
{environment_validation_results}

## ⚠️ Risk Assessment

### Overall Risk Level: {overall_risk_level}
### Complexity Score: {complexity_score}/10

### Identified Risk Factors
{detailed_risk_factor_analysis}

### Critical Dependencies
{critical_dependency_analysis}

### Potential Failure Points
{failure_point_analysis}

## 🎯 Recommendations

### Immediate Actions Required
{immediate_action_items}

### High Priority Improvements
{high_priority_recommendations}

### Medium Priority Enhancements
{medium_priority_recommendations}

## 🔧 Auto-Fix Suggestions

### Automated Fixes Available
{auto_fix_commands_and_scripts}

### Manual Interventions Required
{manual_intervention_requirements}

## 📊 Detailed Validation Metrics

### Validation Gate Results
- File References: {file_validation_score}
- URL Accessibility: {url_validation_score}
- Dependencies: {dependency_validation_score}
- Environment: {environment_validation_score}
- Risk Factors: {risk_assessment_score}

### Quality Indicators
- Context Completeness: {context_completeness_percentage}
- Dependency Readiness: {dependency_readiness_status}
- Environmental Readiness: {environment_readiness_status}

## 🚀 Execution Readiness

### Pre-Execution Checklist
{execution_readiness_checklist}

### Blocking Issues
{blocking_issues_that_prevent_execution}

### Warning Issues
{warning_issues_that_should_be_addressed}

### Optional Improvements
{optional_improvements_for_better_execution}

## 📈 Recommendations for PRP Improvement

### Content Enhancements
{recommendations_for_prp_content_improvement}

### Context Improvements
{recommendations_for_better_context_provision}

### Dependency Clarifications
{recommendations_for_clearer_dependency_specification}

---
*This validation report provides comprehensive analysis of PRP readiness and actionable recommendations for successful execution.*
```

### Integration with Execution Pipeline
```bash
# Integration point for execute commands
validate_before_execution() {
    local prp_file=$1
    
    echo "🔍 Running Pre-Execution Validation..."
    
    # Run comprehensive validation
    validate_prp_comprehensive "$prp_file"
    local validation_exit_code=$?
    
    if [[ $validation_exit_code -eq 0 ]]; then
        echo "✅ Pre-execution validation passed - proceeding with execution"
        return 0
    else
        echo "❌ Pre-execution validation failed"
        echo "📋 Please review validation report and fix issues before execution"
        echo "🔧 Auto-fix suggestions available in validation results"
        return 1
    fi
}

# Export validation function for use by other commands
export -f validate_before_execution
```

## Success Criteria

The PRP validation is complete when:
- [ ] PRP file is successfully parsed and analyzed
- [ ] All file references are validated for existence and currency
- [ ] All URL references are checked for accessibility
- [ ] Pattern references are verified against current codebase
- [ ] All dependencies are validated and available
- [ ] Environment configuration is verified
- [ ] Risk assessment is completed with mitigation strategies
- [ ] Comprehensive validation report is generated
- [ ] Auto-fix suggestions are provided for issues
- [ ] Execution readiness determination is made

## Integration Points

### Development Workflow Integration
- Integrate with pre-execution validation in all PRP execution commands
- Connect with continuous integration for automated validation
- Align with code review processes for PRP quality assurance
- Integrate with project management for readiness tracking

### Quality Assurance Integration
- Leverage existing testing frameworks for validation procedures
- Connect with code quality tools for pattern validation
- Integrate with security scanning for dependency validation
- Align with organizational compliance requirements

### Knowledge Management Integration
- Contribute validation patterns to organizational knowledge base
- Share validation insights with development teams
- Document validation procedures for process improvement
- Update validation criteria based on lessons learned

Remember: Comprehensive validation ensures successful PRP execution by identifying and resolving all potential issues before implementation begins.
```

## Notes
- Focus on comprehensive pre-flight validation across all dimensions
- Provide actionable auto-fix suggestions where possible
- Generate detailed reports for informed decision making
- Integrate seamlessly with execution pipeline for safety
