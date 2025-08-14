---
mode: agent
description: "Analyze and execute Product Requirements Prompts with comprehensive validation and quality assurance"
---

---
inputs:
  - name: analysis_target
    description: Target for analysis (existing-prps, codebase, requirements)
    required: true
  - name: analysis_depth
    description: Analysis depth (surface, detailed, comprehensive)
    required: false
    default: "detailed"
---

# prp-analyze-run.prompt.md

## Metadata
- **Type**: GitHub Copilot Custom Prompt
- **Mode**: agent
- **Purpose**: Post-execution analysis of PRP implementation to capture lessons learned, success metrics, and template improvements
- **Categories**: prp-analysis, metrics-collection, continuous-improvement, knowledge-management
- **Complexity**: advanced
- **Dependencies**: git, testing frameworks, code quality tools

## Input
- **prp_file** (required): Path to the PRP file that was executed
- **implementation_start_time** (optional): When implementation started (for time tracking)
- **target_feature_type** (optional): Type of feature implemented (api, database, cli, etc.)

## Template

```
You are an expert PRP analysis specialist focused on extracting maximum learning from each implementation to continuously improve the PRP methodology. Your task is to conduct comprehensive post-execution analysis that captures success patterns, failure modes, and actionable improvements for future PRPs.

## Input Parameters
- **PRP File**: {prp_file}
- **Implementation Start**: {implementation_start_time} (default: 2 hours ago)
- **Feature Type**: {target_feature_type} (auto-detect if not provided)

## Task Overview
Conduct thorough post-implementation analysis to measure success metrics, identify effective patterns, capture failure modes, and generate actionable recommendations for improving future PRP effectiveness and template quality.

## Phase 1: Implementation Metrics Collection

### Git Statistics Analysis
Use `run_in_terminal` to collect comprehensive git metrics:

```bash
# Get implementation timeframe statistics
COMMITS_DURING_IMPL=$(git rev-list --count HEAD --since="2 hours ago")
FILES_CHANGED=$(git diff --name-only HEAD~$COMMITS_DURING_IMPL HEAD | wc -l)
LINES_ADDED=$(git diff --shortstat HEAD~$COMMITS_DURING_IMPL HEAD | grep -o '[0-9]* insertion' | grep -o '[0-9]*' || echo 0)
LINES_DELETED=$(git diff --shortstat HEAD~$COMMITS_DURING_IMPL HEAD | grep -o '[0-9]* deletion' | grep -o '[0-9]*' || echo 0)

echo "📊 Git Implementation Metrics:"
echo "- Commits during implementation: $COMMITS_DURING_IMPL"
echo "- Files changed: $FILES_CHANGED"
echo "- Lines added: $LINES_ADDED"
echo "- Lines deleted: $LINES_DELETED"
```

### Test Results Analysis
Use `run_tests` to validate current test status and `run_in_terminal` for detailed analysis:

```bash
# Collect comprehensive test metrics
TEST_RESULTS=$(pytest tests/ --tb=no -q 2>&1 | tail -n 1)
TEST_COUNT=$(echo "$TEST_RESULTS" | grep -o '[0-9]* passed' | grep -o '[0-9]*' || echo 0)
TEST_FAILURES=$(echo "$TEST_RESULTS" | grep -o '[0-9]* failed' | grep -o '[0-9]*' || echo 0)
COVERAGE_RESULT=$(coverage report --format=total 2>/dev/null || echo "0")

echo "🧪 Test Implementation Metrics:"
echo "- Tests passing: $TEST_COUNT"
echo "- Tests failing: $TEST_FAILURES"
echo "- Code coverage: $COVERAGE_RESULT%"
```

### Code Quality Assessment
Use `get_errors` on key files and `run_in_terminal` for quality metrics:

```bash
# Collect code quality metrics
LINT_ISSUES=$(eslint . --format=compact 2>&1 | grep -c "error\|warning" || echo 0)
TYPE_ERRORS=$(tsc --noEmit 2>&1 | grep -c "error" || echo 0)
PRETTIER_ISSUES=$(prettier --check . 2>&1 | grep -c "Code style issues" || echo 0)

echo "🎯 Code Quality Metrics:"
echo "- Linting issues: $LINT_ISSUES"
echo "- TypeScript errors: $TYPE_ERRORS"
echo "- Style formatting issues: $PRETTIER_ISSUES"
```

### Performance and Complexity Analysis
```bash
# Analyze implementation complexity
FUNCTION_COUNT=$(grep -r "function\|const.*=.*=>" . --include="*.ts" --include="*.tsx" | wc -l)
COMPONENT_COUNT=$(grep -r "const.*: React\.FC\|function.*Component" . --include="*.tsx" | wc -l)
FILE_SIZE_AVG=$(find . -name "*.ts" -o -name "*.tsx" | xargs wc -l | tail -n 1 | awk '{print $1}')

echo "📈 Complexity Metrics:"
echo "- Functions/hooks created: $FUNCTION_COUNT"
echo "- Components created: $COMPONENT_COUNT"
echo "- Average file size: $FILE_SIZE_AVG lines"
```

## Phase 2: Context Effectiveness Analysis

### PRP Content Analysis
Use `read_file` to analyze the original PRP and measure context effectiveness:

```typescript
// Analyze which context elements were most valuable during implementation
interface ContextElement {
  type: 'documentation' | 'file_reference' | 'example' | 'gotcha' | 'pattern';
  content: string;
  referenced: boolean;
  effectiveness_score: number;
}

function analyzeContextEffectiveness(prpContent: string): ContextElement[] {
  const elements: ContextElement[] = [];
  
  // Extract different types of context elements
  const documentationUrls = prpContent.match(/url: (https?:\/\/[^\s]+)/g) || [];
  const fileReferences = prpContent.match(/file: ([^\s]+)/g) || [];
  const gotchas = prpContent.match(/# CRITICAL: ([^\n]+)/g) || [];
  const patterns = prpContent.match(/# PATTERN: ([^\n]+)/g) || [];
  const examples = prpContent.match(/examples\/([^\s]+)/g) || [];
  
  // Score effectiveness based on git changes and commit messages
  // Elements are effective if they prevented errors or guided implementation
  
  return elements;
}
```

### Implementation Path Analysis
Use `run_in_terminal` to analyze the implementation journey:

```bash
# Analyze commit message patterns for implementation path
git log --oneline --since="2 hours ago" > /tmp/recent_commits.txt

# Check for iteration patterns
FIX_COMMITS=$(grep -i "fix\|error\|bug\|issue" /tmp/recent_commits.txt | wc -l)
REFACTOR_COMMITS=$(grep -i "refactor\|improve\|optimize" /tmp/recent_commits.txt | wc -l)
FEATURE_COMMITS=$(grep -i "add\|implement\|create" /tmp/recent_commits.txt | wc -l)

echo "🔄 Implementation Path Analysis:"
echo "- Fix/error commits: $FIX_COMMITS"
echo "- Refactoring commits: $REFACTOR_COMMITS"
echo "- Feature commits: $FEATURE_COMMITS"
```

## Phase 3: Failure Pattern Detection and Analysis

### Error Pattern Extraction
Use `run_in_terminal` to extract failure patterns from the implementation:

```bash
# Extract failure patterns from commit messages and error logs
echo "🔍 Analyzing failure patterns..."

# Common failure patterns to check for
declare -A FAILURE_PATTERNS
FAILURE_PATTERNS[async_context]="async.*error\|await.*sync\|Promise.*TypeError"
FAILURE_PATTERNS[import_error]="import.*error\|module.*not.*found\|cannot.*resolve"
FAILURE_PATTERNS[type_error]="type.*error\|property.*does.*not.*exist\|argument.*type"
FAILURE_PATTERNS[dependency_error]="dependency.*error\|package.*not.*found\|version.*conflict"
FAILURE_PATTERNS[configuration_error]="config.*error\|environment.*variable\|settings.*invalid"

for pattern_name in "${!FAILURE_PATTERNS[@]}"; do
  pattern="${FAILURE_PATTERNS[$pattern_name]}"
  count=$(git log --oneline --since="2 hours ago" | grep -i -E "$pattern" | wc -l)
  if [ $count -gt 0 ]; then
    echo "❌ $pattern_name: $count occurrences"
  fi
done
```

### Anti-Pattern Detection
Use `semantic_search` to find potential anti-patterns in the implementation:

```typescript
// Check for common anti-patterns that may have been introduced
const antiPatterns = [
  'Mixed async/sync patterns',
  'Direct DOM manipulation in React',
  'Props drilling instead of context',
  'Unused imports or variables',
  'Magic numbers or hardcoded values',
  'Missing error boundaries',
  'Inefficient re-renders',
  'Memory leaks in useEffect'
];

// Search for these patterns in the codebase
antiPatterns.forEach(pattern => {
  // Use semantic_search to find instances
});
```

## Phase 4: Success Pattern Identification

### Effective Implementation Patterns
Use `semantic_search` to identify successful patterns that emerged:

```typescript
// Identify patterns that led to successful implementation
interface SuccessPattern {
  pattern: string;
  description: string;
  reuse_recommendation: string;
  frequency: 'high' | 'medium' | 'low';
  impact: 'high' | 'medium' | 'low';
}

function identifySuccessPatterns(): SuccessPattern[] {
  const patterns: SuccessPattern[] = [];
  
  // Check for comprehensive testing
  if (testResults.passed > 0 && testResults.failed === 0) {
    patterns.push({
      pattern: 'comprehensive_testing',
      description: 'All tests passed on implementation',
      reuse_recommendation: 'Include similar test coverage patterns in future PRPs',
      frequency: 'high',
      impact: 'high'
    });
  }
  
  // Check for clean code quality
  if (lintIssues === 0 && typeErrors === 0) {
    patterns.push({
      pattern: 'clean_code_implementation',
      description: 'No quality issues detected',
      reuse_recommendation: 'Maintain consistent code quality patterns',
      frequency: 'high',
      impact: 'medium'
    });
  }
  
  // Check for proper error handling
  // Use grep_search to find error handling patterns
  
  return patterns;
}
```

### Performance Optimization Patterns
```bash
# Check for performance optimization patterns
echo "⚡ Performance Pattern Analysis:"

# Check for memoization usage
MEMO_USAGE=$(grep -r "useMemo\|useCallback\|React\.memo" . --include="*.tsx" --include="*.ts" | wc -l)

# Check for lazy loading
LAZY_LOADING=$(grep -r "React\.lazy\|dynamic.*import" . --include="*.tsx" --include="*.ts" | wc -l)

# Check for virtualization
VIRTUALIZATION=$(grep -r "react-window\|react-virtualized\|virtual" . --include="*.tsx" | wc -l)

echo "- Memoization implementations: $MEMO_USAGE"
echo "- Lazy loading implementations: $LAZY_LOADING"
echo "- Virtualization implementations: $VIRTUALIZATION"
```

## Phase 5: Knowledge Base Updates and Pattern Storage

### Failure Pattern Database Update
Use `create_file` to update failure pattern knowledge base:

```yaml
# Create or update PRPs/knowledge_base/failure_patterns.yaml
failure_patterns:
  - id: "typescript_strict_mode_errors"
    description: "TypeScript strict mode configuration issues"
    frequency: "medium"
    detection_signs:
      - "Property does not exist on type"
      - "Object is possibly null or undefined"
    prevention:
      - "Enable strict mode gradually"
      - "Use proper type guards and null checks"
    related_technologies: ["typescript", "react"]
    last_seen: {current_timestamp}
    
  - id: "react_state_management_complexity"
    description: "Complex state management leading to bugs"
    frequency: "high"
    detection_signs:
      - "setState called on unmounted component"
      - "Infinite re-render loops"
    prevention:
      - "Use reducer pattern for complex state"
      - "Implement proper cleanup in useEffect"
    related_technologies: ["react", "zustand", "redux"]
    last_seen: {current_timestamp}
```

### Success Metrics Database Update
```yaml
# Update PRPs/knowledge_base/success_metrics.yaml
success_metrics:
  - feature_type: "react_component_library"
    implementations: {total_implementations}
    avg_token_usage: {calculated_average}
    avg_implementation_time: {time_in_minutes}
    success_rate: {percentage_successful}
    common_successful_patterns:
      - "Component composition over inheritance"
      - "Custom hooks for logic separation"
      - "Comprehensive TypeScript typing"
    last_updated: {current_timestamp}
    
  - feature_type: "api_integration"
    implementations: {total_implementations}
    avg_token_usage: {calculated_average}
    avg_implementation_time: {time_in_minutes}
    success_rate: {percentage_successful}
    common_successful_patterns:
      - "React Query for data management"
      - "Proper error boundary implementation"
      - "Loading state management"
    last_updated: {current_timestamp}
```

## Phase 6: Template Improvement Recommendations

### Context Enhancement Suggestions
Use analysis results to suggest template improvements:

```typescript
interface TemplateImprovement {
  section: 'context' | 'validation' | 'examples' | 'gotchas';
  improvement: string;
  rationale: string;
  priority: 'high' | 'medium' | 'low';
}

function generateTemplateImprovements(): TemplateImprovement[] {
  const improvements: TemplateImprovement[] = [];
  
  // Analyze missing context that caused issues
  if (typeErrors > 0) {
    improvements.push({
      section: 'context',
      improvement: 'Add TypeScript configuration validation section',
      rationale: 'Type errors occurred during implementation',
      priority: 'high'
    });
  }
  
  // Analyze validation gaps
  if (testFailures > 0) {
    improvements.push({
      section: 'validation',
      improvement: 'Add comprehensive testing validation loop',
      rationale: 'Test failures occurred that could have been prevented',
      priority: 'high'
    });
  }
  
  return improvements;
}
```

### Validation Loop Enhancements
```bash
# Suggest validation loop improvements based on issues encountered
echo "🔧 Validation Loop Enhancement Suggestions:"

if [ $TYPE_ERRORS -gt 0 ]; then
  echo "- Add 'tsc --noEmit' validation before implementation"
fi

if [ $LINT_ISSUES -gt 0 ]; then
  echo "- Add 'eslint --fix' validation step"
fi

if [ $TEST_FAILURES -gt 0 ]; then
  echo "- Add 'npm test -- --watchAll=false' validation loop"
fi

if [ $PRETTIER_ISSUES -gt 0 ]; then
  echo "- Add 'prettier --write .' formatting step"
fi
```

## Phase 7: Confidence Score Validation

### Original vs Actual Performance Analysis
Use `read_file` to extract original confidence score and compare with actual results:

```typescript
// Validate whether the original confidence score was accurate
interface ConfidenceValidation {
  original_confidence: number;
  actual_performance_score: number;
  accuracy_assessment: 'excellent' | 'good' | 'poor';
  deviation_analysis: string;
  calibration_suggestion: string;
}

function validateConfidenceScore(prpContent: string, metrics: any): ConfidenceValidation {
  // Extract original confidence score
  const confidenceMatch = prpContent.match(/Confidence Score: (\d+)\/10/);
  const originalConfidence = confidenceMatch ? parseInt(confidenceMatch[1]) : 5;
  
  // Calculate actual performance score based on implementation results
  let actualScore = 10;
  
  if (metrics.testFailures > 0) actualScore -= 2;
  if (metrics.typeErrors > 0) actualScore -= 1;
  if (metrics.lintIssues > 10) actualScore -= 1;
  if (metrics.implementationTime > 90) actualScore -= 2;
  if (metrics.commitCount > 10) actualScore -= 1; // Too many iterations
  
  const deviation = Math.abs(originalConfidence - actualScore);
  
  return {
    original_confidence: originalConfidence,
    actual_performance_score: Math.max(actualScore, 1),
    accuracy_assessment: deviation <= 1 ? 'excellent' : deviation <= 2 ? 'good' : 'poor',
    deviation_analysis: `Original estimate was ${deviation} points ${originalConfidence > actualScore ? 'optimistic' : 'pessimistic'}`,
    calibration_suggestion: generateCalibrationSuggestion(deviation, originalConfidence, actualScore)
  };
}
```

## Phase 8: Comprehensive Analysis Report Generation

### Human-Readable Report Creation
Use `create_file` to generate comprehensive analysis report:

```markdown
# PRP Analysis Report: {prp_file}

## 📊 Executive Summary
- **Implementation Date**: {current_timestamp}
- **Feature Type**: {detected_feature_type}
- **Overall Success**: {SUCCESS/PARTIAL/FAILED}
- **Implementation Time**: {time_minutes} minutes
- **Confidence Accuracy**: {accuracy_assessment}

## 📈 Implementation Metrics

### Development Statistics
- **Commits**: {commit_count} during implementation
- **Files Changed**: {files_changed}
- **Code Changes**: +{lines_added}/-{lines_deleted} lines
- **Functions Created**: {function_count}
- **Components Created**: {component_count}

### Quality Metrics
- **Tests**: {tests_passed} passed, {tests_failed} failed
- **Code Coverage**: {coverage_percentage}%
- **Type Errors**: {type_errors}
- **Linting Issues**: {lint_issues}
- **Style Issues**: {style_issues}

## 🎯 Context Effectiveness Analysis

### Most Effective Context Elements
{list_of_effective_context_with_scores}

### Underutilized Context
{list_of_unused_context_elements}

### Missing Context Identified
{list_of_missing_context_that_caused_issues}

## 🔍 Pattern Analysis

### ✅ Success Patterns Discovered
{for each success_pattern}
- **{pattern.name}**: {pattern.description}
  - **Impact**: {pattern.impact}
  - **Reuse Recommendation**: {pattern.reuse_recommendation}

### ❌ Failure Patterns Identified
{for each failure_pattern}
- **{pattern.name}**: {pattern.description}
  - **Frequency**: {pattern.frequency}
  - **Prevention**: {pattern.solution}

## 💡 Improvement Recommendations

### Template Enhancements
{for each template_improvement}
- **Section**: {improvement.section}
- **Improvement**: {improvement.improvement}
- **Priority**: {improvement.priority}
- **Rationale**: {improvement.rationale}

### Validation Loop Enhancements
{list_of_validation_improvements}

### Context Improvements
{list_of_context_improvements}

## 🎯 Confidence Score Analysis
- **Original Confidence**: {original_confidence}/10
- **Actual Performance**: {actual_score}/10
- **Accuracy**: {accuracy_assessment}
- **Calibration Recommendation**: {calibration_suggestion}

## 📚 Knowledge Base Updates
- **New Failure Patterns**: {new_failure_patterns_count} added
- **Updated Success Metrics**: {updated_metrics_count} features
- **Template Improvements**: {template_improvements_count} suggestions

## 🔄 Next Steps
1. Apply template improvements to base PRP template
2. Update failure pattern prevention in guidelines
3. Enhance validation loops based on analysis
4. Calibrate confidence scoring methodology

---
*This analysis was automatically generated and contributes to the continuous improvement of the PRP methodology.*
```

### Structured Data Export
```yaml
# Also create machine-readable analysis for automated processing
analysis_data:
  prp_file: {prp_file}
  timestamp: {current_timestamp}
  metrics:
    implementation_time_minutes: {time_minutes}
    commit_count: {commits}
    files_changed: {files_changed}
    lines_added: {lines_added}
    lines_deleted: {lines_deleted}
    tests_passed: {tests_passed}
    tests_failed: {tests_failed}
    type_errors: {type_errors}
    lint_issues: {lint_issues}
  
  patterns:
    success_patterns: {success_patterns_array}
    failure_patterns: {failure_patterns_array}
  
  confidence_validation:
    original: {original_confidence}
    actual: {actual_score}
    accuracy: {accuracy_assessment}
  
  recommendations:
    template_improvements: {improvements_array}
    validation_enhancements: {validation_array}
    context_improvements: {context_array}
```

## Phase 9: Automated Knowledge Base Integration

### Pattern Database Updates
Use `run_in_terminal` to automatically update knowledge bases:

```bash
# Automatically integrate findings into knowledge base
echo "🔄 Integrating analysis into knowledge base..."

# Update failure patterns database
python3 -c "
import yaml
import sys
from datetime import datetime

# Load analysis results
analysis_file = 'PRPs/analysis_reports/analysis_{timestamp}.yaml'
with open(analysis_file, 'r') as f:
    analysis = yaml.safe_load(f)

# Update failure patterns
failure_db_path = 'PRPs/knowledge_base/failure_patterns.yaml'
try:
    with open(failure_db_path, 'r') as f:
        failure_db = yaml.safe_load(f) or {'failure_patterns': []}
except FileNotFoundError:
    failure_db = {'failure_patterns': []}

# Add new patterns from analysis
for pattern in analysis['patterns']['failure_patterns']:
    existing = next((p for p in failure_db['failure_patterns'] if p.get('id') == pattern['id']), None)
    if existing:
        existing['frequency'] = 'high' if existing.get('frequency') == 'medium' else existing.get('frequency', 'medium')
        existing['last_seen'] = datetime.now().isoformat()
    else:
        pattern['first_seen'] = datetime.now().isoformat()
        pattern['last_seen'] = datetime.now().isoformat()
        failure_db['failure_patterns'].append(pattern)

# Save updated database
with open(failure_db_path, 'w') as f:
    yaml.dump(failure_db, f, default_flow_style=False)

print(f'Updated failure patterns database')
"

# Update success metrics database
python3 -c "
# Similar process for success metrics
# Update running averages and success rates
print('Updated success metrics database')
"
```

### Template Version Control
```bash
# Check if template improvements warrant new version
IMPROVEMENT_COUNT=$(grep -c "priority: high" PRPs/analysis_reports/analysis_${timestamp}.yaml || echo 0)

if [ $IMPROVEMENT_COUNT -ge 3 ]; then
    echo "🚀 Generating improved template version based on analysis..."
    
    # Create new versioned template with improvements
    CURRENT_DATE=$(date +%Y%m%d_%H%M%S)
    cp PRPs/templates/prp_base.md PRPs/templates/prp_base_v${CURRENT_DATE}.md
    
    echo "✅ Created improved template version: prp_base_v${CURRENT_DATE}.md"
    echo "📝 Apply manual improvements based on analysis recommendations"
fi
```

## Success Criteria

The PRP analysis is complete when:
- [ ] All implementation metrics are collected and documented
- [ ] Context effectiveness is measured and analyzed
- [ ] Success and failure patterns are identified and categorized
- [ ] Knowledge base is updated with new learnings
- [ ] Template improvements are recommended with rationale
- [ ] Confidence score validation is performed
- [ ] Comprehensive analysis report is generated
- [ ] Machine-readable data is exported for automation
- [ ] Automated knowledge base integration is completed

## Integration Points

### Continuous Improvement Loop
- **Execute PRP** → Implement feature with guided context
- **Analyze Results** → Extract patterns and measure effectiveness
- **Update Knowledge Base** → Store learnings in structured format
- **Improve Templates** → Apply learnings to future PRPs
- **Enhanced Context** → Higher success rates in subsequent implementations

### Team Learning
- Share analysis reports with development team
- Use pattern analysis to improve coding practices
- Apply template improvements to organizational standards
- Build institutional knowledge about effective development patterns

### Quality Assurance Integration
- Use failure patterns to enhance code review checklists
- Apply success patterns to development guidelines
- Integrate validation improvements into CI/CD pipeline
- Monitor confidence score accuracy for methodology calibration

Remember: The goal is continuous improvement of the PRP methodology through systematic analysis and knowledge capture from each implementation.
```

## Notes
- Focus on extracting actionable insights from every implementation
- Maintain structured knowledge bases for pattern recognition
- Generate both human-readable and machine-readable analysis outputs
- Create feedback loops that improve future PRP effectiveness
