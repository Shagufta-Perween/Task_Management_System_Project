# Team Task Management System

A role-based Task Management Application built with **.NET 9 Web API**, **Entity Framework Core**, **SQL Server**, and **React (Vite)** frontend.

---

## 🌟 Key Features

- **Role-Based Access Control (RBAC)**:
  - **Admin**: Full system management (manage teams, assign tasks, update user roles, delete accounts).
  - **Manager**: Create teams, add team members, create and assign tasks.
  - **User**: View assigned tasks, update task statuses (To Do → In Progress → Done), comment on tasks.
- **JWT Authentication & Authorization**: Secure token generation with password hashing via ASP.NET Identity.
- **Interactive Dashboard**: High-level task metrics, overdue indicators, progress bar, and filters (Deadline, Status, Priority).
- **Task Management**: Kanban-style board, task assignment, status updates, priority levels, and deadlines.
- **Team Management**: Create teams, assign team members.
- **Task Comments**: Real-time collaborative discussion on individual tasks.
- **Notifications**: Automatic in-app notifications on task assignment and status updates.
- **Swagger Documentation**: Interactive OpenAPI testing with JWT Bearer token support.
- **Docker Multi-container Setup**: Complete `docker-compose` setup for API, Frontend, and SQL Server.

---

## 🔑 Sample Credentials

The application automatically seeds 3 sample accounts on first startup:

| Role | Email | Password |
|---|---|---|
| **Admin** | `admin@taskmanager.com` | `Admin@123` |
| **Manager** | `manager@taskmanager.com` | `Manager@123` |
| **User** | `user@taskmanager.com` | `User@123` |

---

## 🛠️ Technology Stack

| Layer | Technology |
|---|---|
| **Backend** | .NET 9.0 Web API, EF Core 9.0 |
| **Database** | SQL Server / LocalDB |
| **Auth** | ASP.NET Core Identity + JWT Bearer Tokens |
| **Frontend** | React 18, Vite, Axios, Lucide Icons |
| **Styling** | Vanilla CSS (Modern Dark Mode & Glassmorphism) |
| **DevOps** | Docker, Docker Compose, GitHub Actions CI/CD |
| **Testing** | xUnit, Moq, EF Core InMemory |

---

## 🚀 How to Run Locally

### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/)

### 1. Run Backend (.NET Web API)
```bash
cd backend/TaskManagement.API
dotnet run --urls="http://localhost:5000"
```
The API will start at `http://localhost:5000`.
Swagger UI will be available at: `http://localhost:5000/swagger`

### 2. Run Frontend (React Vite)
```bash
cd frontend
npm install
npm run dev
```
The web app will run at `http://localhost:3000`.

---

## 🐳 Run with Docker Compose

```bash
docker-compose up --build
```
- Frontend: `http://localhost:3000`
- Backend API: `http://localhost:5000`
