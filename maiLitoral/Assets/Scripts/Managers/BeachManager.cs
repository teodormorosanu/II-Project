using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class BeachManager : MonoBehaviour{

    /* Attributes */

    [SerializeField] private GameObject calendarManager; // Attribute for calendar manager
    [SerializeField] private GameObject beachCalendar; // Attribute for calendar object
    [SerializeField] private GameObject beachesContent; // Attribute for beaches panel content
    [SerializeField] private GameObject propertiesContent; // Attribute for properties panel content
    [SerializeField] private GameObject beachPrefab; // Attribute that represents the standard form of a beach
    [SerializeField] private GameObject propertyPrefab; // Attribute that represents the standard form of a property
    [SerializeField] private GameObject reviewCalendarPrefab; // Attribute that represents the standard form of review and calendar preset
    [SerializeField] private TextMeshProUGUI propertiesText; // Attribute for properties text
    [SerializeField] private List<GameObject> scrollViews; // Attribute for beach manager scroll views
    private bool propertiesTexting = false; // Attribute for checking if the properties text was changed
    private List<GameObject> beaches = new List<GameObject>(); // Attribute for beaches list
    private Color[] statusColors = new Color[] { // Attribute for 5 colors, each for status 0 -> 4
        Color.red,
        new Color(1f, 0.5f, 0f),
        Color.yellow,
        new Color(0.5f, 1f, 0f),
        Color.green,
    };
    private static int currentPressedBeach; // Attribute for identifying the current pressed beach
    private bool reviewMode = false; // Attribute for review mode

    /* Main Methods */

    private void Start() {LoadBeachesFromDatabase();}

    /* Custom Methods */

    private void LoadBeachesFromDatabase() { // Loads top beaches, beach buttons, ranks and properties from the database
        if (beaches == null){return;}

        if (DatabaseManager.Instance == null){
            Debug.LogError("DatabaseManager instance is missing.");
            return;
        }
        string today = DateTime.Now.ToString("yyyy-MM-dd");

        if (SceneManager.GetActiveScene().name == "StartingPage"){
            List<(string name, float rank)> top3Ranks = DatabaseManager.Instance.GetTop3BeachesByRank(today);
            GameObject topBeaches = GameObject.Find("TopBeaches");

            if (topBeaches == null){
                Debug.LogWarning("TopBeaches object is missing.");
                return;
            }

            for (int i = 0; i < top3Ranks.Count && i < 3 && i < topBeaches.transform.childCount; i++){
                Transform topBeach = topBeaches.transform.GetChild(i);

                TextMeshProUGUI beachText = topBeach.GetComponentInChildren<TextMeshProUGUI>();
                Image beachImage = topBeach.GetComponentInChildren<Image>();

                if (beachText != null){beachText.text = top3Ranks[i].name;}

                if (beachImage != null){
                    beachImage.color = statusColors[Mathf.Clamp(Mathf.FloorToInt(top3Ranks[i].rank + 0.5f), 0, 4)];
                }
            }

            return;
        }

        if (beachPrefab == null || beachesContent == null){
            Debug.LogError("Beach prefab or beaches content is missing.");
            return;
        }

        int selectedZoneId = ZonesManager.GetCurrentPressedZone();
        List<BeachData> beachesFromDatabase = DatabaseManager.Instance.GetBeachesByZone(selectedZoneId);

        DateTime firstDayOfThisMonth = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, 1);
        DateTime firstDayOfLastMonth = firstDayOfThisMonth.AddMonths(-1);

        foreach (GameObject beach in beaches){Destroy(beach);}

        beaches.Clear();

        for (int i = 0; i < beachesFromDatabase.Count; i++){
            BeachData beachData = beachesFromDatabase[i];

            if (beachData == null){continue;}
            GameObject newBeach = Instantiate(beachPrefab, beachesContent.transform);

            if (newBeach == null){continue;}

            newBeach.name = beachData.Name;

            TextMeshProUGUI beachText = newBeach.GetComponentInChildren<TextMeshProUGUI>();
            Image beachImage = newBeach.GetComponentInChildren<Image>();

            if (beachText != null){beachText.text = beachData.Name;}
            Beach currentBeachScript = newBeach.GetComponent<Beach>();

            if (currentBeachScript == null){currentBeachScript = newBeach.AddComponent<Beach>();}

            currentBeachScript.SetBeachId(beachData.Id);

            for (DateTime indexDate = firstDayOfLastMonth; indexDate <= DateTime.Now.Date; indexDate = indexDate.AddDays(1)){
                string date = indexDate.ToString("yyyy-MM-dd");
                float beachRank = DatabaseManager.Instance.GetRankForBeachDay(beachData.Id, date);
                currentBeachScript.LoadRankFromDatabase(date, beachRank);

                List<(string description, bool status)> properties = DatabaseManager.Instance.GetPropertiesForBeachDay(beachData.Id, date);

                foreach (var property in properties){
                    currentBeachScript.LoadPropertyFromDatabase(date, property.description, property.status);
                }

                if (indexDate.Date == DateTime.Now.Date && beachImage != null){
                    beachImage.color = statusColors[Mathf.Clamp(Mathf.FloorToInt(beachRank + 0.5f), 0, 4)];
                }
            }

            Button beachButton = newBeach.GetComponent<Button>();

            if (beachButton != null){
                int index = i;
                beachButton.onClick.AddListener(() => SelectBeach(index));
            }

            beaches.Add(newBeach);
        }
    }

    public void LoadBeachProperties(DateTime currentDate, int beachIndex) { // Loads properties for the selected beach and date
        if (beaches == null || SceneManager.GetActiveScene().name != "BeachPage"){return;}

        if (propertiesContent == null || propertyPrefab == null || reviewCalendarPrefab == null || propertiesText == null){
            Debug.LogError("Beach properties references are missing.");
            return;
        }

        if (!IsValidBeachIndex(beachIndex)){
            Debug.LogError("Invalid beach index.");
            return;
        }

        Beach beachScript = beaches[beachIndex].GetComponent<Beach>();

        if (beachScript == null){
            Debug.LogError("Beach component is missing.");
            return;
        }

        foreach (Transform property in propertiesContent.transform){Destroy(property.gameObject);}
        string date = currentDate.ToString("yyyy-MM-dd");

        Dictionary<string, List<(string description, bool status)>> allBeachProperties = beachScript.GetBeachProperties();
        Dictionary<string, List<bool>> allPropertiesModified = beachScript.GetPropertiesModified();

        if (!allBeachProperties.ContainsKey(date) || !allPropertiesModified.ContainsKey(date)){
            Debug.LogWarning("No beach properties found for selected date.");
            return;
        }

        List<(string description, bool status)> beachProperties = allBeachProperties[date];
        List<bool> propertiesModified = allPropertiesModified[date];

        for (int i = 0; i < beachProperties.Count; i++){
            GameObject newProperty = Instantiate(propertyPrefab, propertiesContent.transform);

            if (newProperty == null){continue;}

            newProperty.name = beachProperties[i].description;

            TextMeshProUGUI propertyText = newProperty.GetComponentInChildren<TextMeshProUGUI>();
            Image[] propertyImages = newProperty.GetComponentsInChildren<Image>();

            if (propertyText != null){propertyText.text = newProperty.name;}

            if (propertyImages.Length < 2){
                Debug.LogWarning("Property prefab images are missing.");
                continue;
            }

            if (reviewMode == true){
                propertyImages[0].color = Color.gray;
                propertyImages[1].color = Color.gray;

                Button trueButton = propertyImages[0].gameObject.GetComponent<Button>();

                if (trueButton == null){trueButton = propertyImages[0].gameObject.AddComponent<Button>();}

                Button falseButton = propertyImages[1].gameObject.GetComponent<Button>();

                if (falseButton == null){
                    falseButton = propertyImages[1].gameObject.AddComponent<Button>();
                }

                int propertyIndex = i;

                trueButton.onClick.AddListener(() => ReviewProperty(date, beachIndex, propertyIndex, newProperty, true));
                falseButton.onClick.AddListener(() => ReviewProperty(date, beachIndex, propertyIndex, newProperty, false));

                continue;
            }

            if (i >= propertiesModified.Count || propertiesModified[i] == false){
                propertyImages[0].color = Color.gray;
                propertyImages[1].color = Color.gray;
                continue;
            }

            if (beachProperties[i].status == true){
                propertyImages[0].color = Color.green;
                propertyImages[1].color = Color.gray;
            }else{
                propertyImages[1].color = Color.red;
                propertyImages[0].color = Color.gray;
            }
        }

        if (reviewMode == false){
            GameObject reviewCalendar = Instantiate(reviewCalendarPrefab, propertiesContent.transform);

            if (reviewCalendar != null){
                Button[] buttons = reviewCalendar.GetComponentsInChildren<Button>();

                if (buttons.Length > 0){
                    buttons[0].onClick.AddListener(() => CalendarButton(beaches[beachIndex]));
                }

                if (buttons.Length > 1){
                    buttons[1].onClick.AddListener(() => ReviewButton(currentDate));

                    if (currentDate.Date == DateTime.Now.Date || currentDate.Date == DateTime.Now.AddDays(-1).Date){
                        buttons[1].gameObject.SetActive(true);
                    }
                }
            }

            if (propertiesTexting == false){
                propertiesText.text = propertiesText.text + " " + date;
                propertiesTexting = true;
            }
        }
    }

    private void SelectBeach(int index) { // Opens the selected beach panel
        if (!IsValidBeachIndex(index)){
            Debug.LogError("Invalid beach index.");
            return;
        }

        if (scrollViews == null || scrollViews.Count < 2 || scrollViews[0] == null || scrollViews[1] == null){
            Debug.LogError("Scroll views are missing.");
            return;
        }

        currentPressedBeach = index;

        ButtonsManager.ToggleObject(scrollViews[1]);
        ButtonsManager.ToggleObject(scrollViews[0]);

        LoadBeachProperties(DateTime.Now, index);
    }

    private void ReviewProperty(string date, int beachIndex, int propertyIndex, GameObject property, bool mode) { // Updates a selected property based on the user review
        if (!IsValidBeachIndex(beachIndex) || property == null){
            Debug.LogError("Invalid beach or property.");
            return;
        }

        Beach beachScript = beaches[beachIndex].GetComponent<Beach>();

        if (beachScript == null){
            Debug.LogError("Beach component is missing.");
            return;
        }

        beachScript.ModifyProperty(date, property.name, mode, propertyIndex);
        Image[] propertyImages = property.GetComponentsInChildren<Image>();

        if (propertyImages.Length < 2){
            Debug.LogWarning("Property images are missing.");
            return;
        }

        if (mode == true){
            propertyImages[0].color = Color.green;
            propertyImages[1].color = Color.gray;
        }else{
            propertyImages[1].color = Color.red;
            propertyImages[0].color = Color.gray;
        }
    }

    private void ReviewButton(DateTime currentDate) { // Enters review mode for the selected beach
        reviewMode = true;

        if (scrollViews != null && scrollViews.Count > 1 && scrollViews[1] != null && scrollViews[1].transform.childCount > 1){
            Scrollbar scrollbar = scrollViews[1].transform.GetChild(1).GetComponent<Scrollbar>();

            if (scrollbar != null){scrollbar.value = 1;}
        }

        LoadBeachProperties(currentDate, currentPressedBeach);
    }

    private void CalendarButton(GameObject currentBeach) { // Opens the calendar for the selected beach
        if (calendarManager == null){
            Debug.LogError("CalendarManager object is missing.");
            return;
        }

        CalendarManager manager = calendarManager.GetComponent<CalendarManager>();

        if (manager == null){
            Debug.LogError("CalendarManager component is missing.");
            return;
        }

        if (beachCalendar != null){
            ButtonsManager.ToggleObject(beachCalendar);
        }

        manager.LoadCalendar(DateTime.Now, currentBeach);
    }

    public void BackToBeaches() { // Returns to the beach panel and disables review mode
        reviewMode = false;
    }

    private bool IsValidBeachIndex(int index) { // Checks if the beach index is valid
        if (beaches == null){return false;}

        if (index < 0 || index >= beaches.Count){return false;}

        if (beaches[index] == null){return false;}

        return true;
    }

    /* Getters */

    public static int GetCurrentPressedBeach() { // Returns the index of the currently selected beach
        return currentPressedBeach;
    }
}