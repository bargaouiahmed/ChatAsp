# Quick Setup Guide

## First Time Setup (5 minutes)

### 1. Backend Setup

```powershell
# Navigate to backend
cd MyApp

# Copy configuration template
Copy-Item appsettings.Example.json appsettings.json

# Edit appsettings.json - update:
# - ConnectionStrings:Default (your PostgreSQL credentials)
# - AppSettings:Token (generate a secure 32+ char random string)

# Install EF tools (if not installed)
dotnet tool install --global dotnet-ef

# Create database and run migrations
dotnet ef database update

# Start backend
dotnet run
```

Backend will be running at `http://localhost:5000`

### 2. Frontend Setup

```powershell
# Navigate to frontend
cd frontend

# Copy environment template
Copy-Item .env.example .env

# Install dependencies
npm install

# Start development server
npm run dev
```

Frontend will be running at `http://localhost:5173`

### 3. Test the Application

1. Open browser to `http://localhost:5173`
2. Register a new account
3. Open another browser (or incognito window)
4. Register another account
5. Start chatting! 🎉

## Environment Variables Summary

### Backend (`MyApp/appsettings.json`)
```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=asp_chat_db;Username=YOUR_USER;Password=YOUR_PASSWORD"
  },
  "AppSettings": {
    "Token": "GENERATE-A-SECURE-RANDOM-STRING-AT-LEAST-32-CHARACTERS",
    "Issuer": "MyAppIssuer",
    "Audience": "MyAppAudience"
  },
  "Cors": {
    "AllowedOrigins": "http://localhost:5173"
  }
}
```

### Frontend (`frontend/.env`)
```env
VITE_API_BASE_URL=http://localhost:5000/api
VITE_SIGNALR_HUB_URL=http://localhost:5000/hub/chat
```

## Common Issues

### Port Already in Use
```powershell
# Backend
dotnet run --urls "http://localhost:5001"

# Frontend
npm run dev -- --port 5174
```

### Database Connection Failed
- Verify PostgreSQL is running: `pg_isready`
- Check credentials in `appsettings.json`
- Ensure database exists: `psql -U postgres -c "CREATE DATABASE asp_chat_db;"`

### CORS Error in Browser
- Verify `Cors:AllowedOrigins` in `appsettings.json` matches your frontend URL
- Clear browser cache and reload

## Production Deployment Checklist

### Backend
- [ ] Generate strong JWT secret (32+ characters)
- [ ] Update database connection string for production
- [ ] Set `ASPNETCORE_ENVIRONMENT=Production`
- [ ] Add production CORS origins
- [ ] Enable HTTPS
- [ ] Set up database backups

### Frontend
- [ ] Update `.env` with production API URLs (HTTPS)
- [ ] Run `npm run build`
- [ ] Test the `dist/` folder
- [ ] Deploy to hosting service (Vercel, Netlify, etc.)
- [ ] Verify HTTPS is working

## Useful Commands

### Backend
```powershell
# Development with hot reload
dotnet watch

# Production build
dotnet publish -c Release -o ./publish

# Create new migration
dotnet ef migrations add MigrationName

# Rollback migration
dotnet ef database update PreviousMigrationName

# Reset database (WARNING: deletes data)
dotnet ef database drop -f
dotnet ef database update
```

### Frontend
```powershell
# Development
npm run dev

# Production build
npm run build

# Preview production build
npm run preview

# Clean install
rm -rf node_modules package-lock.json
npm install
```

## Security Best Practices

1. **Never commit sensitive files:**
   - `appsettings.json` ✅ Already in `.gitignore`
   - `.env` ✅ Already in `.gitignore`

2. **Generate secure secrets:**
   ```powershell
   # PowerShell - Generate random 32-byte secret
   -join ((65..90) + (97..122) + (48..57) | Get-Random -Count 32 | % {[char]$_})
   ```

3. **Use environment variables in production:**
   - Azure App Service: Configure in Application Settings
   - Docker: Use `--env-file` or `-e` flags
   - AWS: Use Systems Manager Parameter Store

4. **Keep dependencies updated:**
   ```powershell
   # Backend
   dotnet list package --outdated
   
   # Frontend
   npm outdated
   ```

## Need Help?

- Check the main [README.md](README.md) for detailed documentation
- Review code comments in the source files
- Check SignalR logs in browser console (F12)
- Enable detailed logging in `appsettings.json`:
  ```json
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  }
  ```
