using UnityEngine;
using SQLite4Unity3d;
using System.IO;
using System;
using System.Linq;

public class DatabaseManager : MonoBehaviour
{
    private SQLiteConnection db;

    void Start()
    {
        string path = Path.Combine(UnityEngine.Application.persistentDataPath, "mailitoral.db");
        db = new SQLiteConnection(path);

        CreateTables();
        InsertStartData();

        UnityEngine.Debug.Log("Database created and populated!");
    }

    void CreateTables()
    {
        db.CreateTable<ZoneData>();
        db.CreateTable<BeachData>();
        db.CreateTable<CalendarDayData>();
        db.CreateTable<PropertyData>();
        db.CreateTable<BeachDayPropertyData>();
    }

    void InsertStartData()
    {
        if (db.Table<ZoneData>().Count() > 0)
            return;

        db.Insert(new ZoneData { Name = "Mamaia Nord" });
        db.Insert(new ZoneData { Name = "Constanta" });

        int mamaiaId = db.Table<ZoneData>().First(z => z.Name == "Mamaia Nord").Id;
        int constantaId = db.Table<ZoneData>().First(z => z.Name == "Constanta").Id;

        db.Insert(new BeachData { Name = "Beach One", ZoneId = mamaiaId });
        db.Insert(new BeachData { Name = "Modern Beach", ZoneId = constantaId });

        db.Insert(new PropertyData { Name = "Has Lifeguard", Type = "bool" });
        db.Insert(new PropertyData { Name = "Water Temperature", Type = "int" });
        db.Insert(new PropertyData { Name = "Description", Type = "string" });

        foreach (BeachData beach in db.Table<BeachData>())
        {
            db.Insert(new CalendarDayData
            {
                BeachId = beach.Id,
                Date = DateTime.Now.ToString("yyyy-MM-dd")
            });

            CalendarDayData day = db.Table<CalendarDayData>()
                .Where(d => d.BeachId == beach.Id)
                .First();

            foreach (PropertyData property in db.Table<PropertyData>())
            {
                BeachDayPropertyData value = new BeachDayPropertyData
                {
                    CalendarDayId = day.Id,
                    PropertyId = property.Id
                };

                if (property.Type == "bool")
                    value.BoolValue = true;
                else if (property.Type == "int")
                    value.IntValue = 25;
                else if (property.Type == "string")
                    value.StringValue = "Clean beach";

                db.Insert(value);
            }
        }
    }
}