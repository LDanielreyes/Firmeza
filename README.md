# Firmeza - Business Management System

**Firmeza** is a complete business management application that allows you to manage products, clients, and generate reports in Excel and PDF. The system features three frontend applications (Angular, Blazor) and a robust Backend API.

---

## 📋 Table of Contents

- [Technologies](#-technologies)
- [Features](#-features)
- [Architecture](#-architecture)
- [Prerequisites](#-prerequisites)
- [Installation and Deployment](#-installation-and-deployment)
  - [Option 1: Docker Deployment (Recommended)](#option-1-docker-deployment-recommended)
  - [Option 2: Local Deployment](#option-2-local-deployment)
- [Testing](#-testing)
- [Usage](#-usage)
- [Troubleshooting](#-troubleshooting)

---

## 🛠️ Technologies

### Backend API (`FirmezaAPI`)
- **.NET 8.0** - Main framework
- **ASP.NET Core Web API** - RESTful API
- **Entity Framework Core 9.0** - ORM
- **PostgreSQL** - Database (Npgsql)
- **AutoMapper 11.0** - Object mapping
- **Swagger/OpenAPI** - API documentation
- **JWT Authentication** - Authentication and authorization
- **EPPlus 8.3** - Excel file generation
- **QuestPDF 2025.7** - PDF file generation

### Angular Frontend (`FirmezaFrontend`)
- **Angular 21** - Frontend framework
- **TypeScript 5.9** - Programming language
- **TailwindCSS 3.4** - CSS framework
- **RxJS 7.8** - Reactive programming
- **jsPDF 3.0** - Client-side PDF generation
- **jwt-decode** - JWT token decoding

### Blazor Application (`Firmeza`)
- **.NET 8.0** - Framework
- **Blazor Server** - Interactive UI
- **Entity Framework Core 9.0** - ORM
- **EPPlus & QuestPDF** - Report generation

### Testing (`Firmeza.Tests`)
- **xUnit 2.4** - Testing framework
- **Moq 4.20** - Mocking
- **Entity Framework InMemory** - In-memory database for tests
- **AutoMapper** - Object mapping in tests

### DevOps
- **Docker** - Containerization
- **Docker Compose** - Container orchestration
- **PostgreSQL (Clever Cloud)** - Cloud database

---

## 🎯 Features

### Product Management
- ✅ Complete CRUD operations
- ✅ Advanced filtering and search
- ✅ Bulk import from Excel
- ✅ Export to Excel and PDF
- ✅ Inventory control

### Client Management
- ✅ Complete CRUD operations
- ✅ Bulk import from Excel
- ✅ Export to Excel and PDF
- ✅ Contact information management

### Reports
- ✅ Excel report generation (EPPlus)
- ✅ PDF report generation (QuestPDF)
- ✅ Professional design exports

### Security
- ✅ JWT authentication
- ✅ Role-based access control
- ✅ Data validation

---

## 🏗️ Architecture

```
Firmeza/
├── FirmezaAPI/              # Backend RESTful API (.NET 8)
│   ├── Controllers/         # API controllers
│   ├── Services/            # Business logic
│   ├── DTOs/               # Data transfer objects
│   └── Dockerfile          # Docker image for API
│
├── FirmezaFrontend/         # Angular 21 Frontend
│   ├── src/app/            # Components and services
│   ├── src/styles/         # TailwindCSS styles
│   └── Dockerfile          # Docker image for Frontend
│
├── Firmeza/                 # Blazor Server Application
│   ├── Pages/              # Blazor pages
│   ├── Data/               # Database context
│   └── Dockerfile          # Docker image for Blazor
│
├── Firmeza.Tests/           # xUnit unit tests
│   ├── ProductsControllerTests.cs
│   └── Dockerfile          # Docker image for tests
│
└── docker-compose.yml       # Service orchestration
```

---

## 📦 Prerequisites

### For Docker Deployment:
- **Docker Desktop** installed and running
  - [Download Docker Desktop](https://www.docker.com/products/docker-desktop)
  - Windows 10/11 with WSL2 enabled

### For Local Deployment:
- **.NET 8.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Node.js 20+** and npm - [Download](https://nodejs.org/)
- **PostgreSQL** (optional, cloud database is used by default)

---

## 🚀 Installation and Deployment

### Option 1: Docker Deployment (Recommended)

This is the easiest way to deploy the entire application on any computer.

#### 1. Clone the repository
```bash
git clone https://github.com/LDanielreyes/Firmeza.git
cd Firmeza
```

#### 2. Ensure Docker Desktop is running
On Windows, search for "Docker Desktop" in the start menu and run it. Wait for the Docker icon in the taskbar to show it's active.

#### 3. Deploy with Docker Compose

**Option A: Use the helper script (Windows)**
```powershell
.\test-docker.ps1
```
Select option 2: "Deploy complete with Docker Compose"

**Option B: Manual command**
```bash
docker compose up --build
```

#### 4. Access the applications

Once deployment is complete:

| Application | URL | Description |
|------------|-----|-------------|
| **Angular Frontend** | http://localhost:4200 | Main user interface |
| **Blazor App** | http://localhost:5000 | Alternative Blazor application |
| **Backend API** | http://localhost:5001 | RESTful API |
| **Swagger UI** | http://localhost:5001/swagger | API documentation |

#### 5. Stop the services
```bash
docker compose down
```

---

### Option 2: Local Deployment

If you prefer to run the application directly without Docker:

#### 1. Restore backend dependencies
```bash
dotnet restore Firmeza.sln
```

#### 2. Run the API
```bash
cd FirmezaAPI
dotnet run
```
The API will be available at `https://localhost:7001`

#### 3. Run the Angular frontend
In another terminal:
```bash
cd FirmezaFrontend
npm install
npm start
```
The frontend will be available at `http://localhost:4200`

#### 4. Run the Blazor application (optional)
In another terminal:
```bash
cd Firmeza
dotnet run
```
Blazor will be available at `https://localhost:7000`

---

## 🧪 Testing

### Run Unit Tests

The project includes comprehensive unit tests with xUnit.

```bash
# Run all tests
dotnet test Firmeza.Tests/Firmeza.Tests.csproj

# With detailed output
dotnet test Firmeza.Tests/Firmeza.Tests.csproj --logger "console;verbosity=detailed"
```

**Expected result:** 4/4 tests passing ✅

### Included tests:
- ✅ Controller tests (ProductsController)
- ✅ Integration tests with Entity Framework InMemory
- ✅ Mapping tests with AutoMapper
- ✅ HTTP response validation

### Automated Tests in Docker

When running `docker compose up`, tests run automatically **before** deploying services:
- If tests **pass**, all services are deployed ✅
- If tests **fail**, deployment stops ❌

To view test logs:
```bash
docker compose logs tests
```

---

## 💡 Usage

### 1. Product Management

#### Create a product
```http
POST http://localhost:5001/api/products
Content-Type: application/json

{
  "name": "Example Product",
  "price": 99.99,
  "stock": 50,
  "description": "Product description"
}
```

#### Import products from Excel
1. Navigate to the products section in the frontend
2. Click "Import Excel"
3. Select a file with format:
   ```
   | ProductName | Price | Stock | Description |
   |-------------|-------|-------|-------------|
   | Product 1   | 10.50 | 100   | Desc...     |
   ```

#### Export to PDF/Excel
- Use the "Export PDF" or "Export Excel" buttons in the interface

### 2. Client Management

Similar to products, with the following Excel columns:
```
| Name | Email | Phone | Address |
|------|-------|-------|---------|
```

---

## 🔧 Troubleshooting

### Problem: Docker cannot download images from `mcr.microsoft.com`

**Error:**
```
failed to do request: Head "https://mcr.microsoft.com/v2/dotnet/sdk/manifests/8.0": EOF
```

**Solutions:**

1. **Change DNS:**
   ```powershell
   # Use Google DNS (run as administrator)
   Set-DnsClientServerAddress -InterfaceAlias "Wi-Fi" -ServerAddresses ("8.8.8.8","8.8.4.4")
   ```

2. **Restart Docker Desktop:**
   - Close Docker Desktop completely
   - Reopen and wait for it to fully initialize
   - Try again

3. **Verify connectivity:**
   ```bash
   docker pull hello-world
   ```

4. **Disable VPN/Proxy** if you're using one

### Problem: Port already in use

**Error:** `Port 5001 is already allocated`

**Solution:**
- Change ports in `docker-compose.yml`
- Or stop the process using the port

### Problem: Tests fail

**Solution:**
```bash
# View detailed logs
docker compose logs tests

# Or run locally for more information
dotnet test Firmeza.Tests/Firmeza.Tests.csproj --logger "console;verbosity=detailed"
```

---

## 📄 Database Configuration

The application is configured to use **PostgreSQL** hosted on **Clever Cloud**. The connection string is located in:
- `docker-compose.yml` (for Docker)
- `appsettings.json` (for local execution)

### Environment Variables

To change the database, modify the environment variable `ConnectionStrings__DefaultConnection`:

```yaml
environment:
  - ConnectionStrings__DefaultConnection=Host=your_host;Username=user;Password=password;Database=db;Port=5432
```

---

## 📚 Additional Documentation

- **API Documentation:** http://localhost:5001/swagger (when the API is running)
- **Complete walkthrough:** See `walkthrough.md` in the repository
- **Helper script:** `test-docker.ps1` for Windows PowerShell

---

## 🤝 Contributing

To contribute to the project:

1. Fork the repository
2. Create a branch for your feature (`git checkout -b feature/new-feature`)
3. Run tests: `dotnet test`
4. Commit your changes (`git commit -m 'Add new feature'`)
5. Push to the branch (`git push origin feature/new-feature`)
6. Open a Pull Request

---

## 📝 License

This project is open source and available under the MIT license.

---

## 👥 Authors

By: Lucas Daniel Chacon Reyes
Clan: Caiman
Repo: https://github.com/LDanielreyes/Firmeza.git

---

## 🆘 Support

If you encounter problems during deployment:

1. Review the [Troubleshooting](#-troubleshooting) section
2. Check the logs: `docker compose logs -f`
3. Open an issue in the repository with error details

---

**Ready to use! 🚀** Follow the installation steps and you'll have the entire system running in minutes.
