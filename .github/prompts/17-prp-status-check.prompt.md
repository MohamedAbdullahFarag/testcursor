---
mode: agent
description: "Check PRP implementation status and update the status tracking file"
---

---
inputs:
  - name: prp_file_path
    description: The PRP file to check implementation status for
    required: true
  - name: codebase_path
    description: Path to the codebase to check against
    required: true
  - name: status_file
    description: Path to the status file to update, or where to create it
    required: false
---

# 17-prp-status-check.prompt.md

## Metadata
- **Type**: GitHub Copilot Custom Prompt
- **Mode**: agent
- **Purpose**: Verify PRP implementation status and update tracking documentation
- **Categories**: project-management, status-tracking, verification, quality-assurance
- **Complexity**: intermediate
- **Dependencies**: PRP files, source code, version control history

## Input
- **prp_file_path** (required): The PRP file to check implementation status for
- **codebase_path** (required): Path to the codebase to check against
- **status_file** (optional): Path to the status file to update, or where to create it
- **detailed_analysis** (optional): Whether to include detailed analysis in the status file (true/false)
- **check_tests** (optional): Whether to check for test coverage (true/false)

## Template

```
You are a project implementation auditor responsible for verifying the completion status of Product Requirement Prompts (PRPs) against the actual codebase. Your expertise helps teams track progress and ensure all requirements are implemented correctly.

## PRP Implementation Status Task

I need to check the implementation status of {prp_file_path} and update the corresponding status tracking file. Please analyze the current codebase at {codebase_path} and generate a comprehensive status report.

### Context
- PRP File: {prp_file_path}
- Codebase Path: {codebase_path}
- Status File: {status_file} (if provided)

### Analysis Goals
1. Extract all requirements and tasks from the PRP
2. Check implementation status of each task
3. Calculate overall completion percentage
4. Document current status with evidence
5. Identify any implementation gaps or issues

### PRP Status Check Process
1. First, parse the PRP file to extract all tasks and requirements
2. For each task:
   - Search for implementation evidence in the codebase
   - Determine completion status (complete/partial/missing)
   - Document evidence supporting the status assessment
3. Calculate overall completion metrics
4. Generate or update the status tracking file

### Output Expectations
- Comprehensive status report with task-by-task assessment
- Overall completion percentage and metrics
- Evidence supporting completion claims
- Identification of any implementation gaps
- Recommendations for addressing incomplete items
- If [prp_file_path] is not a relative path to existing file find the full path in the repository and set [prp_file_path] to the correct path.

Please be thorough and accurate in this assessment, as it will be used for project tracking and planning next steps.
```

## Expected Workflow

The agent should follow this workflow to generate a comprehensive status check:

1. **PRP Analysis**:
   - Parse the PRP file to extract requirements and tasks
   - Identify acceptance criteria for each task
   - Organize tasks into logical groups based on PRP structure

2. **Implementation Verification**:
   - For each task, search the codebase for evidence of implementation
   - Use semantic understanding to find code related to tasks even without direct keyword matches
   - Determine implementation completeness (complete, partial, missing)
   - Collect evidence (file paths, function names, etc.) supporting the status assessment

3. **Test Coverage Check (if requested)**:
   - Search for test files covering the implemented features
   - Assess test coverage completeness (thorough, partial, missing)

4. **Status Metrics Calculation**:
   - Calculate overall completion percentage
   - Break down completion by task categories
   - Generate timeline-based metrics if historical data is available

5. **Status File Generation/Update**:
   - Create or update the PRP status file using a consistent format
   - Include timestamp and author information
   - Document evidence for each status assessment
   - Highlight any gaps or issues requiring attention

## Response Format

The response should include:

```
# {PRP_NAME} Implementation Status

## Executive Summary
- **Status**: [Overall completion status]
- **Completion**: [X% complete]
- **Last Updated**: [Date]
- **Key Metrics**:
  - Tasks Completed: [X/Y]
  - Tests Covered: [Z%]
  - Open Issues: [Number]

## Implementation Status by Task

### [Task Category 1]
| Task | Status | Evidence | Notes |
|------|--------|----------|-------|
| [Task 1] | Complete/Partial/Missing | [File path/function] | [Notes on implementation] |
| [Task 2] | Complete/Partial/Missing | [File path/function] | [Notes on implementation] |

### [Task Category 2]
| Task | Status | Evidence | Notes |
|------|--------|----------|-------|
| [Task 3] | Complete/Partial/Missing | [File path/function] | [Notes on implementation] |
| [Task 4] | Complete/Partial/Missing | [File path/function] | [Notes on implementation] |

## Implementation Gaps
[Analysis of tasks with missing or partial implementation]

## Test Coverage
[Analysis of test coverage for implemented features]

## Recommendations
[Prioritized recommendations for addressing gaps]

## Next Steps
[Suggested actions to complete the implementation]
```

## Status File Management

The agent should handle status file management as follows:

1. **New Status File Creation**:
   - If no status file exists or path is not provided, generate a new file named `{prp_name}-status.md`
   - Use the standard format shown above

2. **Status File Updates**:
   - If a status file already exists, preserve its structure
   - Update the status assessments and evidence
   - Update the timestamp and completion metrics
   - Maintain history of previous status checks if available

3. **Status File Location**:
   - By default, create/update status files in a `status` subdirectory
   - If a specific path is provided, use that location

## Handling Edge Cases

The agent should handle these edge cases:

1. **Ambiguous Tasks**: Flag tasks that are ambiguously defined and difficult to verify
2. **Partial Implementations**: Clearly document partially implemented tasks with specific details
3. **Implementation Deviations**: Identify and document cases where implementation differs from PRP specifications
4. **Missing Dependencies**: Note cases where dependencies prevent full implementation

## Technical Implementation Notes

Effective PRP status checking requires:

1. **Pattern Matching**: Using both exact and fuzzy pattern matching for task terms
2. **Semantic Understanding**: Looking for semantic equivalents, not just direct matches
3. **Code Structure Analysis**: Understanding code structure to identify implementation points
4. **Version History Awareness**: Considering code history and changes over time

The agent should prioritize accuracy over speed, providing high-confidence status assessments with appropriate caveats for less certain evaluations.

## Validation Criteria

A good PRP status check:
1. Accounts for all tasks and requirements in the PRP
2. Provides specific evidence for completion claims
3. Calculates accurate completion percentages
4. Clearly identifies implementation gaps with actionable recommendations
5. Generates a well-formatted, easy-to-understand status file
