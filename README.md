# Netter - Social Media API

A simple social media backend built with .NET, following Clean Architecture, Domain-Driven Design (DDD), and Screaming Architecture principles.

## Architecture

The solution follows Clean Architecture with the following layers:

- **Netter.Domain**: Core business entities and domain logic
- **Netter.Application**: Application services, CQRS commands/queries using MediatR
- **Netter.Infrastructure**: Data access with Entity Framework Core and SQLite
- **Netter.WebApi**: Minimal API endpoints and dependency injection setup

## Features

### Core Entities
- **Users**: Registration, profiles, activation/deactivation
- **Posts**: Create, update, delete posts (500 character limit)
- **Social Interactions**: 
  - Follow/Unfollow users
  - Like posts
  - Comment on posts (200 character limit)

### API Endpoints

#### Users
- `POST /api/users` - Create a new user
- `GET /api/users/{id}` - Get user by ID

#### Posts
- `POST /api/posts` - Create a new post
- `GET /api/posts` - Get all posts (non-deleted, ordered by creation date)

#### Health
- `GET /health` - Health check endpoint

## Technology Stack

- **.NET 9**: Latest .NET version
- **Entity Framework Core**: ORM with SQLite database
- **MediatR**: CQRS pattern implementation
- **Minimal APIs**: Lightweight REST endpoints
- **SQLite**: Local database for development

## Getting Started

1. Clone the repository
2. Run the application:
   ```bash
   dotnet run --project src/Netter.WebApi
   ```
3. The API will be available at `http://localhost:5255`
4. View API documentation at `http://localhost:5255/openapi/v1.json` (in development)

## Example Usage

### Create a User
```bash
curl -X POST http://localhost:5255/api/users \
  -H "Content-Type: application/json" \
  -d '{
    "username": "johndoe",
    "email": "john@example.com",
    "displayName": "John Doe"
  }'
```

### Create a Post
```bash
curl -X POST http://localhost:5255/api/posts \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "{user-id-from-previous-call}",
    "content": "Hello, Netter! This is my first post."
  }'
```

### Get All Posts
```bash
curl http://localhost:5255/api/posts
```

## Project Structure

```
src/
├── Netter.Domain/              # Domain entities and business rules
│   ├── Common/                 # Base classes (BaseEntity, ValueObject)
│   ├── Users/                  # User aggregate
│   ├── Posts/                  # Post aggregate
│   ├── SocialInteractions/     # Follow, Like, Comment entities
│   └── Feeds/                  # (Future: Feed generation logic)
├── Netter.Application/         # Application layer
│   ├── Common/                 # Interfaces (IRepository, IUnitOfWork)
│   ├── Users/                  # User commands and queries
│   └── Posts/                  # Post commands and queries
├── Netter.Infrastructure/      # Infrastructure layer
│   └── Persistence/            # Entity Framework configurations and repositories
└── Netter.WebApi/              # API layer with minimal endpoints
```

## Future Enhancements

- Authentication & Authorization (JWT)
- User feed generation based on followed users
- Like and Comment endpoints
- Follow/Unfollow endpoints
- Image upload for profile pictures and posts
- Real-time notifications
- Rate limiting
- Caching
- Full-text search
- Pagination for posts and comments
- User blocking/reporting
- Admin features
- Blazor frontend integration

## Design Patterns Used

- **Clean Architecture**: Separation of concerns with dependency inversion
- **Domain-Driven Design**: Rich domain models with business logic encapsulation  
- **CQRS**: Command Query Responsibility Segregation with MediatR
- **Repository Pattern**: Data access abstraction
- **Unit of Work**: Transaction management
- **Screaming Architecture**: Folder structure reflects business domains