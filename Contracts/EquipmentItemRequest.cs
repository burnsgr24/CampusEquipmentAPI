namespace CampusEquipmentAPI.Contracts;

public class EquipmentItemRequest
{
	public string? Name { get; set; }

	public string? Category { get; set; }

	public bool? IsAvailable { get; set; }
}
