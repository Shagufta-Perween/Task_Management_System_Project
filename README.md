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

## 📖 API Endpoints Summary

| Method | Endpoint | Description | Auth Required |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/Auth/login` | Authenticate user & receive JWT token | ❌ Public |
| `POST` | `/api/Auth/register` | Register new user account | ❌ Public |
| `GET` | `/api/Dashboard` | Fetch metrics overview & recent activity | 🔒 Token Required |
| `GET` | `/api/Tasks` | List tasks (supports filtering & role scoping) | 🔒 Token Required |
| `POST` | `/api/Tasks` | Create new task item | 🔒 Admin / Manager |
| `PUT` | `/api/Tasks/{id}` | Update existing task item | 🔒 Admin / Manager |
| `PATCH` | `/api/Tasks/{id}/status` | Update task status (ToDo → InProgress → Done) | 🔒 Token Required |
| `DELETE` | `/api/Tasks/{id}` | Delete task item | 🔒 Admin / Manager |
| `GET` | `/api/Teams` | List teams | 🔒 Token Required |
| `POST` | `/api/Teams` | Create new team | 🔒 Admin / Manager |
| `POST` | `/api/Teams/{id}/members` | Add member to team | 🔒 Admin / Manager |
| `GET` | `/api/Users` | List system users | 🔒 Admin Only |
| `PUT` | `/api/Users/{id}/role` | Update user system role | 🔒 Admin Only |
| `DELETE` | `/api/Users/{id}` | Delete user account | 🔒 Admin Only |

---

## 📄 License

This project is open-source under the [MIT License](LICENSE).
