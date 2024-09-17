@echo off
echo Optimized database reset process starting...

echo Dropping the database...
dotnet ef database drop -f --context ApplicationDbContext

echo Removing migrations...
if exist "Data\Migrations" (
    rd /s /q "Data\Migrations"
    echo Migrations folder removed.
) else (
    echo Migrations folder not found.
)

echo Creating single migration...
dotnet ef migrations add ResetMigration --context ApplicationDbContext -o Data/Migrations

echo Applying migration to the database...
dotnet ef database update --context ApplicationDbContext

echo Database reset and migration applied successfully!
echo Run 'dotnet run' to start the application.
pause