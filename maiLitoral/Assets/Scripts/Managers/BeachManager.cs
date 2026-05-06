using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BeachManager : MonoBehaviour
{
    /* Attributes */

    [SerializeField] private GameObject calendarManager;
    [SerializeField] private GameObject beachCalendar;
    [SerializeField] private GameObject beachesContent;
    [SerializeField] private GameObject propertiesContent;
    [SerializeField] private GameObject beachPrefab;
    [SerializeField] private GameObject propertyPrefab;
    [SerializeField] private GameObject reviewCalendarPrefab;
    [SerializeField] private TextMeshProUGUI propertiesText;
    [SerializeField] private List<GameObject> scrollViews;

    private List<GameObject> beaches = new List<GameObject>();

    private Color[] statusColors = new Color[] {
        Color.red,
        new Color(1f, 0.5f, 0f),
        Color.yellow,
        new Color(0.5f, 1f, 0f),
        Color.green,
    };

    private static int currentPressedBeach;
    private bool reviewMode = false;

    /* Main Methods */

    private void Start()
    {
        LoadBeachesFromDatabase();
    }

    /* Custom Methods */

    private void LoadBeachesFromDatabase()
    {
        if (beaches == null)
        {
            return;
        }

        string today = DateTime.Now.ToString("dd-MM-yyyy");

        if (SceneManager.GetActiveScene().name == "StartingPage")
        {
            List<(string name, float rank)> top3Ranks =
                DatabaseManager.Instance.GetTop3BeachesByRank(today);

            GameObject topBeaches = GameObject.Find("TopBeaches");

            if (topBeaches == null)
            {
                return;
            }

            for (int i = 0; i < top3Ranks.Count && i < 3; i++)
            {
                topBeaches.transform.GetChild(i).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                    top3Ranks[i].name;

                topBeaches.transform.GetChild(i).transform.GetChild(1).GetComponent<UnityEngine.UI.Image>().color =
                    statusColors[Mathf.Clamp(Mathf.FloorToInt(top3Ranks[i].rank + 0.5f), 0, 4)];
            }

            return;
        }

        int selectedZoneId = ZonesManager.GetCurrentPressedZone();

        List<BeachData> beachesFromDatabase =
            DatabaseManager.Instance.GetBeachesByZone(selectedZoneId);

        DateTime firstDayOfThisMonth =
            new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, 1);

        DateTime firstDayOfLastMonth =
            firstDayOfThisMonth.AddMonths(-1);

        for (int i = 0; i < beachesFromDatabase.Count; i++)
        {
            BeachData beachData = beachesFromDatabase[i];

            GameObject newBeach = Instantiate(beachPrefab, beachesContent.transform);
            newBeach.name = "Beach_" + beachData.Id;
            newBeach.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = beachData.Name;

            newBeach.AddComponent<Beach>();
            Beach currentBeachScript = newBeach.GetComponent<Beach>();
            currentBeachScript.SetBeachId(beachData.Id);

            for (DateTime indexDate = firstDayOfLastMonth; indexDate <= DateTime.Now.Date; indexDate = indexDate.AddDays(1))
            {
                string date = indexDate.ToString("dd-MM-yyyy");

                float beachRank =
                    DatabaseManager.Instance.GetRankForBeachDay(beachData.Id, date);

                currentBeachScript.LoadRankFromDatabase(date, beachRank);

                List<(string description, bool status)> properties =
                    DatabaseManager.Instance.GetPropertiesForBeachDay(beachData.Id, date);

                foreach (var property in properties)
                {
                    currentBeachScript.LoadPropertyFromDatabase(
                        date,
                        property.description,
                        property.status
                    );
                }

                if (indexDate.Date == DateTime.Now.Date)
                {
                    newBeach.transform.GetChild(1).GetComponent<UnityEngine.UI.Image>().color =
                        statusColors[Mathf.Clamp(Mathf.FloorToInt(beachRank + 0.5f), 0, 4)];
                }
            }

            int index = i;
            newBeach.GetComponent<Button>().onClick.AddListener(() => SelectBeach(index));

            beaches.Add(newBeach);
        }
    }

    public void LoadBeachProperties(DateTime currentDate, int beachIndex)
    {
        if (beaches == null || SceneManager.GetActiveScene().name != "BeachPage")
        {
            return;
        }

        foreach (Transform property in propertiesContent.transform)
        {
            Destroy(property.gameObject);
        }

        string date = currentDate.ToString("dd-MM-yyyy");

        List<(string description, bool status)> beachProperties =
            beaches[beachIndex].GetComponent<Beach>().GetBeachProperties()[date];

        List<bool> propertiesModified =
            beaches[beachIndex].GetComponent<Beach>().GetPropertiesModified()[date];

        for (int i = 0; i < beachProperties.Count; i++)
        {
            GameObject newProperty = Instantiate(propertyPrefab, propertiesContent.transform);
            newProperty.name = beachProperties[i].description;
            newProperty.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = newProperty.name;

            if (reviewMode == true)
            {
                newProperty.transform.GetChild(1).GetComponent<UnityEngine.UI.Image>().color = Color.gray;
                newProperty.transform.GetChild(2).GetComponent<UnityEngine.UI.Image>().color = Color.gray;

                newProperty.transform.GetChild(1).gameObject.AddComponent<Button>();
                newProperty.transform.GetChild(2).gameObject.AddComponent<Button>();

                int propertyIndex = i;

                newProperty.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(
                    () => ReviewProperty(date, beachIndex, propertyIndex, newProperty, true)
                );

                newProperty.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(
                    () => ReviewProperty(date, beachIndex, propertyIndex, newProperty, false)
                );

                continue;
            }

            if (propertiesModified[i] == false)
            {
                newProperty.transform.GetChild(1).GetComponent<UnityEngine.UI.Image>().color = Color.gray;
                newProperty.transform.GetChild(2).GetComponent<UnityEngine.UI.Image>().color = Color.gray;
                continue;
            }

            if (beachProperties[i].status == true)
            {
                newProperty.transform.GetChild(1).GetComponent<UnityEngine.UI.Image>().color = Color.green;
                newProperty.transform.GetChild(2).GetComponent<UnityEngine.UI.Image>().color = Color.gray;
            }
            else
            {
                newProperty.transform.GetChild(2).GetComponent<UnityEngine.UI.Image>().color = Color.red;
                newProperty.transform.GetChild(1).GetComponent<UnityEngine.UI.Image>().color = Color.gray;
            }
        }

        if (reviewMode == false)
        {
            GameObject reviewCalendar = Instantiate(reviewCalendarPrefab, propertiesContent.transform);

            reviewCalendar.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => ReviewButton(currentDate));
            reviewCalendar.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => CalendarButton(beaches[beachIndex]));

            if (currentDate.Date == DateTime.Now.Date || currentDate.Date == DateTime.Now.AddDays(-1).Date)
            {
                reviewCalendar.transform.GetChild(1).gameObject.SetActive(true);
            }

            propertiesText.text = "Facilitățile din " + date;
        }
    }

    private void SelectBeach(int index)
    {
        currentPressedBeach = index;
        ButtonsManager.ToggleObject(scrollViews[1]);
        ButtonsManager.ToggleObject(scrollViews[0]);
        LoadBeachProperties(DateTime.Now, index);
    }

    private void ReviewProperty(string date, int beachIndex, int propertyIndex, GameObject property, bool mode)
    {
        beaches[beachIndex].GetComponent<Beach>().ModifyProperty(date, property.name, mode, propertyIndex);

        if (mode == true)
        {
            property.transform.GetChild(1).GetComponent<UnityEngine.UI.Image>().color = Color.green;
            property.transform.GetChild(2).GetComponent<UnityEngine.UI.Image>().color = Color.gray;
        }
        else
        {
            property.transform.GetChild(2).GetComponent<UnityEngine.UI.Image>().color = Color.red;
            property.transform.GetChild(1).GetComponent<UnityEngine.UI.Image>().color = Color.gray;
        }
    }

    private void ReviewButton(DateTime currentDate)
    {
        reviewMode = true;
        propertiesText.text = "Finalizează";
        scrollViews[1].transform.GetChild(1).GetComponent<Scrollbar>().value = 1;
        LoadBeachProperties(currentDate, currentPressedBeach);
    }

    private void CalendarButton(GameObject currentBeach)
    {
        ButtonsManager.ToggleObject(beachCalendar);
        calendarManager.GetComponent<CalendarManager>().LoadCalendar(DateTime.Now, currentBeach);
    }

    public void BackToBeaches()
    {
        reviewMode = false;
    }

    /* Getters */

    public static int GetCurrentPressedBeach()
    {
        return currentPressedBeach;
    }
}