
#!/bin/bash
echo "Creating database migration..."
dotnet ef migrations add InitialCreate
echo "Updating database..."
dotnet ef database update
echo "Done!"
