# Translation API System

A comprehensive translation and chat system built with .NET, utilizing Google's Gemini API for translations and AI-powered conversations.

## Features

### Translation Service
- Multi-language translation support using Google's Gemini API
- Efficient handling of large text translations with automatic text chunking
- Robust error handling and retry mechanisms

### Chat System
- Real-time AI-powered chat sessions
- Support for multiple AI models
- Message tracking and history
- Response time monitoring
- User feedback system for messages

### Authentication & User Management
- Secure user authentication
- User profile management
- Role-based access control

### Web Interface
- Modern Blazor WebAssembly frontend
- Responsive design
- Real-time chat interface
- User-friendly translation form

## Technical Architecture

### API Layer (TranslationApi.API)
- RESTful API endpoints
- Controllers for Translation, Chat, Authentication, and Feedback
- Swagger documentation

### Application Layer (TranslationApi.Application)
- Business logic implementation
- Translation service with Gemini API integration
- Chat and user management services
- DTOs and interfaces

### Domain Layer (TranslationApi.Domain)
- Core business entities
- Domain models:
  - AIModel
  - ChatSession
  - ChatMessage
  - User
  - Feedback
- Business rules and validation

### Infrastructure Layer (TranslationApi.Infrastructure)
- Data persistence
- Entity Framework Core
- Repository implementations
- Database migrations
- External service integrations

### Web UI (TranslationWeb)
- Blazor WebAssembly frontend
- Component-based architecture
- Responsive design
- Real-time updates

## Getting Started

### Prerequisites
- .NET 7.0 or later
- SQL Server
- Gemini API key

### Configuration
1. Clone the repository
2. Update the connection string in `appsettings.json`
3. Configure your Gemini API key in the configuration
4. Run database migrations:
   ```
   dotnet ef database update
   ```

### Running the Application
1. Start the API:
   ```
   cd TranslationApi.API
   dotnet run
   ```
2. Start the Web UI:
   ```
   cd TranslationWeb
   dotnet run
   ```

## Project Structure

```
TranslationApi/
├── TranslationApi.API/          # API endpoints and controllers
├── TranslationApi.Application/  # Business logic and services
├── TranslationApi.Domain/       # Core business entities and rules
├── TranslationApi.Infrastructure/ # Data access and external services
└── TranslationWeb/              # Blazor WebAssembly frontend
```

## Key Features Implementation

### Translation Service
- Implements chunking for large text translations
- Handles special characters and text normalization
- Provides detailed error information
- Implements retry mechanism with exponential backoff

### Chat System
- Real-time message handling
- Support for different message types
- Session management
- Performance monitoring (response time tracking)

### Security
- JWT authentication
- Role-based authorization
- Secure API endpoints
- Input validation and sanitization

## Contributing

1. Fork the repository
2. Create your feature branch
3. Commit your changes
4. Push to the branch
5. Create a new Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.