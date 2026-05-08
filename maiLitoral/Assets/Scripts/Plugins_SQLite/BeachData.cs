using SQLite4Unity3d;

// Database model used for storing beach data and its associated zone.
public class BeachData
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int ZoneId { get; set; }
    public string Name { get; set; }
}
