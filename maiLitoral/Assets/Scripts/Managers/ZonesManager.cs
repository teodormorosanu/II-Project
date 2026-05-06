using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ZonesManager : MonoBehaviour
{

    /* Attributes */

    [SerializeField] private GameObject zonesManager; // Attribute for zones manager
    [SerializeField] private GameObject zonesContent; // Attribute for zones panel content
    [SerializeField] private GameObject zonePrefab; // Attribute that represents the standard form of a zone
    private List<GameObject> zones = new List<GameObject>(); // Attribute for zones list
    private static int currentPressedZone; // Attribute for identifying the current pressed zone

    /* Main Methods */

    private void Awake()
    {
        LoadZonesFromDatabase(); // Loading zones from data base *needs to be implemented*
    }

    /* Custom Methods */

    private void LoadZonesFromDatabase()
    { // Loading zones from data base *needs to be implemented*
        // For each zone in database, add a new object inside zone list
        if (zones == null || SceneManager.GetActiveScene().name != "StartingPage")
        {
            return;
        }

        List<ZoneData> zonesFromDatabase = DatabaseManager.Instance.GetAllZones();

        for (int i = 0; i < zonesFromDatabase.Count; i++)
        {
            ZoneData zoneData = zonesFromDatabase[i];

            GameObject newZone = Instantiate(zonePrefab, zonesContent.transform); // Instantiating a new zone
            newZone.name = "Zone_" + zoneData.Id;
            newZone.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = zoneData.Name; // Setting the zone name in it's text field

            int zoneId = zoneData.Id;
            newZone.GetComponent<Button>().onClick.AddListener(() => SelectZone(zoneId)); // Adding the correspondent listener to zone button

            zones.Add(newZone); // Adding the zone in the list
        }
    }

    private void SelectZone(int index)
    { // Open selected zone panel
        currentPressedZone = index; // Saving the current pressed zone
        ButtonsManager.ReturnToPage("BeachPage"); // Switching scene
    }

    /* Getters */

    public static int GetCurrentPressedZone()
    { // Getter for the current zone pressed
        return currentPressedZone;
    }

    public List<GameObject> GetZones()
    { // Getter for zones list
        return zones;
    }
}