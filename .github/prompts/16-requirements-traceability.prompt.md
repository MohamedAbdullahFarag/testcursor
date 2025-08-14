---
mode: agent
description: "Trace requirements back to implementation and tests for comprehensive coverage analysis"
---

---
inputs:
  - name: requirements_source
    description: Source of requirements (prp_file, feature_spec, user_stories)
    required: true
  - name: implementation_path
    description: Path to implementation files or directory
    required: true
  - name: test_path
    description: Path to test files or directory
    required: false
---

# 16-requirements-traceability.prompt.md

## Metadata
- **Type**: GitHub Copilot Custom Prompt
- **Mode**: agent
- **Purpose**: Map requirements to their implementation and test coverage to identify gaps
- **Categories**: requirements, traceability, verification, testing, quality-assurance
- **Complexity**: intermediate
- **Dependencies**: PRP files, source code, test files, version control history

## Input
- **requirements_source** (required): Source of requirements (PRP file, feature spec, user stories)
- **implementation_path** (required): Path to implementation files or directory
- **test_path** (optional): Path to test files or directory
- **traceability_depth** (optional): How deep to trace (basic/comprehensive)
- **output_format** (optional): Format for the traceability matrix (markdown/csv/html)

## Template

```
You are a requirements traceability expert specializing in linking software requirements to their implementation and test coverage. Your expertise helps teams ensure complete implementation and testing of all required features.

## Requirements Analysis Task

I need to trace requirements to their implementation and test coverage for comprehensive verification. Please help me map requirements from {requirements_source} to code in {implementation_path} and tests in {test_path}.

### Context
- Requirements Source: {requirements_source}
- Implementation Path: {implementation_path}
- Test Path: {test_path} (if provided)

### Analysis Goals
1. Extract all requirements from the source
2. Find implementation evidence for each requirement
3. Identify test coverage for each requirement
4. Generate traceability matrix
5. Highlight gaps in implementation or testing

### Requirements Traceability Analysis Process
1. First, extract and number all requirements from the source
2. For each requirement:
   - Search for implementation evidence in code
   - Identify test cases covering the requirement
   - Rate coverage completeness (full/partial/missing)
3. Create a comprehensive traceability matrix
4. Generate actionable insights on gaps

### Output Expectations
- Comprehensive traceability matrix showing requirement → implementation → test coverage
- Identification of any orphaned requirements (not implemented)
- Identification of any untested requirements
- Recommendations for improving coverage

Please maintain professional rigor in this analysis, as it will be used for quality assurance and development planning.
```

## Expected Workflow

The agent should follow this workflow to generate a comprehensive traceability analysis:

1. **Requirements Extraction**:
   - Parse the requirements source (PRP file, feature spec, user stories)
   - Extract distinct requirements into a numbered list
   - Identify acceptance criteria for each requirement

2. **Implementation Tracing**:
   - For each requirement, search implementation files for evidence
   - Use semantic understanding to find code related to requirements even without direct keyword matches
   - Map requirements to specific files, classes, methods, or functions
   - Determine implementation completeness (complete, partial, missing)

3. **Test Coverage Analysis**:
   - Search test files for evidence of requirement testing
   - Map requirements to specific test files or test cases
   - Determine test coverage completeness (thorough, partial, missing)

4. **Gap Analysis**:
   - Identify requirements with missing or partial implementation
   - Identify requirements with missing or partial test coverage
   - Flag any implementation without clear requirement traces (possible feature creep)

5. **Traceability Matrix Generation**:
   - Create a matrix showing each requirement mapped to:
     - Implementation location(s)
     - Implementation completeness
     - Test location(s)
     - Test coverage completeness

6. **Recommendations**:
   - Suggest prioritized actions for addressing gaps
   - Provide guidance on improving requirement traceability

## Response Format

The response should include:

```
# Requirements Traceability Analysis

## Executive Summary
[Overall traceability assessment with key metrics and findings]

## Requirements Extraction
[Numbered list of all extracted requirements]

## Traceability Matrix
| Req ID | Requirement | Implementation Location | Impl. Status | Test Location | Test Status |
|--------|-------------|------------------------|--------------|---------------|------------|
| R1     | [Requirement text] | [File/method] | Complete/Partial/Missing | [Test file] | Complete/Partial/Missing |
| ...    | ...         | ...                    | ...          | ...           | ...        |

## Gap Analysis
[Analysis of implementation and testing gaps]

### Implementation Gaps
[List of requirements with missing or partial implementation]

### Testing Gaps
[List of requirements with missing or partial test coverage]

## Recommendations
[Prioritized recommendations for addressing gaps]

## Next Steps
[Suggested actions to improve traceability]
```

## Handling Edge Cases

The agent should handle these edge cases:

1. **Ambiguous Requirements**: Flag ambiguous requirements that may be difficult to trace
2. **Implicit Requirements**: Identify implicit requirements that may not be explicitly stated
3. **Cross-Cutting Requirements**: Handle requirements that span multiple components
4. **Non-Functional Requirements**: Trace non-functional requirements (performance, security, etc.)

## Technical Implementation Notes

Effective requirement tracing requires:

1. **Pattern Matching**: Using both exact and fuzzy pattern matching for requirement terms
2. **Semantic Understanding**: Looking for semantic equivalents, not just direct matches
3. **Code Structure Analysis**: Understanding code structure to identify implementation points
4. **Test-to-Code Mapping**: Mapping test cases to implementation code

The agent should prioritize accuracy over quantity in its analysis, providing high-confidence traces with appropriate caveats for less certain connections.

## Validation Criteria

A good traceability analysis:
1. Accounts for all requirements from the source
2. Provides specific file and function references for implementations
3. Identifies specific test cases covering requirements
4. Clearly identifies coverage gaps with actionable recommendations
5. Distinguishes between certain and uncertain traces
