# 🚀 TaskPulse - Team Task Management System

![.NET 9.0](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![React](https://img.shields.io/badge/React-18.3-61DAFB?style=for-the-badge&logo=react&logoColor=black)
![Vite](https://img.shields.io/badge/Vite-5.4-646CFF?style=for-the-badge&logo=vite&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC292B?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Enabled-2496ED?style=for-the-badge&logo=docker&logoColor=white)

A full-stack, production-grade **Team Task Management System** built with **ASP.NET Core 9.0 Web API**, **Entity Framework Core**, **SQL Server**, and a modern, fully responsive **React (Vite)** frontend.

---

## 🌟 Key Features

- **Role-Based Access Control (RBAC)**:
  - 👑 **Admin**: Complete system administration—manage users, assign system roles, create/delete teams, view global analytics, and delete accounts.
  - 👔 **Manager**: Create teams, manage team memberships, create and assign tasks, and monitor team performance.
  - 👤 **User**: Access assigned tasks, update task progress (To Do → In Progress → Done), comment on tasks, and track personal deadlines.
- **🔐 JWT Authentication**: Token-based authentication with ASP.NET Core Identity & password hashing.
- **📊 Dynamic Analytics Dashboard**: Visual metric cards (Total, ToDo, InProgress, Done, Overdue, High Priority), progress bars, and recent task feeds.
- **📌 Task Management & Kanban Board**: Priority indicators (Low, Medium, High), status filters, due date cutoffs, and responsive grid layouts.
- **👥 Team Collaboration**: Create teams, add/remove team members, and scope task visibility by team assignments.
- **💬 Task Discussion & Comments**: Interactive commenting on individual tasks for team discussions.
- **🔔 Real-Time Notifications**: In-app notifications for task assignments and status updates.
- **🌓 Light & Dark Mode Support**: Theme switcher with persistent user preference (Light theme default).
- **📱 Fully Responsive Design**: Mobile-optimized layouts with responsive drawer navigation and stacked filters.
- **📖 Interactive Swagger API Docs**: Comprehensive OpenAPI documentation available at `http://localhost:5000/swagger`.
- **🐳 Dockerized Architecture**: `docker-compose` setup orchestrating API, React frontend, and SQL Server database.

---

## 👥 User Roles & Permissions

The application supports 3 pre-configured system roles:

| Role | System Permissions |
| :--- | :--- |
| 👑 **Admin** | Full administrative system control, user management, role assignments, and account deletion |
| 👔 **Manager** | Team creation, member management, task creation, assignment, and team scoping |
| 👤 **User** | Access to assigned tasks, status updates (ToDo → InProgress → Done), and task comments |

---

## 🛠️ Technology Stack

### **Backend (.NET Web API)**
- **Framework**: ASP.NET Core 9.0 Web API
- **ORM**: Entity Framework Core 9.0 (Code-First)
- **Database**: Microsoft SQL Server / LocalDB
- **Security**: ASP.NET Core Identity, JWT Bearer Authentication, Password Hashing
- **Documentation**: Swashbuckle Swagger UI (OpenAPI 3.0) with XML documentation
- **Testing**: xUnit, Moq, EF Core InMemory Database

### **Frontend (React SPA)**
- **Framework**: React 18 (Vite Bundler)
- **Routing**: React Router DOM v6
- **Icons**: Lucide React Icons
- **HTTP Client**: Axios with JWT Interceptors
- **Styling**: Vanilla CSS (Custom CSS Variables, Glassmorphism, Responsive Media Queries)

### **DevOps & Infrastructure**
- **Containerization**: Docker & Docker Compose
- **CI/CD Pipeline**: GitHub Actions (`.github/workflows/ci.yml`)

---

## 🚀 How to Run Locally

### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 18+](https://nodejs.org/)
- Microsoft SQL Server or LocalDB (Installed with Visual Studio)

---

### Step 1: Run Backend (.NET Web API)

1. Open terminal and navigate to the backend API directory:
   ```bash
   cd backend/TaskManagement.API
   ```

2. Restore dependencies and run the backend server:
   ```bash
   dotnet run --urls="http://localhost:5000"
   ```
   - API Server running at: **`http://localhost:5000`**
   - Swagger API Documentation: **`http://localhost:5000/swagger`**

> **Note**: Database tables and seeded demo users (`admin@taskmanager.com`, etc.) are automatically created on initial backend startup.

---

### Step 2: Run Frontend (React + Vite)

1. Open a new terminal tab and navigate to the frontend directory:
   ```bash
   cd frontend
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Start the Vite development server:
   ```bash
   npm run dev
   ```
   - Web Application running at: **`http://localhost:3000`**

---

### Step 3: Run Unit Tests

To run automated unit tests for service business logic:
```bash
cd backend/TaskManagement.Tests
dotnet test
```

---

## 🐳 Run with Docker Compose

Run the entire multi-container architecture (Database, Backend API, and Frontend React app) with a single command:

```bash
docker-compose up --build
```

### Container Endpoints:
- **Frontend App**: `http://localhost:3000`
- **Backend API & Swagger**: `http://localhost:5000/swagger`

---

## 🌐 How to Deploy for a Live Swagger URL

To get a public, live URL for your Swagger API documentation (e.g. `https://your-api-name.onrender.com/swagger`), follow these deployment steps using free cloud hosting platforms:

### Option A: Deploy on Render (Free & Recommended)
1. Push your latest code to your GitHub repository ([`Task_Management_System_Project`](https://github.com/Shagufta-Perween/Task_Management_System_Project)).
2. Sign up / Log in to [Render.com](https://render.com) using your GitHub account.
3. Click **New +** → select **Web Service**.
4. Connect your repository: `Shagufta-Perween/Task_Management_System_Project`.
5. Select **Docker** as the Runtime (it will automatically detect `backend/TaskManagement.API/Dockerfile`).
6. Set Environment Variables:
   - `ASPNETCORE_ENVIRONMENT`: `Production`
7. Click **Create Web Service**.
8. Once deployment finishes, your live Swagger UI will be accessible at:  
   **`https://<your-render-app-name>.onrender.com/swagger`**

---

### Option B: Deploy on Railway
1. Sign up at [Railway.app](https://railway.app) using GitHub.
2. Click **New Project** → **Deploy from GitHub repo**.
3. Select `Task_Management_System_Project` and specify root directory as `backend/TaskManagement.API`.
4. Railway will automatically build and deploy your .NET 9 API.
5. Generate a public domain under **Settings** → **Networking**. Your live Swagger URL will be live at:  
   **`https://<your-railway-domain>.up.railway.app/swagger`**

---

## 📄 License

This project is open-source under the [MIT License](LICENSE).
