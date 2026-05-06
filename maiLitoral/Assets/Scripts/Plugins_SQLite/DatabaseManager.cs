using UnityEngine;
using SQLite4Unity3d;
using System.IO;
using System;
using System.Linq;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance;

    private SQLiteConnection db;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        string path = Path.Combine(UnityEngine.Application.persistentDataPath, "mailitoral.db");
        db = new SQLiteConnection(path);

        CreateTables();
        InsertStartData();

        UnityEngine.Debug.Log("Database created and populated!");
    }

    private void CreateTables()
    {
        db.CreateTable<ZoneData>();
        db.CreateTable<BeachData>();
        db.CreateTable<CalendarDayData>();
        db.CreateTable<PropertyData>();
        db.CreateTable<BeachDayPropertyData>();
    }

    private void InsertStartData()
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
            CreateCalendarDayIfMissing(beach.Id, DateTime.Now.ToString("yyyy-MM-dd"));
        }

        UnityEngine.Debug.Log("Start data inserted.");
    }

    // ---------------- VALIDATION ----------------

    public bool IsValidText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;

        return text.All(c =>
            char.IsLetterOrDigit(c) ||
            c == ' ' ||
            c == '-' ||
            c == '_' ||
            c == '.'
        );
    }

    public bool IsValidPropertyType(string type)
    {
        return type == "bool" || type == "int" || type == "string";
    }

    public bool IsValidDate(string date)
    {
        return DateTime.TryParse(date, out _);
    }

    // ---------------- ZONES ----------------

    public void AddZone(string zoneName)
    {
        if (!IsValidText(zoneName))
        {
            UnityEngine.Debug.LogError("Invalid zone name.");
            return;
        }

        if (db.Table<ZoneData>().Any(z => z.Name == zoneName))
        {
            UnityEngine.Debug.LogError("Zone already exists.");
            return;
        }

        db.Insert(new ZoneData { Name = zoneName });

        UnityEngine.Debug.Log("Zone added.");
    }

    // ---------------- BEACHES ----------------

    public void AddBeach(string beachName, int zoneId)
    {
        if (!IsValidText(beachName))
        {
            UnityEngine.Debug.LogError("Invalid beach name.");
            return;
        }

        ZoneData zone = db.Find<ZoneData>(zoneId);

        if (zone == null)
        {
            UnityEngine.Debug.LogError("Zone does not exist.");
            return;
        }

        if (db.Table<BeachData>().Any(b => b.Name == beachName))
        {
            UnityEngine.Debug.LogError("Beach already exists.");
            return;
        }

        db.Insert(new BeachData
        {
            Name = beachName,
            ZoneId = zoneId
        });

        UnityEngine.Debug.Log("Beach added.");
    }

    // ---------------- CALENDAR ----------------

    public CalendarDayData CreateCalendarDayIfMissing(int beachId, string date)
    {
        if (!IsValidDate(date))
        {
            UnityEngine.Debug.LogError("Invalid date.");
            return null;
        }

        BeachData beach = db.Find<BeachData>(beachId);

        if (beach == null)
        {
            UnityEngine.Debug.LogError("Beach does not exist.");
            return null;
        }

        CalendarDayData day = db.Table<CalendarDayData>()
            .FirstOrDefault(d => d.BeachId == beachId && d.Date == date);

        if (day != null)
            return day;

        db.Insert(new CalendarDayData
        {
            BeachId = beachId,
            Date = date
        });

        day = db.Table<CalendarDayData>()
            .First(d => d.BeachId == beachId && d.Date == date);

        foreach (PropertyData property in db.Table<PropertyData>())
        {
            db.Insert(new BeachDayPropertyData
            {
                CalendarDayId = day.Id,
                PropertyId = property.Id,
                BoolValue = property.Type == "bool" ? false : (bool?)null,
                IntValue = property.Type == "int" ? 0 : (int?)null,
                StringValue = property.Type == "string" ? "" : null
            });
        }

        return day;
    }

    // ---------------- PROPERTIES ----------------

    public void AddPropertyToAllBeaches(string propertyName, string propertyType)
    {
        if (!IsValidText(propertyName))
        {
            UnityEngine.Debug.LogError("Invalid property name.");
            return;
        }

        if (!IsValidPropertyType(propertyType))
        {
            UnityEngine.Debug.LogError("Invalid property type. Use bool, int or string.");
            return;
        }

        if (db.Table<PropertyData>().Any(p => p.Name == propertyName))
        {
            UnityEngine.Debug.LogError("Property already exists.");
            return;
        }

        db.Insert(new PropertyData
        {
            Name = propertyName,
            Type = propertyType
        });

        PropertyData newProperty = db.Table<PropertyData>()
            .First(p => p.Name == propertyName);

        foreach (CalendarDayData day in db.Table<CalendarDayData>())
        {
            db.Insert(new BeachDayPropertyData
            {
                CalendarDayId = day.Id,
                PropertyId = newProperty.Id,
                BoolValue = propertyType == "bool" ? false : (bool?)null,
                IntValue = propertyType == "int" ? 0 : (int?)null,
                StringValue = propertyType == "string" ? "" : null
            });
        }

        UnityEngine.Debug.Log("Property added to all beaches.");
    }

    public void AddPropertyForBeachDay(int beachId, string date, string propertyName, bool status)
    {
        if (!IsValidText(propertyName))
        {
            UnityEngine.Debug.LogError("Invalid property name.");
            return;
        }

        CalendarDayData day = CreateCalendarDayIfMissing(beachId, date);

        if (day == null)
            return;

        PropertyData property = db.Table<PropertyData>()
            .FirstOrDefault(p => p.Name == propertyName);

        if (property == null)
        {
            AddPropertyToAllBeaches(propertyName, "bool");

            property = db.Table<PropertyData>()
                .First(p => p.Name == propertyName);
        }

        BeachDayPropertyData value = db.Table<BeachDayPropertyData>()
            .FirstOrDefault(v => v.CalendarDayId == day.Id && v.PropertyId == property.Id);

        if (value == null)
        {
            db.Insert(new BeachDayPropertyData
            {
                CalendarDayId = day.Id,
                PropertyId = property.Id,
                BoolValue = status
            });
        }
        else
        {
            value.BoolValue = status;
            db.Update(value);
        }

        UnityEngine.Debug.Log("Property added/updated for beach day.");
    }

    public void ModifyPropertyForBeachDay(int beachId, string date, string oldPropertyName, string newPropertyName, bool newStatus)
    {
        if (!IsValidText(oldPropertyName) || !IsValidText(newPropertyName))
        {
            UnityEngine.Debug.LogError("Invalid property name.");
            return;
        }

        CalendarDayData day = CreateCalendarDayIfMissing(beachId, date);

        if (day == null)
            return;

        PropertyData property = db.Table<PropertyData>()
            .FirstOrDefault(p => p.Name == oldPropertyName);

        if (property == null)
        {
            UnityEngine.Debug.LogError("Property does not exist.");
            return;
        }

        property.Name = newPropertyName;
        db.Update(property);

        BeachDayPropertyData value = db.Table<BeachDayPropertyData>()
            .FirstOrDefault(v => v.CalendarDayId == day.Id && v.PropertyId == property.Id);

        if (value != null)
        {
            value.BoolValue = newStatus;
            db.Update(value);
        }

        UnityEngine.Debug.Log("Property modified.");
    }

    public void DeletePropertyFromAllBeaches(string propertyName)
    {
        if (!IsValidText(propertyName))
        {
            UnityEngine.Debug.LogError("Invalid property name.");
            return;
        }

        PropertyData property = db.Table<PropertyData>()
            .FirstOrDefault(p => p.Name == propertyName);

        if (property == null)
        {
            UnityEngine.Debug.LogError("Property does not exist.");
            return;
        }

        foreach (BeachDayPropertyData value in db.Table<BeachDayPropertyData>()
                     .Where(v => v.PropertyId == property.Id))
        {
            db.Delete(value);
        }

        db.Delete(property);

        UnityEngine.Debug.Log("Property deleted from all beaches.");
    }

    public void DeleteAllPropertiesForBeach(int beachId)
    {
        foreach (CalendarDayData day in db.Table<CalendarDayData>().Where(d => d.BeachId == beachId))
        {
            foreach (BeachDayPropertyData value in db.Table<BeachDayPropertyData>()
                         .Where(v => v.CalendarDayId == day.Id))
            {
                db.Delete(value);
            }
        }

        UnityEngine.Debug.Log("All properties deleted for beach.");
    }

    // ---------------- RANK ----------------

    public void AddRankForBeachDay(int beachId, string date, float rank)
    {
        rank = Mathf.Clamp(rank, 0f, 4f);

        CalendarDayData day = CreateCalendarDayIfMissing(beachId, date);

        if (day == null)
            return;

        PropertyData rankProperty = db.Table<PropertyData>()
            .FirstOrDefault(p => p.Name == "Rank");

        if (rankProperty == null)
        {
            AddPropertyToAllBeaches("Rank", "int");

            rankProperty = db.Table<PropertyData>()
                .First(p => p.Name == "Rank");
        }

        BeachDayPropertyData value = db.Table<BeachDayPropertyData>()
            .FirstOrDefault(v => v.CalendarDayId == day.Id && v.PropertyId == rankProperty.Id);

        if (value == null)
        {
            db.Insert(new BeachDayPropertyData
            {
                CalendarDayId = day.Id,
                PropertyId = rankProperty.Id,
                IntValue = Mathf.RoundToInt(rank)
            });
        }
        else
        {
            value.IntValue = Mathf.RoundToInt(rank);
            db.Update(value);
        }

        UnityEngine.Debug.Log("Rank added/updated.");
    }
}