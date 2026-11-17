namespace CampusEquipmentAPI.Models;

public class EquipmentItem
{
	public int EquipmentItemId { get; set; }

	public string Name { get; set; } = string.Empty;

	public string Category { get; set; } = string.Empty;

	public bool IsAvailable { get; set; }
}
