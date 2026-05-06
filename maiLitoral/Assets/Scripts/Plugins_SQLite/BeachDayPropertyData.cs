using SQLite4Unity3d;

public class BeachDayPropertyData
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int CalendarDayId { get; set; }

    public int PropertyId { get; set; }

    public bool? BoolValue { get; set; }

    public int? IntValue { get; set; }

    public string StringValue { get; set; }
}
