using UnityEngine;
using System.Collections.Generic;
using System;


public class Beach : MonoBehaviour{

    /* Attributes */

    [SerializeField] private int beachId;
    private Dictionary<string, List<(string description, bool status)>> beachProperties = new Dictionary<string, List<(string description, bool status)>>(); // Attribute for beaches parameters list (based on date)
    private Dictionary<string, List<bool>> propertiesModified = new Dictionary<string, List<bool>>(); // Attribute for checking if a property was modified (based on date)
    private Dictionary<string, float> rank = new Dictionary<string, float>(); // Attribute for beach rank (based on date)

    /* Custom methods */

    public void AddProperty(string date, string description, bool status) { // Adding a beach property
        if (string.IsNullOrWhiteSpace(date) || string.IsNullOrWhiteSpace(description)){
            Debug.LogWarning("Invalid date or description.");
            return;
        }

        if (!beachProperties.ContainsKey(date)){
            beachProperties[date] = new List<(string description, bool status)>();
            propertiesModified[date] = new List<bool>();
        }

        beachProperties[date].Add((description, status));
        propertiesModified[date].Add(false); // Needs to be false after database implementation
        DatabaseManager.Instance.AddPropertyForBeachDay(beachId, date, description, status);
    }

    public void ModifyProperty(string date, string newDescription, bool newStatus, int index) { // Modifying a beach property
        if (!IsValidPropertyIndex(date, index)){
            Debug.LogWarning("Invalid date or index.");
            return;
        }
        if (string.IsNullOrWhiteSpace(newDescription)){
            Debug.LogWarning("Invalid description.");
            return;
        }

        string oldDescription = beachProperties[date][index].description;
        beachProperties[date][index] = (newDescription, newStatus);
        propertiesModified[date][index] = true;
        DatabaseManager.Instance.ModifyPropertyForBeachDay(beachId, date, oldDescription, newDescription, newStatus);
    }

    public void CopyBeachProperties(Dictionary<string, List<(string description, bool status)>> source, Dictionary<string, List<bool>> sourceModified) { // Copying a set of properties from another beach
        if (source == null || sourceModified == null){
            Debug.LogWarning("Source dictionaries are null.");
            return;
        }

        beachProperties.Clear();
        propertiesModified.Clear();

        foreach (var property in source){
            if (property.Value == null){continue;}

            beachProperties[property.Key] = new List<(string description, bool status)>(property.Value);

            if (sourceModified.ContainsKey(property.Key) && sourceModified[property.Key] != null){
                propertiesModified[property.Key] = new List<bool>(sourceModified[property.Key]);
            }else{
                propertiesModified[property.Key] = new List<bool>();
            }

            while (propertiesModified[property.Key].Count < beachProperties[property.Key].Count){
                propertiesModified[property.Key].Add(false);
            }

            while (propertiesModified[property.Key].Count > beachProperties[property.Key].Count){
                propertiesModified[property.Key].RemoveAt(propertiesModified[property.Key].Count - 1);
            }
        }

        foreach (var property in beachProperties)
        {
            foreach (var item in property.Value)
            {
                DatabaseManager.Instance.AddPropertyForBeachDay(beachId, property.Key, item.description, item.status);
            }
        }
    }

    public void DeleteProperty(string date, int index) { // Deleting a beach property
        if (!IsValidPropertyIndex(date, index)){
            Debug.LogWarning("Invalid date or index.");
            return;
        }

        string description = beachProperties[date][index].description;
        beachProperties[date].RemoveAt(index);
        propertiesModified[date].RemoveAt(index);
        DatabaseManager.Instance.DeletePropertyFromAllBeaches(description);
    }

    public void DeleteAllProperties() { // Deleting all beach properties
        beachProperties.Clear();
        propertiesModified.Clear();
        DatabaseManager.Instance.DeleteAllPropertiesForBeach(beachId);
    }

    public void AddRank(string date, float newRank) { // Adding beach rank
        if (string.IsNullOrWhiteSpace(date)){
            Debug.LogWarning("Invalid date.");
            return;
        }

        rank[date] = Mathf.Clamp(newRank, 0f, 4f);
        DatabaseManager.Instance.AddRankForBeachDay(beachId, date, rank[date]);
    }

    public void LoadPropertyFromDatabase(string date, string description, bool status) { // Loads a property from the database without marking it as modified
        if (string.IsNullOrWhiteSpace(date) || string.IsNullOrWhiteSpace(description)){
            Debug.LogWarning("Invalid database property.");
            return;
        }

        if (!beachProperties.ContainsKey(date)){
            beachProperties[date] = new List<(string description, bool status)>();
            propertiesModified[date] = new List<bool>();
        }

        beachProperties[date].Add((description, status));
        propertiesModified[date].Add(false);
    }

    public void LoadRankFromDatabase(string date, float beachRank) { // Loads a rank from the database
        if (string.IsNullOrWhiteSpace(date)){
            Debug.LogWarning("Invalid rank date.");
            return;
        }
        rank[date] = Mathf.Clamp(beachRank, 0f, 4f);
    }
    // Sets the beach id
    public void SetBeachId(int id) { beachId = id; }

    private bool IsValidPropertyIndex(string date, int index) { // Validating property index
        if (string.IsNullOrWhiteSpace(date)){return false;}

        if (!beachProperties.ContainsKey(date)){return false;}

        if (!propertiesModified.ContainsKey(date)){return false;}

        if (index < 0 || index >= beachProperties[date].Count){return false;}

        if (index >= propertiesModified[date].Count){return false;}

        return true;
    }

    /* Getters */

    public Dictionary<string, List<(string description, bool status)>> GetBeachProperties() { // Getter for beach properties
        Dictionary<string, List<(string description, bool status)>> copy = new Dictionary<string, List<(string description, bool status)>>();

        foreach (var property in beachProperties)
        {
            copy[property.Key] = new List<(string description, bool status)>(property.Value);
        }

        return copy;
    }

    public Dictionary<string, List<bool>> GetPropertiesModified() { // Getter for properties modified
        Dictionary<string, List<bool>> copy = new Dictionary<string, List<bool>>();

        foreach (var property in propertiesModified)
        {
            copy[property.Key] = new List<bool>(property.Value);
        }

        return copy;
    }

    public Dictionary<string, float> GetRank() { // Getter for beach rank
        return new Dictionary<string, float>(rank);
    }
}