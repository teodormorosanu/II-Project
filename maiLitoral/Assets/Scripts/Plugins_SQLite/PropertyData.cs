using SQLite4Unity3d;

public class PropertyData
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; }

    // bool / int / string
    public string Type { get; set; }
}