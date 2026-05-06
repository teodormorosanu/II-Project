using SQLite4Unity3d;

public class BeachData
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int ZoneId { get; set; }

    public string Name { get; set; }
}
