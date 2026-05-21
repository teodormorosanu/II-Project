using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class SearchManager : MonoBehaviour
{

    /* Attributes */

    [SerializeField] private GameObject zonesManager; // Attribute for zones manager
    [SerializeField] private GameObject zonesContent; // Attribute for zones content
    [SerializeField] private GameObject zoneUnavailable; // Attribute for zone unavailable
    [SerializeField] private GameObject searchButton; // Attribute for search button
    [SerializeField] private TMP_InputField searchInputField; // Attribute for search input field
    [SerializeField] private TMP_Text autoCompleteText; // Attribute for auto complete text
    private List<GameObject> zones = new List<GameObject>(); // Attribute for zones list
    private string currentSuggestion = ""; // Attribute for current zone suggestion

    /* Main Methods */

    private void Start() {
        SearchInit(); // Setup the search suggestion
    }

    /* Custom methods */

    private void SearchInit() { // Setup the search suggestion

        if (zonesManager == null){
            Debug.LogError("ZonesManager object is missing.");
            return;
        }

        ZonesManager manager = zonesManager.GetComponent<ZonesManager>();

        if (manager == null){
            Debug.LogError("ZonesManager component is missing.");
            return;
        }

        if (searchInputField == null){
            Debug.LogError("Search input field is missing.");
            return;
        }

        if (autoCompleteText == null){
            Debug.LogError("Autocomplete text is missing.");
            return;
        }

        if (searchButton == null){
            Debug.LogError("Search button is missing.");
            return;
        }

        zones = manager.GetZones(); // Getting the available zones

        searchButton.GetComponent<Button>().onClick.AddListener(() => SearchButton()); // Added the listener for the search button
        searchInputField.onValueChanged.AddListener(OnSearchValueChanged); // Added the listener for input field

        searchInputField.characterLimit = 35;

        autoCompleteText.text = ""; // Making sure the autocomplete is empty first

        ShowAllZones(); // Showing all zones in the start (because none was searched)
    }

    private void OnSearchValueChanged(string currentText) { // Listener for updating the panels when typing
        UpdateSuggestion(currentText); // Updating the panels when typing
        FilterZones(currentText); // Showing only the matched searched zones
    }

    private void UpdateSuggestion(string currentText) { // Updating the autocomplete suggestion based on the typed text

        currentSuggestion = ""; // Resetting the current suggestion first

        if (string.IsNullOrWhiteSpace(currentText)){ // Checking if the input field is empty
            autoCompleteText.text = ""; // Clearing the autocomplete text
            return; // Stopping the method because there is no text to search for
        }

        string lowerText = currentText.ToLower(); // Converting the typed text to lowercase for case insensitive search

        List<string> suggestions = new List<string>();

        foreach (GameObject zone in zones)
        { // Going through all available zones

            if (zone == null){continue;}

            string label = zone.name; // Getting the current zone name

            if (label.ToLower().StartsWith(lowerText)){ // Checking if the zone starts with the typed text
                suggestions.Add(label);
            }
        }

        if (suggestions.Count == 0){ // Checking if no suggestion was found
            autoCompleteText.text = ""; // Clearing the autocomplete text
            return; // Stopping the method because there is no valid suggestion
        }

        currentSuggestion = suggestions[0];

        bool sameCase = currentSuggestion.StartsWith(currentText); // Checking if the typed text matches the same uppercase/lowercase format

        if (!sameCase){ // Checking if the user typed with different uppercase/lowercase letters
            autoCompleteText.text = ""; // Hiding the autocomplete text because it looks visually incorrect
            return; // Stopping the method because the casing does not match
        }

        autoCompleteText.text = currentSuggestion; // Showing the autocomplete suggestion if the casing matches correctly
    }

    private void SearchButton() { // Opens the right zone based on autocompleted text

        if (autoCompleteText.text == "" || autoCompleteText.text == null){

            if (zoneUnavailable != null && zoneUnavailable.activeSelf == false){ButtonsManager.ToggleObject(zoneUnavailable);}

            return;
        }

        if (zonesContent == null){
            Debug.LogError("Zones content is missing.");
            return;
        }

        for (int i = 0; i < zonesContent.transform.childCount; i++){ // Calls the zone button method for the available zone name in the autocomplete text

            GameObject zone = zonesContent.transform.GetChild(i).gameObject;

            if (zone == null){continue;}

            if (zone.name == autoCompleteText.text)
            {
                Button button = zone.GetComponent<Button>();

                if (button != null) { button.onClick.Invoke(); } // Invoking the right zone button method
                break;
            }
        }
    }

    private void FilterZones(string currentText) { // Showing only the matched searched zones

        if (string.IsNullOrWhiteSpace(currentText)){
            ShowAllZones();
            return;
        }

        string lowerText = currentText.ToLower();

        foreach (GameObject zone in zones)
        { // Showing only the relevant zone

            if (zone == null){continue;}

            bool matches = zone.name.ToLower().Contains(lowerText);

            zone.SetActive(matches);
        }
    }

    private void ShowAllZones() { // Showing all zones

        foreach (GameObject zone in zones)
        {
            if (zone == null){continue;}
            zone.SetActive(true);
        }
    }

    public void ShowZonesManager(bool mode) { // Activate / Deactivate the zones panel

        if (zonesManager == null){
            Debug.LogError("ZonesManager object is missing.");
            return;
        }

        zonesManager.SetActive(mode);
    }

    public void HideZoneUnavailable() { // Deactivate the zone unavailable panel

        if (zoneUnavailable == null){
            Debug.LogError("ZoneUnavailable object is missing.");
            return;
        }

        zoneUnavailable.SetActive(false);
    }
}