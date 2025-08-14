# PRP Status Tracker for Ikhtibar Project
# This script tracks the execution status of all PRPs

param(
    [Parameter(Mandatory=$false)]
    [switch]$ShowAll,
    
    [Parameter(Mandatory=$false)]
    [switch]$ShowFailed,
    
    [Parameter(Mandatory=$false)]
    [switch]$ShowPending,
    
    [Parameter(Mandatory=$false)]
    [switch]$UpdateStatus
)

# Configuration
$ProjectRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$PRPsPath = Join-Path $ProjectRoot ".github\PRPs"
$StatusFile = Join-Path $ProjectRoot ".cursor\prp-execution-status.json"

# Colors for output
$Green = "Green"
$Yellow = "Yellow"
$Red = "Red"
$Cyan = "Cyan"
$White = "White"

function Write-Status {
    param([string]$Message, [string]$Color = "White")
    Write-Host "[$(Get-Date -Format 'HH:mm:ss')] $Message" -ForegroundColor $Color
}

function Initialize-StatusFile {
    if (-not (Test-Path $StatusFile)) {
        $StatusData = @{
            LastUpdated = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
            TotalPRPs = 32
            CompletedPRPs = 0
            FailedPRPs = 0
            PendingPRPs = 32
            Phases = @{
                "01-foundation" = @{
                    Name = "Foundation Layer"
                    Total = 14
                    Completed = 0
                    Failed = 0
                    Pending = 14
                    PRPs = @{
                        "01-core-entities-setup-prp.md" = @{ Status = "Pending"; LastAttempt = $null; Error = $null }
                        "02-base-repository-pattern-prp.md" = @{ Status = "Pending"; LastAttempt = $null; Error = $null }
                        "03-api-foundation-prp.md" = @{ Status = "Pending"; LastAttempt = $null; Error = $null }
                        "04-frontend-foundation-prp.md" = @{ Status = "Pending"; LastAttempt = $null; Error = $null }
                        "05-database-initialization-prp.md" = @{ Status = "Pending"; LastAttempt = $null; Error = $null }
                        "06-notification-system-comprehensive-prp.md" = @{ Status = "Pending"; LastAttempt = $null; Error = $null }
                        "07-authentication-system-prp.md" = @{ Status = "Pending"; LastAttempt = $null; Error = $null }
                        "08-frontend-auth-prp.md" = @{ Status = "Pending"; LastAttempt = $null; Error = $null }
                        "09-authentication-system-comprehensive-prp.md" = @{ Status = "Pending"; LastAttempt = $null; Error = $null }
                        "10-backend-services-prp.md" = @{ Status = "Pending"; LastAttempt = $null; Error = $null }
                        "11-frontend-components-prp.md" = @{ Status = "Pending"; LastAttempt = $null; Error = $null }
                        "12-backend-hierarchy-prp.md" = @{ Status = "Pending"; LastAttempt = $null; Error = $null }
                        "13-audit-logging-comprehensive-prp.md" = @{ Status = "Pending"; LastAttempt = $null; Error = $null }
                        "14-role-management-comprehensive-prp.md" = @{ Status = "Pending"; LastAttempt = $null; Error = $null }
                    }
                }
                "02-infrastructure" = @{
                    Name = "Infrastructure Layer"
                    Total = 2
                    Completed = 0
                    Failed = 0
                    Pending = 2
                    PRPs = @{
                        "15-tree-management-comprehensive-prp.md" = @{ Status = "Pending"; LastAttempt = $null; Error = $null }
                        "16-media-management-comprehensive-prp.md" = @{ Status = "Pending"; LastAttempt = $null; Error = $null }
                    }
                }
                "03-content" = @{
                    Name = "Content Management Layer"
                    Total = 3
                    Completed = 0
                    Failed = 0
                    Pending = 3
                    PRPs = @{
                        "17-question-management-comprehensive-prp.md" = @{ Status = "Pending"; LastAttempt = $null; Error = $null }
                        "18-question-review-prp.md" = @{ Status = "Pending"; LastAttempt = $null; Error = $null }
                        "19-question-creation-workflow-prp.md" = @{ Status = "Pending"; LastAttempt = $null; Error = $null }
                    }
                }
                "04-assessment" = @{
                    Name = "Assessment Layer"
                    Total = 6
                    Completed = 0
                    Failed = 0
                    Pending = 6
                    PRPs = @{
                        "20-publish-exam-workflow-prp.md" = @{ Status = "Pending"; LastAttempt = $null; Error = $null }
                        "21-exam-creation-workflow-prp.md" = @{ Status = "Pending"; LastAttempt = $null; Error = $null }
                        "22-exam-creation-prp.md" = @{ Status = "Pending"; LastAttempt = $null; Error = $null }
                        "23-exam-publishing-prp.md" = @{ Status = "Pending"; LastAttempt = $null; Error = $null }
                        "25-student-exam-interface-prp.md" = @{ Status = "Pending"; LastAttempt = $null; Error = $null }
                        "26-exam-monitoring-prp.md" = @{ Status = "Pending"; LastAttempt = $null; Error = $null }
                    }
                }
                "05-evaluation" = @{
                    Name = "Evaluation Layer"
                    Total = 4
                    Completed = 0
                    Failed = 0
                    Pending = 4
                    PRPs = @{
                        "27-manual-grading-prp.md" = @{ Status = "Pending"; LastAttempt = $null; Error = $null }
                        "28-auto-grading-prp.md" = @{ Status = "Pending"; LastAttempt = $null; Error = $null }
                        "29-results-finalization-prp.md" = @{ Status = "Pending"; LastAttempt = $null; Error = $null }
                        "30-grading-workflow-prp.md" = @{ Status = "Pending"; LastAttempt = $null; Error = $null }
                    }
                }
                "06-analytics" = @{
                    Name = "Analytics Layer"
                    Total = 2
                    Completed = 0
                    Failed = 0
                    Pending = 2
                    PRPs = @{
                        "31-analytics-dashboard-prp.md" = @{ Status = "Pending"; LastAttempt = $null; Error = $null }
                        "32-custom-reports-prp.md" = @{ Status = "Pending"; LastAttempt = $null; Error = $null }
                    }
                }
            }
        }
        
        $StatusData | ConvertTo-Json -Depth 10 | Set-Content $StatusFile
        Write-Status "Status file initialized" $Green
    }
}

function Load-StatusData {
    if (Test-Path $StatusFile) {
        $content = Get-Content $StatusFile -Raw
        return $content | ConvertFrom-Json
    } else {
        Initialize-StatusFile
        return Load-StatusData
    }
}

function Save-StatusData {
    param($StatusData)
    $StatusData.LastUpdated = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $StatusData | ConvertTo-Json -Depth 10 | Set-Content $StatusFile
}

function Update-PRPStatus {
    param(
        [string]$Phase,
        [string]$PRPFile,
        [string]$Status,
        [string]$Error = $null
    )
    
    $StatusData = Load-StatusData
    
    if ($StatusData.Phases.$Phase.PRPs.$PRPFile) {
        $StatusData.Phases.$Phase.PRPs.$PRPFile.Status = $Status
        $StatusData.Phases.$Phase.PRPs.$PRPFile.LastAttempt = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
        
        if ($Error) {
            $StatusData.Phases.$Phase.PRPs.$PRPFile.Error = $Error
        }
        
        # Update phase counts
        $phase = $StatusData.Phases.$Phase
        $phase.Completed = ($phase.PRPs.Values | Where-Object { $_.Status -eq "Completed" }).Count
        $phase.Failed = ($phase.PRPs.Values | Where-Object { $_.Status -eq "Failed" }).Count
        $phase.Pending = ($phase.PRPs.Values | Where-Object { $_.Status -eq "Pending" }).Count
        
        # Update overall counts
        $StatusData.CompletedPRPs = ($StatusData.Phases.Values | ForEach-Object { $_.Completed } | Measure-Object -Sum).Sum
        $StatusData.FailedPRPs = ($StatusData.Phases.Values | ForEach-Object { $_.Failed } | Measure-Object -Sum).Sum
        $StatusData.PendingPRPs = ($StatusData.Phases.Values | ForEach-Object { $_.Pending } | Measure-Object -Sum).Sum
        
        Save-StatusData $StatusData
        Write-Status "Updated status for $PRPFile - $Status" $Green
    } else {
        Write-Status "ERROR: PRP $PRPFile not found in phase $Phase" $Red
    }
}

function Show-StatusSummary {
    $StatusData = Load-StatusData
    
    Write-Status "=== Ikhtibar PRP Execution Status ===" $Cyan
    Write-Status "Last Updated: $($StatusData.LastUpdated)" $White
    Write-Status "Overall Progress: $($StatusData.CompletedPRPs)/$($StatusData.TotalPRPs) PRPs completed" $White
    
    $progressPercent = [math]::Round(($StatusData.CompletedPRPs / $StatusData.TotalPRPs) * 100, 1)
    Write-Status "Progress: $progressPercent%" $Green
    
    if ($StatusData.FailedPRPs -gt 0) {
        Write-Status "Failed PRPs: $($StatusData.FailedPRPs)" $Red
    }
    
    Write-Status "" $White
    
    # Show phase-by-phase status
    foreach ($phaseKey in $StatusData.Phases.Keys | Sort-Object) {
        $phase = $StatusData.Phases.$phaseKey
        $phaseProgress = [math]::Round(($phase.Completed / $phase.Total) * 100, 1)
        
        Write-Status "$($phase.Name) ($phaseKey)" $Cyan
        Write-Status "  Progress: $($phase.Completed)/$($phase.Total) ($phaseProgress%)" $White
        
        if ($phase.Failed -gt 0) {
            Write-Status "  Failed: $($phase.Failed)" $Red
        }
        
        if ($phase.Pending -gt 0) {
            Write-Status "  Pending: $($phase.Pending)" $Yellow
        }
        
        Write-Status "" $White
    }
}

function Show-FailedPRPs {
    $StatusData = Load-StatusData
    
    Write-Status "=== Failed PRPs ===" $Red
    
    $failedPRPs = @()
    foreach ($phaseKey in $StatusData.Phases.Keys) {
        $phase = $StatusData.Phases.$phaseKey
        foreach ($prpKey in $phase.PRPs.Keys) {
            $prp = $phase.PRPs.$prpKey
            if ($prp.Status -eq "Failed") {
                $failedPRPs += @{
                    Phase = $phaseKey
                    PhaseName = $phase.Name
                    PRP = $prpKey
                    Error = $prp.Error
                    LastAttempt = $prp.LastAttempt
                }
            }
        }
    }
    
    if ($failedPRPs.Count -eq 0) {
        Write-Status "No failed PRPs found" $Green
        return
    }
    
    foreach ($failedPRP in $failedPRPs) {
        Write-Status "Phase: $($failedPRP.PhaseName) ($($failedPRP.Phase))" $Cyan
        Write-Status "PRP: $($failedPRP.PRP)" $Yellow
        Write-Status "Error: $($failedPRP.Error)" $Red
        Write-Status "Last Attempt: $($failedPRP.LastAttempt)" $White
        Write-Status "" $White
    }
}

function Show-PendingPRPs {
    $StatusData = Load-StatusData
    
    Write-Status "=== Pending PRPs ===" $Yellow
    
    $pendingPRPs = @()
    foreach ($phaseKey in $StatusData.Phases.Keys | Sort-Object) {
        $phase = $StatusData.Phases.$phaseKey
        foreach ($prpKey in $phase.PRPs.Keys | Sort-Object) {
            $prp = $phase.PRPs.$prpKey
            if ($prp.Status -eq "Pending") {
                $pendingPRPs += @{
                    Phase = $phaseKey
                    PhaseName = $phase.Name
                    PRP = $prpKey
                }
            }
        }
    }
    
    if ($pendingPRPs.Count -eq 0) {
        Write-Status "No pending PRPs found" $Green
        return
    }
    
    foreach ($pendingPRP in $pendingPRPs) {
        Write-Status "Phase: $($pendingPRP.PhaseName) ($($pendingPRP.Phase))" $Cyan
        Write-Status "PRP: $($pendingPRP.PRP)" $Yellow
        Write-Status "" $White
    }
}

function Show-AllPRPs {
    $StatusData = Load-StatusData
    
    Write-Status "=== All PRPs Status ===" $Cyan
    
    foreach ($phaseKey in $StatusData.Phases.Keys | Sort-Object) {
        $phase = $StatusData.Phases.$phaseKey
        Write-Status "$($phase.Name) ($phaseKey)" $Cyan
        
        foreach ($prpKey in $phase.PRPs.Keys | Sort-Object) {
            $prp = $phase.PRPs.$prpKey
            $statusColor = switch ($prp.Status) {
                "Completed" { $Green }
                "Failed" { $Red }
                "Pending" { $Yellow }
                default { $White }
            }
            
            Write-Status "  $prpKey - $($prp.Status)" $statusColor
            
            if ($prp.Status -eq "Failed" -and $prp.Error) {
                Write-Status "    Error: $($prp.Error)" $Red
            }
            
            if ($prp.LastAttempt) {
                Write-Status "    Last Attempt: $($prp.LastAttempt)" $White
            }
        }
        
        Write-Status "" $White
    }
}

# Main execution
function Main {
    Write-Status "=== Ikhtibar PRP Status Tracker ===" $Cyan
    
    # Initialize status file if it doesn't exist
    Initialize-StatusFile
    
    if ($ShowAll) {
        Show-AllPRPs
    } elseif ($ShowFailed) {
        Show-FailedPRPs
    } elseif ($ShowPending) {
        Show-PendingPRPs
    } else {
        Show-StatusSummary
    }
}

# Run main function
Main
