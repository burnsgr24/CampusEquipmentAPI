using CampusEquipmentAPI.Contracts;
using CampusEquipmentAPI.Data;
using CampusEquipmentAPI.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("EquipmentDatabase")
	?? throw new InvalidOperationException("Connection string 'EquipmentDatabase' not found.");

builder.Services.AddDbContext<EquipmentDbContext>(options =>
	options.UseSqlite(connectionString));

var app = builder.Build();

app.MapGet("/", () => "Campus Equipment API is running.");

app.MapGet("/equipment", async (EquipmentDbContext db) =>
	await db.EquipmentItems
		.OrderBy(item => item.EquipmentItemId)
		.ToListAsync());

app.MapGet("/equipment/{id:int}", async (int id, EquipmentDbContext db) =>
{
	var item = await db.EquipmentItems.FindAsync(id);

	return item is null ? Results.NotFound() : Results.Ok(item);
});

app.MapPost("/equipment", async (EquipmentItemRequest request, EquipmentDbContext db) =>
{
	if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length > 100 ||
		string.IsNullOrWhiteSpace(request.Category) || request.Category.Length > 50 ||
		request.IsAvailable is null)
	{
		return Results.BadRequest("Invalid equipment information.");
	}

	var item = new EquipmentItem
	{
		Name = request.Name,
		Category = request.Category,
		IsAvailable = request.IsAvailable.Value
	};

	db.EquipmentItems.Add(item);
	await db.SaveChangesAsync();

	return Results.Created($"/equipment/{item.EquipmentItemId}", item);
});

app.MapPut("/equipment/{id:int}", async (int id, EquipmentItemRequest request, EquipmentDbContext db) =>
{
	if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length > 100 ||
		string.IsNullOrWhiteSpace(request.Category) || request.Category.Length > 50 ||
		request.IsAvailable is null)
	{
		return Results.BadRequest("Invalid equipment information.");
	}

	var item = await db.EquipmentItems.FindAsync(id);

	if (item is null)
	{
		return Results.NotFound();
	}

	item.Name = request.Name;
	item.Category = request.Category;
	item.IsAvailable = request.IsAvailable.Value;

	await db.SaveChangesAsync();

	return Results.NoContent();
});

app.MapDelete("/equipment/{id:int}", async (int id, EquipmentDbContext db) =>
{
	var item = await db.EquipmentItems.FindAsync(id);

	if (item is null)
	{
		return Results.NotFound();
	}

	db.EquipmentItems.Remove(item);
	await db.SaveChangesAsync();

	return Results.NoContent();
});

app.Run();
