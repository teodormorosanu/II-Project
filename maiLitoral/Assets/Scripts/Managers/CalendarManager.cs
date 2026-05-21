using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CalendarManager : MonoBehaviour{

    /* Attributes */

    [SerializeField] private GameObject beachManager; // Attribute for beach manager (reference from beach manager)
    [SerializeField] private GameObject beachCalendar; // Attribute for beach calendar
    [SerializeField] private GameObject dayPrefab; // Attribute that represents the standard form of a day
    [SerializeField] private TextMeshProUGUI currentDateText; // Attribute for current date text
    [SerializeField] private TextMeshProUGUI currentBeachText; // Attribute for current beach text
    [SerializeField] private List<GameObject> weeks; // Attribute for list of weeks in calendar
    private GameObject currentBeach; // Attribute for current beach (reference from beach manager) 

    private Color[] statusColors = new Color[] { // Attribute for 5 colors (each for status 0->4)
        Color.red,
        new Color(1f, 0.5f, 0f),
        Color.yellow,
        new Color(0.5f, 1f, 0f),
        Color.green,
    };

    /* Custom methods */

    private CultureInfo GetCurrentCulture() { // Returns the culture that matches the selected language

        if (SettingsManager.Instance == null){return new CultureInfo("ro-RO");}

        if (SettingsManager.Instance.GetSelectedLanguageIndex() == 0){return new CultureInfo("ro-RO");}

        return new CultureInfo("en-US");
    }

    public void LoadCalendar(DateTime currentDate, GameObject currentBeach) { // Loading the calendar data

        if (currentBeach == null){
            Debug.LogError("Current beach is missing.");
            return;
        }

        if (dayPrefab == null){
            Debug.LogError("Day prefab is missing.");
            return;
        }

        if (currentDateText == null || currentBeachText == null){
            Debug.LogError("Calendar text references are missing.");
            return;
        }

        if (weeks == null || weeks.Count < 6){
            Debug.LogError("Calendar requires 6 week containers.");
            return;
        }

        Beach currentBeachScript = currentBeach.GetComponent<Beach>();

        if (currentBeachScript == null){
            Debug.LogError("Beach component is missing.");
            return;
        }

        this.currentBeach = currentBeach;

        for (int i = 0; i < weeks.Count; i++){ // Destroying already shown days (for each week)

            if (weeks[i] == null){continue;}

            foreach (Transform day in weeks[i].transform){Destroy(day.gameObject);}
        }

        currentBeachText.text = currentBeach.name;

        CultureInfo culture = GetCurrentCulture(); // Gets the culture based on the selected language
        string text = currentDate.ToString("MMMM yyyy", culture); // Formats the date using the selected language

        currentDateText.text = char.ToUpper(text[0]) + text.Substring(1); // Capitalizes the first letter

        int totalDaysLastMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.AddMonths(-1).Month); // Last month number of days
        int totalDaysThisMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month); // Current month number of days

        DateTime firstDayInMonth = new DateTime(currentDate.Year, currentDate.Month, 1);

        int firstDayInMonthIndex = (firstDayInMonth.DayOfWeek == DayOfWeek.Sunday) ? 6 : (int)firstDayInMonth.DayOfWeek - 1; // First day in month

        int totalCalendarDays = totalDaysThisMonth + firstDayInMonthIndex;
        int totalRows = Mathf.CeilToInt(totalCalendarDays / 7f);

        for (int i = 0; i < totalCalendarDays; i++)
        { // Creating first the days in current month, and the days in last month

            if (i / 7 >= weeks.Count){
                Debug.LogError("Not enough calendar rows.");
                return;
            }

            GameObject newDay = Instantiate(dayPrefab, weeks[i / 7].transform); // Instantiating a new day

            if (newDay == null){continue;}

            TextMeshProUGUI dayText = newDay.GetComponentInChildren<TextMeshProUGUI>();
            Image dayImage = newDay.GetComponentInChildren<Image>();

            Transform unavailableMarker = null;

            if (newDay.transform.childCount > 2){unavailableMarker = newDay.transform.GetChild(2);}

            if (firstDayInMonthIndex > i){ // Adding the days from the last month

                int lastMonthDay = totalDaysLastMonth - firstDayInMonthIndex + i + 1;
                newDay.name = "Day_" + lastMonthDay + "_Last";

                if (dayText != null){dayText.text = lastMonthDay.ToString();}

                Button button = newDay.GetComponent<Button>();

                if (button != null){Destroy(button);}

                if (unavailableMarker != null){unavailableMarker.gameObject.SetActive(true);}

                continue;
            }

            int dayNumber = i - firstDayInMonthIndex + 1;

            newDay.name = "Day_" + dayNumber;

            if (dayText != null){dayText.text = dayNumber.ToString();}

            if (firstDayInMonthIndex + currentDate.Day <= i){ // Disabling the buttons on future days in current month

                Button button = newDay.GetComponent<Button>();

                if (button != null){Destroy(button);}

                if (unavailableMarker != null){unavailableMarker.gameObject.SetActive(true);}

                continue;
            }

            DateTime loopDate = new DateTime(currentDate.Year, currentDate.Month, dayNumber); // Getting the relevant date for each day button

            string date = loopDate.ToString("yyyy-MM-dd"); // Consistent database format

            if (currentBeachScript.GetBeachProperties().ContainsKey(date)){

                if (dayImage != null && currentBeachScript.GetRank().ContainsKey(date)){
                    dayImage.color = statusColors[Mathf.FloorToInt(currentBeachScript.GetRank()[date] + 0.5f)];
                }

                Button button = newDay.GetComponent<Button>();

                if (button != null){button.onClick.AddListener(() => SelectDay(loopDate));}
            }else{

                if (dayImage != null){dayImage.color = Color.gray;}

                Button button = newDay.GetComponent<Button>();

                if (button != null){Destroy(button);}
            }
        }

        int totalCells = totalRows * 7;
        int daysLeftInCalendar = totalCells - totalCalendarDays;

        for (int i = 0; i < daysLeftInCalendar; i++)
        { // Adding the days from the next month, if needed

            GameObject newDay = Instantiate(dayPrefab, weeks[totalRows - 1].transform); // Instantiating a new day

            if (newDay == null){continue;}

            TextMeshProUGUI dayText = newDay.GetComponentInChildren<TextMeshProUGUI>();

            Transform unavailableMarker = null;

            if (newDay.transform.childCount > 2){unavailableMarker = newDay.transform.GetChild(2);}

            newDay.name = "Day_" + (i + 1) + "_New";

            if (dayText != null) { dayText.text = (i + 1).ToString();}

            Button button = newDay.GetComponent<Button>();

            if (button != null){Destroy(button);}

            if (unavailableMarker != null){unavailableMarker.gameObject.SetActive(true);}
        }
    }

    private void SelectDay(DateTime currentDate) { // Open properties for selected day

        if (beachManager == null){
            Debug.LogError("BeachManager object is missing.");
            return;
        }

        BeachManager manager = beachManager.GetComponent<BeachManager>();

        if (manager == null){
            Debug.LogError("BeachManager component is missing.");
            return;
        }

        int currentBeachIndex = BeachManager.GetCurrentPressedBeach();

        if (currentBeachIndex < 0){
            Debug.LogError("Invalid beach index.");
            return;
        }

        manager.LoadBeachProperties(currentDate, currentBeachIndex); // Loading properties for selected day

        if (beachCalendar != null){
            ButtonsManager.ToggleObject(beachCalendar); // Disabling calendar panel
        }
    }

    public void NextPreviousMonth(int direction) { // Switch calendar month (last or next month)

        if (currentBeach == null){
            Debug.LogError("Current beach is missing.");
            return;
        }

        if (direction >= 0){ // Next month is only the current month
            LoadCalendar(DateTime.Now, currentBeach);
        }else{ // User can see only the last month

            DateTime lastDate = DateTime.Now.AddMonths(direction);
            lastDate = lastDate.AddDays(DateTime.DaysInMonth(lastDate.Year, lastDate.Month) - lastDate.Day);
            LoadCalendar(lastDate, currentBeach);
        }
    }
}