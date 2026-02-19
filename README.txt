# TaskTracker — Microservice Web Application

TaskTracker is a microservice-based web application built with ASP.NET Core and Angular.  
It allows users to create, view, update, and delete their personal tasks.

The application uses JWT authentication and ensures users can access only their own data.

This project was created as part of a technical interview assignment.

---

# Architecture

The application consists of three main components:

## Frontend
- Angular
- Served by nginx inside Docker
- Communicates with backend via REST API

## Identity Service
- ASP.NET Core Web API
- Handles user authentication
- Issues JWT tokens

## Tasks Service
- ASP.NET Core Web API
- Manages tasks (CRUD operations)
- Requires valid JWT token

---

# Technology Stack

Frontend:
- Angular
- TypeScript
- nginx

Backend:
- ASP.NET Core Web API (.NET 8)
- Clean Architecture
- REST API

Authentication:
- JWT Bearer authentication

Database:
- SQLite

Infrastructure:
- Docker
- Docker Compose
- nginx reverse proxy

---

# Features

Authentication:
- User login
- JWT token-based authorization

Tasks:
- Create task
- View own tasks
- Update task
- Delete task
- Task priority
- Optional due date
- Task completion status

Security:
- Users can access only their own tasks

Architecture:
- Clean Architecture principles
- Separation of concerns
- Microservice design

---

# Project Structure

TaskTracker/
│
├── frontend/
│   └── tasktracker-ui/        # Angular frontend
│
├── backend/
│   └── src/
│       ├── TaskTracker.IdentityService/
│       │   ├── Api/
│       │   ├── Application/
│       │   ├── Domain/
│       │   └── Infrastructure/
│       │
│       └── TaskTracker.TasksService/
│           ├── Api/
│           ├── Application/
│           ├── Domain/
│           └── Infrastructure/
│
├── nginx/
│   └── nginx.conf             # Reverse proxy configuration
│
├── docker-compose.yml
└── README.md

# How to Run the Application

## Requirements

Install:

- Docker
- Docker Compose

---

## Start application

From project root directory run:

```bash
docker compose up --build -d
```

This will start:

- frontend (nginx + Angular)
- identity-api
- tasks-api

---

## Open application

Open browser:

http://localhost:4200

---

## Demo Users

The Identity Service is seeded with the following test users:

User 1
Username: alice
Password: Demo1234!

User 2
Username: bob
Password: Demo1234!

User 3
Username: marks
Password: Demo1234!

---

## Notes

User IDs are generated automatically when the application starts.

Each user has a unique UserId stored in the database and embedded in the JWT token after login.

Tasks are always linked to the authenticated user's UserId.

Users cannot access tasks created by other users.

---

# API Documentation (Swagger)

After starting the application with Docker Compose, Swagger UI is available for both microservices:

Identity Service
Authentication and JWT token endpoints:
http://localhost:5001/swagger/index.html

Provides:

User authentication

JWT token generation

Login endpoint

Tasks Service
Task management endpoints:
http://localhost:5000/swagger/index.html

Provides:

Create task

Get user tasks

Update task

Delete task

Swagger allows testing all API endpoints directly from the browser.

## Authentication

All Tasks Service endpoints require JWT authentication.

Authentication flow:

User logs in via Identity Service

POST /api/auth/login

Identity Service returns JWT token

Frontend (or Swagger) sends token in Authorization header:

Authorization: Bearer <your_token>

Tasks Service validates the token and returns only the authenticated user's tasks

In Swagger UI, click the Authorize button and enter:

Bearer <your_token>
Tasks Service Endpoints

Requires JWT token.

GET    /api/tasks/        - Get current user's tasks
POST   /api/tasks/        - Create task
PUT    /api/tasks/{id}    - Update task
DELETE /api/tasks/{id}    - Delete task

---

# Clean Architecture Layers

Each backend service uses:

Domain
- Entities
- Business rules

Application
- Interfaces
- Use cases

Infrastructure
- Database
- Repositories

API
- Controllers
- HTTP endpoints

---

# Database

SQLite is used for simplicity.

Database files are created automatically on first run.

---

# Docker

The entire application runs in Docker containers:

- frontend container (nginx + Angular)
- identity-api container
- tasks-api container

nginx acts as reverse proxy and routes:

/api/auth -> identity-api
/api/tasks -> tasks-api


---

# Development Notes

Frontend communicates with backend through nginx reverse proxy.

No CORS configuration is required.

All services communicate inside Docker network.

---

# Author

Marks Slonimskis
