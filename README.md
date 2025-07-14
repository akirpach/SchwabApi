<<<<<<< HEAD
# SchwabApi
=======
# SampleStartUp

A modern, containerized SaaS starter template built with:

- ⚙️ **ASP.NET Core Web API** (Backend)
- ⚛️ **React + Vite** (Frontend)
- 🌐 **Nginx** (Reverse Proxy)
- 🐳 **Docker + Docker Compose** (Environment)

> ✅ Ready for development. Easily extendable for staging/production.

---

## 🗂 Project Structure
SampleStartUp/
├─ backend/ # ASP.NET Core Web API project
├─ client/ # React frontend built with Vite
├─ nginx/ # Custom Nginx configuration
├─ docker-compose.yml
├─ SampleStartUp.sln


---

## 🚀 Getting Started (Local Dev)

### 🔧 Requirements

- [Docker](https://www.docker.com/)
- [Docker Compose](https://docs.docker.com/compose/)
- [.NET SDK 8+](https://dotnet.microsoft.com/en-us/download) (for local backend work)
- [Node.js 20+](https://nodejs.org/) (for local frontend work)

---

### 🏁 Spin up full dev environment

```bash
docker-compose up --build

Once started:

Frontend: http://localhost

Backend API: http://localhost/api

| Service   | Description               | URL                    |
| --------- | ------------------------- | ---------------------- |
| `client`  | Vite dev server (React)   | `http://localhost/`    |
| `backend` | ASP.NET Core API          | `http://localhost/api` |
| `nginx`   | Reverse proxy for routing | `http://localhost/`    |

>>>>>>> 2592346 (init)
