# Update PRP Status based on actual execution results
# This script updates the status file to reflect what was actually executed

$ProjectRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$StatusFile = Join-Path $ProjectRoot ".cursor\prp-execution-status.json"

# Load current status
$StatusData = Get-Content $StatusFile -Raw | ConvertFrom-Json

# Update Foundation Phase (01-foundation) - All 14 PRPs completed
$foundationPRPs = @(
    "01-core-entities-setup-prp.md",
    "02-base-repository-pattern-prp.md", 
    "03-api-foundation-prp.md",
    "04-frontend-foundation-prp.md",
    "05-database-initialization-prp.md",
    "06-notification-system-comprehensive-prp.md",
    "07-authentication-system-prp.md",
    "08-frontend-auth-prp.md",
    "09-authentication-system-comprehensive-prp.md",
    "10-backend-services-prp.md",
    "11-frontend-components-prp.md",
    "12-backend-hierarchy-prp.md",
    "13-audit-logging-comprehensive-prp.md",
    "14-role-management-comprehensive-prp.md"
)

foreach ($prp in $foundationPRPs) {
    $StatusData.Phases."01-foundation".PRPs.$prp.Status = "Completed"
    $StatusData.Phases."01-foundation".PRPs.$prp.LastAttempt = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $StatusData.Phases."01-foundation".PRPs.$prp.Error = $null
}

# Update Infrastructure Phase (02-infrastructure) - All 2 PRPs completed
$infrastructurePRPs = @(
    "15-tree-management-comprehensive-prp.md",
    "16-media-management-comprehensive-prp.md"
)

foreach ($prp in $infrastructurePRPs) {
    $StatusData.Phases."02-infrastructure".PRPs.$prp.Status = "Completed"
    $StatusData.Phases."02-infrastructure".PRPs.$prp.LastAttempt = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $StatusData.Phases."02-infrastructure".PRPs.$prp.Error = $null
}

# Update Content Phase (03-content) - All 3 PRPs completed
$contentPRPs = @(
    "17-question-management-comprehensive-prp.md",
    "18-question-review-prp.md",
    "19-question-creation-workflow-prp.md"
)

foreach ($prp in $contentPRPs) {
    $StatusData.Phases."03-content".PRPs.$prp.Status = "Completed"
    $StatusData.Phases."03-content".PRPs.$prp.LastAttempt = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $StatusData.Phases."03-content".PRPs.$prp.Error = $null
}

# Update Assessment Phase (04-assessment) - 1 PRP completed, 5 pending
$StatusData.Phases."04-assessment".PRPs."20-publish-exam-workflow-prp.md".Status = "Completed"
$StatusData.Phases."04-assessment".PRPs."20-publish-exam-workflow-prp.md".LastAttempt = Get-Date -Format "yyyy-MM-dd HH:mm:ss"

# Update phase counts
foreach ($phaseKey in $StatusData.Phases.Keys) {
    $phase = $StatusData.Phases.$phaseKey
    $phase.Completed = ($phase.PRPs.Values | Where-Object { $_.Status -eq "Completed" }).Count
    $phase.Failed = ($phase.PRPs.Values | Where-Object { $_.Status -eq "Failed" }).Count
    $phase.Pending = ($phase.PRPs.Values | Where-Object { $_.Status -eq "Pending" }).Count
}

# Update overall counts
$StatusData.CompletedPRPs = ($StatusData.Phases.Values | ForEach-Object { $_.Completed } | Measure-Object -Sum).Sum
$StatusData.FailedPRPs = ($StatusData.Phases.Values | ForEach-Object { $_.Failed } | Measure-Object -Sum).Sum
$StatusData.PendingPRPs = ($StatusData.Phases.Values | ForEach-Object { $_.Pending } | Measure-Object -Sum).Sum
$StatusData.LastUpdated = Get-Date -Format "yyyy-MM-dd HH:mm:ss"

# Save updated status
$StatusData | ConvertTo-Json -Depth 10 | Set-Content $StatusFile

Write-Host "PRP Status updated successfully!" -ForegroundColor Green
Write-Host "Completed: $($StatusData.CompletedPRPs)/$($StatusData.TotalPRPs)" -ForegroundColor Green
Write-Host "Progress: $([math]::Round(($StatusData.CompletedPRPs / $StatusData.TotalPRPs) * 100, 1))%" -ForegroundColor Green
