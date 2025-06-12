# Quiz Application

![Quiz App Banner](https://img.shields.io/badge/Quiz%20Application-1.0.0-blue)
![.NET](https://img.shields.io/badge/.NET%208-512BD4?style=flat&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=flat&logo=c-sharp&logoColor=white)
![EF Core](https://img.shields.io/badge/EF%20Core-purple?style=flat)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=flat&logo=docker&logoColor=white)
![SQLite](https://img.shields.io/badge/SQLite-003B57?style=flat&logo=sqlite&logoColor=white)
![License](https://img.shields.io/badge/License-MIT-green.svg)

## 📝 Overview

A powerful quiz application built with .NET 8 and C# using a clean architecture approach. This application allows users to take quizzes with various question types, track their progress, and review their results.

## ✨ Features

- **Multiple Quiz Types**: Support for different quiz configurations
- **Flexible Question Ordering**: Sequential or randomized question delivery
- **Time Limits**: Configurable time constraints for each quiz
- **Result Tracking**: Track user quiz attempts and provide detailed feedback
- **REST API**: Comprehensive API endpoints for quiz management
- **Docker Support**: Easy deployment with Docker and Docker Compose
- **Clean Architecture**: Follows best practices with domain-driven design

## 🏗️ Architecture

The application follows Clean Architecture principles with four main layers:

- **Domain**: Contains business entities, enums, and domain logic
- **Application**: Contains business logic, DTOs, services, and interfaces
- **Infrastructure**: Contains data access implementations, migrations, and seed data
- **API**: Contains controllers, middleware, and configuration

### Key Components:
- **Entity Framework Core**: For data access and ORM
- **SQLite Database**: For data storage
- **REST API**: For frontend communication
- **Swagger/OpenAPI**: For API documentation

## 🚀 Getting Started

### Prerequisites

- .NET 8 SDK
- Docker (optional, for containerized deployment)

### Running Locally

1. Clone the repository:
   ```
   git clone https://github.com/yourusername/quiz_application.git
   cd quiz_application
   ```

2. Build and run the application:
   ```
   dotnet build
   cd quiz_application.Api
   dotnet run
   ```

3. Access the API:
   - API: http://localhost:5000/api/quiz
   - Swagger UI: http://localhost:5000/swagger

### Docker Deployment

1. Build and run with Docker Compose:
   ```
   docker-compose up -d
   ```

2. Access the API at: http://localhost:5000/api/quiz

## 📊 API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET    | `/api/quiz` | Get all available quizzes |
| POST   | `/api/quiz/{quizId}/start` | Start a new quiz |
| POST   | `/api/quiz/submit-answer` | Submit an answer for a question |
| GET    | `/api/quiz/{attemptId}/result` | Get quiz result |
| GET    | `/api/quiz/{attemptId}/review` | Review quiz answers |

## 📋 Data Models

### Main Entities:
- **Quiz**: Defines a quiz with questions and settings
- **Question**: Represents a question with multiple options
- **Option**: Represents an answer choice for a question
- **UserQuizAttempt**: Tracks a user's attempt at a quiz
- **UserAnswer**: Records a user's selected answer for a question

## 🔧 Configuration

The application uses appsettings.json for configuration:
- Database connection strings
- Application settings
- Logging options

## 🧪 Testing

Run the tests with:
```
dotnet test
```

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 👤 Contact

Created with ❤️ by [Your Name]

[GitHub](https://github.com/yourusername) | [LinkedIn](https://linkedin.com/in/yourprofile)