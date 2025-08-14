# PRP Execution Commands for Cursor
# This script provides easy-to-use commands for PRP execution

# Import the PRP execution functions
. "$PSScriptRoot\execute-prp.ps1"
. "$PSScriptRoot\prp-status-tracker.ps1"

# Function to execute a specific PRP
function Execute-SpecificPRP {
    param([string]$PRPNumber, [string]$Phase = "01-foundation")
    
    Write-Host "Executing PRP $PRPNumber in phase $Phase..." -ForegroundColor Cyan
    
    # Call the main execution script
    & "$PSScriptRoot\execute-prp.ps1" -PRPNumber $PRPNumber -Phase $Phase
}

# Function to execute all PRPs in sequence
function Execute-AllPRPs {
    Write-Host "Executing all PRPs in sequence..." -ForegroundColor Cyan
    
    # Call the main execution script with ExecuteAll flag
    & "$PSScriptRoot\execute-prp.ps1" -ExecuteAll
}

# Function to execute a specific phase
function Execute-Phase {
    param([string]$Phase)
    
    Write-Host "Executing phase: $Phase" -ForegroundColor Cyan
    
    $phasePRPs = switch ($Phase) {
        "01-foundation" { @("01-core-entities-setup-prp.md", "02-base-repository-pattern-prp.md", "03-api-foundation-prp.md", "04-frontend-foundation-prp.md", "05-database-initialization-prp.md", "06-notification-system-comprehensive-prp.md", "07-authentication-system-prp.md", "08-frontend-auth-prp.md", "09-authentication-system-comprehensive-prp.md", "10-backend-services-prp.md", "11-frontend-components-prp.md", "12-backend-hierarchy-prp.md", "13-audit-logging-comprehensive-prp.md", "14-role-management-comprehensive-prp.md") }
        "02-infrastructure" { @("15-tree-management-comprehensive-prp.md", "16-media-management-comprehensive-prp.md") }
        "03-content" { @("17-question-management-comprehensive-prp.md", "18-question-review-prp.md", "19-question-creation-workflow-prp.md") }
        "04-assessment" { @("20-publish-exam-workflow-prp.md", "21-exam-creation-workflow-prp.md", "22-exam-creation-prp.md", "23-exam-publishing-prp.md", "25-student-exam-interface-prp.md", "26-exam-monitoring-prp.md") }
        "05-evaluation" { @("27-manual-grading-prp.md", "28-auto-grading-prp.md", "29-results-finalization-prp.md", "30-grading-workflow-prp.md") }
        "06-analytics" { @("31-analytics-dashboard-prp.md", "32-custom-reports-prp.md") }
        default { Write-Host "Unknown phase: $Phase" -ForegroundColor Red; return }
    }
    
    foreach ($prp in $phasePRPs) {
        $prpNumber = $prp -replace "-.*\.md$", ""
        Execute-SpecificPRP -PRPNumber $prpNumber -Phase $Phase
    }
}

# Function to show PRP status
function Show-PRPStatus {
    param([switch]$All, [switch]$Failed, [switch]$Pending)
    
    if ($All) {
        & "$PSScriptRoot\prp-status-tracker.ps1" -ShowAll
    } elseif ($Failed) {
        & "$PSScriptRoot\prp-status-tracker.ps1" -ShowFailed
    } elseif ($Pending) {
        & "$PSScriptRoot\prp-status-tracker.ps1" -ShowPending
    } else {
        & "$PSScriptRoot\prp-status-tracker.ps1"
    }
}

# Function to run validation only
function Test-Validation {
    Write-Host "Running validation tests..." -ForegroundColor Cyan
    & "$PSScriptRoot\execute-prp.ps1" -PRPNumber "validation" -ValidateOnly
}

# Functions are now available for direct use
# Execute-SpecificPRP, Execute-AllPRPs, Execute-Phase, Show-PRPStatus, Test-Validation

# Display available commands
Write-Host "=== Available PRP Commands ===" -ForegroundColor Green
Write-Host "Execute-SpecificPRP -PRPNumber '01' -Phase '01-foundation'" -ForegroundColor Yellow
Write-Host "Execute-AllPRPs" -ForegroundColor Yellow
Write-Host "Execute-Phase -Phase '01-foundation'" -ForegroundColor Yellow
Write-Host "Show-PRPStatus" -ForegroundColor Yellow
Write-Host "Show-PRPStatus -All" -ForegroundColor Yellow
Write-Host "Show-PRPStatus -Failed" -ForegroundColor Yellow
Write-Host "Show-PRPStatus -Pending" -ForegroundColor Yellow
Write-Host "Test-Validation" -ForegroundColor Yellow
Write-Host "" -ForegroundColor White
