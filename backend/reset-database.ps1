# PowerShell script to reset the Ikhtibar database to match schema.sql and data.sql

$connectionString = "Server=(localdb)\mssqllocaldb;Database=master;Trusted_Connection=true;"
$originalDatabaseName = "Ekhtibar"  # Database name used in schema.sql
$appDatabaseName = "IkhtibarDb_Development"  # Database name used by application

Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "Database Reset Process Starting..." -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan

try {
    # Connect to master database
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    
    # Drop both databases if they exist
    Write-Host "Dropping existing databases..." -ForegroundColor Yellow
    
    $databases = @($originalDatabaseName, $appDatabaseName)
    foreach ($dbName in $databases) {
        $checkQuery = "IF EXISTS (SELECT 1 FROM sys.databases WHERE name = '$dbName') BEGIN ALTER DATABASE [$dbName] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [$dbName]; END"
        $command = New-Object System.Data.SqlClient.SqlCommand($checkQuery, $connection)
        $command.ExecuteNonQuery()
        Write-Host "  ✓ Dropped database: $dbName" -ForegroundColor Green
    }
    
    # Create the original database name for schema.sql
    Write-Host "Creating database: $originalDatabaseName" -ForegroundColor Yellow
    $createQuery = "CREATE DATABASE [$originalDatabaseName]"
    $command = New-Object System.Data.SqlClient.SqlCommand($createQuery, $connection)
    $command.ExecuteNonQuery()
    Write-Host "  ✓ Database $originalDatabaseName created successfully." -ForegroundColor Green
    
    $connection.Close()
    
    # Apply database schema to original database
    Write-Host "Applying schema from schema.sql..." -ForegroundColor Yellow
    $schemaFile = Join-Path $PSScriptRoot "..\.github\requirements\schema.sql"
    
    if (-not (Test-Path $schemaFile)) {
        throw "Schema file not found: $schemaFile"
    }
    
    $schemaResult = sqlcmd -S "(localdb)\mssqllocaldb" -d $originalDatabaseName -i $schemaFile -h -1 -W
    if ($LASTEXITCODE -ne 0) {
        throw "Schema application failed with exit code: $LASTEXITCODE"
    }
    Write-Host "  ✓ Schema applied successfully." -ForegroundColor Green
    
    # Seed initial data
    Write-Host "Seeding data from data.sql..." -ForegroundColor Yellow
    $dataFile = Join-Path $PSScriptRoot "..\.github\requirements\data.sql"
    
    if (-not (Test-Path $dataFile)) {
        throw "Data file not found: $dataFile"
    }
    
    $dataResult = sqlcmd -S "(localdb)\mssqllocaldb" -d $originalDatabaseName -i $dataFile -h -1 -W
    if ($LASTEXITCODE -ne 0) {
        throw "Data seeding failed with exit code: $LASTEXITCODE"
    }
    Write-Host "  ✓ Data seeded successfully." -ForegroundColor Green
    
    # Copy the database to the application database name
    Write-Host "Creating application database: $appDatabaseName" -ForegroundColor Yellow
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    
    # Use SQL Server BACKUP and RESTORE to copy the database
    $backupPath = [System.IO.Path]::GetTempPath() + "Ekhtibar_Backup.bak"
    
    # Backup original database
    $backupQuery = "BACKUP DATABASE [$originalDatabaseName] TO DISK = '$backupPath' WITH FORMAT, INIT"
    $command = New-Object System.Data.SqlClient.SqlCommand($backupQuery, $connection)
    $command.CommandTimeout = 300  # 5 minutes
    $command.ExecuteNonQuery()
    
    # Restore to application database name
    $restoreQuery = @"
RESTORE DATABASE [$appDatabaseName] FROM DISK = '$backupPath' 
WITH REPLACE,
MOVE '$originalDatabaseName' TO '$(Split-Path (sqlcmd -S "(localdb)\mssqllocaldb" -Q "SELECT physical_name FROM sys.master_files WHERE database_id = DB_ID('$originalDatabaseName') AND type = 0" -h -1 -W).Trim())\..\$appDatabaseName.mdf',
MOVE '$($originalDatabaseName)_Log' TO '$(Split-Path (sqlcmd -S "(localdb)\mssqllocaldb" -Q "SELECT physical_name FROM sys.master_files WHERE database_id = DB_ID('$originalDatabaseName') AND type = 1" -h -1 -W).Trim())\..\$($appDatabaseName)_Log.ldf'
"@
    
    # Simplified restore approach
    $simpleRestoreQuery = "RESTORE DATABASE [$appDatabaseName] FROM DISK = '$backupPath' WITH REPLACE"
    $command = New-Object System.Data.SqlClient.SqlCommand($simpleRestoreQuery, $connection)
    $command.CommandTimeout = 300  # 5 minutes
    $command.ExecuteNonQuery()
    
    Write-Host "  ✓ Application database created successfully." -ForegroundColor Green
    
    # Clean up backup file
    if (Test-Path $backupPath) {
        Remove-Item $backupPath -Force
    }
    
    $connection.Close()
    
    Write-Host "=============================================" -ForegroundColor Green
    Write-Host "Database Reset Completed Successfully!" -ForegroundColor Green
    Write-Host "=============================================" -ForegroundColor Green
    Write-Host "Databases created:" -ForegroundColor White
    Write-Host "  • $originalDatabaseName (schema source)" -ForegroundColor Gray
    Write-Host "  • $appDatabaseName (application database)" -ForegroundColor Gray
    
    # Verify the reset by checking table count
    Write-Host "`nVerifying database structure..." -ForegroundColor Yellow
    $verifyQuery = "SELECT COUNT(*) as TableCount FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'"
    $tableCount = sqlcmd -S "(localdb)\mssqllocaldb" -d $appDatabaseName -Q $verifyQuery -h -1 -W
    Write-Host "  ✓ Found $($tableCount.Trim()) tables in $appDatabaseName" -ForegroundColor Green
    
}
catch {
    Write-Host "=============================================" -ForegroundColor Red
    Write-Host "Database Reset Failed!" -ForegroundColor Red
    Write-Host "=============================================" -ForegroundColor Red
    Write-Error "Error: $($_.Exception.Message)"
    exit 1
}
finally {
    if ($connection -and $connection.State -eq 'Open') {
        $connection.Close()
    }
}
