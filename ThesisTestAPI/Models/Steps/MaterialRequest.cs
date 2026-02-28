namespace ThesisTestAPI.Models.Steps;

public class MaterialRequest
{
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string UnitOfMeasurement { get; set; } = string.Empty;
    public string Supplier { get; set; } = string.Empty;
    public long Cost { get; set; }
}