# 1. Build to verify no errors
dotnet build src/TeamTrack.Api

# 2. If migrations folder exists, delete it
Remove-Item -Recurse -Force "src\TeamTrack.Api\Migrations" -ErrorAction SilentlyContinue

# 3. Create fresh migration
Add-Migration InitialCreate --project src/TeamTrack.Api

# 4. Run the app (it will auto-migrate and seed)
dotnet run --project src/TeamTrack.Api