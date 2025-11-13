# Netter Tests

This directory contains unit tests for the Netter social media application.

## Test Projects

### Netter.Domain.Tests
Tests for domain entities and business logic.

**Coverage:**
- `UserTests` - Tests for User entity including:
  - Constructor validation
  - Profile updates
  - Activation/deactivation
  - Entity equality

- `PostTests` - Tests for Post entity including:
  - Content validation (500 char limit)
  - Content trimming
  - Update restrictions on deleted posts
  - Delete/restore functionality

### Netter.Application.Tests  
Tests for application layer handlers using CQRS pattern.

**Coverage:**
- `CreateUserHandlerTests` - Tests for user creation command handler:
  - Valid user creation
  - Input validation
  - Username/email normalization
  - Cancellation token propagation
  - Repository interaction verification

- `GetUserByIdHandlerTests` - Tests for user query handler:
  - Successful user retrieval
  - Handling non-existent users
  - Profile data inclusion
  - Active/inactive user states

## Technology Stack

- **xUnit** - Test framework
- **NSubstitute** - Mocking library (no Moq)
- **FluentAssertions** - Fluent assertion library for readable tests

## Running Tests

Run all tests:
```bash
dotnet test
```

Run tests for specific project:
```bash
dotnet test tests/Netter.Domain.Tests
dotnet test tests/Netter.Application.Tests
```

Run tests with detailed output:
```bash
dotnet test --verbosity detailed
```

Run tests with coverage (requires coverlet):
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Test Patterns

### Arrange-Act-Assert (AAA)
All tests follow the AAA pattern for clarity:
```csharp
[Fact]
public void Method_Scenario_ExpectedBehavior()
{
    // Arrange - Set up test data and dependencies
    var user = new User("johndoe", "john@example.com", "John Doe");
    
    // Act - Execute the method under test
    user.Deactivate();
    
    // Assert - Verify the expected outcome
    user.IsActive.Should().BeFalse();
}
```

### NSubstitute Mocking
Use `Substitute.For<T>()` to create mocks:
```csharp
var repository = Substitute.For<IRepository<User>>();
repository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
    .Returns(Task.FromResult<User?>(user));
```

Verify interactions:
```csharp
await repository.Received(1).GetByIdAsync(userId, Arg.Any<CancellationToken>());
```

### FluentAssertions
Use fluent syntax for assertions:
```csharp
result.Username.Should().Be("johndoe");
result.Should().NotBeNull();
act.Should().Throw<ArgumentException>().WithMessage("*Username*");
```

## Best Practices Demonstrated

1. **Test Naming**: `MethodName_Scenario_ExpectedBehavior`
2. **One Assertion Per Test**: Each test verifies one behavior
3. **Test Independence**: Tests don't depend on each other
4. **Mock Verification**: Verify dependencies are called correctly
5. **Theory Tests**: Use `[Theory]` for parameterized tests
6. **Domain Tests**: No mocking - test domain logic directly
7. **Application Tests**: Mock infrastructure dependencies

## Extending Tests

To add more tests:

1. **Domain Tests**: Add to `Netter.Domain.Tests`
   - Test entity behavior directly
   - No mocking needed
   - Focus on business rules

2. **Application Tests**: Add to `Netter.Application.Tests`
   - Use NSubstitute for dependencies
   - Test command/query handlers
   - Verify repository interactions

3. **Integration Tests** (future): Create `Netter.Integration.Tests`
   - Test with real database (in-memory)
   - Test API endpoints end-to-end
   - Use WebApplicationFactory

## Example: Adding a New Test

```csharp
using FluentAssertions;
using Netter.Domain.SocialInteractions;

namespace Netter.Domain.Tests.SocialInteractions;

public class FollowTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateFollow()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        var followeeId = Guid.NewGuid();

        // Act
        var follow = new Follow(followerId, followeeId);

        // Assert
        follow.FollowerId.Should().Be(followerId);
        follow.FolloweeId.Should().Be(followeeId);
        follow.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Constructor_WithSameUserIds_ShouldThrowArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var act = () => new Follow(userId, userId);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot follow themselves*");
    }
}
```

## Current Test Coverage

- ✅ User entity (10 tests)
- ✅ Post entity (13 tests)
- ✅ CreateUserHandler (5 tests)
- ✅ GetUserByIdHandler (4 tests)
- ⏳ Follow entity (todo)
- ⏳ Like entity (todo)
- ⏳ Comment entity (todo)
- ⏳ CreatePostHandler (todo)
- ⏳ GetPostsHandler (todo)

Total: **44 tests passing** ✅