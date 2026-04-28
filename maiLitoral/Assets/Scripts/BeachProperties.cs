using System.Globalization;
using UnityEngine;

public class BeachProperties
{
    // General properties

    public string beachName;
    public string beachDescription;
    public string beachLocation;
    public string sandType;
    public bool hasSand;
    public bool hasRocks;

    // Unique properties

    public bool HasSunbeds;
    public bool HasUmbrellas;
    public bool HasCabins;
    public bool HasShowers;
    public bool HasToilets;
    public bool HasParking;
    public bool IsEasyAccess;
    public bool HasLifeguard;
    public bool IsSafe;
    public bool IsClean;

    // name and location of the beach
    public string GetBeachInfo()
    {
        return beachName + " - " + beachLocation;
    }

    public void SetBeachInfo(string name, string location)
    {
        beachName = name;
        beachLocation = location;
    }

    //description of the beach

    public string GetBeachDescription()
    {
        return beachDescription;
    }

    public void SetBeachDescription(string description)
    {

        beachDescription = description;
    }
    public string GetFacilities()
    {
        string facilities = "";
        if (HasCabins) { facilities += "Cabins, "; }
        if (HasSunbeds) { facilities += "Sunbeds, "; }
        if (HasUmbrellas) { facilities += "Umbrellas, "; }
        if (HasShowers) { facilities += "Showers, "; }
        if (HasToilets) { facilities += "Toilets, "; }
        if (HasParking) { facilities += "Parking space, "; }
        if (IsEasyAccess) { facilities += "Has easy access, "; }
        if (HasLifeguard) { facilities += "Has lifeguard, "; }
        if (IsSafe) { facilities += "Is safe to go, "; }
        if (IsClean) { facilities += "It is clean, "; }
        return facilities.TrimEnd(',', ' ');

    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}