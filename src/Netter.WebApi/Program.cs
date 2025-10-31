using MediatR;
using Microsoft.EntityFrameworkCore;
using Netter.Application.Common;
using Netter.Application.Users.Commands;
using Netter.Application.Users.Queries;
using Netter.Application.Posts.Commands;
using Netter.Application.Posts.Queries;
using Netter.Infrastructure.Persistence;
using Netter.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddOpenApi();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateUserCommand).Assembly));

// Database
builder.Services.AddDbContext<NetterDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=netter.db"));

// Repositories and Unit of Work
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// CORS for development
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseCors();
}

app.UseHttpsRedirection();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<NetterDbContext>();
    context.Database.EnsureCreated();
}

// User endpoints
app.MapPost("/api/users", async (CreateUserCommand command, IMediator mediator) =>
{
    var result = await mediator.Send(command);
    return Results.CreatedAtRoute("GetUser", new { id = result.UserId }, result);
})
.WithName("CreateUser")
.WithTags("Users");

app.MapGet("/api/users/{id:guid}", async (Guid id, IMediator mediator) =>
{
    var result = await mediator.Send(new GetUserByIdQuery(id));
    return result is not null ? Results.Ok(result) : Results.NotFound();
})
.WithName("GetUser")
.WithTags("Users");

// Post endpoints
app.MapPost("/api/posts", async (CreatePostCommand command, IMediator mediator) =>
{
    var result = await mediator.Send(command);
    return Results.CreatedAtRoute("GetPost", new { id = result.PostId }, result);
})
.WithName("CreatePost")
.WithTags("Posts");

app.MapGet("/api/posts", async (IMediator mediator) =>
{
    var result = await mediator.Send(new GetPostsQuery());
    return Results.Ok(result);
})
.WithName("GetPosts")
.WithTags("Posts");

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }))
.WithName("HealthCheck")
.WithTags("Health");

app.Run();
