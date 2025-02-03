using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using TodoApi;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// הוספת DbContext
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB"), 
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ToDoDB"))));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// הגדרת ה-routes
app.MapGet("/items", async (ToDoDbContext db) =>
{
    return await db.Items.ToListAsync(); // שליפת כל המשימות
});

app.MapPost("/items", async (ToDoDbContext db, [FromBody] Item item) =>
{

    try
    {

        db.Items.Add(item);
        await db.SaveChangesAsync();
        return Results.Created($"/items/{item.Id}", item);
    }
    catch (Exception ex)
    {
        return Results.Problem("An error occurred while adding the item: " + ex.Message);
    }
});

app.MapPut("/items/{id}", async (ToDoDbContext db, int id) =>
{
    try
    {
        var item = await db.Items.FindAsync(id);
        if (item is null) return Results.NotFound();

        item.IsComplete=!item.IsComplete;

        System.Console.WriteLine(item.IsComplete);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    catch (Exception ex)
    {
        return Results.Problem("An error occurred while updating the item: " + ex.Message);
    }
});

app.MapDelete("/items/{id}", async (int id, ToDoDbContext db) =>
{
    var item = await db.Items.FindAsync(id);
    if (item is null) return Results.NotFound();

    db.Items.Remove(item); // מחיקת משימה
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();