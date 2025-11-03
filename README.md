# Real-Time Chat Application

A modern, real-time chat application built with ASP.NET Core, SignalR, React, and PostgreSQL. Features include real-time messaging, online status tracking, JWT authentication with automatic token refresh, and a beautiful dark mode UI.

## ✨ Features

- 🔐 **JWT Authentication** with automatic token refresh
- 💬 **Real-Time Messaging** using SignalR
- 🟢 **Online/Offline Status** tracking
- 👥 **User Discovery** with live registration notifications
- 🎨 **Modern Dark Mode UI** with gradient accents
- 📱 **Fully Responsive** - works on desktop, tablet, and mobile
- 🔄 **Auto-Reconnection** with exponential backoff
- 🎯 **Optimistic UI Updates** for better UX

## 🛠️ Tech Stack

### Backend
- **ASP.NET Core 9.0** - Web API framework
- **SignalR** - Real-time communication
- **Entity Framework Core** - ORM
- **PostgreSQL** - Database
- **JWT Bearer Authentication** - Security

### Frontend
- **React 18+** - UI library
- **Vite** - Build tool and dev server
- **Axios** - HTTP client with interceptors
- **@microsoft/signalr** - SignalR client
- **React Router** - Client-side routing

## 📋 Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/)
- [PostgreSQL 14+](https://www.postgresql.org/download/)

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone <your-repo-url>
cd ChatAsp
```

### 2. Backend Setup

#### Configure Environment

1. Navigate to the backend folder:
```bash
cd MyApp
```

2. Copy the example configuration:
```bash
cp appsettings.Example.json appsettings.json
```

3. Edit `appsettings.json` with your settings:
```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=asp_chat_db;Username=your_username;Password=your_password"
  },
  "AppSettings": {
    "Token": "your-super-secret-jwt-key-minimum-32-characters-long",
    "Issuer": "YourAppIssuer",
    "Audience": "YourAppAudience"
  },
  "Cors": {
    "AllowedOrigins": "http://localhost:5173"
  }
}
```

#### Create Database

1. Create a PostgreSQL database:
```bash
createdb asp_chat_db
```

2. Run migrations:
```bash
dotnet ef database update
```

Or if you don't have EF tools installed:
```bash
dotnet tool install --global dotnet-ef
dotnet ef database update
```

#### Run Backend

```bash
dotnet run
```

Backend will start on `http://localhost:5000`

### 3. Frontend Setup

#### Configure Environment

1. Navigate to the frontend folder:
```bash
cd frontend
```

2. Copy the example environment file:
```bash
cp .env.example .env
```

3. The `.env` file should contain:
```env
VITE_API_BASE_URL=http://localhost:5000/api
VITE_SIGNALR_HUB_URL=http://localhost:5000/hub/chat
```

#### Install Dependencies

```bash
npm install
```

#### Run Frontend

```bash
npm run dev
```

Frontend will start on `http://localhost:5173`

## 📁 Project Structure

```
ChatAsp/
├── MyApp/                          # Backend (ASP.NET Core)
│   ├── Controllers/
│   │   └── AuthController.cs       # Authentication endpoints
│   ├── Hubs/
│   │   └── ChatHub.cs              # SignalR hub for real-time communication
│   ├── Infrastructure/
│   │   └── PasswordHasher.cs       # Password hashing utilities
│   ├── Migrations/                 # EF Core migrations
│   ├── Models/
│   │   ├── AppDbContext.cs         # Database context
│   │   ├── AppUser.cs              # User entity
│   │   ├── AppRoom.cs              # Chat room entity
│   │   ├── AppMessage.cs           # Message entity
│   │   └── AppUserConnection.cs    # SignalR connection tracking
│   ├── Services/
│   │   ├── AuthService.cs          # Authentication logic
│   │   ├── UserService.cs          # User management
│   │   ├── RealtimeService.cs      # SignalR data access
│   │   └── DTOS/                   # Data transfer objects
│   ├── appsettings.Example.json    # Example configuration
│   └── Program.cs                  # Application entry point
│
└── frontend/                       # Frontend (React)
    ├── src/
    │   ├── components/
    │   │   ├── auth/               # Authentication components
    │   │   └── home/               # Main app components
    │   │       ├── Home.jsx        # Main layout
    │   │       ├── ChatModal.jsx   # Chat interface
    │   │       ├── ConversationsSidebar.jsx  # Active chats
    │   │       └── UserList.jsx    # User discovery
    │   ├── services/
    │   │   ├── AxiosProvider.js    # HTTP client with auth
    │   │   └── SignalRService.js   # SignalR client
    │   └── App.jsx                 # Root component
    ├── .env.example                # Example environment variables
    └── package.json
```

## 🔐 Security Features

### JWT Authentication
- Access tokens (short-lived, ~15min)
- Refresh tokens (long-lived, ~7 days)
- Automatic token refresh on 401 errors
- Secure password hashing with BCrypt

### SignalR Security
- JWT authentication for WebSocket connections
- User-scoped message delivery
- Connection tracking per user

## 🎨 UI/UX Features

### Dark Mode Theme
- Modern slate/navy color scheme
- Purple/pink gradient accents
- High contrast for readability
- Custom scrollbars

### Responsive Design
- **Desktop** (1200px+): Two-column layout with sidebar
- **Tablet** (768px-1199px): Stacked layout, compact header
- **Mobile** (< 768px): Single column, touch-optimized
- **Small Mobile** (< 480px): Ultra-compact, icon-only buttons

### Animations
- Smooth transitions and micro-interactions
- Slide-in modals and messages
- Fade effects for status changes
- Loading spinners

## 🔧 Configuration

### Backend Configuration (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "Default": "PostgreSQL connection string"
  },
  "AppSettings": {
    "Token": "JWT secret key (min 32 chars)",
    "Issuer": "JWT issuer",
    "Audience": "JWT audience"
  },
  "Cors": {
    "AllowedOrigins": "Comma-separated list of allowed origins"
  }
}
```

### Frontend Configuration (`.env`)

```env
VITE_API_BASE_URL=http://localhost:5000/api
VITE_SIGNALR_HUB_URL=http://localhost:5000/hub/chat
```

## 📡 API Endpoints

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login user
- `GET /api/auth/refresh-token?token=...` - Refresh access token

### Users
- `GET /api/user/users` - List all users (authenticated)

### Chat
- `GET /api/chat/messages/{roomId}` - Get message history
- `GET /api/chat/rooms/{userId}` - Get user's chat rooms

### SignalR Hub (`/hub/chat`)
- `CreateOrJoinChatRoom(int otherUserId)` - Start/join chat
- `SendMessage(int roomId, string messageContent)` - Send message
- Events: `ReceiveMessage`, `UserJoined`, `UserStatusChanged`, `UserRegistered`

## 🚢 Deployment

### Backend (Production)

1. Update `appsettings.json` with production database and secrets
2. Set `ASPNETCORE_ENVIRONMENT=Production`
3. Build the app:
```bash
dotnet publish -c Release
```

4. Run with Kestrel or deploy to Azure/AWS/Docker

### Frontend (Production)

1. Update `.env` with production API URL:
```env
VITE_API_BASE_URL=https://your-api.com/api
VITE_SIGNALR_HUB_URL=https://your-api.com/hub/chat
```

2. Build:
```bash
npm run build
```

3. Deploy the `dist/` folder to Vercel, Netlify, or any static host

## 🐛 Troubleshooting

### Hot Reload Issues
If you encounter `HotReloadException` errors:
```bash
# Stop dotnet watch and restart
dotnet run
```

### SignalR Connection Fails
- Verify JWT token is valid
- Check CORS configuration matches frontend URL
- Ensure WebSocket support is enabled

### Database Migration Errors
```bash
# Reset database (WARNING: deletes all data)
dotnet ef database drop
dotnet ef database update
```

## 📝 License

This project is open source and available under the [MIT License](LICENSE).

## 👤 Author

Built as a learning project to explore SignalR, real-time web applications, and modern React patterns.

## 🙏 Acknowledgments

- ASP.NET Core Team for SignalR
- React Team for the amazing framework
- Tailwind inspiration for the color scheme
