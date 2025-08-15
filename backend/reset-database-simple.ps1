# Simple Database Reset Script for Ikhtibar
# Resets both Ekhtibar and IkhtibarDb_Development databases

param(
    [string]$ServerInstance = "(localdb)\mssqllocaldb",
    [string]$SchemaFile = "..\\.cursor\\requirements\\schema.sql",
    [string]$DataFile = "..\\.cursor\\requirements\\data.sql"
)

# Color output functions
function Write-Success($Message) { Write-Host "  ✓ $Message" -ForegroundColor Green }
function Write-Error($Message) { Write-Host "  ❌ $Message" -ForegroundColor Red }
function Write-Info($Message) { Write-Host "$Message" -ForegroundColor Cyan }

Write-Host "=============================================" -ForegroundColor Blue
Write-Host "Database Reset Process Starting..." -ForegroundColor Blue
Write-Host "=============================================" -ForegroundColor Blue

# Check if schema and data files exist
if (-not (Test-Path $SchemaFile)) {
    Write-Error "Schema file not found: $SchemaFile"
    exit 1
}

if (-not (Test-Path $DataFile)) {
    Write-Error "Data file not found: $DataFile"
    exit 1
}

try {
    # Step 1: Drop existing databases
    Write-Info "Dropping existing databases..."
    
    sqlcmd -S $ServerInstance -Q "DROP DATABASE IF EXISTS [Ekhtibar]" -v ON
    Write-Success "Dropped database: Ekhtibar"
    
    sqlcmd -S $ServerInstance -Q "DROP DATABASE IF EXISTS [IkhtibarDb_Development]" -v ON
    Write-Success "Dropped database: IkhtibarDb_Development"

    # Step 2: Create and populate Ekhtibar database
    Write-Info "Creating database: Ekhtibar"
    sqlcmd -S $ServerInstance -Q "CREATE DATABASE [Ekhtibar]" -v ON
    Write-Success "Database Ekhtibar created successfully."

    Write-Info "Applying schema from schema.sql..."
    sqlcmd -S $ServerInstance -i $SchemaFile -v ON
    Write-Success "Schema applied successfully."

    Write-Info "Seeding data from data.sql..."
    sqlcmd -S $ServerInstance -i $DataFile -v ON
    Write-Success "Data seeded successfully."

    # Step 3: Create application database (IkhtibarDb_Development)
    Write-Info "Creating application database: IkhtibarDb_Development"
    sqlcmd -S $ServerInstance -Q "CREATE DATABASE [IkhtibarDb_Development]" -v ON
    Write-Success "Database IkhtibarDb_Development created successfully."
    
    # Step 4: Apply schema to application database
    Write-Info "Applying schema to IkhtibarDb_Development..."
    
    $schemaContent = Get-Content $SchemaFile -Raw
    $appSchemaContent = $schemaContent -replace "USE Ekhtibar;", "USE IkhtibarDb_Development;"
    $tempSchemaFile = "temp_app_schema.sql"
    $appSchemaContent | Out-File -FilePath $tempSchemaFile -Encoding UTF8
    
    sqlcmd -S $ServerInstance -i $tempSchemaFile -v ON
    Write-Success "Schema applied to IkhtibarDb_Development successfully."
    Remove-Item $tempSchemaFile -Force
    
    # Step 5: Seed data in application database
    Write-Info "Seeding data in IkhtibarDb_Development..."
    
    $dataContent = Get-Content $DataFile -Raw
    $appDataContent = $dataContent -replace "USE \[Ekhtibar\];", "USE [IkhtibarDb_Development];"
    $tempDataFile = "temp_app_data.sql"
    $appDataContent | Out-File -FilePath $tempDataFile -Encoding UTF8
    
    sqlcmd -S $ServerInstance -i $tempDataFile -v ON
    Write-Success "Data seeded in IkhtibarDb_Development successfully."
    Remove-Item $tempDataFile -Force

    # Step 6: Verify databases
    Write-Info "Verifying database creation..."
    
    $verifyQuery = @"
SELECT 
    name as DatabaseName,
    database_id as ID,
    create_date as Created
FROM sys.databases 
WHERE name IN ('Ekhtibar', 'IkhtibarDb_Development')
ORDER BY name
"@
    
    sqlcmd -S $ServerInstance -Q $verifyQuery -v ON
    Write-Success "Database verification completed."

    Write-Host "=============================================" -ForegroundColor Green
    Write-Host "Database Reset Completed Successfully!" -ForegroundColor Green
    Write-Host "=============================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "✓ Ekhtibar database: Schema and data applied" -ForegroundColor Green
    Write-Host "✓ IkhtibarDb_Development database: Schema and data applied" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Cyan
    Write-Host "1. Start backend: dotnet run --project Ikhtibar.API" -ForegroundColor White
    Write-Host "2. Test connection: https://localhost:5001/api/health/ping" -ForegroundColor White

} catch {
    Write-Host "=============================================" -ForegroundColor Red
    Write-Host "Database Reset Failed!" -ForegroundColor Red
    Write-Host "=============================================" -ForegroundColor Red
    Write-Error "Error: $($_.Exception.Message)"
    exit 1
}
