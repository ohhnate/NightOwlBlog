@echo off
echo Cleaning the project...
dotnet clean

echo Dropping the database...
dotnet ef database drop --force

echo Removing migrations...
:remove_migrations
dotnet ef migrations remove
if %errorlevel% == 0 goto remove_migrations
echo All migrations removed.

echo Adding initial migration...
dotnet ef migrations add InitialCreate

echo Updating database...
dotnet ef database update

echo Rebuilding the project...
dotnet build

echo Database reset and project rebuild complete!
echo Run 'dotnet run' to start the application.
pause