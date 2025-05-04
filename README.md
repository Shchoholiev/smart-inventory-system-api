# smart-inventory-system-api

A back-end API for Smart Inventory System that helps to keep track of your items by converting shelves into smart devices using IoT.

## Table of Contents
- [Features](#features)
- [Stack](#stack)
- [Installation](#installation)
  - [Prerequisites](#prerequisites)
  - [Setup Instructions](#setup-instructions)
- [Configuration](#configuration)

## Features
- Create, read, update, and manage IoT-connected devices, shelves, groups, and inventory items.
- User authentication and role-based authorization (e.g., Admin, Owner).
- Register new users and handle login with JWT authentication.
- Image recognition and QR/barcode scanning for identifying items via access point devices.
- Control physical shelf lights remotely via Azure IoT Hub direct methods.
- Track item history and scan history with pagination support.
- View statistics and analytics such as item popularity, shelf load, user activities, and items taken.
- Group management with the ability to add, remove, and update group members.

## Stack
- C# and .NET 7 for backend development
- ASP.NET Core Web API
- MongoDB as the database
- Azure IoT Hub for device management and communication
- Azure Cognitive Services for image recognition (tags extraction)

## Installation

### Prerequisites
- [.NET 7 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
- [MongoDB](https://www.mongodb.com/try/download/community)
- Access to [Azure IoT Hub](https://azure.microsoft.com/en-us/services/iot-hub/)
- Azure App Configuration for external configuration management
- Python ML service for image barcode/QR code recognition (if replicating all features)

### Setup Instructions

1. Clone the repository:
   ```bash
   git clone https://github.com/Shchoholiev/smart-inventory-system-api.git
   cd smart-inventory-system-api
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. To build the solution:
   ```bash
   dotnet build
   ```

4. Run the application locally:
   ```bash
   dotnet run --project SmartInventorySystemApi.Api/SmartInventorySystemApi.Api.csproj
   ```
   
5. Open your browser and navigate to the Swagger UI for API exploration:
   ```
   http://localhost:5132/swagger
   ```

6. (Optional) Use the provided `.devcontainer` to develop inside a containerized environment with VSCode Remote Containers.

## Configuration

The application supports configuration through `appsettings.json` and environment variables, with optional Azure App Configuration integration.

### Important Configuration Settings
  
- **MongoDB Connection:**

  Configure your MongoDB database connection string and database name in environment or `appsettings.json`:
  ```json
  "ConnectionStrings": {
    "MongoDatabaseName": "SmartInventorySystem"
  }
  ```

- **JWT Authentication:**

  Settings for JSON Web Token validation are found under:
  ```json
  "JsonWebTokenKeys": {
    "ValidateIssuer": true,
    "ValidateAudience": true,
    "ValidateLifetime": true,
    "ValidateIssuerSigningKey": true
  }
  ```

- **Azure App Configuration:**

  Provide the connection string for Azure App Configuration using environment variable:
  ```
  APP_CONFIG=<your-azure-app-configuration-connection-string>
  ```

- **Azure IoT Hub:**

  Device registration and control requires access credentials and connection to your Azure IoT Hub instance. These should be configured as per Azure SDK requirements and environment variables (not included in source).
