using SQLite4Unity3d;

public class ZoneData
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; }
}