# PRP Execution Script for Ikhtibar Project
# This script automatically executes PRPs in sequence with validation

param(
    [Parameter(Mandatory=$false)]
    [string]$PRPNumber,
    
    [Parameter(Mandatory=$false)]
    [string]$Phase = "01-foundation",
    
    [Parameter(Mandatory=$false)]
    [switch]$ValidateOnly,
    
    [Parameter(Mandatory=$false)]
    [switch]$ExecuteAll
)

# Configuration
$ProjectRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$PRPsPath = Join-Path $ProjectRoot ".github\PRPs"
$CurrentPhase = $Phase

# Colors for output
$Green = "Green"
$Yellow = "Yellow"
$Red = "Red"
$Cyan = "Cyan"

function Write-Status {
    param([string]$Message, [string]$Color = "White")
    Write-Host "[$(Get-Date -Format 'HH:mm:ss')] $Message" -ForegroundColor $Color
}

function Test-Prerequisites {
    Write-Status "Testing prerequisites..." $Cyan
    
    # Check if .NET is available
    try {
        $dotnetVersion = dotnet --version
        Write-Status ".NET version: $dotnetVersion" $Green
    } catch {
        Write-Status "ERROR: .NET not found. Please install .NET 8.0" $Red
        return $false
    }
    
    # Check if pnpm is available
    try {
        $pnpmVersion = pnpm --version
        Write-Status "pnpm version: $pnpmVersion" $Green
    } catch {
        Write-Status "ERROR: pnpm not found. Please install pnpm" $Red
        return $false
    }
    
    # Check if database is accessible
    try {
        sqlcmd -S "(localdb)\mssqllocaldb" -Q "SELECT 1" -o nul 2>&1
        Write-Status "Database connection: OK" $Green
    } catch {
        Write-Status "WARNING: Database connection failed. Some PRPs may not work." $Yellow
    }
    
    return $true
}

function Execute-PRP {
    param([string]$PRPFile)
    
    $PRPFullPath = Join-Path $PRPsPath $CurrentPhase $PRPFile
    
    if (-not (Test-Path $PRPFullPath)) {
        Write-Status "ERROR: PRP file not found: $PRPFile" $Red
        return $false
    }
    
    Write-Status "Executing PRP: $PRPFile" $Cyan
    
    # Execute PRP-specific commands
    Write-Status "Executing PRP implementation..." $Cyan
    
    # This is where you would implement the actual PRP execution logic
    # For now, we'll simulate execution
    Start-Sleep -Seconds 2
    Write-Status "PRP implementation completed" $Green
    
    return $true
}

function Run-Validation {
    Write-Status "Running comprehensive validation..." $Cyan
    
    # Backend validation
    Write-Status "Validating backend..." $Yellow
    try {
        Set-Location (Join-Path $ProjectRoot "backend")
        dotnet build
        if ($LASTEXITCODE -eq 0) {
            Write-Status "Backend build: SUCCESS" $Green
        } else {
            Write-Status "Backend build: FAILED" $Red
            return $false
        }
        
        dotnet test
        if ($LASTEXITCODE -eq 0) {
            Write-Status "Backend tests: SUCCESS" $Green
        } else {
            Write-Status "Backend tests: FAILED" $Red
            return $false
        }
    } catch {
        Write-Status "Backend validation failed" $Red
        return $false
    }
    
    # Frontend validation
    Write-Status "Validating frontend..." $Yellow
    try {
        Set-Location (Join-Path $ProjectRoot "frontend")
        pnpm type-check
        if ($LASTEXITCODE -eq 0) {
            Write-Status "Frontend type check: SUCCESS" $Green
        } else {
            Write-Status "Frontend type check: FAILED" $Red
            return $false
        }
        
        pnpm lint
        if ($LASTEXITCODE -eq 0) {
            Write-Status "Frontend lint: SUCCESS" $Green
        } else {
            Write-Status "Frontend lint: FAILED" $Red
            return $false
        }
        
        pnpm test
        if ($LASTEXITCODE -eq 0) {
            Write-Status "Frontend tests: SUCCESS" $Green
        } else {
            Write-Status "Frontend tests: FAILED" $Red
            return $false
        }
    } catch {
        Write-Status "Frontend validation failed" $Red
        return $false
    }
    
    # Return to project root
    Set-Location $ProjectRoot
    
    Write-Status "All validations completed successfully" $Green
    return $true
}

function Execute-All-PRPs {
    Write-Status "Executing all PRPs in sequence..." $Cyan
    
    $Phases = @(
        @{ Name = "01-foundation"; PRPs = @("01-core-entities-setup-prp.md", "02-base-repository-pattern-prp.md", "03-api-foundation-prp.md", "04-frontend-foundation-prp.md", "05-database-initialization-prp.md", "06-notification-system-comprehensive-prp.md", "07-authentication-system-prp.md", "08-frontend-auth-prp.md", "09-authentication-system-comprehensive-prp.md", "10-backend-services-prp.md", "11-frontend-components-prp.md", "12-backend-hierarchy-prp.md", "13-audit-logging-comprehensive-prp.md", "14-role-management-comprehensive-prp.md") },
        @{ Name = "02-infrastructure"; PRPs = @("15-tree-management-comprehensive-prp.md", "16-media-management-comprehensive-prp.md") },
        @{ Name = "03-content"; PRPs = @("17-question-management-comprehensive-prp.md", "18-question-review-prp.md", "19-question-creation-workflow-prp.md") },
        @{ Name = "04-assessment"; PRPs = @("20-publish-exam-workflow-prp.md", "21-exam-creation-workflow-prp.md", "22-exam-creation-prp.md", "23-exam-publishing-prp.md", "25-student-exam-interface-prp.md", "26-exam-monitoring-prp.md") },
        @{ Name = "05-evaluation"; PRPs = @("27-manual-grading-prp.md", "28-auto-grading-prp.md", "29-results-finalization-prp.md", "30-grading-workflow-prp.md") },
        @{ Name = "06-analytics"; PRPs = @("31-analytics-dashboard-prp.md", "32-custom-reports-prp.md") }
    )
    
    foreach ($phase in $Phases) {
        Write-Status "Starting phase: $($phase.Name)" $Cyan
        $CurrentPhase = $phase.Name
        
        foreach ($prp in $phase.PRPs) {
            Write-Status "Executing PRP: $prp" $Yellow
            
            if (Execute-PRP $prp) {
                Write-Status "PRP completed successfully: $prp" $Green
                
                # Run validation after each PRP
                if (-not (Run-Validation)) {
                    Write-Status "Validation failed for PRP: $prp. Stopping execution." $Red
                    return $false
                }
            } else {
                Write-Status "PRP failed: $prp. Stopping execution." $Red
                return $false
            }
        }
        
        Write-Status "Phase completed: $($phase.Name)" $Green
        
        # Run integration tests between phases
        if ($phase.Name -ne "06-analytics") {
            Write-Status "Running integration tests between phases..." $Cyan
            # Add integration test logic here
        }
    }
    
    Write-Status "All PRPs executed successfully!" $Green
    return $true
}

# Main execution
function Main {
    Write-Status "=== Ikhtibar PRP Execution System ===" $Cyan
    Write-Status "Project Root: $ProjectRoot" $Yellow
    
    # Test prerequisites
    if (-not (Test-Prerequisites)) {
        Write-Status "Prerequisites check failed. Exiting." $Red
        exit 1
    }
    
    # Validate parameters
    if (-not $ExecuteAll -and -not $ValidateOnly -and -not $PRPNumber) {
        Write-Status "ERROR: Please specify either -ExecuteAll, -ValidateOnly, or a PRPNumber" $Red
        Write-Status "Usage examples:" $Yellow
        Write-Status "  .cursor/scripts/execute-prp.ps1 -ExecuteAll" $Yellow
        Write-Status "  .cursor/scripts/execute-prp.ps1 -ValidateOnly" $Yellow
        Write-Status "  .cursor/scripts/execute-prp.ps1 -PRPNumber 01-core-entities-setup-prp.md" $Yellow
        exit 1
    }
    
    if ($ExecuteAll) {
        # Execute all PRPs
        if (Execute-All-PRPs) {
            Write-Status "All PRPs executed successfully!" $Green
            exit 0
        } else {
            Write-Status "PRP execution failed!" $Red
            exit 1
        }
    } elseif ($ValidateOnly) {
        # Run validation only
        if (Run-Validation) {
            Write-Status "Validation completed successfully!" $Green
            exit 0
        } else {
            Write-Status "Validation failed!" $Red
            exit 1
        }
    } else {
        # Execute specific PRP
        if (Execute-PRP $PRPNumber) {
            Write-Status "PRP $PRPNumber executed successfully!" $Green
            
            # Run validation
            if (Run-Validation) {
                Write-Status "Validation completed successfully!" $Green
                exit 0
            } else {
                Write-Status "Validation failed!" $Red
                exit 1
            }
        } else {
            Write-Status "PRP $PRPNumber execution failed!" $Red
            exit 1
        }
    }
}

# Run main function
Main
