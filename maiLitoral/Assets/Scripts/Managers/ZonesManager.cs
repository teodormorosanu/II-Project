using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ZonesManager : MonoBehaviour{

    /* Attributes */

    [SerializeField] private GameObject zonesManager; // Attribute for zones manager
    [SerializeField] private GameObject zonesContent; // Attribute for zones panel content
    [SerializeField] private GameObject zonePrefab; // Attribute that represents the standard form of a zone
    private List<GameObject> zones = new List<GameObject>(); // Attribute for zones list
    private static int currentPressedZone; // Attribute for identifying the current pressed zone

    /* Main Methods */

    private void Start() { // Loads all zones from the database and creates a button for each one
        LoadZonesFromDatabase();
    }

    /* Custom Methods */

    private void LoadZonesFromDatabase() { // Loads all zones from the database and creates a button for each one
        if (zones == null || SceneManager.GetActiveScene().name != "StartingPage"){return;}

        if (DatabaseManager.Instance == null){
            UnityEngine.Debug.LogError("DatabaseManager instance is missing.");
            return;
        }

        if (zonePrefab == null){
            UnityEngine.Debug.LogError("Zone prefab is missing.");
            return;
        }

        if (zonesContent == null){
            UnityEngine.Debug.LogError("Zones content is missing.");
            return;
        }

        foreach (GameObject zone in zones)
        {
            Destroy(zone);
        }

        zones.Clear();
        List<ZoneData> zonesFromDatabase = DatabaseManager.Instance.GetAllZones();

        foreach (ZoneData zoneData in zonesFromDatabase)
        {
            if (zoneData == null) { continue;}

            bool zoneAlreadyExists = false;

            foreach (GameObject zone in zones)
            {
                if (zone != null && zone.name == zoneData.Name){
                    zoneAlreadyExists = true;
                    break;}
            }

            if (zoneAlreadyExists){continue;}

            GameObject newZone = Instantiate(zonePrefab, zonesContent.transform);

            if (newZone == null){continue;}

            newZone.name = zoneData.Name;

            if (newZone.transform.childCount > 0){
                TextMeshProUGUI zoneText = newZone.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

                if (zoneText != null){zoneText.text = zoneData.Name;
                }else{
                    UnityEngine.Debug.LogWarning("Zone prefab child text component is missing.");
                }
            }else{
                UnityEngine.Debug.LogWarning("Zone prefab child text is missing.");
            }

            Button zoneButton = newZone.GetComponent<Button>();

            if (zoneButton != null){
                int zoneId = zoneData.Id;
                zoneButton.onClick.AddListener(() => SelectZone(zoneId));
            }else{
                UnityEngine.Debug.LogWarning("Zone prefab button component is missing.");
            }

            zones.Add(newZone);
        }
    }

    private void SelectZone(int index) { // Saves the selected zone id and opens the beach page
        bool zoneExists = false;

        foreach (GameObject zone in zones)
        {
            if (zone != null){
                zoneExists = true;
                break;
            }
        }

        if (!zoneExists){
            UnityEngine.Debug.LogWarning("Cannot select zone. Zones list is empty.");
            return;
        }

        currentPressedZone = index;
        ButtonsManager.ReturnToPage("BeachPage");
    }

    /* Getters */
    // Returns the id of the currently selected zone
    public static int GetCurrentPressedZone() { return currentPressedZone;}
    // Returns the instantiated zone objects
    public List<GameObject> GetZones() { return new List<GameObject>(zones); }
}