using SQLite4Unity3d;

public class CalendarDayData
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int BeachId { get; set; }

    public string Date { get; set; }
}