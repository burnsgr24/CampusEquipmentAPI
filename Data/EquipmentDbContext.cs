using CampusEquipmentAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusEquipmentAPI.Data;

public class EquipmentDbContext : DbContext
{
	public EquipmentDbContext(DbContextOptions<EquipmentDbContext> options)
		: base(options)
	{
	}

	public DbSet<EquipmentItem> EquipmentItems => Set<EquipmentItem>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<EquipmentItem>(entity =>
		{
			entity.HasKey(item => item.EquipmentItemId);

			entity.Property(item => item.Name)
				.IsRequired()
				.HasMaxLength(100);

			entity.Property(item => item.Category)
				.IsRequired()
				.HasMaxLength(50);
		});
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseSeeding((context, _) =>
		{
			var db = (EquipmentDbContext)context;

			if (!db.EquipmentItems.Any())
			{
				db.EquipmentItems.AddRange(
					new EquipmentItem
					{
						Name = "Dell Latitude Laptop",
						Category = "Computers",
						IsAvailable = true
					},
					new EquipmentItem
					{
						Name = "Epson Projector",
						Category = "Presentation",
						IsAvailable = true
					},
					new EquipmentItem
					{
						Name = "Canon Camera",
						Category = "Photography",
						IsAvailable = false
					}
				);

				db.SaveChanges();
			}
		});
	}
}
