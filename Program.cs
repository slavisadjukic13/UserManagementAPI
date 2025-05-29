using System.Collections.Concurrent;

var users = new ConcurrentDictionary<int, User>
{
    [1] = new User(1, "Alice", "alice@example.com"),
    [2] = new User(2, "Bob", "bob@example.com")
};

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () => "root!");


app.MapGet("/users", () => Results.Ok(users.Values));

app.MapGet("/users/{id}", (int id) =>
    users.TryGetValue(id, out var user) ? Results.Ok(user) : Results.NotFound());

app.MapPost("/users", (User newUser) =>
{
    if (!users.TryAdd(newUser.Id, newUser))
        return Results.Conflict("User with this ID already exists.");

    return Results.Created($"/users/{newUser.Id}", newUser);
});

app.MapPut("/users/{id}", (int id, User updatedUser) =>
{
    if (!users.ContainsKey(id))
        return Results.NotFound();

    users[id] = updatedUser;
    return Results.Ok(updatedUser);
});

app.MapDelete("/users/{id}", (int id) =>
{
    return users.TryRemove(id, out _) ? Results.NoContent() : Results.NotFound();
});

app.Run();

record User(int Id, string Name, string Email);
